using Plugin.Media;
using Plugin.Media.Abstractions;
using project2.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using ImageCircle;
using System.Text;
using System.Threading.Tasks;


using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace project2.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage : ContentPage
    {
        public MediaFile profilepic;
        public RegisterPage()
        {
            InitializeComponent();
            imgButtonTakePic.Source = ImageSource.FromResource("project2.Assets.TakePic.png");
            imgBackground.Source = ImageSource.FromResource("project2.Assets.LoginBg.png");
            imgLogo.Source = ImageSource.FromResource("project2.Assets.LogoWhite.png");
            imgMail.Source = ImageSource.FromResource("project2.Assets.Mail.png");
            imgLock.Source = ImageSource.FromResource("project2.Assets.Lock.png");
            imgName.Source = ImageSource.FromResource("project2.Assets.avatar.png");

        }

        private async void lblAlreadyAcc_Tapped(object sender, EventArgs e)
        {
            Application.Current.MainPage = new LoginPage();
        }

        private async Task btnRegistreer_Clicked(object sender, EventArgs e)
        {
            //Objec user klaarmaken om te versturen 
            User reguser = new User();
            Guid guid = Guid.NewGuid();

            reguser.UserId = guid;
            reguser.Name = entName.Text;
            reguser.Password = entPassword.Text;
            reguser.Email = entMail.Text;
            //Schrijven naar database en foto in blob zetten
            if (imgProfilePic.Source == null)
            {
                await DisplayAlert("Attention!", "Don't be shy, take a picture", "OK");
                frameLayer.IsVisible = false;
                indicatorLoader.IsRunning = false;
            }
            else if (entMail.Text != "" && entPassword.Text != "" && entName.Text != "" && imgProfilePic.Source != null && entMail.Text.Contains("@") && entMail.Text.Contains("."))
            {
                //LadenRegistreren.IsRunning = true;
                var semiTransparentColor = new Color(0, 0, 0, 0.5);
                frameLayer.BackgroundColor = semiTransparentColor;
                frameLayer.IsVisible = true;
                indicatorLoader.IsRunning = true;
                RegisterUser(reguser);
            }

            else
            {
                await DisplayAlert("Attention!", "Please fill in all fields correctly", "OK");
                frameLayer.IsVisible = false;
                indicatorLoader.IsRunning = false;
            }

        }

        private async void SendPicToBlob(MediaFile photo, Guid guid)
        {
            using (var memoryStream = new MemoryStream())
            {
                photo.GetStream().CopyTo(memoryStream);
                photo.Dispose();
                var resultstream = memoryStream.ToArray();

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                try
                {
                    string containerName = "profilepictures";
                    string fileName = guid.ToString() + ".jpg";

                    string url = String.Format("https://imaniaf.azurewebsites.net/api/pictures/send/{0}/{1}", containerName, fileName);

                    //build the binary content
                    var binaryContent = new ByteArrayContent(resultstream);
                    binaryContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

                    //send the content in a POST
                    var result = await client.PostAsync(url, binaryContent);

                    //catch the result
                    if (result.IsSuccessStatusCode)
                    {
                        string imageUrl = await result.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        Debug.WriteLine("something went wrong");
                        frameLayer.IsVisible = false;
                        indicatorLoader.IsRunning = false;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        private async void RegisterUser(User user)
        {
            User user_controle = await ImaniManager.AddUserAsync(user);

            if (user_controle.Email != "NOK")
            {
                SendPicToBlob(profilepic, user.UserId);
                await DisplayAlert("Succes", "You are successfully registered!", "OK");
                Application.Current.MainPage = new LoginPage();
            }
            else if (imgProfilePic.Source == null)
            {
                await DisplayAlert("Attention!", "Don't be shy, take a picture", "OK");
                frameLayer.IsVisible = false;
                indicatorLoader.IsRunning = false;
            }
            else if (user_controle.Email == "NOK")
            {
                await DisplayAlert("Attention!", "This email already exsists!", "OK");
                frameLayer.IsVisible = false;
                indicatorLoader.IsRunning = false;
            }
            else
            {
                await DisplayAlert("Attention!", "Something went wrong!", "OK");
                frameLayer.IsVisible = false;
                indicatorLoader.IsRunning = false;
            }
        }

        private async void TakePic()
        {
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", "No camera avaialble.", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                DefaultCamera = CameraDevice.Front, //Standaar voor camera
                AllowCropping = true, //Gebruiker kan de foto croppen
                PhotoSize = PhotoSize.Small,
                Directory = "Profilepic",
                Name = "profilepic.jpg"
            });

            if (file == null)
                return;

            //Deze lijn code is echt overbodig vind ik persoonlijk. De gebruiker heeft niet echt interesse in het lokale pad van zijn profielfoto. (was dan ook voor mij om te zien of het werkte)
            //await DisplayAlert("File Location", file.Path, "OK");

            //Lege Profielfoto ZICHTBAAR maken
            imgProfilePic.IsVisible = true;

            //Profielfoto invullen
            imgProfilePic.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                return stream;
            });

            //Knop onzichtbaar maken
            imgButtonTakePic.IsVisible = false;

            //media stream klaarzetten
            profilepic = file;
        }

        private void imgTakePic_Tapped(object sender, EventArgs e)
        {
            TakePic();
        }

        private void CirlceImage_Tapped(object sender, EventArgs e)
        {
            TakePic();
        }
    }
}
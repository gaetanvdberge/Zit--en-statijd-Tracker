using Acr.UserDialogs;
using Plugin.Media;
using Plugin.Media.Abstractions;
using project2.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace project2.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfileSettingsPage : ContentPage
    {
        public MediaFile profilepic;

        public ProfileSettingsPage()
        {
            InitializeComponent();
            iconEditPicture.Source = ImageSource.FromResource("project2.Assets.EditPic.png");
            imgProfilePic.Source = GlobalVariables.user.UrlProfilePicture;
            txtName.Text = GlobalVariables.user.Name;
            txtEmail.Text = GlobalVariables.user.Email;
            lblUserName.Text = GlobalVariables.user.Name;
        }


        //BIJ KLIKKEN OP DELETE ACCOUNT
        private async void btnDeleteAccount_Clicked(object sender, EventArgs e)
        {
            //Na het succesvol verwijderen terug naar de login pagina gaan (LAAT DIT STAAN, DIT IS AL IN ORDE):
            var resultaat = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
            {
                OkText = "Ok",
                CancelText = "Cancel",
                Message = "Are you sure you would like to delete your account? You will lose all your data!",
                Title = "Alert!",
            });

            if (resultaat)
            {
                var result = await UserDialogs.Instance.PromptAsync(new PromptConfig
                {
                    Title = "Confirm",
                    Message = "To DELETE your account, please enter your password!",
                    OkText = "Ok",
                    CancelText = "Cancel",
                    InputType = InputType.Password,
                });

                var EnteredPassword = result.Value;
                try
                {
                    User user_try = new User();
                    user_try.UserId = GlobalVariables.user.UserId;
                    user_try.Password = EnteredPassword;
                    var response = await ImaniManager.CHeckPswAsync(user_try);
                    if (response == "NOK")
                        await DisplayAlert("Attention!", "The password does not match!", "Ok");
                    else if (response == "OK")
                    {
                        await ImaniManager.DeleteUserAsync(GlobalVariables.user.UserId.ToString());
                        await DisplayAlert("Succes!", "Succefully deleted your account! We are now navigating to the login page", "Ok");
                        GlobalVariables.displayedScreen = "Dashboard";
                        Application.Current.MainPage = new LoginPage();
                    }
                }
                catch
                {
                    await DisplayAlert("Alert!", "Something went wrong!", "Ok");
                }
            }
           
            
        }

        private async Task btnSaveChanges_Clicked(object sender, EventArgs e)
        {
            var result = await UserDialogs.Instance.PromptAsync(new PromptConfig
            {
                Title = "Confirm",
                Message = "To confirm changes, please enter your old password!",
                OkText = "Ok",
                CancelText = "Cancel",
                InputType = InputType.Password,
            });

            var EnteredPassword = result.Value;
            if (result.Ok)
            {
                try
                {
                    User user_try = new User();
                    user_try.UserId = GlobalVariables.user.UserId;
                    user_try.Password = EnteredPassword;
                    var response = await ImaniManager.CHeckPswAsync(user_try);
                    if (response == "NOK")
                        await DisplayAlert("Attention!", "The password does not match!", "Ok");
                    else if (response == "OK")
                    {
                        user_try.Email = txtEmail.Text;
                        user_try.Name = txtName.Text;
                        if (txtNewPass.Text != null)
                            user_try.Password = txtNewPass.Text;
                        else
                            user_try.Password = EnteredPassword;
                        await ImaniManager.UpdateUserAsync(user_try);
                        GlobalVariables.user.Name = txtName.Text;
                        await DisplayAlert("Succes!", "Succefully saved!", "Ok");
                    }
                }
                catch
                {
                    await DisplayAlert("Alert!", "Something went wrong!", "Ok");
                }

                Application.Current.MainPage = new SettingsPage();
            }

        }


        //PROFIELFOTO NEMEN
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

            if (file != null)
            {
                profilepic = file;
                SendPicToBlob(profilepic, GlobalVariables.user.UserId);
                await DisplayAlert("Succes", "The picture is succefully uploaded, it will take two days to review!", "OK");
            }


        }

        //PRofielfoto naar BLOB
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
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private void CirlceImage_Tapped(object sender, EventArgs e)
        {
            TakePic();
        }
    }
}
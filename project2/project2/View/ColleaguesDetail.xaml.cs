using Acr.UserDialogs;
using project2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

namespace project2.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ColleaguesDetail : ContentPage
    {
        public ColleaguesDetail()
        {
            InitializeComponent();
            AddColleague.Source = ImageSource.FromResource("project2.Assets.AddColleague.png");
            GetColleaguesAsync();

            lvwColleaguesDelete.IsVisible = false;
            
        }

        private async void GetColleaguesAsync()
        {
            List<User> users = await ImaniManager.GetFollowersAsync(GlobalVariables.user.UserId.ToString());
            if (users.Count() == 0)
            {
                lvwColleagues.IsVisible = false;
                StackNoFriends.IsVisible = true;
            }
            else
            {
                lvwColleagues.ItemsSource = users;
                lvwColleaguesDelete.ItemsSource = users;
            }
        }

        private void lvwColleagues_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            GlobalVariables.selectedColleague = lvwColleagues.SelectedItem as User; //De geselecteerde Colleague wordt opgeslagen in de globale Variabelen
            if (GlobalVariables.selectedColleague != null)
            {
                Navigation.PushAsync(new ContactPage());
                lvwColleagues.SelectedItem = null;

            }
        }

        private async void  AddColleague_Tapped(object sender, EventArgs e)
        {

            var action = await DisplayActionSheet("How would you like to add a colleague?", "Cancel", null, "Scan QR-Code", "Enter code manually");
            switch (action)
            {
                case "Scan QR-Code":
                    //Hier komt de code voor de QR-Code Camera
                    var ResultaatCode = "0";
                    var scan = new ZXingScannerPage();
                    Navigation.PushAsync(scan);
                    scan.OnScanResult += (resultaat) =>
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            Navigation.PopAsync();
                            ResultaatCode = resultaat.Text;

                            if (ResultaatCode.Length == 6)
                            {
                                try
                                {
                                    var response = await ImaniManager.FollowUserAsync(GlobalVariables.user.UserId.ToString().ToUpper(), ResultaatCode);
                                    if (response != "User niet gevonden")
                                        await DisplayAlert("Attention!", "You are now succesfully following: " + response, "Ok");
                                    else
                                        await DisplayAlert("Attention!", "The sharekey does not match a user!", "Ok");

                                }
                                catch
                                {
                                    await DisplayAlert("Alert!", "Something went wrong!", "Ok");
                                }

                                Application.Current.MainPage = new ColleaguesPage();
                            }
                        });
                    };

                    

                    break;
                case "Enter code manually":
                    var result = await UserDialogs.Instance.PromptAsync(new PromptConfig
                    {
                        Title = "Add Colleague",
                        Message = "Give the share key!",
                        OkText = "Ok",
                        CancelText = "Cancel",
                        MaxLength = 6
                    });

                    var EnteredCode = result.Value;
                    if (result.Ok && EnteredCode.Length == 6)
                    {
                       
                        try
                        {
                            var response = await ImaniManager.FollowUserAsync(GlobalVariables.user.UserId.ToString().ToUpper(), EnteredCode);
                            if (EnteredCode.ToUpper() == GlobalVariables.user.Sharekey)
                                await DisplayAlert("UHMMM", "You can't follow your own account, please find some friends :)", "OK :'(");
                            else if (response != "User niet gevonden")
                                await DisplayAlert("Attention!", "You are now succesfully following: " + response, "Ok");
                            else
                                await DisplayAlert("Attention!", "The sharekey does not match a user!", "Ok");
                            if (response == "Er is iets fout gegaan")
                                await DisplayAlert("Attention!", "There went something wrong!", "Ok");

                        }
                        catch
                        {
                            await DisplayAlert("Alert!", "Something went wrong!", "Ok");
                        }

                        Application.Current.MainPage = new ColleaguesPage();
                    }
                    else
                    {
                        await DisplayAlert("Invalid!", "Please enter a valid share key!", "Ok, sorry");
                    }
                    break;

            }


        }

        private void ToolbarEdit_Activated(object sender, EventArgs e)
        {
            if (ToolbarEdit.Text == "Edit")
            {
                ToolbarEdit.Text = "Done";
                lvwColleaguesDelete.IsVisible = true;
                lvwColleagues.IsVisible = false;
            }
            else if (ToolbarEdit.Text == "Done")
            {
                ToolbarEdit.Text = "Edit";
                lvwColleagues.IsVisible = true;
                lvwColleaguesDelete.IsVisible = false;

            }
        }


        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {

            Image t = (sender) as Image;
            User GeselecteerdeGebruiker = t.BindingContext as User;
                try
                {
                    await ImaniManager.UnfollowUserAsync(GlobalVariables.user.UserId.ToString().ToUpper(), GeselecteerdeGebruiker.UserId.ToString().ToUpper());
                    await DisplayAlert("Succes!", "Colleague successfully unfollowed!", "Ok");
                }
                catch
                {
                    await DisplayAlert("Alert!", "Something went wrong!", "Ok");
                }

            Application.Current.MainPage = new ColleaguesPage();
        }
    }
}
using project2.Model;
using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Diagnostics;
using Plugin.LocalNotifications;

namespace project2.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsDetail : ContentPage
    {
        public SettingsDetail()
        {
            InitializeComponent();
            imgSharekey.Source = ImageSource.FromResource("project2.Assets.ShareKey.png");
            imgPrivacyPolicy.Source = ImageSource.FromResource("project2.Assets.PrivacyPolicy.png");
            imgProfileSettings.Source = ImageSource.FromResource("project2.Assets.myProfileSettings.png");
            imgPushNotifications.Source = ImageSource.FromResource("project2.Assets.PushNotifications.png");
            imgAddASole.Source = ImageSource.FromResource("project2.Assets.bluetooth.png");
            imgReportProblem.Source = ImageSource.FromResource("project2.Assets.ReportProblem.png");
            imgQr.Source = ImageSource.FromResource("project2.Assets.qr-code.png");
            lblUserSharekey.Text = GlobalVariables.user.Sharekey;
            toggleNotifications.IsToggled = GlobalVariables.user.WantsNotifications;
            bluetoothLoop();



            if (GlobalVariables.doorverwijzenNaarProfSettings == true)
            {
                GlobalVariables.doorverwijzenNaarProfSettings = false;
                Navigation.PushAsync(new ProfileSettingsPage());
            }
            if (GlobalVariables.doorverwijzenBluetooth == true)
            {
                GlobalVariables.doorverwijzenBluetooth = false;
                Navigation.PushAsync(new AddASolePage());
            }

        }

        private void MyProfileSettings_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ProfileSettingsPage());
        }

        private void GoToPrivacyPolicy_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new PrivacyPolicy());
        }

        private async void ReportProblem_Tapped(object sender, EventArgs e)
        {
            var result = await UserDialogs.Instance.PromptAsync(new PromptConfig
            {
                Title = "Report a Problem",
                Message = "Add an explanation",
                OkText = "Submit",
                CancelText = "Cancel",
                MaxLength = 255,
                InputType = InputType.Default
            });

            String problem = result.Value;
            Debug.WriteLine(result);
            if (result.Ok && problem.Length < 255)
            {
                //Objec bug klaarmaken om te versturen 
                Bug bug = new Bug();
                bug.UserId = GlobalVariables.user.UserId;
                bug.Date = DateTime.Now;
                bug.BugText = problem;
                //Schrijven naar database en foto in blob zetten
                await ImaniManager.AddBugAsync(bug);
                await DisplayAlert("Succes", "Thank you for your feedback!", "Ok");
            }
            else if (result.Ok == false) { }
            else
            { 
                await DisplayAlert("Error", "There went something wrong", "OK");
            }

        }

        private void imgQr_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new QrCodePage());
        }

        private void AddASole_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AddASolePage());
        }

        private async void bluetoothLoop()
        {
            while (true)
            {
                if (GlobalVariables.connection == true)
                {
                    txtBluetoothState.Text = "Connected";
                    await Task.Delay(500);
                }

                else
                {
                    txtBluetoothState.Text = "Disconnected";
                    await Task.Delay(500);
                }
            }
        }

        private async Task Switch_Toggled(object sender, ToggledEventArgs e)
        {
            if (toggleNotifications.IsToggled == true)
            {
                await DisplayAlert("Notifications enabled", "You will recieve a notification if you haven't stood for almost an hour", "OK");
                await ImaniManager.UpdateNotification(GlobalVariables.user.UserId.ToString(), "true");
                GlobalVariables.user.WantsNotifications = true;
            }
            else if(toggleNotifications.IsToggled == false)
            {
                await DisplayAlert("Notifications disabled", "You will no longer recieve standing notifications", "OK");
                await ImaniManager.UpdateNotification(GlobalVariables.user.UserId.ToString(), "false");
                GlobalVariables.user.WantsNotifications = false;

            }
        }
    }
}
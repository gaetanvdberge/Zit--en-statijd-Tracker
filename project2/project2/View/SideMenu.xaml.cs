using project2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace project2.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SideMenu : ContentPage
    {
        bool isCurrent;
        public SideMenu()
        {
            InitializeComponent();



            ResetData();
            
            if (GlobalVariables.displayedScreen == "Dashboard")
            {
                imgDashboard.Source = ImageSource.FromResource("project2.Assets.dashboard-oranje.png");
                lblDashboard.TextColor = Color.FromHex("#fe8d13");
            }
            else if (GlobalVariables.displayedScreen == "Colleagues")
            {
                imgColleagues.Source = ImageSource.FromResource("project2.Assets.colleagues-oranje.png");
                lblColleagues.TextColor = Color.FromHex("#fe8d13");
            }
            else if (GlobalVariables.displayedScreen == "Settings")
            {
                imgSettings.Source = ImageSource.FromResource("project2.Assets.settings-oranje.png");
                lblSettings.TextColor = Color.FromHex("#fe8d13");
            }

            lblName.Text = GlobalVariables.user.Name;
            lblEmail.Text = GlobalVariables.user.Email;
            imgProfilePic.Source = GlobalVariables.user.UrlProfilePicture;
        }


        private void ResetData()
        {
            imgBg.Source = ImageSource.FromResource("project2.Assets.SideMenuTopImage.png");


            imgDashboard.Source = ImageSource.FromResource("project2.Assets.dashboard-grijs.png");
            imgColleagues.Source = ImageSource.FromResource("project2.Assets.colleagues-grijs.png");
            imgSettings.Source = ImageSource.FromResource("project2.Assets.settings-grijs.png");
            imgSignOut.Source = ImageSource.FromResource("project2.Assets.sign-out-grijs.png");


            lblDashboard.TextColor = Color.FromHex("#676767");
        }

        //Dashboard
        private async void TapDashboard_Tapped(object sender, EventArgs e)
        {
            GlobalVariables.selectedColleague = null;
            GlobalVariables.displayedScreen = "Dashboard";
            Application.Current.MainPage = new Dashboard.DashboardContent();
        }

        //Colleagues
        private async void TapColleagues_Tapped(object sender, EventArgs e)
        {
            GlobalVariables.displayedScreen = "Colleagues";

            Application.Current.MainPage = new ColleaguesPage();

        }

        //Settings
        private async void TapSettings_Tapped(object sender, EventArgs e)
        {
            GlobalVariables.displayedScreen = "Settings";

            Application.Current.MainPage = new SettingsPage();

        }

        //Sign out
        private async void TapSignOut_Tapped(object sender, EventArgs e)
        {
            GlobalVariables.displayedScreen = "Dashboard";

            Application.Current.MainPage = new LoginPage();
        }

        private async void CirlceImage_Tapped(object sender, EventArgs e)
        {
            GlobalVariables.doorverwijzenNaarProfSettings = true;
            GlobalVariables.displayedScreen = "Settings";
            Application.Current.MainPage = new SettingsPage();

        }
    }
}
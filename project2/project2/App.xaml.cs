using Plugin.BLE.Abstractions.Contracts;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using Plugin.LocalNotifications;
using project2.Model;
using project2.View;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace project2
{
    public partial class App : Application
    {
        IService service;
        ICharacteristic characteristic;

        Stopwatch timer = new Stopwatch();
        TimeSpan timerTime = new TimeSpan();
        TimeSpan FiftyFive = new TimeSpan();

        public App()
        {
            InitializeComponent();            

            MainPage = new NavigationPage(CrossConnectivity.Current.IsConnected
               ? (Page)new LoginPage()
               : new NoNetworkpage())

            {
                BarBackgroundColor = Color.FromHex("#29292B"),
                BarTextColor = Color.White,
            };

            loopNotification();
            loop();
    }

        protected override void OnStart()
        {
            base.OnStart();
            //Als er geen netwerk is no network page opstarten.
            CrossConnectivity.Current.ConnectivityChanged += HandleConnectivityChanged;
        }

        void HandleConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.IsConnected)
                if (GlobalVariables.user.Email != "" || GlobalVariables.user.Email != null)
                    this.MainPage = new LoginPage();
                else
                    this.MainPage = new View.Dashboard.DashboardContent();
            else if (!e.IsConnected)
                this.MainPage = new NoNetworkpage();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        private async void loopNotification() //Dit is de code die een melding geeft als je al bijna een uur zit.
        {
            FiftyFive = TimeSpan.FromMinutes(55); //Melding na 55 minuten

            try
            {
                while (true)
                {
                    if (GlobalVariables.user != null)
                    {
                        if (GlobalVariables.user.WantsNotifications == true)
                        {
                            if (GlobalVariables.connection == true) //als er verbinding is met het device, DAN:
                            {
                                timer.Start();
                                await Task.Delay(5000);
                                timerTime = timer.Elapsed;
                                Debug.WriteLine(GlobalVariables.result.ToString());

                                if (GlobalVariables.result == "1.00") //Als men gaat staan, reset de timer
                                {
                                    timer.Reset();
                                    timer.Start();
                                }

                                if ((timerTime > FiftyFive) && ((GlobalVariables.result == "0.00") || (GlobalVariables.result == ""))) //Als je langer dan 55 minuten zit, wordt de melding verstuurd & de timer gereset.
                                {
                                    CrossLocalNotifications.Current.Show("Stand up!", "Your are sitting for almost an hour now. Please stand up!");
                                    timer.Reset();
                                    timer.Start();

                                }
                                Debug.WriteLine(timerTime);
                                Debug.WriteLine(FiftyFive);
                            }
                            else
                            {
                                Debug.WriteLine("No connection with shoe");
                                await Task.Delay(5000);

                            }
                        }
                        else
                        {
                            Debug.WriteLine("Gebruiker wil geen notificaties");
                            await Task.Delay(5000);
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Nog geen gebruiker ingelogd");
                        await Task.Delay(5000);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }


        private async void loop()
        {
            bool previousResult = false;

            try
            {
                while (true)
                {

                    if (GlobalVariables.connection == true)
                    {
                        //Specifieke GuiD van de SERVICE (nodig)
                        service = await GlobalVariables.device.GetServiceAsync(Guid.Parse("6E400001-B5A3-F393-E0A9-E50E24DCCA9E"));

                        //Specifieke GuiD van de characteristic NOTIFY
                        characteristic = await service.GetCharacteristicAsync(Guid.Parse("6E400003-B5A3-F393-E0A9-E50E24DCCA9E"));

                        //De waarden van die characteristic Updaten en vervolgens in een variabele plaatsen genaamd "bytes"
                        characteristic.ValueUpdated += (o, args) =>
                        {
                        //var bytes = args.Characteristic.Value;
                    };

                        await characteristic.StartUpdatesAsync();

                        
                        //de bytes opvangen in een variabele
                        var bytes = characteristic.Value;

                        //Bytes omzetten naar een string
                        GlobalVariables.result = System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length);

                        var date = DateTime.Now;

                        
                        //Boolean voor 1 of 0
                        Boolean isStanding = false;
                        if (GlobalVariables.result == "1.00")
                            isStanding = true;

                        //Als er verandering is in de waarde dan verstuurd hij deze naar de database
                        if (previousResult != isStanding)
                        {
                            Track track = new Track();
                            track.UserId = GlobalVariables.user.UserId;
                            track.Date = DateTime.Now.AddHours(1);
                            track.isStanding = isStanding;
                            track.MacDevice = GlobalVariables.device.Name;

                            await ImaniManager.AddTrack(track);
                            previousResult = isStanding;
                        }
                        await Task.Delay(5000);

                    }

                    else if (GlobalVariables.connection == false)
                    {
                        await Task.Delay(1000);
                    }

                }
            }
            catch (Exception)
            {
                await MainPage.DisplayAlert("Disconnected", "You are no longer connected with your shoe", "Ok");
                CrossLocalNotifications.Current.Show("Connection lost", "You are no longer connected with your shoe");
                GlobalVariables.connection = false;
            }
        }

    }
}

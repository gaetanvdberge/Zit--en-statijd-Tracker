using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCLCrypto;
using PInvoke;
using project2.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Acr.UserDialogs;
using Plugin.BLE.Abstractions.Contracts;
using System.Diagnostics;
using Plugin.BLE;

namespace project2.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        IBluetoothLE ble;
        IAdapter adapter;
        public LoginPage()
        {
            InitializeComponent();

            ble = CrossBluetoothLE.Current;
            adapter = CrossBluetoothLE.Current.Adapter;

            imgBackground.Source =  ImageSource.FromResource("project2.Assets.LoginBg.png");
            imgLogo.Source = ImageSource.FromResource("project2.Assets.LogoWhite.png");
            imgMail.Source = ImageSource.FromResource("project2.Assets.Mail.png");
            imgLock.Source = ImageSource.FromResource("project2.Assets.Lock.png");
        }

        private async void btnLogin_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(entMail.Text) && !string.IsNullOrWhiteSpace(entPassword.Text))
                {
                    var semiTransparentColor = new Color(0, 0, 0, 0.5);
                    frameLayer.BackgroundColor = semiTransparentColor;
                    frameLayer.IsVisible = true;
                    indicatorLoader.IsRunning = true;

                    User user_try = new User();
                    user_try.Email = entMail.Text.Trim();
                    user_try.Password = entPassword.Text;

                    User user = await ImaniManager.LoginUserAsync(user_try);

                    if (user.Name != null)
                    {
                        GlobalVariables.user = user;
                        Application.Current.MainPage = new Dashboard.DashboardContent();

                        try
                        {
                            if (ble.State == BluetoothState.Off)
                            {
                                Debug.WriteLine("Bluetooth uit");
                            }
                            else if (ble.State == BluetoothState.On)
                            {
                                adapter.DeviceDiscovered += (s, a) =>
                                {
                                    if (a.Device.Name == "Imani tracker 0001")
                                    {
                                        GlobalVariables.device = a.Device;
                                        GlobalVariables.deviceAanwezig = true;
                                    }

                                };
                                await adapter.StartScanningForDevicesAsync();

                                if (GlobalVariables.deviceAanwezig == true)
                                {
                                    await adapter.ConnectToDeviceAsync(GlobalVariables.device);
                                    GlobalVariables.connection = true;
                                    Debug.WriteLine("Succes");
                                }
                                else
                                {
                                    Debug.WriteLine("No device");
                                }
                            }
                            else
                            {
                                Debug.WriteLine("ERROR");
                            }


                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }


                        if (GlobalVariables.connection == false)
                        {
                            var reslutaat = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
                            {
                                Title = "Not connected",
                                Message = "You are not connected to your shoe sole!",
                                OkText = "Go to settings",
                                CancelText = "I'm okay",
                            });

                            if (reslutaat)
                            {
                                GlobalVariables.doorverwijzenBluetooth = true;
                                GlobalVariables.displayedScreen = "Settings";
                                Application.Current.MainPage = new SettingsPage();
                            }
                        }
                    }
                    else
                    {
                        await DisplayAlert("Attention", "Email/wachtwoord incorrect", "OK");
                        frameLayer.IsVisible = false;
                        indicatorLoader.IsRunning = false;
                    }
                }
                else
                {
                    await DisplayAlert("Attention!", "Your input was incorrect!", "OK");
                    frameLayer.IsVisible = false;
                    indicatorLoader.IsRunning = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void lblCreateAcc_Tapped(object sender, EventArgs e)
        {
           Application.Current.MainPage = new RegisterPage();
        }
    }
}
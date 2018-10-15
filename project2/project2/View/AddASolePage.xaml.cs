using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;
using project2.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace project2.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddASolePage : ContentPage
    {
        IBluetoothLE ble;
        IAdapter adapter;

        public AddASolePage()
        {
            InitializeComponent();

            ble = CrossBluetoothLE.Current;
            adapter = CrossBluetoothLE.Current.Adapter;
            deviceList = new ObservableCollection<IDevice>();
            lv.ItemsSource = deviceList;

            scanloop();
            lv.ItemSelected += lv_ItemSelected;

            var state = ble.State;
            imgBluetooth.Source = ImageSource.FromResource("project2.Assets.bluetooth.png");
            imgSoleConnection.Source = ImageSource.FromResource("project2.Assets.link.png");

            liveStatusLoop();
            bluetoothLoop();



        }


        ObservableCollection<IDevice> deviceList;

        public async void scanloop()
        {
            deviceList.Clear();
            try
            {
                if (ble.State == BluetoothState.Off)
                {
                    await DisplayAlert("Bluetooth is turned off", "Please turn on your bluetooth to be able to connect to your device", "OK");
                }
                else
                {
                    adapter.DeviceDiscovered += (s, a) =>
                    {
                        if (a.Device.Name == "Imani tracker 0001")
                        {
                            deviceList.Add(a.Device);
                        }

                    };

                    await adapter.StartScanningForDevicesAsync();
                    Debug.WriteLine(deviceList);
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async void lv_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            GlobalVariables.device = lv.SelectedItem as IDevice;
            if (GlobalVariables.device != null)
            {
                try
                {
                    //Connecteren met de device
                    await adapter.ConnectToDeviceAsync(GlobalVariables.device);

                    GlobalVariables.connection = true;
                    lblSoleStatus.Text = "Connected";

                    await this.DisplayAlert("Succesfully connected", "You are connected!", "OK");
                    //lv.SelectedItem = null;

                }
                catch (DeviceConnectionException)
                {
                    //    //Could not connect to device
                    GlobalVariables.connection = false;
                    await this.DisplayAlert("Bluetooth Error", "Failed to connect", "OK");
                    //lv.SelectedItem = null;
                }
                catch (Exception)
                {
                    GlobalVariables.connection = false;
                    await this.DisplayAlert("Error", "Something went wrong", "OK");
                    //lv.SelectedItem = null;
                }
            }

        }

        public static string ByteArrayToHexString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        private async void liveStatusLoop()
        {
            while (true)
            {
                if ((GlobalVariables.result != null) && (GlobalVariables.connection == true))
                {
                    if (GlobalVariables.result == "1.00")
                    {
                        lblPositionStatus.Text = "STANDING";

                    }
                    else
                    {
                        lblPositionStatus.Text = "SITTING";
                    }
                    await Task.Delay(500);

                }
                else
                {
                    lblPositionStatus.Text = "---";
                    await Task.Delay(500);
                }
            }
        }

        private async void bluetoothLoop()
        {
            while (true)
            {
                if (ble.State == BluetoothState.On)
                {
                    lblBluetoothStatus.Text = "Enabled";
                    await Task.Delay(2000);
                }
                else if (ble.State == BluetoothState.Off)
                {
                    lblBluetoothStatus.Text = "Disabled";
                    await Task.Delay(2000);
                }

                if (GlobalVariables.connection == true)
                {
                    lblSoleStatus.Text = "Connected";
                    await Task.Delay(2000);
                }
                else
                {
                    lblSoleStatus.Text = "Disconnected";
                    await Task.Delay(2000);
                }
            }

        }
    }
}
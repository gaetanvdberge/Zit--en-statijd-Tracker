using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project2.Model
{
    public static class GlobalVariables
    {
        public static User user;
        public static User selectedColleague;
        public static string displayedScreen = "Dashboard";
        public static bool doorverwijzenNaarProfSettings = false;
        public static bool doorverwijzenBluetooth = false;
        public static bool deviceAanwezig;

        public static IDevice device;
        public static Boolean connection = false;


        public static string result;

    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;

namespace project2.Model
{
    public class User
    {
        // GELIEVE HIER GEEN AANPASSING IN TE MAKEN
        [JsonProperty("userid")]
        public Guid UserId { get; set; }
        [JsonProperty("name")]
        public String Name { get; set; }
        [JsonProperty("email")]
        public String Email { get; set; }
        [JsonProperty("password")]
        public String Password { get; set; }
        [JsonProperty("sharekey")]
        public String Sharekey { get; set; }
        [JsonProperty("wantsnotifications")]
        public Boolean WantsNotifications { get; set; }

        private string _urlProfilePicture;

        public string UrlProfilePicture
        {
            get { return "https://imanistorage.blob.core.windows.net/profilepictures/" + UserId + ".jpg"; }
            set { _urlProfilePicture = value; }
        }

    }
}

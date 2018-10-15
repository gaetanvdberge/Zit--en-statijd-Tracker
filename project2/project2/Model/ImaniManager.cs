using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace project2.Model
{
    public class ImaniManager
    {
        //vergeet niet + voor elke specifieke api
        public static string BASEURL = "https://imaniaf.azurewebsites.net/api/";

        #region REGISTREREN VAN EEN USER
        public static async Task<User> AddUserAsync(User reg_user)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = BASEURL + "registeruser";
                try
                {
                    string json = JsonConvert.SerializeObject(reg_user);
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(url, content);
                    string result = await response.Content.ReadAsStringAsync();
                    User user = JsonConvert.DeserializeObject<User>(result);
                    if (response.IsSuccessStatusCode)
                    {
                        if (user.Email != null)
                            return user;
                        else
                        {
                            user.Email = "NOK";
                            return user;
                        }
                    }
                    else
                    {
                        user.Email = "NOK";
                        return user;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        #endregion

        #region LOGIN VAN EEN USER
        public static async Task<User> LoginUserAsync(User login_user)
        {

            using (HttpClient client = new HttpClient())
            {
                string url = String.Format(BASEURL + "loginuser");

                try
                {
                    string json = JsonConvert.SerializeObject(login_user);
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(url, content);
                    string result = await response.Content.ReadAsStringAsync();
                    User user = JsonConvert.DeserializeObject<User>(result);
                    if (response.IsSuccessStatusCode)
                    {
                        return user;
                    }
                    else
                    {
                        return user;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        #endregion

        #region ADD FOLLOWER
        public static async Task<String> FollowUserAsync(String userID, String sharekey)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = String.Format(BASEURL + "addfollower/{0}/{1}", userID, sharekey);
                try
                {
                    var response = await client.PostAsync(url, null);
                    string result = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                       if(result == "\"User niet gevonden\"")
                       {
                            return "User niet gevonden";
                       }
                       else
                       {
                            User user = JsonConvert.DeserializeObject<User>(result);
                            return user.Name;
                        }
                    }
                    else
                    {
                        return "Er is iets fout gegaan";
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        #endregion

        #region UNFOLOW USER
        public static async Task UnfollowUserAsync(String userID, String ColleagueId)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = String.Format(BASEURL + "deletefollower/{0}/{1}", userID, ColleagueId);
                try
                {
                    var response = await client.DeleteAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        Debug.WriteLine("succes");
                    }
                    else
                    {
                        Debug.WriteLine("failed");
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        #endregion

        #region GET FOLLOWING USERS
        public static async Task<List<User>> GetFollowersAsync(String userID)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = BASEURL + "getfollowers/" + userID;
                try
                {
                    String result = await client.GetStringAsync(url);
                    List<User> users = JsonConvert.DeserializeObject<List<User>>(result);
                    return users;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        #endregion

        #region GET DAY STANDING
        public static async Task<List<TimeStandingDay>> GetTimeStandingDayAsync(String userID, DateTime first_date,DateTime last_date )
        {
            using (HttpClient client = new HttpClient())
            {
                string url = BASEURL + "gettimestandingday/" + userID + "/" + first_date.ToString(("yyyy-MM-dd HH:mm:ss")).Replace("/", "-") + "/" + last_date.ToString("yyyy-MM-dd HH:mm:ss").Replace("/", "-");
                try
                {
                    String result = await client.GetStringAsync(url);
                    List<TimeStandingDay> list_timestanding = JsonConvert.DeserializeObject<List<TimeStandingDay>>(result);
                    return list_timestanding;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        #endregion

        #region Add Bug
        public static async Task AddBugAsync(Bug bug)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = BASEURL + "addbug";

                try
                {
                    string json = JsonConvert.SerializeObject(bug);
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(url, content);
                    if (response.IsSuccessStatusCode)
                    {
                        Debug.WriteLine("Succes vol bug gerapporteerd!");
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        #endregion

        #region ADD TRACK
        public static async Task AddTrack(Track track)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = BASEURL + "addtrack";

                try
                {
                    string json = JsonConvert.SerializeObject(track);
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(url, content);
                    if (response.IsSuccessStatusCode)
                    {
                        Debug.WriteLine("Succes vol track aangevuld!");
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        #endregion

        #region UPDATE PROFILE
        public static async Task UpdateUserAsync(User user)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = BASEURL + "updateuser";

                try
                {
                    string json = JsonConvert.SerializeObject(user);
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PutAsync(url, content);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        #endregion

        #region UPDATE NOTIFICATIONS
        public static async Task UpdateNotification(string userID,string wantsNotifications)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = String.Format(BASEURL + "updatenotifications//{0}/{1}", userID, wantsNotifications);

                try
                { 
                    var response = await client.PutAsync(url, null);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        #endregion

        #region DELETE USER
        public static async Task DeleteUserAsync(String userID)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = String.Format(BASEURL + "deleteeuser/{0}", userID);
                try
                {
                    var response = await client.DeleteAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        Debug.WriteLine("succes");
                    }
                    else
                    {
                        Debug.WriteLine("failed");
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        #endregion

        #region CHECK PASSWORD
        public static async Task<String> CHeckPswAsync(User try_user)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = BASEURL + "checkpsw";

                try
                {
                    string json = JsonConvert.SerializeObject(try_user);
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(url, content);
                    string result = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        if (result == "\"NOK\"")
                        {
                            return "NOK";
                        }
                        else
                        {
                            return "OK";
                        }
                    }
                    else
                    {
                        return "Er is iets fout gegaan";
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        #endregion
    }
}
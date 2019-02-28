using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace WGO.JSON
{
    static public class JSONBase
    {
        #region " Properties "
        /// <summary>
        /// 
        /// </summary>
        public static List<string> Errors { get; set; } = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        public static bool CharacterNotFound { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        public static string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public static DateTime Expiration { get; set; }
        #endregion

        /// <summary>
        /// Example: https://us.api.blizzard.com/wow/character/Thrall/Purdee?fields=items%2Cprofessions%2Ctalents&locale=en_US&access_token=jwhk8mw8kfpcng2y86as895gufku9kfa
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <returns></returns>
        static public JSONCharacter GetCharacterJSON(string characterName, string realm)
        {
            // Reset Character Not Found First...
            CharacterNotFound = false;

            JSONCharacter result = null;
            string requestUrl = $"{System.Configuration.ConfigurationManager.AppSettings["URLWowAPI"].ToString()}character/{realm}/{characterName}?fields=items,professions,talents&access_token={AccessToken}";            
            string responseData = GetJSONData(requestUrl);

            // Convert the data to the proper object
            if (!string.IsNullOrEmpty(responseData))
            {
                result = JsonConvert.DeserializeObject<JSONCharacter>(responseData);
            }

            return result;
        }

        /// <summary>
        /// Example: https://us.api.blizzard.com/wow/character/Thrall/Purdee?fields=audit&locale=en_US&access_token=jwhk8mw8kfpcng2y86as895gufku9kfa
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <returns></returns>
        static public JSONCharacterAudit GetCharacterAuditJSON(string characterName, string realm)
        {
            // Reset Character Not Found First...
            CharacterNotFound = false;

            JSONCharacterAudit result = null;
            string requestUrl = $"{System.Configuration.ConfigurationManager.AppSettings["URLWowAPI"].ToString()}character/{realm}/{characterName}?fields=audit&access_token={AccessToken}";
            string responseData = GetJSONData(requestUrl);

            // Convert the data to the proper object
            if (!string.IsNullOrEmpty(responseData))
            {
                result = JsonConvert.DeserializeObject<JSONCharacterAudit>(responseData);
            }

            return result;
        }

        /// <summary>
        /// Example: https://us.api.blizzard.com/wow/guild/Thrall/Secondnorth?fields=members&locale=en_US&access_token=jwhk8mw8kfpcng2y86as895gufku9kfa
        /// </summary>
        /// <param name="guild"></param>
        /// /// <param name="realm"></param>
        /// <returns></returns>
        static public JSONGuild GetGuildJSON(string guild, string realm)
        {
            JSONGuild result = null;
            string requestUrl = $"{System.Configuration.ConfigurationManager.AppSettings["URLWowAPI"].ToString()}guild/{realm}/{guild}?fields=members&locale=en_US&access_token={AccessToken}";
            string responseData = GetJSONData(requestUrl);

            // Convert the data to the proper object
            if (!string.IsNullOrEmpty(responseData))
            {
                result = JsonConvert.DeserializeObject<JSONGuild>(responseData);
            }

            return result;
        }

        /// <summary>
        /// Example: https://us.api.blizzard.com/wow/guild/Thrall/Secondnorth?fields=news&locale=en_US&access_token=jwhk8mw8kfpcng2y86as895gufku9kfa
        /// </summary>
        /// <param name="guild"></param>
        /// /// <param name="realm"></param>
        /// <returns></returns>
        static public JSONGuildNews GetGuildNewsJSON(string guild, string realm)
        {
            JSONGuildNews result = null;
            string requestUrl = $"{System.Configuration.ConfigurationManager.AppSettings["URLWowAPI"].ToString()}guild/{realm}/{guild}?fields=news&locale=en_US&access_token={AccessToken}";
            string responseData = GetJSONData(requestUrl);

            // Convert the data to the proper object
            if (!string.IsNullOrEmpty(responseData))
            {
                result = JsonConvert.DeserializeObject<JSONGuildNews>(responseData);
            }

            return result;
        }

        /// <summary>
        /// Requests data from a RESTful site.  Response is in string format and will need to be converted to class format.
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <returns></returns>
        static private string GetJSONData(string requestUrl)
        {
            string result = null;

            try
            {
                // First check the expiration in case we need to get a new access token
                //   A new Access Token is needed every ~24 hours
                JSONBase.CheckExpiration();

                // Now setup the Security Protocol of the Web Request
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;

                // Clear all errors before we request data
                Errors.Clear();

                // Request the Data
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception($"Server error (HTTP {response.StatusCode}: {response.StatusDescription}).");
                    }

                    Stream receiveStream = response.GetResponseStream();
                    Encoding encode = System.Text.Encoding.GetEncoding("utf-8");

                    // Pipes the stream to a higher level stream reader with the required encoding format. 
                    using (StreamReader readStream = new StreamReader(receiveStream, encode))
                    {
                        Char[] read = new Char[256];

                        // Reads 256 characters at a time.    
                        int count = readStream.Read(read, 0, 256);

                        while (count > 0)
                        {
                            // Dumps the 256 characters on a string and displays the string to the console.
                            String str = new String(read, 0, count);
                            result += str;
                            count = readStream.Read(read, 0, 256);
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                string errorCheck = string.Empty;

                if (ex.Response != null)
                {
                    using (WebResponse response = ex.Response)
                    {
                        HttpWebResponse httpResponse = (HttpWebResponse)response;
                        Console.WriteLine($"Error code: {httpResponse.StatusCode}");
                        Errors.Add($"Error code: {httpResponse.StatusCode}");

                        using (Stream data = response.GetResponseStream())

                        using (var reader = new StreamReader(data))
                        {
                            errorCheck = reader.ReadToEnd();
                            Console.WriteLine(errorCheck);
                        }
                    }
                }

                // Web Exception Errors were found!
                if (errorCheck.StartsWith(@"{""status"":""nok"", ""reason"": "))
                {
                    Errors.Add($"Character could not be found on the Battle.net Site any more... ignoring.");
                    CharacterNotFound = true;
                }
                else if (ex.HResult == -2146233079)
                {
                    // Battle.Net cannot be reached
                    Errors.Add($"Battle.Net cannot be reached: {ex.Message}");
                }
                else
                {
                    // This is a 404 error, usually because a character hasn't been logged into for a while
                    //   collect this error for use in the function that is calling this function
                    Errors.Add($"(404) {ex.Message} in Parse() in JSONBase.cs");
                }
            }
            catch (Exception ex)
            {
                Errors.Add($"ERROR: {ex.Message} in GetCharacterJSONData() in JSONBase.cs");
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        static private void CheckExpiration()
        {
            // If Access Token is blank, the first time an API call is made or token has expired
            if (string.IsNullOrWhiteSpace(JSONBase.AccessToken) || TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Central Standard Time") > JSONBase.Expiration)
            {
                // Get a new Token
                bool goOn = JSONBase.GetAccessToken();
            }
        }

        /// <summary>
        /// Use this URL to get an Access Token:
        /// https://us.battle.net/oauth/token?grant_type=client_credentials&client_id={clientID}&client_secret={clientSecret}
        /// Can use this URL to see how long a Access_Token has before expiration:
        /// https://us.battle.net/oauth/check_token?token={accessToken}
        /// </summary>
        /// <returns></returns>
        static private bool GetAccessToken()
        {
            bool result = false;

            try
            {
                string clientID = System.Configuration.ConfigurationManager.AppSettings["ClientID"].ToString();
                string clientSecret = System.Configuration.ConfigurationManager.AppSettings["ClientSecret"].ToString();
                string auth = clientID + ":" + clientSecret;

                #region " Previous Examples "
                /* Example #1
                var request = new HttpRequestMessage(HttpMethod.Post, "http://server.com/token");
                request.Content = new FormUrlEncodedContent(new Dictionary<string, string> {
                        { "client_id", "your client_id" },
                        { "client_secret", "your client_secret" },
                        { "grant_type", "client_credentials" }
                    });

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var payload = JObject.Parse(await response.Content.ReadAsStringAsync());
                var token = payload.Value<string>("access_token");
                */


                /* Example #2
                WebRequest myReq = WebRequest.Create(url);
                CredentialCache mycache = new CredentialCache();
                mycache.Add(new Uri(url), "client_credentials", new NetworkCredential(clientID, clientSecret));
                myReq.Credentials = mycache;
                myReq.Headers.Add("Authorization", "client_credentials " + Convert.ToBase64String(new ASCIIEncoding().GetBytes(auth)));

                WebResponse wr = myReq.GetResponse();
                Stream receiveStream = wr.GetResponseStream();
                StreamReader reader = new StreamReader(receiveStream, Encoding.UTF8);
                string content = reader.ReadToEnd();
                */

                /* Example #3 
                string fullurl = $"https://us.battle.net/oauth/token?grant_type=authorization_code&code={clientID}&scope=wow.profile&redirect_uri=http%3A%2F%2Flocalhost%3A11119%2Flogin";

                ASCIIEncoding encoding = new ASCIIEncoding();
                string postData = $"client_id={clientID}&client_secret={clientSecret}";
                byte[] data = Encoding.GetEncoding("UTF-8").GetBytes(postData);
                WebRequest request = WebRequest.Create(url);
                request.Method = "POST";
                request.Credentials = new NetworkCredential(clientID, clientSecret);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                Stream stream = request.GetRequestStream();
                stream.Write(data, 0, data.Length);
                stream.Close();
                string res = "";

                WebResponse response = request.GetResponse();
                using (stream = response.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        res = sr.ReadToEnd();
                    }
                }
                stream.Close();
                */
                #endregion

                /* Example #4 */
                // Now need to use OAuth2 to authenticate before we can get JSON data
                string fullurl = $"https://us.battle.net/oauth/token?grant_type=client_credentials&client_id={clientID}&client_secret={clientSecret}";
                ASCIIEncoding encoding = new ASCIIEncoding();
                string postData = $"client_id={clientID}&client_secret={clientSecret}";
                byte[] data = Encoding.GetEncoding("UTF-8").GetBytes(postData);
                WebRequest request = WebRequest.Create(fullurl);
                request.Method = "POST";
                request.Credentials = new NetworkCredential(clientID, clientSecret);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                Stream stream = request.GetRequestStream();
                stream.Write(data, 0, data.Length);
                stream.Close();
                string res = "";

                WebResponse response = request.GetResponse();
                using (stream = response.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        res = sr.ReadToEnd();
                    }
                }

                stream.Close();

                string accessToken = res.Substring(17, res.IndexOf("\"", 17) - 17);
                JSONBase.AccessToken = accessToken;
                JSONBase.Expiration = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Central Standard Time").AddDays(1);
            }
            catch (WebException wex)
            {
                Errors.Add($"WEB ERROR: {wex.Message} in GetAccessToken() in JSONBase.cs");
            }
            catch (Exception ex)
            {
                Errors.Add($"ERROR: {ex.Message} in GetAccessToken() in JSONBase.cs");
            }

            return result;
        }
    }
}
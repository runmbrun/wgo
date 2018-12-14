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
        #endregion

        /// <summary>
        /// Example: https://us.api.battle.net/wow/character/Thrall/Purdee?fields=items%2Cprofessions%2Ctalents&locale=en_US&apikey=jwhk8mw8kfpcng2y86as895gufku9kfa
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <returns></returns>
        static public JSONCharacter GetCharacterJSON(string characterName, string realm)
        {
            // Reset Character Not Found First...
            CharacterNotFound = false;

            JSONCharacter result = null;
            string requestUrl = $"{System.Configuration.ConfigurationManager.AppSettings["URLWowAPI"].ToString()}character/{realm}/{characterName}?fields=items,professions,talents&apikey={System.Configuration.ConfigurationManager.AppSettings["APIKey"].ToString()}";            
            string responseData = GetJSONData(requestUrl);

            // Convert the data to the proper object
            if (!string.IsNullOrEmpty(responseData))
            {
                result = JsonConvert.DeserializeObject<JSONCharacter>(responseData);
            }

            return result;
        }

        /// <summary>
        /// Example: https://us.api.battle.net/wow/character/Thrall/Purdee?fields=audit&locale=en_US&apikey=jwhk8mw8kfpcng2y86as895gufku9kfa
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <returns></returns>
        static public JSONCharacterAudit GetCharacterAuditJSON(string characterName, string realm)
        {
            // Reset Character Not Found First...
            CharacterNotFound = false;

            JSONCharacterAudit result = null;
            string requestUrl = $"{System.Configuration.ConfigurationManager.AppSettings["URLWowAPI"].ToString()}character/{realm}/{characterName}?fields=audit&apikey={System.Configuration.ConfigurationManager.AppSettings["APIKey"].ToString()}";
            string responseData = GetJSONData(requestUrl);

            // Convert the data to the proper object
            if (!string.IsNullOrEmpty(responseData))
            {
                result = JsonConvert.DeserializeObject<JSONCharacterAudit>(responseData);
            }

            return result;
        }

        /// <summary>
        /// Example: https://us.api.battle.net/wow/guild/Thrall/Secondnorth?fields=members&locale=en_US&apikey=jwhk8mw8kfpcng2y86as895gufku9kfa
        /// </summary>
        /// <param name="guild"></param>
        /// /// <param name="realm"></param>
        /// <returns></returns>
        static public JSONGuild GetGuildJSON(string guild, string realm)
        {
            JSONGuild result = null;
            string requestUrl = $"{System.Configuration.ConfigurationManager.AppSettings["URLWowAPI"].ToString()}guild/{realm}/{guild}?fields=members&locale=en_US&apikey={System.Configuration.ConfigurationManager.AppSettings["APIKey"].ToString()}";
            string responseData = GetJSONData(requestUrl);

            // Convert the data to the proper object
            if (!string.IsNullOrEmpty(responseData))
            {
                result = JsonConvert.DeserializeObject<JSONGuild>(responseData);
            }

            return result;
        }

        /// <summary>
        /// Example: https://us.api.battle.net/wow/guild/Thrall/Secondnorth?fields=news&locale=en_US&apikey=jwhk8mw8kfpcng2y86as895gufku9kfa
        /// </summary>
        /// <param name="guild"></param>
        /// /// <param name="realm"></param>
        /// <returns></returns>
        static public JSONGuildNews GetGuildNewsJSON(string guild, string realm)
        {
            JSONGuildNews result = null;
            string requestUrl = $"{System.Configuration.ConfigurationManager.AppSettings["URLWowAPI"].ToString()}guild/{realm}/{guild}?fields=news&locale=en_US&apikey={System.Configuration.ConfigurationManager.AppSettings["APIKey"].ToString()}";
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
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;

                Errors.Clear();

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
                    Errors.Add($"(404) {ex.Message} in Parse() in GetWebSiteData.cs");
                }
            }
            catch (Exception ex)
            {
                Errors.Add($"ERROR: {ex.Message} in GetCharacterJSONData() in GetWebJSONData.cs");
            }

            return result;
        }
    }
}
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WGO.JSON;
using WGO.Models;

namespace WGO.Controllers
{
    public class WGOController : Controller
    {
        // GET: WGO
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Search(SearchViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {
                JSONCharacter character = this.GetCharacterJSONData(model.Character, model.Realm);

                if (character == null)
                {
                    ModelState.AddModelError("Character", "Character not found.");
                    return View(model);
                }

                //return Display(character);
                return View("Display", character);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public ActionResult Display(JSONCharacter model, string returnUrl)
        {
            return View(model);
        }

        /// <summary>
        /// Example: https://us.api.battle.net/wow/character/Thrall/Purdee?fields=items%2Cprofessions%2Ctalents&locale=en_US&apikey=jwhk8mw8kfpcng2y86as895gufku9kfa                
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <returns></returns>
        public JSONCharacter GetCharacterJSONData(string characterName, string realm)
        {
            JSONCharacter result = null;
            string requestUrl = $"{Session["URLWowAPI"].ToString()}character/{realm}/{characterName}?fields=items,professions,talents&apikey={Session["APIKey"]}";
            string responseData = GetJSONData(requestUrl);

            // Convert the data to the proper object
            if (!string.IsNullOrEmpty(responseData))
            {
                result = JsonConvert.DeserializeObject<JSONCharacter>(responseData);
            }

            return result;
        }

        /// <summary>
        /// Requests data from a RESTful site.  Response is in string format and will need to be converted to class format.
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <returns></returns>
        private string GetJSONData(string requestUrl)
        {
            string result = null;

            if (Convert.ToBoolean(Session["WebSiteOnline"]))
            {
                try
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;

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
                            Console.WriteLine("Error code: {0}", httpResponse.StatusCode);

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
                        //Logging.Debug($"Character could not be found on the Battle.net Site any more... ignoring.");
                    }
                    else if (ex.HResult == -2146233079)
                    {
                        // Battle.Net cannot be reached
                        //Logging.DisplayError($"Battle.Net cannot be reached: {ex.Message}");
                        Session["WebSiteOnline"] = false;
                    }
                    else
                    {
                        // This is a 404 error, usually because a character hasn't been logged into for a while
                        //   collect this error for use in the function that is calling this function
                        //Logging.Debug($"(404) {ex.Message} in Parse() in GetWebSiteData.cs");
                    }
                }
                catch (Exception ex)
                {
                    //Logging.Log($"ERROR: {ex.Message} in GetCharacterJSONData() in GetWebJSONData.cs");
                    //Logging.DisplayError($"ERROR: {ex.Message} in GetCharacterJSONData() in GetWebJSONData.cs");
                }
            }

            return result;
        }
    }
}
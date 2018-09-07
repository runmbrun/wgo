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
        private WGODBContext db = new WGODBContext();

        // GET: WGO
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Search()
        {
            // Reset Fields
            ViewBag.CharacterName = string.Empty;
            ViewBag.CharacterRealm = string.Empty;

            return View();
        }

        [HttpPost]
        public ActionResult Search(SearchViewModel model, string returnUrl)
        {
            // Reset Fields
            ViewBag.CharacterName = string.Empty;
            ViewBag.CharacterRealm = string.Empty;

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {
                if (this.RetrieveCharacter(model.Character, model.Realm, true) == 0)
                {
                    return View(model);
                }

                // Now display it!
                return View();
                
            }
        }

        /// <summary>
        /// todo: not used anymore
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public ActionResult Display(IEnumerable<Character> model, string returnUrl)
        {
            return View(model);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reload"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Guild(string reload)
        {
            return View();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult RaidFunctions(string function, string name, string realm)
        {
            if (!string.IsNullOrWhiteSpace(function) && !string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(realm))
            {
                if (function == "Rescan")
                {
                    if (name == "@All")
                    {
                        // Rescan all the characters
                        List<Character> rescan = (from m in db.Characters where m.Roster == 2 || m.Roster == 3 select m).ToList();

                        foreach (Character c in rescan)
                        {
                            this.RetrieveCharacter(c.Name, c.Realm, false);
                        }
                    }
                    else
                    {
                        // Reload just 1 character
                        this.RetrieveCharacter(name, realm, false);
                    }
                }
                else if (function == "Delete")
                {
                    // Delete a user from the Raid Roster
                    var dbRaiders = from m in db.Characters where (m.Roster == 2 || m.Roster == 3) && m.Name == name && m.Realm == realm select m;
                    foreach (Character c in dbRaiders)
                    {
                        if (c.Roster == JSONBase.GetAllRoster())
                        {
                            c.Roster = JSONBase.GetGuildRoster();
                        }
                        else if (c.Roster == JSONBase.GetRaidRoster())
                        {
                            db.Characters.Remove(c);
                        }
                    }

                    db.SaveChanges();
                }
            }

            return RedirectToAction("Raid");
        }

        [HttpGet]
        public ActionResult Raid()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Raid(SearchViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {
                if (this.RetrieveCharacter(model.Character, model.Realm, false) == 0)
                {
                    return View(model);
                }
            }

            // Now display it!
            return View();
        }

        /// <summary>
        /// Result = 0 -> Error
        /// Result = 1 -> Success
        /// Result = 2 -> Character not found, but no errors.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="realm"></param>
        /// <returns></returns>
        private int RetrieveCharacter(string character, string realm, bool isGuild)
        {
            int result = 0;
            JSONCharacter charFromWeb = JSONBase.GetCharacterJSON(character, realm);

            ViewBag.CharacterName = character;
            ViewBag.CharacterRealm = realm;

            if (charFromWeb == null)
            {
                if (JSONBase.CharacterNotFound)
                {
                    ModelState.AddModelError(string.Empty, $"Character {character} from {realm} not found.");
                    result = 2;
                }
                else
                {
                    foreach (string e in JSONBase.Errors)
                    {
                        ModelState.AddModelError(string.Empty, $"Error: {e}");
                    }
                }
            }
            else
            {
                string spec = string.Empty;
                string role = string.Empty;

                if (charFromWeb.Talents != null && charFromWeb.Talents.Count > 0)
                {
                    spec = charFromWeb.Talents[0].Spec.Name;
                    role = charFromWeb.Talents[0].Spec.Role;
                }

                var dbChars = from m in db.Characters select m;
                var search = dbChars.Where(s => s.Name == charFromWeb.Name && s.Realm == charFromWeb.Realm && s.Role == role);

                if (search.Count() > 0)
                {
                    // Already exists in the DB - update it
                    foreach (Character searchChar in search)
                    {
                        // Check for differences
                        if (searchChar.Level != charFromWeb.Level)
                        {
                            searchChar.Modified_Level = DateTime.Now;
                        }

                        if (searchChar.AchievementPoints != charFromWeb.AchievementPoints)
                        {
                            searchChar.Modified_AchievementPoints = DateTime.Now;
                        }

                        if (searchChar.Equipped_iLevel != charFromWeb.Items.AverageItemLevelEquipped)
                        {
                            searchChar.Modified_Equipped_iLevel = DateTime.Now;
                        }

                        if (searchChar.Max_iLevel != charFromWeb.Items.AverageItemLevel)
                        {
                            searchChar.Modified_Max_iLevel = DateTime.Now;
                        }

                        // Update Character
                        searchChar.Level = charFromWeb.Level;
                        searchChar.Class = JSONBase.ConvertClass(charFromWeb.Class);
                        searchChar.Race = JSONBase.ConvertRace(charFromWeb.Race);
                        searchChar.AchievementPoints = charFromWeb.AchievementPoints;
                        searchChar.Max_iLevel = charFromWeb.Items.AverageItemLevel;
                        searchChar.Equipped_iLevel = charFromWeb.Items.AverageItemLevelEquipped;
                        searchChar.LastUpdated = DateTime.Now;
                        searchChar.Spec = spec;
                        searchChar.Role = role;

                        if (isGuild)
                        {
                            if (searchChar.Roster == JSONBase.GetRaidRoster())
                            {
                                searchChar.Roster = JSONBase.GetAllRoster();
                            }
                        }
                        else
                        {
                            if (searchChar.Roster == JSONBase.GetGuildRoster())
                            {
                                searchChar.Roster = JSONBase.GetAllRoster();
                            }
                        }
                    }

                    try
                    {
                        db.SaveChanges();
                        result = 1;
                    }
                    catch (Exception ex)
                    {
                        // no op
                        ModelState.AddModelError(string.Empty, $"Error saving character: {ex.Message}");
                    }
                }
                else
                {
                    // Now insert the data inot the database
                    Character charToDB = new Character();
                    charToDB.Name = charFromWeb.Name;
                    charToDB.Level = charFromWeb.Level;
                    charToDB.Class = JSONBase.ConvertClass(charFromWeb.Class);
                    charToDB.Race = JSONBase.ConvertRace(charFromWeb.Race);
                    charToDB.AchievementPoints = charFromWeb.AchievementPoints;
                    charToDB.Max_iLevel = charFromWeb.Items.AverageItemLevel;
                    charToDB.Equipped_iLevel = charFromWeb.Items.AverageItemLevelEquipped;
                    charToDB.LastUpdated = DateTime.Now;
                    charToDB.Realm = charFromWeb.Realm;
                    charToDB.Spec = spec;
                    charToDB.Modified_AchievementPoints = DateTime.Now;
                    charToDB.Modified_Equipped_iLevel = DateTime.Now;
                    charToDB.Modified_Max_iLevel = DateTime.Now;
                    charToDB.Modified_Level = DateTime.Now;

                    if (isGuild)
                    {
                        charToDB.Roster = JSONBase.GetGuildRoster();
                    }
                    else
                    {
                        charToDB.Roster = JSONBase.GetRaidRoster();
                    }


                    // does it exist in the db?  then add it!
                    db.Characters.Add(charToDB);

                    try
                    {
                        db.SaveChanges();
                        result = 1;
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(string.Empty, $"Error saving character: {ex.Message}");
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Character(string name, string realm)
        {
            // Calculate the UrlReferrer link, for a return Url
            if (System.Web.HttpContext.Current.Request.UrlReferrer != null)
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    ViewBag.Name = name;
                }

                if (System.Web.HttpContext.Current.Request.UrlReferrer.ToString().Contains("Guild"))
                {
                    ViewBag.ReturnUrl = "/WGO/Guild";
                }
                else if (System.Web.HttpContext.Current.Request.UrlReferrer.ToString().Contains("Raid"))
                {
                    ViewBag.ReturnUrl = "/WGO/Raid";
                }
            }

            // Get the Character info
            CharacterViewModel model = new CharacterViewModel();
            model.Character = null;
            var chars = db.Characters.Where(s => s.Name == name && s.Realm == realm);

            if (chars.Any())
            {
                // Store the Character data
                model.Character = chars.First();

                // Now get the Audit Data
                JSONCharacterAudit audit = JSONBase.GetCharacterAuditJSON(model.Character.Name, model.Character.Realm);
                if (audit != null)
                {
                    model.Audit = audit;
                }
            }

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult DatabaseTest()
        {
            try
            {
                var dbChars = from m in db.Characters select m;
                var guildCount = dbChars.Where(s => s.Roster == JSONBase.GetGuildRoster() || s.Roster == JSONBase.GetAllRoster());
                var raidCount = dbChars.Where(s => s.Roster == JSONBase.GetRaidRoster() || s.Roster == JSONBase.GetAllRoster());

                ViewBag.GuildCount = 0;
                ViewBag.RaidCount = 0;

                if (guildCount.Count() > 0)
                {
                    ViewBag.GuildCount = guildCount.Count();
                }

                if (raidCount.Count() > 0)
                {
                    ViewBag.RaidCount = raidCount.Count();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error: {ex.Message}";
            }

            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GuildRosterRetrieve()
        {
            string guild = "Secondnorth";
            string realm = "Thrall";

            this.RetrieveGuild(guild, realm);

            return RedirectToAction("Guild");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guild"></param>
        /// <param name="realm"></param>
        /// <returns></returns>
        private bool RetrieveGuild(string guild, string realm)
        {
            bool result = false;

            try
            {
                JSONGuild data = JSONBase.GetGuildJSON(guild, realm);

                if (data != null && data.Members.Count > 0)
                {
                    foreach (JSONGuildCharacter guildie in data.Members)
                    {
                        if (RetrieveCharacter(guildie.Character.Name, realm, true) == 0)
                        {
                            ModelState.AddModelError(string.Empty, $"Error: retrieving character data for: {guildie.Character.Name}");
                            break;
                        }
                    }

                    // No errors up to this point?  Success!
                    result = true;
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error: {ex.Message} in CollectJSONData() in GetGuildInfo.cs");
            }

            return result;
        }
    }
}
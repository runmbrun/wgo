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
                if (this.RetrieveCharacter(model.Character, model.Realm, true))
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
                        if (c.Roster == 3)
                        {
                            c.Roster = 1;
                        }
                        else if (c.Roster == 2)
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
                if (this.RetrieveCharacter(model.Character, model.Realm, false))
                {
                    return View(model);
                }
            }

            // Now display it!
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <param name="realm"></param>
        /// <returns></returns>
        private bool RetrieveCharacter(string character, string realm, bool isGuild)
        {
            bool result = false;
            JSONCharacter charFromWeb = JSONBase.GetCharacterJSONData(character, realm);

            if (charFromWeb == null)
            {
                ModelState.AddModelError("Raid", "Character not found.");
            }
            else
            {
                string spec = string.Empty;
                string role = string.Empty;
                var dbChars = from m in db.Characters select m;

                ViewBag.CharacterName = character;
                ViewBag.CharacterRealm = realm;

                if (charFromWeb.Talents != null && charFromWeb.Talents.Count > 0)
                {
                    spec = charFromWeb.Talents[0].Spec.Name;
                    role = charFromWeb.Talents[0].Spec.Role;
                }

                var search = dbChars.Where(s => s.Name == charFromWeb.Name && s.Realm == charFromWeb.Realm && s.Spec == spec);

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

                        if (isGuild)
                        {
                            if (searchChar.Roster == 2)
                            {
                                searchChar.Roster = 3;
                            }
                        }
                        else
                        {
                            if (searchChar.Roster == 1)
                            {
                                searchChar.Roster = 3;
                            }
                        }
                    }

                    try
                    {
                        db.SaveChanges();
                        result = true;
                    }
                    catch (Exception ex)
                    {
                        // no op
                        ModelState.AddModelError("Character", $"Error saving character: {ex.Message}");
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
                        charToDB.Roster = 1;
                    }
                    else
                    {
                        charToDB.Roster = 2;
                    }
                    

                    // does it exist in the db?  then add it!
                    db.Characters.Add(charToDB);

                    try
                    {
                        db.SaveChanges();
                        result = true;
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("Character", $"Error saving character: {ex.Message}");
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
        public ActionResult Character(string name)
        {
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

            return View();
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
                var guildCount = dbChars.Where(s => s.Roster == 1 || s.Roster == 3);
                var raidCount = dbChars.Where(s => s.Roster == 2 || s.Roster == 3);

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
    }
}
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
                List<Character> chars = new List<Character>();
                JSONCharacter charFromWeb = JSONBase.GetCharacterJSONData(model.Character, model.Realm);

                if (charFromWeb == null)
                {
                    ModelState.AddModelError("Character", "Character not found.");
                    return View(model);
                }
                else
                {
                    string spec = string.Empty;
                    string role = string.Empty;
                    var dbChars = from m in db.Characters select m;

                    ViewBag.CharacterName = model.Character;
                    ViewBag.CharacterRealm = model.Realm;

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
                            chars.Add(searchChar);

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
                        }

                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            // no op
                            ModelState.AddModelError("Character", $"Error saving character: {ex.Message}");
                            return View(model);
                        }
                    }
                    else
                    {
                        // Now copy the data for the 
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
                        charToDB.Roster = 1;

                        // does it exist in the db?  then add it!
                        chars.Add(charToDB);
                        db.Characters.Add(charToDB);

                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            // no op
                            ModelState.AddModelError("Character", $"Error saving character: {ex.Message}");
                            return View(model);
                        }
                    }

                    // Now display it!
                    return View();
                }
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

        [HttpGet]
        public ActionResult Guild()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Raid()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Raid(SearchViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {
                JSONCharacter charFromWeb = JSONBase.GetCharacterJSONData(model.Character, model.Realm);

                if (charFromWeb == null)
                {
                    ModelState.AddModelError("Character", "Character not found.");
                    return View(model);
                }
                else
                {
                    string spec = string.Empty;
                    string role = string.Empty;
                    var dbChars = from m in db.Characters select m;

                    ViewBag.CharacterName = model.Character;
                    ViewBag.CharacterRealm = model.Realm;

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

                            if (searchChar.Roster == 1)
                            {
                                searchChar.Roster = 3;
                            }
                        }

                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            // no op
                            ModelState.AddModelError("Character", $"Error saving character: {ex.Message}");
                            return View(model);
                        }
                    }
                    else
                    {
                        // Now copy the data for the 
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
                        charToDB.Roster = 2;

                        // does it exist in the db?  then add it!
                        db.Characters.Add(charToDB);

                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            // no op
                            ModelState.AddModelError("Character", $"Error saving character: {ex.Message}");
                            return View(model);
                        }
                    }

                    // Now display it!
                    return View();
                }
            }
        }

        [HttpGet]
        public ActionResult Character()
        {
            return View();
        }
    }
}
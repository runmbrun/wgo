using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
                if (this.RetrieveCharacter(model.Character, model.Realm, JSONBase.Rosters.Guild) == 0)
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
        public ActionResult Rescan(string name, string realm, string role)
        {
            JSONBase.Rosters roster = JSONBase.Rosters.None;
            string returnTo = string.Empty;
            string returnToParams = string.Empty;

            // Calculate the UrlReferrer link, for a return Url
            if (System.Web.HttpContext.Current.Request.UrlReferrer != null)
            {
                if (System.Web.HttpContext.Current.Request.UrlReferrer.ToString().Contains("/Guild"))
                {
                    ViewBag.ReturnUrl = "/WGO/Guild";
                    ViewBag.ReturnUrl = System.Web.HttpContext.Current.Request.UrlReferrer.ToString().Substring(System.Web.HttpContext.Current.Request.UrlReferrer.ToString().IndexOf("/WGO"));
                    roster = JSONBase.Rosters.Guild;
                    returnTo = "Guild";
                }
                else if (System.Web.HttpContext.Current.Request.UrlReferrer.ToString().Contains("/Raid"))
                {
                    ViewBag.ReturnUrl = "/WGO/Raid";
                    ViewBag.ReturnUrl = System.Web.HttpContext.Current.Request.UrlReferrer.ToString().Substring(System.Web.HttpContext.Current.Request.UrlReferrer.ToString().IndexOf("/WGO"));
                    roster = JSONBase.Rosters.Raid;
                    returnTo = "Raid";
                }

                if (System.Web.HttpContext.Current.Request.UrlReferrer.ToString().Contains("?page="))
                {
                    returnToParams = System.Web.HttpContext.Current.Request.UrlReferrer.ToString().Substring(System.Web.HttpContext.Current.Request.UrlReferrer.ToString().IndexOf("?page=") + "?page=".Length);
                }
            }

            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(realm))
            {
                if (name == "@All")
                {
                    // Rescan all the characters
                    List<Character> rescan = null;

                    if (roster == JSONBase.Rosters.Guild)
                    {
                        rescan = (from m in db.Characters where m.Roster == 1 select m).ToList();
                    }
                    else
                    {
                        rescan = (from m in db.Characters where m.Roster == 2 select m).ToList();
                    }

                    foreach (Character c in rescan)
                    {
                        this.RetrieveCharacter(c.Name, c.Realm, roster, roster == JSONBase.Rosters.Raid ? c.Role : string.Empty);
                    }
                }
                else
                {
                    // Reload just 1 character
                    this.RetrieveCharacter(name, realm, roster, roster == JSONBase.Rosters.Raid && !string.IsNullOrWhiteSpace(role) ? role : string.Empty);
                }
            }

            if (!string.IsNullOrWhiteSpace(returnToParams))
            {
                return RedirectToAction(returnTo, new { page=returnToParams });
            }
            else
            {
                return RedirectToAction(returnTo);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Delete(string name, string realm, string role)
        {
            string returnTo = "Guild";

            // Calculate the UrlReferrer link, for a return Url
            if (System.Web.HttpContext.Current.Request.UrlReferrer != null)
            {
                if (System.Web.HttpContext.Current.Request.UrlReferrer.ToString().Contains("/Guild"))
                {
                    ViewBag.ReturnUrl = "/WGO/Guild";
                }
                else if (System.Web.HttpContext.Current.Request.UrlReferrer.ToString().Contains("/Raid"))
                {
                    ViewBag.ReturnUrl = "/WGO/Raid";
                    returnTo = "Raid";
                }
            }

            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(realm))
            {
                // Delete a user from the Raid Roster
                List<Character> delete = null;
                if (returnTo == "Guild")
                {
                    delete = (from m in db.Characters where (m.Roster == 1) && m.Name == name && m.Realm == realm select m).ToList();
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(role))
                    {
                        delete = (from m in db.Characters where m.Roster == 2 && m.Name == name && m.Realm == realm && m.Role == role select m).ToList();
                    }
                    else
                    {
                        delete = (from m in db.Characters where (m.Roster == 2) && m.Name == name && m.Realm == realm select m).ToList();
                    }
                }

                foreach (Character c in delete)
                {
                    if (c.Roster == JSONBase.GetRaidRoster())
                    {
                        db.Characters.Remove(c);
                    }
                }

                db.SaveChanges();                
            }

            return RedirectToAction(returnTo);
        }

        /// <summary>
        /// The Raid View.
        /// </summary>
        /// <returns>Action Result.</returns>
        [HttpGet]
        public ActionResult Raid()
        {
            var model = new SearchViewModel
            {
                Roles = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem { Selected = true, Text = "DPS", Value = "DPS"},
                    new SelectListItem { Selected = false, Text = "Tank", Value = "TANK"},
                    new SelectListItem { Selected = false, Text = "Healing", Value = "HEALING"},
                }, "Value", "Text", 1)
            };

            return View(model);
        }

        /// <summary>
        /// Attempt to add a character to the raid roster.
        /// </summary>
        /// <param name="model">The object that contains the character name, realm and role.</param>
        /// <returns>Action Result.</returns>
        [HttpPost]
        public ActionResult Raid(SearchViewModel model)
        {
            model.Roles = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Selected = true, Text = "DPS", Value = "DPS"},
                new SelectListItem { Selected = false, Text = "Tank", Value = "TANK"},
                new SelectListItem { Selected = false, Text = "Healing", Value = "HEALING"},
            }, "Value", "Text", 1);

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {
                if (this.RetrieveCharacter(model.Character, model.Realm, JSONBase.Rosters.Raid, model.Role) == 0)
                {
                    return View(model);
                }
            }

            // Now display it!
            return View(model);
        }

        /// <summary>
        /// Result = 0 -> Error
        /// Result = 1 -> Success
        /// Result = 2 -> Character not found, but no errors.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="realm"></param>
        /// <returns></returns>
        private int RetrieveCharacter(string character, string realm, JSONBase.Rosters roster, string charRole = "")
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

                List<Character> search = null;

                if (roster == JSONBase.Rosters.Guild)
                {
                    search = db.Characters.Where(s => s.Roster == 1 && s.Name == charFromWeb.Name && s.Realm == charFromWeb.Realm).ToList();
                }
                else if (roster == JSONBase.Rosters.Raid)
                {
                    search= db.Characters.Where(s => s.Roster == 2 && s.Name == charFromWeb.Name && s.Realm == charFromWeb.Realm).ToList();
                }

                if (search.Count() > 0)
                {
                    // Already exists in the DB - update it
                    foreach (Character searchChar in search)
                    {
                        // Check for differences
                        if (searchChar.Level != charFromWeb.Level)
                        {
                            searchChar.Modified_Level = GetCentralTime();
                        }

                        if (searchChar.AchievementPoints != charFromWeb.AchievementPoints)
                        {
                            searchChar.Modified_AchievementPoints = GetCentralTime();
                        }

                        if (searchChar.Max_iLevel != charFromWeb.Items.AverageItemLevel)
                        {
                            searchChar.Modified_Max_iLevel = GetCentralTime();
                        }

                        // Update Character
                        searchChar.Level = charFromWeb.Level;
                        searchChar.Class = WoWConverter.ConvertClass(charFromWeb.Class);
                        searchChar.Race = WoWConverter.ConvertRace(charFromWeb.Race);
                        searchChar.AchievementPoints = charFromWeb.AchievementPoints;

                        // Only check for updates on these fields if it's the guild roster or it's the raid roster and the role is the same
                        if (roster == JSONBase.Rosters.Guild || (roster == JSONBase.Rosters.Raid && role == search[0].Role))
                        {
                            // This is a different Role than we are tracking... so don't update these fields
                            if (searchChar.Equipped_iLevel != charFromWeb.Items.AverageItemLevelEquipped)
                            {
                                searchChar.Modified_Equipped_iLevel = GetCentralTime();
                            }

                            searchChar.Equipped_iLevel = charFromWeb.Items.AverageItemLevelEquipped;
                            searchChar.Spec = spec;
                            searchChar.Role = role;
                            searchChar.Items = JsonConvert.SerializeObject(charFromWeb.Items);
                        }

                        searchChar.Max_iLevel = charFromWeb.Items.AverageItemLevel;
                        searchChar.LastUpdated = GetCentralTime();
                        
                        if (roster == JSONBase.Rosters.Guild)
                        {
                            searchChar.Roster = JSONBase.GetGuildRoster();
                        }
                        else if (roster == JSONBase.Rosters.Raid)
                        {
                            searchChar.Roster = JSONBase.GetRaidRoster();
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
                else if ((roster == JSONBase.Rosters.Guild) || (roster == JSONBase.Rosters.Raid && charRole == role))
                {
                    // Now insert the data inot the database
                    Character charToDB = new Character();
                    charToDB.Name = charFromWeb.Name;
                    charToDB.Level = charFromWeb.Level;
                    charToDB.Class = WoWConverter.ConvertClass(charFromWeb.Class);
                    charToDB.Race = WoWConverter.ConvertRace(charFromWeb.Race);
                    charToDB.AchievementPoints = charFromWeb.AchievementPoints;
                    charToDB.Max_iLevel = charFromWeb.Items.AverageItemLevel;
                    charToDB.Equipped_iLevel = charFromWeb.Items.AverageItemLevelEquipped;
                    charToDB.LastUpdated = GetCentralTime();
                    charToDB.Realm = charFromWeb.Realm;
                    charToDB.Spec = spec;
                    charToDB.Role = role;
                    charToDB.Modified_AchievementPoints = GetCentralTime();
                    charToDB.Modified_Equipped_iLevel = GetCentralTime();
                    charToDB.Modified_Max_iLevel = GetCentralTime();
                    charToDB.Modified_Level = GetCentralTime();

                    if (roster == JSONBase.Rosters.Guild)
                    {
                        charToDB.Roster = JSONBase.GetGuildRoster();
                    }
                    else if (roster == JSONBase.Rosters.Raid)
                    {
                        charToDB.Roster = JSONBase.GetRaidRoster();
                    }

                    // Items = List<CharacterItems> 
                    charToDB.Items = JsonConvert.SerializeObject(charFromWeb.Items);

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
            int roster = 1;

            // Calculate the UrlReferrer link, for a return Url
            if (System.Web.HttpContext.Current.Request.UrlReferrer != null)
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    ViewBag.Name = name;
                }

                if (System.Web.HttpContext.Current.Request.UrlReferrer.ToString().Contains("/Guild"))
                {
                    ViewBag.ReturnUrl = "/WGO/Guild";
                    ViewBag.ReturnUrl = System.Web.HttpContext.Current.Request.UrlReferrer.ToString().Substring(System.Web.HttpContext.Current.Request.UrlReferrer.ToString().IndexOf("/WGO"));
                    roster = 1;
                }
                else if (System.Web.HttpContext.Current.Request.UrlReferrer.ToString().Contains("/Raid"))
                {
                    ViewBag.ReturnUrl = "/WGO/Raid";
                    ViewBag.ReturnUrl = System.Web.HttpContext.Current.Request.UrlReferrer.ToString().Substring(System.Web.HttpContext.Current.Request.UrlReferrer.ToString().IndexOf("/WGO"));
                    roster = 2;
                }
            }

            // Get the Character info
            CharacterViewModel model = new CharacterViewModel();
            model.Character = null;
            var chars = db.Characters.Where(s => s.Name == name && s.Realm == realm && s.Roster == roster);

            if (chars.Any())
            {
                // Store the Character data
                model.Character = chars.First();
                model.AuditHtml = this.CalculateAuditHtml(model.Character);
            }

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        private string CalculateAuditHtml(Character character)
        {
            string html = string.Empty;
            CharacterAuditResult audit = new CharacterAuditResult();

            if (audit.DoAudit(character))
            {
                // Now add to the Audit section
                if (audit.IssueCount > 0)
                {
                    html = $"<div class=\"container\"><h1>Character Audit</h1><p class=\"bg-danger text-white col-sm-2\">Issues found: {audit.IssueCount}</p></div><div class=\"container row m-3\">";

                    // Check for missing enchants - 1 weapon enchant and 2 ring enchants
                    if (audit.MissingEnchant)
                    {
                        html += $"<div class=\"card border-danger mb-3 col-sm-3\"><div class=\"card-header\"><h4 class=\"text-danger\">Missing Enchants:</h4></div><div class=\"card-body\"><ul>";

                        foreach (string slot in audit.MissingEnchants)
                        {
                            html += $"<li>{slot}</li>";
                        }

                        html += $"</ul></div></div>";
                    }

                    // Check for missing gems - go through every slot to make sure... 
                    if (audit.MissingGem)
                    {
                        html += $"<div class=\"card border-danger mb-3 col-sm-3\"><div class=\"card-header\"><h4 class=\"text-danger\">Missing Gems:</h4></div><div class=\"card-body\"><ul>";

                        foreach (string slot in audit.MissingGems)
                        {
                            html += $"<li>{slot}</li>";
                        }

                        html += $"</ul></div></div>";
                    }

                    // close up the tags
                    html += "</div>";
                }
                else
                {
                    html = $"<div class=\"container\"><h1>Character Audit</h1><p class=\"bg-success text-white col-sm-2\">No Issues Found</p></div>";
                }
            }

            return html;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult RaidRosterAudit()
        {
            List<CharacterAuditResult> auditRoster = new List<CharacterAuditResult>();
            var chars = db.Characters.Where(s => s.Roster == 2);

            if (chars.Any())
            {
                // Store the Character data
                
                foreach (Character raider in chars)
                {
                    CharacterAuditResult audit = new CharacterAuditResult();

                    if (audit.DoAudit(raider))
                    {
                        auditRoster.Add(audit);
                    }
                }
            }

            return View(auditRoster);
        }

        /// <summary>
        /// Audit - This is a separate call via the wow api.  Doesn't seem to have been updated lately.  
        ///   Doesn't seem to be worth using at this point, especially since the audit will only happen for the current spec.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="realm"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Audit(string name, string realm)
        {
            JSONCharacterAudit audit = JSONBase.GetCharacterAuditJSON(name, realm);
            string html = string.Empty;

            if (audit != null)
            {
                // Parse the audit data...
                //  Currently Audit tracks the following issues:
                //  1.  numberOfIssues - Not all are relevant so do our own count
                //  2.  emptyGlyphSlots - Glyphs are not comestic so ignore
                //  3.  unspentTalentPoints - Need to calculate this because this could be 1 if PvP talents aren't picked or used.
                //  4.  noSpec - Import so count this
                //  5.  unenchantedItems - Import so count this
                //  6.  emptySockets - Import so count this
                //  7.  itemsWithEmptySockets - Import so count this
                //  8.  appropriateArmorType - Import so count this
                //  9.  inappropriateArmorType - Import so count this
                //  10. lowLevelItems - Import so count this
                //  11. lowLevelThreshold - Import so count this
                //  12. missingExtraSockets - No Belt Buckles in BFA so ignore
                //  13. recommendedBeltBuckle - No Belt Buckles in BFA so ignore

                // Count the number of relevant issues...
                int issueCount = 0;

                // unspentTalentPoints
                // noSpec
                issueCount += audit.Audit.UnenchantedItems.Count > 0 ? 1 : 0;
                issueCount += audit.Audit.ItemsWithEmptySockets.Count > 0 ? 1 : 0;
                // appropriateArmorType
                // lowLevelItems

                /*
                 * 15 = Weapon or Hands
                 * 9 = Weapon or Hands
                 */

                /* Old Way - Table
                html =
                    $"<div class=\"card-body\"><h2>Audit</h2><table border=\"1\"> " +
                    $"<tr><td width=\"20%\"><b>Number of Issues</b></td><td width=\"30%\"><b>{audit.Audit.NumberOfIssues}</b></td></tr>" +
                    $"<tr><td width=\"20%\">Empty Glyph Slots</td><td width=\"30%\">{audit.Audit.EmptyGlyphSlots}</td></tr>" +
                    $"<tr><td width=\"20%\">Unspent Talent Points</td><td width=\"30%\">{audit.Audit.UnspentTalentPoints}</td></tr>" +
                    $"<tr><td width=\"20%\">No Spec</td><td width=\"30%\">{audit.Audit.NoSpec}</td></tr>" +
                    $"<tr><td width=\"20%\">Empty Sockets</td><td width=\"30%\">{audit.Audit.EmptySockets}</td></tr>" +
                    $"<tr><td width=\"20%\">Low Level Threshold</td><td width=\"30%\">{audit.Audit.LowLevelThreshold}</td></tr>" +
                    $"</table></div>";*/

                // New Way - Grid via Bootstrap
                if (issueCount == 0)
                {
                    html = $"<div class=\"container\"><h1>Character Audit</h1><p class=\"lead text-muted bg-success\">Issues found: {issueCount}</p></div>";
                }
                else
                {
                    html = $"<div class=\"container\"><h1>Character Audit</h1><p class=\"bg-danger text-white col-sm-2\">Issues found: {issueCount}</p></div>";
                    html += $"<div class=\"container\">";

                    // Display Missing Sockets - if needed
                    if (audit.Audit.ItemsWithEmptySockets.Count > 0)
                    {
                        html += $"<div class=\"card border-danger mb-3 col-sm-3\"><div class=\"card-header\"><h4 class=\"text-danger\">Missing Sockets:</h4></div><div class=\"card-body\"><ul>";

                        foreach (KeyValuePair<string, int> entry in audit.Audit.ItemsWithEmptySockets)
                        {
                            html += $"<li>{entry.Key}</li>";
                        }

                        html += $"</ul></div></div>";
                    }

                    // Display Missing Enchants - if needed
                    if (audit.Audit.UnenchantedItems.Count > 0)
                    {
                        html += $"<div class=\"card border-danger mb-3 col-sm-3\"><div class=\"card-header\"><h4 class=\"text-danger\">Missing Enchants:</h4></div><div class=\"card-body\"><ul>";

                        foreach (KeyValuePair<string, int> entry in audit.Audit.UnenchantedItems)
                        {
                            html += $"<li>{entry.Key}</li>";
                        }

                        html += $"</ul></div></div>";
                    }

                    // Finish up the missing html
                    html += $"</div>";
                }

            }

            return Content(html);
        }

        /// <summary>
        /// Test View.
        /// </summary>
        /// <returns>Action Result.</returns>
        public ActionResult Test()
        {
            return View();
        }

        #region " Admin Menu - Debugging Functions "
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Admin()
        {
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
                int guildRoster = JSONBase.GetGuildRoster();
                int raidRoster = JSONBase.GetRaidRoster();
                var dbChars = from m in db.Characters select m;
                var guildCount = dbChars.Where(s => s.Roster == guildRoster);
                var raidCount = dbChars.Where(s => s.Roster == raidRoster);
                
                ViewBag.GuildCount = 0;
                ViewBag.RaidCount = 0;

                if (guildCount.Any())
                {
                    ViewBag.GuildCount = guildCount.Count();
                }

                if (raidCount.Any())
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
        /// Retrieves an entire guild roster.
        /// </summary>
        /// <returns>Action Result</returns>
        [HttpGet]
        public ActionResult GuildRosterRetrieve()
        {
            string guild = "Secondnorth";
            string realm = "Thrall";

            this.RetrieveGuild(guild, realm);

            return RedirectToAction("Guild");
        }

        /// <summary>
        /// Retrieves an entire guild roster.
        /// </summary>
        /// <param name="guild">The guild to retrieve.</param>
        /// <param name="realm">The realm to retrieve.</param>
        /// <returns>True if retrieval was successful.</returns>
        public bool RetrieveGuild(string guild, string realm)
        {
            bool result = false;

            try
            {
                JSONGuild data = JSONBase.GetGuildJSON(guild, realm);

                if (data != null && data.Members.Count > 0)
                {
                    foreach (JSONGuildCharacter guildie in data.Members)
                    {
                        if (RetrieveCharacter(guildie.Character.Name, realm, JSONBase.Rosters.Guild) == 0)
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

        /// <summary>
        /// Deletes an entire guild roster.
        /// </summary>
        /// <returns>Action Result</returns>
        [HttpGet]
        public ActionResult DeleteGuildRoster()
        {
            var test = db.Characters.Where(s => s.Roster == (int)JSONBase.Rosters.Guild);
            if (test.Count() > 0)
            {
                foreach (Character c in test)
                {
                    db.Characters.Remove(c);
                }

                db.SaveChanges();
            }

            return RedirectToAction("Guild");
        }

        /// <summary>
        /// Deletes an entire guild roster.
        /// </summary>
        /// <returns>Action Result</returns>
        [HttpGet]
        public ActionResult DeleteRaidRoster()
        {
            var test = db.Characters.Where(s => s.Roster == (int)JSONBase.Rosters.Raid);
            if (test.Count() > 0)
            {
                foreach (Character c in test)
                {
                    db.Characters.Remove(c);
                }

                db.SaveChanges();
            }

            return RedirectToAction("Raid");
        }

        /// <summary>
        /// Deletes an entire guild roster.
        /// </summary>
        /// <returns>Action Result</returns>
        [HttpGet]
        public ActionResult DeleteAllRosters()
        {
            var test = db.Characters.AsQueryable();
            if (test.Count() > 0)
            {
                foreach (Character c in test)
                {
                    db.Characters.Remove(c);
                }

                db.SaveChanges();
            }

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Get the current Central Standard Time value.
        /// </summary>
        /// <returns></returns>
        private DateTime GetCentralTime()
        {
            DateTime centralDateTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Central Standard Time");

            return centralDateTime;
        }
        #endregion
    }
}
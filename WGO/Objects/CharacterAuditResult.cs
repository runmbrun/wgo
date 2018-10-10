using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WGO.JSON;

namespace WGO
{
    public class CharacterAuditResult
    {
        public string Name { get; set; }
        public string Realm { get; set; }
        public string Role { get; set; }
        public int IssueCount { get; set; }
        public bool MissingEnchant { get; set; }
        public bool MissingGem { get; set; }

        public List<string> MissingEnchants { get; set; } = new List<string>();
        public List<string> MissingGems { get; set; } = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public bool DoAudit(Models.Character character)
        {
            bool result = false;
            JSONCharacterItems items = null;

            try
            {
                // Store the name and realm
                this.Name = character.Name;
                this.Realm = character.Realm;
                this.Role = character.Role;

                // Get the item data
                items = JsonConvert.DeserializeObject<JSONCharacterItems>(character.Items);

                #region " Check for missing enchants - 1 weapon enchant and 2 ring enchants "
                if (items.MainHand.ToolTipParams.Enchant == null)
                {
                    this.MissingEnchants.Add("Main Hand");
                    this.MissingEnchant = true;
                    this.IssueCount++;
                }

                if (items.Finger1.ToolTipParams.Enchant == null)
                {
                    this.MissingEnchants.Add("Finger1");
                    this.MissingEnchant = true;
                    this.IssueCount++;
                }

                if (items.Finger2.ToolTipParams.Enchant == null)
                {
                    this.MissingEnchants.Add("Finger1");
                    this.MissingEnchant = true;
                    this.IssueCount++;
                }
                #endregion

                #region " Check for missing gems - go through every slot to make sure... "
                if (items.Head.BonusLists.Contains(4802) && items.Head.ToolTipParams.Gem0 == null)
                {
                    this.MissingGems.Add("Head");
                    this.MissingGem = true;
                    this.IssueCount++;
                }
                if (items.Neck.BonusLists.Contains(4802) && items.Neck.ToolTipParams.Gem0 == null)
                {
                    this.MissingGems.Add("Neck");
                    this.MissingGem = true;
                    this.IssueCount++;
                }
                if (items.Shoulder.BonusLists.Contains(4802) && items.Shoulder.ToolTipParams.Gem0 == null)
                {
                    this.MissingGems.Add("Shoulder");
                    this.MissingGem = true;
                    this.IssueCount++;
                }
                if (items.Back.BonusLists.Contains(4802) && items.Back.ToolTipParams.Gem0 == null)
                {
                    this.MissingGems.Add("Back");
                    this.MissingGem = true;
                    this.IssueCount++;
                }
                if (items.Chest.BonusLists.Contains(4802) && items.Chest.ToolTipParams.Gem0 == null)
                {
                    this.MissingGems.Add("Chest");
                    this.MissingGem = true;
                    this.IssueCount++;
                }
                if (items.Wrist.BonusLists.Contains(4802) && items.Wrist.ToolTipParams.Gem0 == null)
                {
                    this.MissingGems.Add("Wrist");
                    this.MissingGem = true;
                    this.IssueCount++;
                }
                if (items.Hands.BonusLists.Contains(4802) && items.Hands.ToolTipParams.Gem0 == null)
                {
                    this.MissingGems.Add("Hands");
                    this.MissingGem = true;
                    this.IssueCount++;
                }
                if (items.Waist.BonusLists.Contains(4802) && items.Waist.ToolTipParams.Gem0 == null)
                {
                    this.MissingGems.Add("Waist");
                    this.MissingGem = true;
                    this.IssueCount++;
                }
                if (items.Legs.BonusLists.Contains(4802) && items.Legs.ToolTipParams.Gem0 == null)
                {
                    this.MissingGems.Add("Legs");
                    this.MissingGem = true;
                    this.IssueCount++;
                }
                if (items.Feet.BonusLists.Contains(4802) && items.Feet.ToolTipParams.Gem0 == null)
                {
                    this.MissingGems.Add("Feet");
                    this.MissingGem = true;
                    this.IssueCount++;
                }
                if (items.Finger1.BonusLists.Contains(4802) && items.Finger1.ToolTipParams.Gem0 == null)
                {
                    this.MissingGems.Add("Finger1");
                    this.MissingGem = true;
                    this.IssueCount++;
                }
                if (items.Finger2.BonusLists.Contains(4802) && items.Finger2.ToolTipParams.Gem0 == null)
                {
                    this.MissingGems.Add("Finger2");
                    this.MissingGem = true;
                    this.IssueCount++;
                }
                if (items.Trinket1.BonusLists.Contains(4802) && items.Trinket1.ToolTipParams.Gem0 == null)
                {
                    this.MissingGems.Add("Trinket1");
                    this.MissingGem = true;
                    this.IssueCount++;
                }
                if (items.Trinket2.BonusLists.Contains(4802) && items.Trinket2.ToolTipParams.Gem0 == null)
                {
                    this.MissingGems.Add("Trinket2");
                    this.MissingGem = true;
                    this.IssueCount++;
                }
                if (items.MainHand.BonusLists.Contains(4802) && items.MainHand.ToolTipParams.Gem0 == null)
                {
                    this.MissingGems.Add("Main Hand");
                    this.MissingGem = true;
                    this.IssueCount++;
                }
                if (items.OffHand != null && items.OffHand.BonusLists.Contains(4802) && items.OffHand.ToolTipParams.Gem0 == null)
                {
                    this.MissingGems.Add("Off Hand");
                    this.MissingGem = true;
                    this.IssueCount++;
                }
                #endregion

                // If we got here, then success!
                result = true;
            }
            catch (Exception)
            {
                // No Op
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public bool DoAudit(List<Models.Character> characters)
        {
            bool result = true;

            foreach (Models.Character character in characters)
            {
                result &= this.DoAudit(character);
            }

            return result;
        }
    }
}
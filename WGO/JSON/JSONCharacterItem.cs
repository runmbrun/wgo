using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WGO.JSON
{
    public class JSONCharacterItems
    {
        public int AverageItemLevel { get; set; }
        public int AverageItemLevelEquipped { get; set; }
        public JSONCharacterItem Head { get; set; }
        public JSONCharacterItem Neck { get; set; }
        public JSONCharacterItem Shoulder { get; set; }
        public JSONCharacterItem Back { get; set; }
        public JSONCharacterItem Chest { get; set; }
        public JSONCharacterItem Tabard { get; set; }
        public JSONCharacterItem Shirt { get; set; }
        public JSONCharacterItem Wrist { get; set; }
        public JSONCharacterItem Hands { get; set; }
        public JSONCharacterItem Waist { get; set; }
        public JSONCharacterItem Legs { get; set; }
        public JSONCharacterItem Feet { get; set; }
        public JSONCharacterItem Finger1 { get; set; }
        public JSONCharacterItem Finger2 { get; set; }

        public JSONCharacterItem Trinket1 { get; set; }
        public JSONCharacterItem Trinket2 { get; set; }
        public JSONCharacterItem MainHand { get; set; }
        public JSONCharacterItem OffHand { get; set; }
    }

    public class JSONCharacterItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quality { get; set; }
        public int ItemLevel { get; set; }
        public JSONCharacterItemToolTipParams ToolTipParams { get; set; }
        public IList<JSONCharacterItemStats> Stats { get; set; }
        public int Armor { get; set; }
        public string Context { get; set; }
        public IList<int> BonusLists { get; set; }
        public int ArtifactId { get; set; }
    }

    public class JSONItemStats
    {
        public int Stat { get; set; }
        public int Amount { get; set; }
    }

    public class JSONCharacterItemStats
    {
        public string StatId { get; set; }
        public int Delta { get; set; }
        public int MaxDelta { get; set; }
    }

    public class JSONCharacterItemToolTipParams
    {
        public int Gem0 { get; set; }
        public int Gem1 { get; set; }
        public int Enchant { get; set; }
        public IList<int> Set { get; set; }
        public int TimewalkerLevel { get; set; }
    }

    public class JSONAzeriteItem
    {
        public int AzeriteLevel { get; set; }
        public int AzeriteExperience { get; set; }
        public int AzeriteExperienceRemaining { get; set; }
    }
}
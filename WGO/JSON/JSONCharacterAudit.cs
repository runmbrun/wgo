using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WGO.JSON
{
    public class JSONCharacterAudit
    {
        public JSONAudit Audit { get; set; }
    }

    public class JSONAudit
    {
        public int NumberOfIssues { get; set; }
        //public int Slots { get; set; }
        public int EmptyGlyphSlots { get; set; }
        public int UnspentTalentPoints { get; set; }
        public bool NoSpec { get; set; }
        //public int UnenchantedItems { get; set; }
        public int EmptySockets { get; set; }
        //public int ItemsWithEmptySockets { get; set; }
        public int AppropriateArmorType { get; set; }
        //public int inappropriateArmorType { get; set; }
        //public int lowLevelItems { get; set; }
        public int LowLevelThreshold { get; set; }
        //public int missingExtraSockets { get; set; }
    }
}
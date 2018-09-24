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
        public IList<JSONAuditSlot> Slots { get; set; }
        public int EmptyGlyphSlots { get; set; }
        public int UnspentTalentPoints { get; set; }
        public bool NoSpec { get; set; }
        public IList<JSONAuditSlot> UnenchantedItems { get; set; }
        public int EmptySockets { get; set; }
        public IList<JSONAuditSlot> ItemsWithEmptySockets { get; set; }
        public int AppropriateArmorType { get; set; }
        public IList<JSONAuditSlot> InappropriateArmorType { get; set; }
        public IList<JSONAuditSlot> LowLevelItems { get; set; }
        public int LowLevelThreshold { get; set; }
        public IList<JSONAuditSlot> MissingExtraSockets { get; set; }
    }

    public class JSONAuditSlot
    {
        public string Slot { get; set; }
        public int Value { get; set; }
    }
}
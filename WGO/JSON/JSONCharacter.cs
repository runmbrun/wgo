using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WGO.JSON
{
    public class JSONCharacter
    {
        public long LastModified { get; set; }
        public string Name { get; set; }
        public string Realm { get; set; }
        public string Battlegroup { get; set; }
        public int Class { get; set; }
        public int Race { get; set; }
        public int Gender { get; set; }
        public int Level { get; set; }
        public int AchievementPoints { get; set; }
        public string Thumbnail { get; set; }
        public string CalcClass { get; set; }
        public int Faction { get; set; }
        public JSONCharacterItems Items { get; set; }
        public JSONCharacterProfession Professions { get; set; }
        public IList<JSONCharacterTalent> Talents { get; set; }
        public string TotalHonorableKills { get; set; }
    }

    public class JSONCharacterTalent
    {
        public string Selected { get; set; }
        public JSONSpec Spec { get; set; }
    }

    public class JSONCharacterProfession
    {
        public IList<JSONProfession> Primary { get; set; }
        public IList<JSONProfession> Secondary { get; set; }
    }

    public class JSONProfession
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Rank { get; set; }
    }
}
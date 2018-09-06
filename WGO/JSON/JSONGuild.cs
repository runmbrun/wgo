using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WGO.JSON
{
    public class JSONGuild
    {
        //{"lastModified":1473267560000,"name":"SecondNorth","realm":"Thrall","battlegroup":"Rampage","level":25,"side":1,"achievementPoints":1195,"members":[{"character":
        public string LastModified { get; set; }

        public string Name { get; set; }
        public string Realm { get; set; }
        public string Battlegroup { get; set; }
        public int Level { get; set; }
        public int Side { get; set; }
        public int AchievementPoints { get; set; }
        public IList<JSONGuildCharacter> Members { get; set; }
    }

    public class JSONGuildCharacter
    {
        public JSONGuildMember Character { get; set; }
        public int Rank { get; set; }
    }

    public class JSONGuildMember
    {
        public string Name { get; set; }
        public string Realm { get; set; }
        public string Battlegroup { get; set; }
        public int Class { get; set; }
        public int Race { get; set; }
        public int Gender { get; set; }
        public int Level { get; set; }
        public int AchievementPoints { get; set; }
        public string Thumbnail { get; set; }
        public JSONSpec Spec { get; set; }
        public JSONSpec Spec2 { get; set; }
        public string GuildRealm { get; set; }
        public string LastModified { get; set; }
        public string Rank { get; set; }
    }
}
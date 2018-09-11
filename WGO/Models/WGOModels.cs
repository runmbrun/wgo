using System;
using System.Data.Entity;

namespace WGO.Models
{
    public class Character
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public string Class { get; set; }
        public string Race { get; set; }
        public int AchievementPoints { get; set; }
        public int Max_iLevel { get; set; }
        public int Equipped_iLevel { get; set; }
        public DateTime? LastUpdated { get; set; }
        public string Realm { get; set; }
        public string Spec { get; set; }

        public DateTime? Modified_Level { get; set; }
        public DateTime? Modified_Max_iLevel { get; set; }
        public DateTime? Modified_Equipped_iLevel { get; set; }
        public DateTime? Modified_AchievementPoints { get; set; }

        public int Roster { get; set; }
        public string Role { get; set; }
        public string Items { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class WGODBContext : DbContext
    {
        public WGODBContext() : base("WGOConnection")
        {
        }

        public static WGODBContext Create()
        {
            return new WGODBContext();
        }

        //public System.Data.Entity.DbSet<WGO.Models.Character> Characters { get; set; }

        public DbSet<Character> Characters { get; set; }
    }
}
// <copyright file="WGOModels.cs" company="Secondnorth.com">
//     Secondnorth.com. All rights reserved.
// </copyright>
// <author>Fainn</author>

namespace WGO.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;

    /// <summary>
    /// Model of data for a character.
    /// </summary>
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

        /// <summary>
        /// This is not from the database, but will help some pages rank the characters
        /// </summary>
        [NotMapped]
        public int Rank { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class WGODBContext : DbContext
    {
        /// <summary>
        /// 
        /// </summary>
        public WGODBContext() : base("WGOConnection")
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static WGODBContext Create()
        {
            return new WGODBContext();
        }

        /// <summary>
        /// 
        /// </summary>
        public DbSet<Character> Characters { get; set; }
    }
}
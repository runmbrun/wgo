using System.Collections.Generic;

namespace WGO.JSON
{
    /// <summary>
    /// 
    /// </summary>
    public class JSONGuildNews
    {
        public string LastModified { get; set; }
        public string Name { get; set; }
        public string Realm { get; set; }
        public string Battlegroup { get; set; }
        public int Level { get; set; }
        public int Side { get; set; }
        public int AchievementPoints { get; set; }
        // public JSONGuildEmblem Emblem { get; set; }
        public IList<JSONGuildNewsItem> News { get; set; }
    }

    /// <summary>
    /// There are at least 2 types of Guild News Items:
    /// 1. Item Loot
    /// 2. Achievement
    /// 3. No others found at this time
    /// </summary>
    public class JSONGuildNewsItem
    {
        public string Type { get; set; }
        public string Character { get; set; }
        public string Timestamp { get; set; }
        public int ItemId { get; set; }
        public string Context { get; set; }
        public IList<int> BonusLists { get; set; }

        // "achievement":{"id":5723,"title":"50 Exalted Reputations","points":10,"description":"Raise 50 reputations to Exalted.","rewardItems":[],"icon":"achievement_reputation_08","criteria":[{"id":982,"description":"50 reputations to Exalted","orderIndex":1,"max":50
    }

    /// <summary>
    /// The Character Achievement object
    /// </summary>
    public class JSONGuildNewsAchievement
    {
        // "achievement":{"id":5723,"title":"50 Exalted Reputations","points":10,"description":"Raise 50 reputations to Exalted.","rewardItems":[],"icon":"achievement_reputation_08","criteria":[{"id":982,"description":"50 reputations to Exalted","orderIndex":1,"max":50
        public int Id { get; set; }
        public string Title { get; set; }
        public int Points { get; set; }
        public string Description { get; set; }
        public IList<string> RewardItems { get; set; }
        public string Icon { get; set; }
        // todo: Criteria
        public bool AccountWide { get; set; }
        public int FactionId { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Emit;

namespace WGO
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRankedCharacterRepository
    {
        IEnumerable<Models.RankedCharacter> GetData(out int totalRecords, int roster, string globalSearch, int? limitOffset, int? limitRowCount, string orderBy, bool desc);
    }

    /// <summary>
    /// 
    /// </summary>
    public class RankedCharacterRepository : IRankedCharacterRepository
    {
        public IEnumerable<Models.RankedCharacter> GetData(out int totalRecords, int roster, string globalSearch, int? limitOffset, int? limitRowCount, string orderBy, bool asc)
        {
            IEnumerable<Models.RankedCharacter> result = null;

            using (var context = new Models.WGODBContext())
            {
                context.Configuration.AutoDetectChangesEnabled = false;

                // Create the sql
                string orderByClause = $"order by { (string.IsNullOrWhiteSpace(orderBy) || orderBy.CompareTo("default") == 0 ? "level desc, Equipped_iLevel desc, Max_iLevel desc, AchievementPoints desc, name asc" : $"{orderBy}{(asc ? "" : " desc")}")}";
                string sql = $"SELECT Name, Level, Class, AchievementPoints, Max_iLevel, Equipped_iLevel, LastUpdated, Realm, Roster, Modified_Level, Modified_Max_iLevel, Modified_Equipped_iLevel, Modified_AchievementPoints," +
                    $"ROW_NUMBER() OVER ({orderByClause}) as Rank " +
                    $"FROM Characters " +
                    $"where roster = 1 " + 
                    $"{orderByClause}";

                // Get the results
                result = context.Database.SqlQuery<Models.RankedCharacter>(sql);

                // Check to see if there is a search
                if (!string.IsNullOrWhiteSpace(globalSearch))
                {
                    result = result.Where(p => p.Name.ToLower().Contains(globalSearch.ToLower()));
                }

                // Get the total record count
                totalRecords = result.Count();

                // Paging
                if (limitOffset.HasValue && limitRowCount.HasValue)
                {
                    result = result.Skip(limitOffset.Value).Take(limitRowCount.Value);
                }

                // Return data as a List
                result = result.ToList();
            }

            return result;
        }
    }
}
using System.Collections.Generic;
using System.Linq;

namespace WGO
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICharacterRepository
    {
        IEnumerable<Models.Character> GetData(out int totalRecords, string globalSearch, int? limitOffset, int? limitRowCount, string orderBy, bool desc);
    }

    /// <summary>
    /// 
    /// </summary>
    public class CharacterRepository : ICharacterRepository
    {
        public IEnumerable<Models.Character> GetData(out int totalRecords, string globalSearch, int? limitOffset, int? limitRowCount, string orderBy, bool asc)
        {
            using (Models.WGODBContext db = new Models.WGODBContext())
            {
                var query = db.Characters.AsQueryable().Where(s => s.Roster == 1 || s.Roster == 3);

                if (!string.IsNullOrWhiteSpace(globalSearch))
                {
                    query = query.Where(p => p.Name.Contains(globalSearch));
                }

                // Sort the data
                switch (orderBy)
                {
                    case "name":
                        query = asc ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name);
                        break;

                    case "level":
                        query = asc ? query.OrderBy(p => p.Level) : query.OrderByDescending(p => p.Level);
                        break;

                    case "class":
                        query = asc ? query.OrderBy(p => p.Class) : query.OrderByDescending(p => p.Class);
                        break;

                    case "achievementpoints":
                        query = asc ? query.OrderBy(p => p.AchievementPoints) : query.OrderByDescending(p => p.AchievementPoints);
                        break;

                    case "maxilevel":
                        query = query.OrderByDescending(p => p.Level).ThenByDescending(p => p.Max_iLevel).ThenBy(p => p.Name);
                        break;

                    case "equippedilevel":
                        query = query.OrderByDescending(p => p.Level).ThenByDescending(p => p.Equipped_iLevel).ThenBy(p => p.Name);
                        break;

                    case "lastmodified":
                        query = asc ? query.OrderBy(p => p.LastUpdated) : query.OrderByDescending(p => p.LastUpdated);
                        break;

                    default:
                        query = query.OrderByDescending(p => p.Level).ThenByDescending(p => p.Equipped_iLevel).ThenBy(p => p.Name);
                        break;
                }

                totalRecords = query.Count();

                // Paging
                if (limitOffset.HasValue && limitRowCount.HasValue)
                {
                    query = query.Skip(limitOffset.Value).Take(limitRowCount.Value);
                }

                return query.ToList();
            }
        }
    }
}
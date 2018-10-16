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
        IEnumerable<Models.Character> GetData(out int totalRecords, int roster, string globalSearch, int? limitOffset, int? limitRowCount, string orderBy, bool desc);
    }

    /// <summary>
    /// 
    /// </summary>
    public class RankedCharacterRepository : IRankedCharacterRepository
    {
        public IEnumerable<Models.Character> GetData(out int totalRecords, int roster, string globalSearch, int? limitOffset, int? limitRowCount, string orderBy, bool asc)
        {
            using (Models.WGODBContext db = new Models.WGODBContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;

                // First get the full roster of data
                /*using (var ctx = new Models.WGODBContext())
                using (var cmd = ctx.Database.Connection.CreateCommand())
                {
                    ctx.Database.Connection.Open();
                    cmd.CommandText = "SELECT Name, Level, Class, AchievementPoints, MaxiLevel, EquippediLeve, LastModified FROM Character where roster = 1";
                    using (var reader = cmd.ExecuteReader())
                    {
                        var query = reader.Read();
                        */
                using (var context = new Models.WGODBContext())
                {
                    string sql = $"SELECT ID, Name, Level, Class, Race, AchievementPoints, Max_iLevel, Equipped_iLevel, LastUpdated as LastUpdated, Realm, Spec, Roster, Role, Items, Modified_Level, Modified_Max_iLevel, Modified_Equipped_iLevel, Modified_AchievementPoints," +
                        $"ROW_NUMBER() OVER (order by {(!string.IsNullOrWhiteSpace(orderBy) ? "level desc, Equipped_iLevel desc, Max_iLevel desc, AchievementPoints desc, name asc" : orderBy)}) as Rank " +
                        $"FROM Characters " +
                        $"where roster = 1 {(!string.IsNullOrWhiteSpace(globalSearch) ? $"and name like %{globalSearch}%" : "")}" +
                        $"order by {(!string.IsNullOrWhiteSpace(orderBy) ? "level desc, Equipped_iLevel desc, Max_iLevel desc, AchievementPoints desc" : orderBy)}";
                    var query = context.Database.SqlQuery<Models.Character>(sql);

                    // Get the total record count
                    totalRecords = query.Count();

                    foreach (Models.Character c in query)
                    {
                        int test = c.Rank;
                    }

                    // Paging
                    if (limitOffset.HasValue && limitRowCount.HasValue)
                    {
                        //query = query.Skip(limitOffset.Value).Take(limitRowCount.Value);
                        //query = query.Select($"Rank > ");
                    }
                    
                    // Return data as a List
                    return query.ToList();
                }
            }
        }

        public IEnumerable<Models.Character> GetData2(out int totalRecords, int roster, string globalSearch, int? limitOffset, int? limitRowCount, string orderBy, bool asc)
        {
            using (Models.WGODBContext db = new Models.WGODBContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;

                // First get the full roster of data
                var query = db.Characters/*.AsNoTracking()*/.AsQueryable().Where(s => s.Roster == roster);
                int rank = 1;

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
                        query = query.OrderByDescending(p => p.Level).ThenByDescending(p => p.Equipped_iLevel).ThenByDescending(p => p.Max_iLevel).ThenByDescending(p => p.AchievementPoints).ThenBy(p => p.Name);
                        break;
                }

                // Rank Testing...
                /*{
                    rank = 1;
                    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

                    sw.Start();
                    foreach (Models.Character c in query)
                    {
                        c.Rank = rank++;
                    }
                    sw.Stop();
                    var elapsedMs = sw.ElapsedMilliseconds;

                    rank = 1;
                    sw.Restart();
                    foreach (Models.Character c in query.ToList())
                    {
                        c.Rank = rank++;
                    }
                    sw.Stop();
                    elapsedMs = sw.ElapsedMilliseconds;

                    rank = 1;
                    sw.Restart();
                    query.ToList().ForEach(x => x.Rank = rank++);
                    sw.Stop();
                    elapsedMs = sw.ElapsedMilliseconds;

                    rank = 1;
                    sw.Restart();
                    foreach (var c in query.ToList())
                    {
                        c.Rank = rank++;
                    }
                    sw.Stop();
                    elapsedMs = sw.ElapsedMilliseconds;

                    List<int> rows = new List<int>();
                    rows.Add(5);
                    rows.Add(4);
                    rows.Add(3);
                    rows.Add(2);
                    rows.Add(1);

                    foreach (int i in rows)
                    {
                        int test = rows.IndexOf(i);
                    }
                }*/


                // Fill out the Rank field
                //query.ToList().ForEach(x => x.Rank = rank++);

                // Check to see if there is a search
                if (!string.IsNullOrWhiteSpace(globalSearch))
                {
                    query = query.Where(p => p.Name.Contains(globalSearch));
                }

                // Get the total record count
                totalRecords = query.Count();

                // Paging
                if (limitOffset.HasValue && limitRowCount.HasValue)
                {
                    query = query.Skip(limitOffset.Value).Take(limitRowCount.Value);
                }

                // Return data as a List
                return query.ToList();


                /*
                //List<Models.Character> result = query.ToList();
                var result = query.ToList();
                result.ToList().ForEach(x => x.Rank = rank++);
                if (!string.IsNullOrWhiteSpace(globalSearch))
                {
                    result = result.Where(p => p.Name.Contains(globalSearch));
                }

                // Get the total record count
                totalRecords = result.Count();

                // Paging
                if (limitOffset.HasValue && limitRowCount.HasValue)
                {
                    result = result.Skip(limitOffset.Value).Take(limitRowCount.Value);
                }
                return result;
                */
            }
        }
    }
}
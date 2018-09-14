[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(WGO.MVCGridConfig), "RegisterGrids")]

namespace WGO
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using JSON;
    using MVCGrid.Models;
    using MVCGrid.Web;

    public static class MVCGridConfig
    {
        /// <summary>
        /// Register all the MVCGrid.Net
        /// </summary>
        public static void RegisterGrids()
        {
            #region " MVCGrid.Net Example "
            /* Example: 
            MVCGridDefinitionTable.Add("UsageExample", new MVCGridBuilder<YourModelItem>()
                .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                .AddColumns(cols =>
                {
                    // Add your columns here
                    cols.Add().WithColumnName("UniqueColumnName")
                        .WithHeaderText("Any Header")
                        .WithValueExpression(i => i.YourProperty); // use the Value Expression to return the cell text for this column
                    cols.Add().WithColumnName("UrlExample")
                        .WithHeaderText("Edit")
                        .WithValueExpression((i, c) => c.UrlHelper.Action("detail", "demo", new { id = i.Id }));
                })
                .WithRetrieveDataMethod((context) =>
                {
                    // Query your data here. Obey Ordering, paging and filtering parameters given in the context.QueryOptions.
                    // Use Entity Framework, a module from your IoC Container, or any other method.
                    // Return QueryResult object containing IEnumerable<YouModelItem>

                    return new QueryResult<YourModelItem>()
                    {
                        Items = new List<YourModelItem>(),
                        TotalRecords = 0 // if paging is enabled, return the total number of records of all pages
                    };

                })
            );
            */
            #endregion

            #region " The Search Page Grid "
            // The Search Page
            MVCGridDefinitionTable.Add("SearchGrid", new MVCGridBuilder<Models.Character>()
                .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                .AddColumns(cols =>
                {
                    // Add your columns here
                    cols.Add().WithColumnName("Name")
                        .WithHeaderText("Name")
                        .WithSorting(true)
                        .WithValueTemplate("<a href='{Value}'>{Model.Name}</a>", false)
                        .WithValueExpression((p, c) => c.UrlHelper.Action("Character", "WGO", new { name = p.Name, realm = p.Realm }));
                    cols.Add().WithColumnName("Level")
                        .WithHeaderText("Level")
                        .WithSorting(true)
                        .WithValueExpression(i => i.Level.ToString())
                        .WithCellCssClassExpression(p => DateModifiedLately(p.Modified_Level) ? "table-success" : "");
                    cols.Add().WithColumnName("Class")
                        .WithSorting(true)
                        .WithValueExpression(i => i.Class);
                    cols.Add().WithColumnName("AchievementPoints")
                        .WithHeaderText("Achievement Points")
                        .WithSorting(true)
                        .WithValueExpression(i => i.AchievementPoints.ToString())
                        .WithCellCssClassExpression(p => DateModifiedLately(p.Modified_AchievementPoints) ? "table-success" : "");
                    cols.Add().WithColumnName("MaxiLevel")
                        .WithHeaderText("Max iLevel")
                        .WithSorting(true)
                        .WithValueExpression(i => i.Max_iLevel.ToString())
                        .WithCellCssClassExpression(p => DateModifiedLately(p.Modified_Max_iLevel) ? "table-success" : "");
                    cols.Add().WithColumnName("EquippediLevel")
                        .WithHeaderText("Equipped iLevel")
                        .WithSorting(true)
                        .WithValueExpression(i => i.Equipped_iLevel.ToString())
                        .WithCellCssClassExpression(p => DateModifiedLately(p.Modified_Equipped_iLevel) ? "table-success" : "");
                    cols.Add().WithColumnName("LastModified")
                        .WithHeaderText("Last Modified")
                        .WithSorting(true)
                        .WithValueExpression(i => i.LastUpdated.ToString());
                })
                .WithPageParameterNames("name")
                .WithPageParameterNames("realm")
                .WithPreloadData(false)
                .WithSorting(true, "EquippediLevel")
                .WithRetrieveDataMethod((context) =>
                {
                    // Query your data here. Obey Ordering, paging and filtering parameters given in the context.QueryOptions.
                    // Use Entity Framework, a module from your IoC Container, or any other method.
                    // Return QueryResult object containing IEnumerable<YouModelItem>
                    var options = context.QueryOptions;
                    string searchName = options.GetPageParameterString("name");
                    string searchRealm = options.GetPageParameterString("realm");
                    Models.WGODBContext db = new Models.WGODBContext();
                    var query = db.Characters.Where(s => s.Name == searchName && s.Realm == searchRealm).OrderByDescending(s => s.Equipped_iLevel);

                    int itemCount = query.Count();

                    return new QueryResult<Models.Character>()
                    {
                        // Return the List of Characters
                        Items = query,

                        // if paging is enabled, return the total number of records of all pages
                        TotalRecords = query.Count()
                    };

                })
            );
            #endregion

            #region " The Guild Page Grid "
            // The Guild Page
            MVCGridDefinitionTable.Add("GuildRoster", new MVCGridBuilder<WGO.Models.Character>()
                .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                .WithSorting(sorting: true, defaultSortColumn: "EquippediLevel", defaultSortDirection: SortDirection.Dsc)
                .WithPaging(paging: true, itemsPerPage: 10, allowChangePageSize: true, maxItemsPerPage: 100)
                .WithAdditionalQueryOptionNames("search")
                .AddColumns(cols =>
                {
                    // Add your columns here
                    cols.Add().WithColumnName("Name")
                        .WithHeaderText("Name")
                        .WithSorting(true)
                        .WithValueTemplate("<a href='{Value}'>{Model.Name}</a>", false)
                        .WithValueExpression((p, c) => c.UrlHelper.Action("Character", "WGO", new { name = p.Name, realm = p.Realm }));
                    cols.Add().WithColumnName("Level")
                        .WithHeaderText("Level")
                        .WithSorting(true)
                        .WithValueExpression(i => i.Level.ToString())
                        .WithCellCssClassExpression(p => DateModifiedLately(p.Modified_Level) ? "table-success" : "");
                    cols.Add().WithColumnName("Class")
                        .WithSorting(true)
                        .WithValueExpression(i => i.Class);
                    cols.Add().WithColumnName("AchievementPoints")
                        .WithHeaderText("Achievement Points")
                        .WithSorting(true)
                        .WithValueExpression(i => i.AchievementPoints.ToString())
                        .WithCellCssClassExpression(p => DateModifiedLately(p.Modified_AchievementPoints) ? "table-success" : "");
                    cols.Add().WithColumnName("MaxiLevel")
                        .WithHeaderText("Max iLevel")
                        .WithSorting(true)
                        .WithValueExpression(i => i.Max_iLevel.ToString())
                        .WithCellCssClassExpression(p => DateModifiedLately(p.Modified_Max_iLevel) ? "table-success" : "");
                    cols.Add().WithColumnName("EquippediLevel")
                        .WithHeaderText("Equipped iLevel")
                        .WithSorting(true)
                        .WithValueExpression(i => i.Equipped_iLevel.ToString())
                        .WithCellCssClassExpression(p => DateModifiedLately(p.Modified_Equipped_iLevel) ? "table-success" : "");
                    cols.Add().WithColumnName("LastModified")
                        .WithHeaderText("Last Modified")
                        .WithSorting(true)
                        .WithValueExpression(i => i.LastUpdated.ToString());
                    cols.Add("Rescan").WithHtmlEncoding(false)
                        .WithSorting(false)
                        .WithHeaderText(" ")
                        .WithValueExpression((p, c) => c.UrlHelper.Action("Rescan", "WGO", new { name = p.Name, realm = p.Realm }))
                        .WithValueTemplate("<a href='{Value}' class='btn btn-primary' role='button'>Reload</a>");
                })
                .WithRetrieveDataMethod((context) =>
                {
                    // Query your data here. Obey Ordering, paging and filtering parameters given in the context.QueryOptions.
                    // Use Entity Framework, a module from your IoC Container, or any other method.
                    // Return QueryResult object containing IEnumerable<YouModelItem>
                    var options = context.QueryOptions;
                    string globalSearch = options.GetAdditionalQueryOptionString("search") == null ? string.Empty : options.GetAdditionalQueryOptionString("search");
                    string sortColumn = options.GetSortColumnData<string>() == null ? string.Empty : options.GetSortColumnData<string>();
                    var result = new QueryResult<Models.Character>();
                    int guildRoster = JSONBase.GetGuildRoster();

                    // Dependency Testing...
                    //CharacterRepository charRepo2 = new CharacterRepository();
                    int totalRecords = 0;
                    var repo = DependencyResolver.Current.GetService<ICharacterRepository>();

                    if (repo != null)
                    {
                        var test = repo.GetData(out totalRecords, globalSearch, options.GetLimitOffset(), options.GetLimitRowcount(), sortColumn, options.SortDirection == SortDirection.Asc);
                        System.Diagnostics.Debug.WriteLine($"DEBUG: TotalRecords = {totalRecords}.");

                        result.TotalRecords = totalRecords;
                        result.Items = test;
                    }
                    else
                    {
                        // Get the current data now...
                        using (var db = new Models.WGODBContext())
                        {
                            // Get the data
                            var query = db.Characters.AsQueryable().Where(s => s.Roster == guildRoster);

                            // Sort the data
                            switch (options.SortColumnName.ToLower())
                            {
                                case "name":
                                    query = options.SortDirection == SortDirection.Asc ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name);
                                    break;

                                case "level":
                                    query = options.SortDirection == SortDirection.Asc ? query.OrderBy(p => p.Level) : query.OrderByDescending(p => p.Level);
                                    break;

                                case "class":
                                    query = options.SortDirection == SortDirection.Asc ? query.OrderBy(p => p.Class) : query.OrderByDescending(p => p.Class);
                                    break;

                                case "achievementpoints":
                                    query = options.SortDirection == SortDirection.Asc ? query.OrderBy(p => p.AchievementPoints) : query.OrderByDescending(p => p.AchievementPoints);
                                    break;

                                case "maxilevel":
                                    query = query.OrderByDescending(p => p.Level).ThenByDescending(p => p.Max_iLevel).ThenBy(p => p.Name);
                                    break;

                                case "equippedilevel":
                                    query = query.OrderByDescending(p => p.Level).ThenByDescending(p => p.Equipped_iLevel).ThenBy(p => p.Name);
                                    break;

                                case "lastmodified":
                                    query = options.SortDirection == SortDirection.Asc ? query.OrderBy(p => p.LastUpdated) : query.OrderByDescending(p => p.LastUpdated);
                                    break;

                                default:
                                    query = query.OrderByDescending(p => p.Level).ThenByDescending(p => p.Equipped_iLevel).ThenBy(p => p.Name);
                                    break;
                            }

                            result.TotalRecords = query.Count();

                            // Paging
                            if (options.GetLimitOffset().HasValue)
                            {
                                query = query.Skip(options.GetLimitOffset().Value).Take(options.GetLimitRowcount().Value);
                            }

                            // Done!
                            result.Items = query.ToList();
                        }
                    }
                    
                    return result;
                })
            );
            #endregion

            #region " The Raid Page Grid "
            // The Raid page
            MVCGridDefinitionTable.Add("RaidRoster", new MVCGridBuilder<Models.Character>()
                .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                .AddColumns(cols =>
                {
                    // Add your columns here
                    cols.Add().WithColumnName("Role")
                        .WithHeaderText("Role")
                        .WithSorting(true)
                        .WithValueExpression(i => i.Role);
                    cols.Add().WithColumnName("Name")
                        .WithHeaderText("Name")
                        .WithSorting(true)
                        .WithValueTemplate("<a href='{Value}'>{Model.Name}</a>", false)
                        .WithValueExpression((p, c) => c.UrlHelper.Action("Character", "WGO", new { name = p.Name, realm = p.Realm }));
                    cols.Add().WithColumnName("Level")
                        .WithHeaderText("Level")
                        .WithSorting(true)
                        .WithValueExpression(i => i.Level.ToString())
                        .WithCellCssClassExpression(p => DateModifiedLately(p.Modified_Level) ? "table-success" : "");
                    cols.Add().WithColumnName("Class")
                        .WithSorting(true)
                        .WithValueExpression(i => i.Class);
                    cols.Add().WithColumnName("AchievementPoints")
                        .WithHeaderText("Achievement Points")
                        .WithSorting(true)
                        .WithValueExpression(i => i.AchievementPoints.ToString())
                        .WithCellCssClassExpression(p => DateModifiedLately(p.Modified_AchievementPoints) ? "table-success" : "");
                    cols.Add().WithColumnName("MaxiLevel")
                        .WithHeaderText("Max iLevel")
                        .WithSorting(true)
                        .WithValueExpression(i => i.Max_iLevel.ToString())
                        .WithCellCssClassExpression(p => DateModifiedLately(p.Modified_Max_iLevel) ? "table-success" : "");
                    cols.Add().WithColumnName("EquippediLevel")
                        .WithHeaderText("Equipped iLevel")
                        .WithSorting(true)
                        .WithValueExpression(i => i.Equipped_iLevel.ToString())
                        .WithCellCssClassExpression(p => DateModifiedLately(p.Modified_Equipped_iLevel) ? "table-success" : "");
                    cols.Add().WithColumnName("LastModified")
                        .WithHeaderText("Last Modified")
                        .WithSorting(true)
                        .WithValueExpression(i => i.LastUpdated.ToString());
                    cols.Add("Recan").WithHtmlEncoding(false)
                        .WithSorting(false)
                        .WithHeaderText(" ")
                        .WithValueExpression((p, c) => c.UrlHelper.Action("Rescan", "WGO", new { name = p.Name, realm = p.Realm, role = p.Role }))
                        .WithValueTemplate("<a href='{Value}' class='btn btn-primary' role='button' onclick=$.blockUI()>Rescan</a>");
                    cols.Add("Delete").WithHtmlEncoding(false)
                        .WithSorting(false)
                        .WithHeaderText(" ")
                        .WithValueExpression((p, c) => c.UrlHelper.Action("Delete", "WGO", new { name = p.Name, realm = p.Realm, role = p.Role }))
                        .WithValueTemplate("<a href='{Value}' class='btn btn-danger' role='button' onclick=$.blockUI()>Delete</a>");
                })
                .WithPreloadData(false)
                .WithSorting(sorting: true, defaultSortColumn: "EquippediLevel", defaultSortDirection: SortDirection.Dsc)
                .WithPaging(true, 20)
                .WithClientSideLoadingCompleteFunctionName("hideLoading")
                .WithRetrieveDataMethod((context) =>
                {
                    var options = context.QueryOptions;
                    string globalSearch = options.GetAdditionalQueryOptionString("search");
                    string sortColumn = options.GetSortColumnData<string>();
                    var result = new QueryResult<Models.Character>();
                    int raidRoster = JSONBase.GetRaidRoster();
                    
                    // Get the current data now...
                    using (var db = new Models.WGODBContext())
                    {
                        // Get the data
                        var query = db.Characters.AsQueryable().Where(s => s.Roster == raidRoster);

                        // Sort the data
                        switch (options.SortColumnName.ToLower())
                        {
                            case "role":
                                query = query.OrderByDescending(p => p.Role).ThenByDescending(p => p.Level).ThenByDescending(p => p.Equipped_iLevel).ThenBy(p => p.Name);
                                break;

                            case "name":
                                query = options.SortDirection == SortDirection.Asc ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name);
                                break;

                            case "level":
                                query = options.SortDirection == SortDirection.Asc ? query.OrderBy(p => p.Level) : query.OrderByDescending(p => p.Level);
                                break;

                            case "class":
                                query = options.SortDirection == SortDirection.Asc ? query.OrderBy(p => p.Class) : query.OrderByDescending(p => p.Class);
                                break;

                            case "achievementpoints":
                                query = options.SortDirection == SortDirection.Asc ? query.OrderBy(p => p.AchievementPoints) : query.OrderByDescending(p => p.AchievementPoints);
                                break;

                            case "maxilevel":
                                query = query.OrderByDescending(p => p.Role).ThenByDescending(p => p.Level).ThenByDescending(p => p.Max_iLevel).ThenBy(p => p.Name);
                                break;

                            case "equippedilevel":
                                query = query.OrderByDescending(p => p.Role).ThenByDescending(p => p.Level).ThenByDescending(p => p.Equipped_iLevel).ThenBy(p => p.Name);
                                break;

                            case "lastmodified":
                                query = options.SortDirection == SortDirection.Asc ? query.OrderBy(p => p.LastUpdated) : query.OrderByDescending(p => p.LastUpdated);
                                break;

                            default:
                                query = query.OrderByDescending(p => p.Role).ThenByDescending(p => p.Level).ThenByDescending(p => p.Equipped_iLevel).ThenBy(p => p.Name);
                                break;
                        }

                        // Get the full record count, before the paging
                        result.TotalRecords = query.Count();

                        // Paging
                        if (options.GetLimitOffset().HasValue)
                        {
                            query = query.Skip(options.GetLimitOffset().Value).Take(options.GetLimitRowcount().Value);
                        }

                        // Done!
                        result.Items = query.ToList();
                    }

                    return result;
                })
            );
            #endregion
        }

        #region " Private Static Functions "
        /// <summary>
        /// Checks to see if a date has been modified lately.
        /// What is lately?  Good question! 2 to 4 hours seems to be about right at this moment.
        /// </summary>
        /// <param name="date">The DataTime to compare.</param>
        /// <returns>True if Date has been modified lately.</returns>
        private static bool DateModifiedLately(DateTime? date)
        {
            bool result = false;

            if (date != null)
            {
                // Is this date less than 2 hours old?  All Central Time Zone
                DateTime clientDateTime = DateTime.Now;
                DateTime centralDateTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(clientDateTime, "Central Standard Time");
                if (centralDateTime.AddHours(-2) <= date.Value)
                {
                    result = true;
                }
            }
            else
            {
                result = true;
            }

            return result;
        }
        #endregion
    }
}

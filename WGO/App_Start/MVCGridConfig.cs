[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(WGO.MVCGridConfig), "RegisterGrids")]

namespace WGO
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Linq;
    using System.Collections.Generic;

    using MVCGrid.Models;
    using MVCGrid.Web;
    using WGO.JSON;

    public static class MVCGridConfig 
    {
        public static void RegisterGrids()
        {
            
            /*
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

            // The Search Page
            MVCGridDefinitionTable.Add("SearchGrid", new MVCGridBuilder<Models.Character>()
                .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                .AddColumns(cols =>
                {
                    // Add your columns here
                    cols.Add().WithColumnName("Name")
                        .WithHeaderText("Name")
                        .WithSorting(true)
                        .WithValueExpression(i => i.Name); // use the Value Expression to return the cell text for this column
                    cols.Add().WithColumnName("Level")
                        .WithHeaderText("Level")
                        .WithSorting(true)
                        .WithValueExpression(i => i.Level.ToString());
                    cols.Add().WithColumnName("Class")
                        .WithSorting(true)
                        .WithValueExpression(i => i.Class);
                    cols.Add().WithColumnName("AchievementPoints")
                        .WithHeaderText("Achievement Points")
                        .WithSorting(true)
                        .WithValueExpression(i => i.AchievementPoints.ToString());
                    cols.Add().WithColumnName("MaxiLevel")
                        .WithHeaderText("Max iLevel")
                        .WithSorting(true)
                        .WithValueExpression(i => i.Max_iLevel.ToString());
                    cols.Add().WithColumnName("EquippediLevel")
                        .WithHeaderText("Equipped iLevel")
                        .WithSorting(true)
                        .WithValueExpression(i => i.Equipped_iLevel.ToString());
                    cols.Add().WithColumnName("LastModified")
                        .WithHeaderText("Last Modified")
                        .WithSorting(true)
                        .WithValueExpression(i => i.LastUpdated.ToString());
                })
                .WithPageParameterNames("name")
                .WithPageParameterNames("realm")
                .WithPreloadData(false)
                .WithSorting(true, "Level, Name, EquippediLevel")
                .WithPaging(true, 20)
                
                .WithRetrieveDataMethod((context) =>
                {
                    // Query your data here. Obey Ordering, paging and filtering parameters given in the context.QueryOptions.
                    // Use Entity Framework, a module from your IoC Container, or any other method.
                    // Return QueryResult object containing IEnumerable<YouModelItem>
                    var options = context.QueryOptions;
                    string searchName = options.GetPageParameterString("name");
                    string searchRealm = options.GetPageParameterString("realm");
                    Models.WGODBContext db = new Models.WGODBContext();
                    var dbChars = from m in db.Characters select m;
                    var search = dbChars.Where(s => s.Name == searchName && s.Realm == searchRealm);

                    return new QueryResult<Models.Character>()
                    {
                        // Return the List of Characters
                        Items = search,

                        // if paging is enabled, return the total number of records of all pages
                        TotalRecords = search.Count() 
                    };

                })
            );

            // The Guild Page
            MVCGridDefinitionTable.Add("GuildRoster", new MVCGridBuilder<WGO.Models.Character>()
                .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                .AddColumns(cols =>
                {
                    // Add your columns here
                    cols.Add().WithColumnName("Name")
                        .WithHeaderText("Name")
                        .WithSorting(true)
                        .WithValueExpression(i => i.Name); // use the Value Expression to return the cell text for this column
                    cols.Add().WithColumnName("Level")
                        .WithHeaderText("Level")
                        .WithSorting(true)
                        .WithValueExpression(i => i.Level.ToString());
                    cols.Add().WithColumnName("Class")
                        .WithSorting(true)
                        .WithValueExpression(i => i.Class);
                    cols.Add().WithColumnName("AchievementPoints")
                        .WithHeaderText("Achievement Points")
                        .WithSorting(true)
                        .WithValueExpression(i => i.AchievementPoints.ToString());
                    cols.Add().WithColumnName("MaxiLevel")
                        .WithHeaderText("Max iLevel")
                        .WithSorting(true)
                        .WithValueExpression(i => i.Max_iLevel.ToString());
                    cols.Add().WithColumnName("EquippediLevel")
                        .WithHeaderText("Equipped iLevel")
                        .WithSorting(true)
                        .WithValueExpression(i => i.Equipped_iLevel.ToString());
                    cols.Add().WithColumnName("LastModified")
                        .WithHeaderText("Last Modified")
                        .WithSorting(true)
                        .WithValueExpression(i => i.LastUpdated.ToString());
                    cols.Add("Reload").WithHtmlEncoding(false)
                        .WithSorting(false)
                        .WithHeaderText(" ")
                        .WithValueExpression((p, c) => c.UrlHelper.Action("Guild", "WGO", new { id = "Reload" }))
                        .WithValueTemplate("<a href='{Value}' class='btn btn-primary' role='button'>Reload</a>");
                })
                .WithPreloadData(false)
                .WithSorting(true, "Level, Name, EquippediLevel")
                .WithPaging(true, 20)
                .WithRetrieveDataMethod((context) =>
                {
                    // Query your data here. Obey Ordering, paging and filtering parameters given in the context.QueryOptions.
                    // Use Entity Framework, a module from your IoC Container, or any other method.
                    // Return QueryResult object containing IEnumerable<YouModelItem>
                    Models.WGODBContext db = new Models.WGODBContext();
                    var dbChars = from m in db.Characters select m;
                    var guild = dbChars.Where(s => s.Roster == 1 || s.Roster == 3);

                    return new QueryResult<Models.Character>()
                    {
                        // Return a list of the characters
                        Items = guild,

                        // if paging is enabled, return the total number of records of all pages
                        TotalRecords = guild.Count()
                };

                })
            );

            // The Raid page
            MVCGridDefinitionTable.Add("RaidRoster", new MVCGridBuilder<Models.Character>()
                .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                .AddColumns(cols =>
                {
                    // Add your columns here
                    cols.Add().WithColumnName("Name")
                        .WithHeaderText("Name")
                        .WithSorting(true)
                        .WithValueExpression(i => i.Name); // use the Value Expression to return the cell text for this column
                    cols.Add().WithColumnName("Level")
                        .WithHeaderText("Level")
                        .WithSorting(true)
                        .WithValueExpression(i => i.Level.ToString());
                    cols.Add().WithColumnName("Class")
                        .WithSorting(true)
                        .WithValueExpression(i => i.Class);
                    cols.Add().WithColumnName("AchievementPoints")
                        .WithHeaderText("Achievement Points")
                        .WithSorting(true)
                        .WithValueExpression(i => i.AchievementPoints.ToString());
                    cols.Add().WithColumnName("MaxiLevel")
                        .WithHeaderText("Max iLevel")
                        .WithSorting(true)
                        .WithValueExpression(i => i.Max_iLevel.ToString());
                    cols.Add().WithColumnName("EquippediLevel")
                        .WithHeaderText("Equipped iLevel")
                        .WithSorting(true)
                        .WithValueExpression(i => i.Equipped_iLevel.ToString());
                    cols.Add().WithColumnName("LastModified")
                        .WithHeaderText("Last Modified")
                        .WithSorting(true)
                        .WithValueExpression(i => i.LastUpdated.ToString());
                    cols.Add("Reload").WithHtmlEncoding(false)
                        .WithSorting(false)
                        .WithHeaderText(" ")
                        .WithValueExpression((p, c) => c.UrlHelper.Action("Raid", "WGO", new { id = "Reload" }))
                        .WithValueTemplate("<a href='{Value}' class='btn btn-primary' role='button'>Reload</a>");
                })
                .WithPreloadData(false)
                .WithSorting(true, "Level, Name, EquippediLevel")
                .WithPaging(true, 20)
                .WithRetrieveDataMethod((context) =>
                {
                    // Query your data here. Obey Ordering, paging and filtering parameters given in the context.QueryOptions.
                    // Use Entity Framework, a module from your IoC Container, or any other method.
                    // Return QueryResult object containing IEnumerable<YouModelItem>
                    Models.WGODBContext db = new Models.WGODBContext();
                    var dbChars = from m in db.Characters select m;
                    var raider = dbChars.Where(s => s.Roster == 2 || s.Roster == 3);

                    return new QueryResult<Models.Character>()
                    {
                        // The list of characters
                        Items = raider,

                        // if paging is enabled, return the total number of records of all pages
                        TotalRecords = raider.Count() 
                    };

                })
            );

            // The Raid page - Rescan
            MVCGridDefinitionTable.Add("RescanRaidRoster", new MVCGridBuilder<JSONCharacter>()
                .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                .AddColumns(cols =>
                {
                    // Add your columns here
                    cols.Add().WithColumnName("Name")
                        .WithHeaderText("Name")
                        .WithSorting(true)
                        .WithValueExpression(i => i.Name); // use the Value Expression to return the cell text for this column
                    cols.Add().WithColumnName("Level")
                        .WithHeaderText("Level")
                        .WithSorting(true)
                        .WithValueExpression(i => i.Level.ToString());
                    cols.Add().WithColumnName("Class")
                        .WithSorting(true)
                        .WithValueExpression(i => JSONBase.ConvertClass(i.Class));
                    cols.Add().WithColumnName("AchievementPoints")
                        .WithHeaderText("Achievement Points")
                        .WithSorting(true)
                        .WithValueExpression(i => i.AchievementPoints.ToString());
                    cols.Add().WithColumnName("MaxiLevel")
                        .WithHeaderText("Max iLevel")
                        .WithSorting(true)
                        .WithValueExpression(i => i.Items.AverageItemLevel.ToString());
                    cols.Add().WithColumnName("EquippediLevel")
                        .WithHeaderText("Equipped iLevel")
                        .WithSorting(true)
                        .WithValueExpression(i => i.Items.AverageItemLevelEquipped.ToString());
                    cols.Add().WithColumnName("LastModified")
                        .WithHeaderText("Last Modified")
                        .WithSorting(true)
                        .WithValueExpression(i => i.LastModified.ToString());
                    cols.Add("Reload").WithHtmlEncoding(false)
                        .WithSorting(false)
                        .WithHeaderText(" ")
                        .WithValueExpression((p, c) => c.UrlHelper.Action("Raid", "WGO", new { id = "Reload" }))
                        .WithValueTemplate("<a href='{Value}' class='btn btn-primary' role='button'>Reload</a>");
                })
                .WithQueryOnPageLoad(false)
                .WithPreloadData(false)
                .WithSorting(true, "Level, Name, EquippediLevel")
                .WithPaging(true, 20)
                .WithRetrieveDataMethod((context) =>
                {
                    // Query your data here. Obey Ordering, paging and filtering parameters given in the context.QueryOptions.
                    // Use Entity Framework, a module from your IoC Container, or any other method.
                    // Return QueryResult object containing IEnumerable<YouModelItem>
                    int totalRecords = 3;
                    var items = new List<JSONCharacter>();

                    items.Add(JSONBase.GetCharacterJSONData("Purdee", "Thrall"));
                    items.Add(JSONBase.GetCharacterJSONData("Mervrick", "Thrall"));
                    items.Add(JSONBase.GetCharacterJSONData("Elinga", "Thrall"));

                    return new QueryResult<JSONCharacter>()
                    {
                        // Return a list of characters
                        Items = items,

                        // if paging is enabled, return the total number of records of all pages
                        TotalRecords = totalRecords 
                    };

                })
            );
        }
    }
}
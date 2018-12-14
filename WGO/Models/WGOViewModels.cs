using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WGO.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class WGOViewModels
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public class SearchViewModel
    {
        [Required]
        [Display(Name = "Realm")]
        public string Realm { get; set; }

        [Required]
        [Display(Name = "Character")]
        public string Character { get; set; }

        [Display(Name = "Role")]
        public string Role { get; set; }
        public IEnumerable<System.Web.Mvc.SelectListItem> Roles { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CharacterViewModel
    {
        public Character Character { get; set; }
        //public JSON.JSONCharacterAudit Audit { get; set; }
        public string AuditHtml { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class GuildNewsViewModel
    {
        public IList<string> GuildNews { get; set; }
    }
}
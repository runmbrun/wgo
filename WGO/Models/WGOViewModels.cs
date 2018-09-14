using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WGO.Models
{
    public class WGOViewModels
    {
    }

    public class SearchViewModel
    {
        [Required]
        [Display(Name = "Realm")]
        public string Realm { get; set; }

        [Required]
        [Display(Name = "Character")]
        public string Character { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; }
        public IEnumerable<System.Web.Mvc.SelectListItem> Roles { get; set; }
    }

    public class CharacterViewModel
    {
        public Character Character { get; set; }
        public JSON.JSONCharacterAudit Audit { get; set; }
    }
}
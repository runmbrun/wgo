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
    }
}
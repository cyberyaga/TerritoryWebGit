using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TerritoryWeb.Models
{
    [MetadataTypeAttribute(typeof(LanguageHelper))]
    public partial class Language 
    {
         
    }

    public class LanguageHelper
    {
        [Required]
        [Display(Name = "LanguageDesc", ResourceType = typeof(Resources.Models.Language))]
        public string Description { get; set; }
    }
}
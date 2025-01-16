using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TerritoryWeb.Models
{
    [MetadataTypeAttribute(typeof (DoorHelper))]
    public partial class Door
    {
        public List<SelectListItem> ListOfLanguagesDP { get; set; }
    }

    public class DoorHelper
    {
        [Display(Name = "TerritoryID", ResourceType = typeof(Resources.Models.Door))]
        public int TerritoryID { get; set; }

        [Display(Name = "Address", ResourceType = typeof(Resources.Models.Door))]
        [Required]
        public string Address { get; set; }

        [Display(Name = "Apartment", ResourceType = typeof(Resources.Models.Door))]
        public string Apartment { get; set; }

        [Display(Name = "Street", ResourceType = typeof(Resources.Models.Door))]
        [Required]
        public string Street { get; set; }

        [Display(Name = "Language", ResourceType = typeof(Resources.Models.Door))]
        public Nullable<int> LanguageID { get; set; }

        [Display(Name = "Comments", ResourceType = typeof(Resources.Models.Door))]
        public string Comments { get; set; }
        
        [Display(Name = "Name", ResourceType = typeof(Resources.Models.Door))]
        public string Name { get; set; }

        [Display(Name = "Telephone", ResourceType = typeof(Resources.Models.Door))]
        public string Telephone { get; set; }

        [Display(Name = "PostalCode", ResourceType = typeof(Resources.Models.Door))]
        public string PostalCode { get; set; }

        [Display(Name = "DoorCode", ResourceType = typeof(Resources.Models.Door))]
        public Nullable<int> CodeID { get; set; }
    }
}
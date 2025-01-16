using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TerritoryWeb.Models
{
    [MetadataTypeAttribute(typeof(TerritoryHelper))]
    public partial class Territory
    {

    }

    public class TerritoryHelper
    {
        public int Id { get; set; }

        [Display(Name = "TerritoryName", ResourceType = typeof(Resources.Models.Territory))]
        [Required]
        public string TerritoryName { get; set; }

        [Display(Name = "City", ResourceType = typeof(Resources.Models.Territory))]
        [Required]
        public string City { get; set; }

        [Display(Name = "Type", ResourceType = typeof(Resources.Models.Territory))]
        [Required]
        public int Type { get; set; }

        [Display(Name = "Notes", ResourceType = typeof(Resources.Models.Territory))]
        public string Notes { get; set; }
        public int CongregationID { get; set; }

        [Display(Name = "AssignedPublisherID", ResourceType = typeof(Resources.Models.Territory))]
        public string AssignedPublisherID { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime Added { get; set; }
        public string ModifiedBy { get; set; }
        public System.DateTime Modified { get; set; }

        [Display(Name = "CheckedOut", ResourceType = typeof(Resources.Models.Territory))]
        public Nullable<System.DateTime> CheckedOut { get; set; }

        [Display(Name = "CheckedIn", ResourceType = typeof(Resources.Models.Territory))]
        public Nullable<System.DateTime> CheckedIn { get; set; }

        [Display(Name = "LastCheckedInBy", ResourceType = typeof(Resources.Models.Territory))]
        public string LastCheckedInBy { get; set; }
    
    }
}
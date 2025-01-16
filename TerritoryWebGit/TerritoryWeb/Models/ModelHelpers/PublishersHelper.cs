using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TerritoryWeb.Models
{

    [MetadataTypeAttribute(typeof(PublishersHelper))]
    public partial class AspNetUser
    {
        [DisplayName("Name")]
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
    }
    public class PublishersHelper
    {
        [Display(Name = "FirstName", ResourceType = typeof(Resources.Models.ModelAspNetUser))]
        public string FirstName { get; set; }

        [Display(Name = "LastName", ResourceType = typeof(Resources.Models.ModelAspNetUser))]
        public string LastName { get; set; }
    }
}
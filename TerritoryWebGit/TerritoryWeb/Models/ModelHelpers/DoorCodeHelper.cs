using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TerritoryWeb.Models
{
    [MetadataTypeAttribute(typeof(DoorCodeHelper))]
    public partial class DoorCode
    {

    }


    public class DoorCodeHelper
    {
        [DisplayName("Door Code")]
        [Required]
        public string Description { get; set; }
    }
}
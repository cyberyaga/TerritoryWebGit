using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TerritoryWeb.Models
{
    [MetadataTypeAttribute(typeof(CongregationHelper))]
    public partial class Congregation
    {

    }

    public class CongregationHelper
    {
        [DisplayName("Congregation")]
        public string Description { get; set; }

    }
}
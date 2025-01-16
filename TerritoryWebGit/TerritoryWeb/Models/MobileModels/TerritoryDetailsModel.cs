using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace TerritoryWeb.Models.MobileModels
{
    public class TerritoryDetailsModel
    {
        public int TerritoryID { get; set; }
        public string TerritoryName { get; set; }
        public int DoorCount { get; set; }
        public string City { get; set; }
        public string TerritoryType { get; set; }
        public string AssignedPublisher { get; set; }
        public DateTime? AssignedDate { get; set; }
        public List<PointF> HullCoordinates { get; set; }

    }
}
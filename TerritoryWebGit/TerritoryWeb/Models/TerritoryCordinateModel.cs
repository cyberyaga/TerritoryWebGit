using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace TerritoryWeb.Models
{
    public class TerritoryCordinateModel
    {
        public TerritoryCordinateModel()
        {
            HullCoordinates = new List<PointF>();
            DoorCoordinates = new List<PointF>();
        }

        public int TerritoryID { get; set; }
        public string TerritoryName { get; set; }
        public List<PointF> HullCoordinates { get; set; }
        public List<PointF> DoorCoordinates { get; set; }
    }

    public class TerritoryCordinateIDModel
    {
        public TerritoryCordinateIDModel()
        {
            HullCoordinates = new List<PointF>();
            DoorCoordinates = new List<DoorIDModel>();
        }

        public List<PointF> HullCoordinates { get; set; }
        public List<DoorIDModel> DoorCoordinates { get; set; }

        public class DoorIDModel
        {
            public int DoorID { get; set; }
            public string Address { get; set; }
            public PointF Coordinates { get; set; }
        }
    }
}
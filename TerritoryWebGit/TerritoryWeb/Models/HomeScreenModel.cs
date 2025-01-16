using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TerritoryWeb.Models
{
    public class HomeScreenModel
    {
        public List<CheckedInTerritoriesModel> CheckedInTerritories { get; set; }
        public List<TerritoriesModifiedAdded> TerritoriesAdded { get; set; }
        public List<TerritoriesModifiedAdded> TerritoriesModified { get; set; }
        public bool ShowDash { get; set; }

        public class CheckedInTerritoriesModel
        {
            public int TerritoryID { get; set; }
            public string Territory { get; set; }
            public DateTime CheckedInTime { get; set; }
        }

        public class TerritoriesModifiedAdded
        {
            public int TerritoryID { get; set; }
            public string Territory { get; set; }
            public int count { get; set; }
            public DateTime AddedModDate { get; set; }
        }
    }
}
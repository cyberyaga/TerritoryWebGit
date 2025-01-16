using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TerritoryWeb.Common;

namespace TerritoryWeb.Models.Reports
{
    public class TerritoryPrintOutModel
    {
        public int TerritoryID { get; set; }
        public string TerritoryName { get; set; }
        public int DoorID { get; set; }
        public string DoorAddress { get; set; }
        public string DoorApartment { get; set; }
        public string DoorStreet { get; set; }
        public string DoorComments { get; set; }
    
        public static List<TerritoryPrintOutModel> GetTerritoryPrintout(int TerritoryID, int CongID, bool ProximitySort = false)
        {

            List<TerritoryPrintOutModel> results = new List<TerritoryPrintOutModel>();

            using (TerritoryWebEntities db = new TerritoryWebEntities())
            {
                //Get Data
                var doors = (from t in db.Doors
                             where t.TerritoryID == TerritoryID && t.Territory.CongregationID == CongID
                              select t).ToList();

                //Proximity Sort
                if (ProximitySort && doors.Count() > 0)
                {
                    List<Door> DoorList = new List<Door>();
                    Door lastAdded = doors[0];
                    DoorList.Add(lastAdded);
                    Haversine h = new Haversine();
                    //Loop Through 
                    for (int i = 1; i < doors.Count(); i++)
                    {
                        double Prox = 50;
                        Door closest = new Door();
                        TerritoryWeb.Common.Haversine.Position p1 = new TerritoryWeb.Common.Haversine.Position { Latitude = (double)lastAdded.GeoLat, Longitude = (double)lastAdded.GeoLong };

                        //Get the shortest distance (from whatever is not in the DoorList)
                        foreach (Door d in doors.Where(x => !DoorList.Any(s => s.Id == x.Id)))
                        {
                            TerritoryWeb.Common.Haversine.Position p2 = new TerritoryWeb.Common.Haversine.Position { Latitude = (double)d.GeoLat, Longitude = (double)d.GeoLong };
                            //Calculate Proximity
                            double curProx = h.Distance(p1, p2, Haversine.DistanceType.Miles);

                            //If the street is the same, skew distance
                            curProx = (lastAdded.Street == d.Street && curProx > 0.04) ? curProx - 0.04 : curProx;

                            //If distance is shorter, take notes
                            if (Prox > curProx)
                            {
                                Prox = curProx;
                                closest = d;
                            }
                        }

                        //Add to list
                        DoorList.Add(closest);
                        lastAdded = closest;
                    }
                    doors = DoorList;
                }

                //Convert to Model
                foreach (var d in doors)
                {
                    TerritoryPrintOutModel p = new TerritoryPrintOutModel();
                    p.TerritoryID = d.Territory.Id;
                    p.TerritoryName = d.Territory.TerritoryName;
                    p.DoorID = d.Id;
                    p.DoorAddress = d.Address;
                    p.DoorApartment = d.Apartment;
                    p.DoorStreet = d.Street;
                    p.DoorComments = ((d.DoorCode != null) ? "*" + d.DoorCode.Description.Trim() + ". " : "") + d.Comments;


                    results.Add(p);
                }

                //Only sort, if not proximity sort.
                if (!ProximitySort)
                {
                    results = (from t in results
                               orderby t.DoorStreet ascending, t.DoorAddress ascending
                               select t).ToList();
                }
                
            }

            return results;
            
        }
    }


}
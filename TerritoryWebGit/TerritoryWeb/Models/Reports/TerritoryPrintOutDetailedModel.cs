using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TerritoryWeb.Models.Reports
{
    public class TerritoryPrintOutDetailedModel
    {
        public int TerritoryID { get; set; }
        public string TerritoryName { get; set; }
        public int DoorID { get; set; }
        public string Address { get; set; }
        public string Street { get; set; }
        public string Apartment { get; set; }
        public string Comments { get; set; }
        public string Name { get; set; }
        public string Telephone { get; set; }
        public string PostalCode { get; set; }

        public static List<TerritoryPrintOutDetailedModel> GetTerritoryPrintout(int TerritoryID, int CongID)
        {

            List<TerritoryPrintOutDetailedModel> results = new List<TerritoryPrintOutDetailedModel>();

            using (TerritoryWebEntities db = new TerritoryWebEntities())
            {
                var tertemp = from t in db.Territories
                              where t.Id == TerritoryID && t.CongregationID == CongID
                              select t;

                foreach (var t in tertemp)
                {
                    foreach (var d in t.Doors)
                    {
                        TerritoryPrintOutDetailedModel p = new TerritoryPrintOutDetailedModel();
                        p.TerritoryID = t.Id;
                        p.TerritoryName = t.TerritoryName;
                        p.DoorID = d.Id;
                        p.Address = d.Address;
                        p.Street = d.Street;
                        p.Apartment = d.Apartment;
                        p.Comments = d.Comments;
                        p.Name = d.Name;
                        p.Telephone = d.Telephone;
                        p.PostalCode = d.PostalCode;

                        results.Add(p);
                    }
                }

                results = (from t in results
                           orderby t.Street ascending, t.Address ascending
                           select t).ToList();
            }

            return results;

        }
    }
}
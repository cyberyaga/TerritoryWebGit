using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TerritoryWeb.Models;

namespace TerritoryWeb.Common
{
    public class Methods
    {
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string ShrinkURL(string URL)
        {
            using (TerritoryWebEntities db = new TerritoryWebEntities())
            {
                string newURLcode = "";
                do
                {
                    newURLcode = RandomString(5);
                    
                } while (db.URLMinimizeStores.Any(x => x.shortURL == newURLcode));

                URLMinimizeStore u = new URLMinimizeStore();
                u.shortURL = newURLcode;
                u.longURL = URL;
                u.dateCreated = DateTime.Now;
                db.URLMinimizeStores.Add(u);
                db.SaveChanges();
                
                //New URL
                return "https://www.myterritoryweb.com/u/" + newURLcode;
            }
        }

        public static string GrowURL(string urlCode)
        {
            try
            {
                using (TerritoryWebEntities db = new TerritoryWebEntities())
                {
                    URLMinimizeStore u = db.URLMinimizeStores.Single(s => s.shortURL == urlCode);
                    u.used = DateTime.Now;
                    db.SaveChanges();

                    //Housekeeping
                    DateTime olds = DateTime.Now.AddDays(-30).Date;
                    DateTime useds = DateTime.Now.AddDays(-2).Date;

                    var dels = from x in db.URLMinimizeStores
                               where x.dateCreated < olds || x.used < useds
                               select x;

                    //Remove
                    if (dels.Count() > 0)
                    {
                        db.URLMinimizeStores.RemoveRange(dels);
                        db.SaveChanges();
                    }

                    return u.longURL;
                }
            }
            catch { }

            return "";
        }

        public static string GetCultureURL(Uri CurrentURL, string Culture)
        {
            //If Locale is present
            if (CurrentURL.Segments.Count() > 1)
            {
                if (CurrentURL.Segments[1].StartsWith(Culture))
                {
                    return CurrentURL.ToString();                    
                }
                else
                {
                    return CurrentURL.ToString().Replace(CurrentURL.Segments[1], Culture + "/");
                }
            }
            else
            {
                return CurrentURL.ToString() + Culture;
            }
        }
    }
}
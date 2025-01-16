using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TerritoryWeb.Controllers
{
    public class ShortURLController : Controller
    {
        // GET: ShortURL
        public ActionResult Index(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                string newurl = Common.Methods.GrowURL(id);

                if (newurl != "")
                {
                    Response.Redirect(newurl);                
                }

            }

            return View();
        }
    }
}
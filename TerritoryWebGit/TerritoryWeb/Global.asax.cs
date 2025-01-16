using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TerritoryWeb.App_Start;

namespace TerritoryWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ControllerBuilder.Current.SetControllerFactory(new DefaultControllerFactory(new LocalizedControllerActivator()));
        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            if (!Request.IsLocal)
            {
                switch (Request.Url.Scheme)
                {
                    case "https":
                        //Response.AddHeader("Strict-Transport-Security", "max-age=300");
                        break;
                    case "http":
                        string newurl = "";

                        newurl = Request.Url.ToString();
                        newurl = newurl.Replace("//myterritoryweb.com", "www.myterritoryweb.com");
                        newurl = newurl.Replace("http:", "https:");
                        Response.Redirect(newurl);

                        //var path = "https://" + Request.Url.Host + Request.Url.PathAndQuery;
                        //Response.Status = "301 Moved Permanently";
                        //Response.AddHeader("Location", path);
                        break;
                }
            }
        }
    }
}

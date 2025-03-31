using JSON_DAL;
using PhotosManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace PhotosManager
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            DB.Users.ResetAllUsersOnlineStatus();
        }
        protected void Session_Start()
        {
            Session["CurrentDate"] = DateTime.Now.Year;
        }
        protected void Session_End()
        {
            Models.DB.Users.SetOnline(Session["ConnectedUser"], false);
        }
    }
}

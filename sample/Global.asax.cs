using System;
using System.Web.Mvc;
using System.Web.Routing;
using Knowit.EPiModules.Replaceables.Sample.Business.ServiceLocation;

namespace Knowit.EPiModules.Replaceables.Sample
{
    public class EPiServerApplication : EPiServer.Global
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteTable.Routes.MapMvcAttributeRoutes();
            ServiceLocationConfiguration.Setup();
            //Tip: Want to call the EPiServer API on startup? Add an initialization module instead (Add -> New Item.. -> EPiServer -> Initialization Module)
        }
    }
}
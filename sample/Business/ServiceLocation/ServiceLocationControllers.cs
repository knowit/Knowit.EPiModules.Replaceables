using System;
using System.Web.Mvc;
using System.Web.Routing;
using EPiServer.ServiceLocation;

namespace Knowit.EPiModules.Replaceables.Sample.Business.ServiceLocation
{
    public class ServiceLocationControllerFactory : DefaultControllerFactory
    {
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            return ServiceLocator.Current.GetInstance(controllerType) as IController;
        }
    }


}
using System;
using System.Web.Mvc;
using StructureMap;

namespace Knowit.EPiModules.Replaceables.Sample.Business.ServiceLocation
{
    public static class ServiceLocationConfiguration
    {
        public static Action<ConfigurationExpression> Current
        {
            get
            {
                return configuration =>
                {
                    

                    
                };
            }
        }

        public static void Setup()
        {
            ControllerBuilder.Current.SetControllerFactory(
                new ServiceLocationControllerFactory()
            );
        }
    }
}
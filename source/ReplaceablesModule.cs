using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using Knowit.EPiModules.Replaceables.Filters;
using Knowit.EPiModules.Replaceables.Storage;

namespace Knowit.EPiModules.Replaceables
{
    [InitializableModule]
    [ModuleDependency(typeof(ServiceContainerInitialization))]
    public class ReplaceablesModule : IConfigurableModule
    {
        public void Initialize(InitializationEngine context)
        {
            GlobalFilters.Filters.Add(ServiceLocator.Current.GetInstance<ReplaceablesResultFilter>());
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Container.Configure(x => x.For<IReplaceableStorageProvider>().Use<ReplaceableStorageProvider>());
            context.Container.Configure(x => x.For<IReplaceablesManager>().Use<ReplaceablesManager>());
        }
    }
}

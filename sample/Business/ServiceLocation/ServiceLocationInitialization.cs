using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;

namespace Knowit.EPiModules.Replaceables.Sample.Business.ServiceLocation
{
    [InitializableModule]
    [ModuleDependency(typeof(ServiceContainerInitialization))]
    public class DependenciesInitializationModule : IConfigurableModule
    {
        public void Initialize(InitializationEngine context)
        {
        }
        public void Preload(string[] parameters) { }
        public void Uninitialize(InitializationEngine context)
        {
            //Add uninitialization logic
        }
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Container.Configure(ServiceLocationConfiguration.Current);
        }
    }
}
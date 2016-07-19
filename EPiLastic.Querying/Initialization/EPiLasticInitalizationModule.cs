using EpiLastic.Querying;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;

namespace EPiLastic.Querying.Initialization
{
    [ModuleDependency(typeof(ServiceContainerInitialization))]
    [InitializableModule]
    public class EPiLasticInitalizationModule : IConfigurableModule
    {
        void IConfigurableModule.ConfigureContainer(ServiceConfigurationContext context)
        {         
            context.Container.Configure(c => c.For<ISearchClient>().Use<SearchClient>());
        }

        public void Initialize(InitializationEngine context)
        {
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        public void Preload(string[] parameters)
        {
        }
    }
}

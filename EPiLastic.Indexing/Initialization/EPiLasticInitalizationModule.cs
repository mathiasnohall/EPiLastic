using EPiLastic.Indexing.EventHandling;
using EPiLastic.Indexing.ReIndexJob;
using EPiLastic.Indexing.Services;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;

namespace EPiLastic.Indexing.Initialization
{
    [ModuleDependency(typeof(ServiceContainerInitialization))]
    [InitializableModule]
    public class EPiLasticInitalizationModule : IConfigurableModule
    {
        void IConfigurableModule.ConfigureContainer(ServiceConfigurationContext context)
        {         
            context.Container.Configure(c => c.For<IIndexClient>().Use<IndexClient>());
            context.Container.Configure(c => c.For<IReIndexJobHandler>().Use<ReIndexJobHandler>());
            context.Container.Configure(c => c.For<IEPiServerEventHandler>().Use<EPiServerEventHandler>());
            context.Container.Configure(c => c.For<IPageHelper>().Use<PageHelper>());
            context.Container.Configure(c => c.For<IReIndexJobHandler>().Use<ReIndexJobHandler>());
            context.Container.Configure(c => c.For<IIndexingHandler>().Use<IndexingHandler>());
            context.Container.Configure(c => c.For<EPiLastic.Wrappers.IDateTimeWrapper>().Use<EPiLastic.Wrappers.DateTimeWrapper>());
            context.Container.Configure(c => c.For<ISuggestionHelper>().Use<SuggestionHelper>());
            context.Container.Configure(c => c.For<IObjectMapper>().Use<ObjectMapper>());
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

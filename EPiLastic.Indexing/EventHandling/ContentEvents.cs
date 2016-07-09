using EPiServer;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using System.Threading.Tasks;

namespace EpiLastic.Indexing.EventHandling
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class ContentEvents : IInitializableModule
    {
        private IEPiServerEventHandler _ePiServerEventHandler;

        private void Instance_MovedContent(object sender, ContentEventArgs e)
        {
            Task.Run(() => _ePiServerEventHandler.MovedContent(e));
        }

        private void PublishedContent(object sender, ContentEventArgs e)
        {
            Task.Run(() => _ePiServerEventHandler.PublishedContent(e));
        }

        #region IInitializaionModule members

        private bool _pageEventAttached;

        public void Initialize(InitializationEngine context)
        {
            _ePiServerEventHandler = ServiceLocator.Current.GetInstance<IEPiServerEventHandler>();

            if (!_pageEventAttached)
            {
                DataFactory.Instance.PublishedContent += PublishedContent;
                DataFactory.Instance.MovedContent += Instance_MovedContent;
            }

            _pageEventAttached = true;
        }


        public void Preload(string[] parameters) { }

        public void Uninitialize(InitializationEngine context)
        {
            DataFactory.Instance.PublishedContent -= PublishedContent;
            DataFactory.Instance.MovedContent -= Instance_MovedContent;
            _pageEventAttached = false;
        }

        #endregion
    }
}

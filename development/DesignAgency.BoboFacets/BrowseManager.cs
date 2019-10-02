using System.Linq;
using DesignAgency.BoboFacets.Browsers;

namespace DesignAgency.BoboFacets
{
    public class BrowseManager : IBrowseManager
    {
        private readonly FacetBrowserCollection _facetBrowserCollection;

        public BrowseManager(FacetBrowserCollection facetBrowserCollection)
        {
            _facetBrowserCollection = facetBrowserCollection;
        }
        public TBrowser Browser<TBrowser>() where TBrowser : class, IFacetBrowser
        {
            return _facetBrowserCollection.OfType<TBrowser>().FirstOrDefault();
        }
    }
}

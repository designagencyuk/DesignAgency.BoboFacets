using DesignAgency.BoboFacets.Services;
using System.Linq;

namespace DesignAgency.BoboFacets
{
    public static class BrowseManager 
    {
        public static TBrowser Browser<TBrowser>() where TBrowser : class, IFacetBrowser
        {
            return BoboBrowsersResolver.Current.Browsers.OfType<TBrowser>().FirstOrDefault();
        }
    }
}

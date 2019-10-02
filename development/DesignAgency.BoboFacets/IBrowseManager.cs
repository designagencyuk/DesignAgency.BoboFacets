using DesignAgency.BoboFacets.Browsers;

namespace DesignAgency.BoboFacets
{
    public interface IBrowseManager
    {
        TBrowser Browser<TBrowser>() where TBrowser : class, IFacetBrowser;
    }
}
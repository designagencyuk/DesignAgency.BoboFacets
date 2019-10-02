using DesignAgency.BoboFacets.Browsers;
using Umbraco.Core.Composing;

namespace DesignAgency.BoboFacets
{
    public class FacetBrowserCollectionBuilder : OrderedCollectionBuilderBase<FacetBrowserCollectionBuilder, FacetBrowserCollection, IFacetBrowser>
    {
        protected override FacetBrowserCollectionBuilder This => this;
    }
}

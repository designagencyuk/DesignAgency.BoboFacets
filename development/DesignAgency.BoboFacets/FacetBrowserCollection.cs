using System.Collections.Generic;
using DesignAgency.BoboFacets.Browsers;
using Umbraco.Core.Composing;

namespace DesignAgency.BoboFacets
{
    public class FacetBrowserCollection: BuilderCollectionBase<IFacetBrowser>
    {
        public FacetBrowserCollection(IEnumerable<IFacetBrowser> items)
            : base(items)
        { }
    }
}

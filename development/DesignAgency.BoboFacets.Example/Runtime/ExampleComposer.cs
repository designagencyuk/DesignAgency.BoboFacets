using DesignAgency.BoboFacets.Extensions;
using DesignAgency.BoboFacets.Runtime;
using Umbraco.Core.Composing;

namespace DesignAgency.BoboFacets.Example.Runtime
{
    [ComposeAfter(typeof(BoboFacetsComposer))]
    public class ExampleComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.FacetBrowsers().Append<ProductBrowser>();
        }
    }
}
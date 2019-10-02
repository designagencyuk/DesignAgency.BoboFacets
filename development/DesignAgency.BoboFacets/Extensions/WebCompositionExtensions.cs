using Umbraco.Core.Composing;

namespace DesignAgency.BoboFacets.Extensions
{
    public static class WebCompositionExtensions
    {
        public static FacetBrowserCollectionBuilder FacetBrowsers(this Composition composition)
            => composition.WithCollectionBuilder<FacetBrowserCollectionBuilder>();
    }
}

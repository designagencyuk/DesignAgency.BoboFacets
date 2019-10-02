using DesignAgency.BoboFacets.Extensions;
using DesignAgency.BoboFacets.Runtime.Componets;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace DesignAgency.BoboFacets.Runtime
{
    public class BoboFacetsComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.FacetBrowsers();
            composition.RegisterUnique<IBrowseManager, BrowseManager>();
            composition.Components().Append<BoboFacetsComponent>();
        }
    }
}

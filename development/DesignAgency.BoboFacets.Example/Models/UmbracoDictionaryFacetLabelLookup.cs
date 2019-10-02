using DesignAgency.BoboFacets.Models;
using Umbraco.Web;

namespace DesignAgency.BoboFacets.Example.Models
{
    public class UmbracoDictionaryFacetLabelLookup : IFacetLabelLookup
    {
        private readonly UmbracoHelper _umbracoHelper;

        public UmbracoDictionaryFacetLabelLookup(UmbracoHelper umbracoHelper)
        {
            _umbracoHelper = umbracoHelper;
        }

        public string LookupLabel(string value, IFacetField facetField)
        {
            return _umbracoHelper.GetDictionaryValue(value, value);
        }
    }
}
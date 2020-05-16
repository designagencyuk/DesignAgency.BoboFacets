using DesignAgency.BoboFacets.Domain;
using DesignAgency.BoboFacets.Models;

namespace DesignAgency.BoboFacets.Extensions
{
    public static class FacetFieldExtensions
    {
        public static string FacetFieldAlias(this IFacetField facetField, string cultureCode)
        {
            if (facetField.CultureDependant && !string.IsNullOrEmpty(cultureCode))
                return Constants.Conventions.FacetFieldPrefix + facetField.OriginalAlias + "_" + cultureCode.ToLower();
            return Constants.Conventions.FacetFieldPrefix + facetField.OriginalAlias;
        }
    }
}

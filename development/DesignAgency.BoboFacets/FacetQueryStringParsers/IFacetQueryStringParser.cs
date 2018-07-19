using System.Collections.Generic;
using DesignAgency.BoboFacets.Models;

namespace DesignAgency.BoboFacets.FacetQueryStringParsers
{
    public interface IFacetQueryStringParser
    {
        IDictionary<IFacetField, IEnumerable<string>> ParseQueryString(List<KeyValuePair<string, string>> querystring, IEnumerable<IFacetField> facetFields);
    }
}

using System.Collections.Generic;
using System.Collections.Specialized;
using DesignAgency.BoboFacets.Models;

namespace DesignAgency.BoboFacets.FacetQueryStringParsers
{
    public interface IFacetQueryStringParser
    {
        IDictionary<IFacetField, IEnumerable<string>> ParseQueryString(NameValueCollection querystring, IEnumerable<IFacetField> facetFields, string cultureCode);
    }
}

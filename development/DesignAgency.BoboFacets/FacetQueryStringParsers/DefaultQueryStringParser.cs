using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using DesignAgency.BoboFacets.Models;

namespace DesignAgency.BoboFacets.FacetQueryStringParsers
{
    /// <summary>
    /// The standard parser will parse querystrings with keys appearing multiple times as a single facet with multiple selections.
    /// E.g. ?facet1=a&amp;facet1=b&amp;facet2=s would result in the following selection: facet1 = a,b facet2 = s
    /// </summary>
    public class DefaultQueryStringParser : IFacetQueryStringParser
    {
        public virtual IDictionary<IFacetField, IEnumerable<string>> ParseQueryString(NameValueCollection querystring, IEnumerable<IFacetField> facetFields, string cultureCode)
        {
            var facetSelection = new Dictionary<IFacetField, IEnumerable<string>>();
            var queryStringKeys = querystring.AllKeys;
            foreach (var facetField in facetFields.Where(x => queryStringKeys.Contains(x.CreateFacetFieldAlias(cultureCode))))
            {
                var values = querystring.GetValues(facetField.CreateFacetFieldAlias(cultureCode));
                facetSelection.Add(facetField, values);
            }
            return facetSelection;
        }
    }
}

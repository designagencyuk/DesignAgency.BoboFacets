using System.Collections.Generic;
using System.Linq;
using DesignAgency.BoboFacets.Domain;
using DesignAgency.BoboFacets.Models;

namespace DesignAgency.BoboFacets.FacetQueryStringParsers
{
    /// <summary>
    /// The standard parser will parse querystrings with keys appearing multiple times as a single facet with multiple selections.
    /// E.g. ?facet1=a&amp;facet1=b&amp;facet2=s would result in the following selection: facet1 = a,b facet2 = s
    /// </summary>
    public class DefaultQueryStringParser : IFacetQueryStringParser
    {
        public virtual IDictionary<IFacetField, IEnumerable<string>> ParseQueryString(List<KeyValuePair<string, string>> querystring, IEnumerable<IFacetField> facetFields)
        {
            var facetSelection = new Dictionary<IFacetField, IEnumerable<string>>();
            var queryStringKeys = querystring.Select(x => x.Key);
            foreach (var facetField in facetFields.Where(x => queryStringKeys.Contains(x.Alias.FacetFieldAlias())))
            {
                var facetKvps = querystring.Where(x => x.Key == facetField.Alias.FacetFieldAlias());
                facetSelection.Add(facetField, facetKvps.Select(facetKvp => facetKvp.Value));
            }
            return facetSelection;
        }
    }
}

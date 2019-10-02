using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using DesignAgency.BoboFacets.Models;

namespace DesignAgency.BoboFacets.FacetQueryStringParsers
{
    public class DelimiterQueryStringParser : DefaultQueryStringParser
    {
        private readonly string _delimiter;

        public DelimiterQueryStringParser(string delimiter = ",")
        {
            _delimiter = delimiter;
        }
        public override IDictionary<IFacetField, IEnumerable<string>> ParseQueryString(NameValueCollection querystring, IEnumerable<IFacetField> facetFields, string cultureCode)
        {
            var facetSelection = base.ParseQueryString(querystring, facetFields, cultureCode);
            var keys = new List<IFacetField>(facetSelection.Keys);
            foreach (var facetField in keys)
            {
                var selection = new List<string>();
                foreach (var s in facetSelection[facetField])
                {
                    selection.AddRange(s.Split(new []{_delimiter}, StringSplitOptions.RemoveEmptyEntries));
                }
                facetSelection[facetField] = selection;
            }
            return facetSelection;
        }
    }
}

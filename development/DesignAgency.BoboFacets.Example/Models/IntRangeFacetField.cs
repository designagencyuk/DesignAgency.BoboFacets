using System;
using System.Linq;
using BoboBrowse.Net;
using BoboBrowse.Net.Facets;
using BoboBrowse.Net.Facets.Data;
using BoboBrowse.Net.Facets.Impl;
using DesignAgency.BoboFacets.Models;
using Lucene.Net.Documents;
using Umbraco.Core;

namespace DesignAgency.BoboFacets.Example.Models
{
    public class IntRangeFacetField : FacetField
    {
        public IntRangeFacetField(string alias, string label, bool cultureDependant = false, FacetSpec.FacetSortSpec valueOrderBy = FacetSpec.FacetSortSpec.OrderValueAsc, bool expandSelection = true, int minHitCount = 0, int maxCount = 0, BrowseSelection.ValueOperation valueOperation = BrowseSelection.ValueOperation.ValueOperationOr) : base(alias, label, false, cultureDependant, valueOrderBy, expandSelection, minHitCount, maxCount, valueOperation)
        {
        }

        public override IFacetHandler CreateFacetHandler(string cultureCode)
        {
            string[] currencyRanges = new string[] { "[* TO 49]",       // -∞ - 9.99
                "[50 TO 99]",   // 10.00 - 19.99
                "[100 TO 149]",   // 20.00 - 49.99
                "[150 TO *]" };
            return new RangeFacetHandler(CreateFacetFieldAlias(cultureCode), new PredefinedTermListFactory<float>("00000000000000000000"), currencyRanges.ToList());
        }

        public override IFieldable CreateIndexField(string fieldValue, string cultureCode)
        {
            var doubleValue = int.Parse(fieldValue);
            return new Field(CreateFacetFieldAlias(cultureCode), doubleValue.ToString("D20"), Field.Store.YES, Field.Index.NOT_ANALYZED);
        }

        public override string CreateValueLabel(string value)
        {
            var range = GetRangeStrings(value);
            var lower = range[0];
            var upper = range[1];
            if (lower.InvariantEquals("*"))
            {
                return $"Less than {int.Parse(upper)}";
            }
            if (upper.InvariantEquals("*"))
            {
                return $"Greater than {int.Parse(lower)}";
            }
            return $"{int.Parse(lower)} - {int.Parse(upper)}";
        }

        public string[] GetRangeStrings(string rangeString)
        {
            var index = rangeString.IndexOf('[');
            var index2 = rangeString.IndexOf(" TO ", StringComparison.InvariantCultureIgnoreCase);
            var index3 = rangeString.LastIndexOf(']');

            var lower = rangeString.Substring(index + 1, index2 - index - 1).Trim();
            var upper = rangeString.Substring(index2 + 4, index3 - index2 - 4).Trim();

            return new[] { lower, upper };
        }
    }
}
using System;
using System.Linq;
using BoboBrowse.Api;
using BoboBrowse.Facets;
using BoboBrowse.Facets.impl;
using DesignAgency.BoboFacets.Domain;
using DesignAgency.BoboFacets.Models;
using Lucene.Net.Documents;
using Umbraco.Core;

namespace DesignAgency.BoboFacets.Example.Models
{
    public class IntRangeFacetField : FacetField
    {
        public IntRangeFacetField(string alias, string label, FacetSpec.FacetSortSpec valueOrderBy = FacetSpec.FacetSortSpec.OrderValueAsc, bool expandSelection = true, int minHitCount = 0, BrowseSelection.ValueOperation valueOperation = BrowseSelection.ValueOperation.ValueOperationOr) : base(alias, label, false, valueOrderBy, expandSelection, minHitCount, valueOperation)
        {
        }

        public override FacetHandler CreateFacetHandler()
        {
            string[] currencyRanges = new string[] { "[* TO 00000000000000000049]",       // -∞ - 9.99
                "[00000000000000000050 TO 00000000000000000099]",   // 10.00 - 19.99
                "[00000000000000000100 TO 00000000000000000149]",   // 20.00 - 49.99
                "[00000000000000000150 TO *]" };
            return new RangeFacetHandler(Alias.FacetFieldAlias(), currencyRanges.ToList());
        }

        public override Fieldable CreateIndexField(string fieldValue)
        {
            var doubleValue = int.Parse(fieldValue);
            return new Field(Alias.FacetFieldAlias(), doubleValue.ToString("D20"), Field.Store.YES, Field.Index.NOT_ANALYZED);
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
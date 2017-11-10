using DesignAgency.BoboFacets.Services;
using System.Collections.Generic;
using DesignAgency.BoboFacets.Example.Models;
using DesignAgency.BoboFacets.Models;
using Lucene.Net.Search;

namespace DesignAgency.BoboFacets.Example
{
    public class MyBrowser : BaseBrowser
    {
        public override IEnumerable<IFacetField> FacetFields => new List<FacetField>() { new FacetField("category", "Category", true), new DoubleRangeFacetField("price", "Price") };

        public override string SearchProvider => "ExternalSearcher";

        public override string IndexProvider => "ExternalIndexer";

        public override SortField[] DefaultSort => new[] { new SortField("sortOrder", SortField.INT, false) };
    }
}
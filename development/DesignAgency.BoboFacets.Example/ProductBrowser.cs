using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using DesignAgency.BoboFacets.Browsers;
using DesignAgency.BoboFacets.Example.Models;
using DesignAgency.BoboFacets.Models;
using Examine;
using Examine.LuceneEngine.Search;
using Lucene.Net.Search;
using Umbraco.Examine;
using Umbraco.Web.PublishedModels;

namespace DesignAgency.BoboFacets.Example
{
    public class ProductBrowser : BaseBrowser
    {
        public ProductBrowser(IExamineManager examineManager) : base(examineManager)
        {
            FacetFields = new List<IFacetField> { new FacetField("category", "Category", true, true), new IntRangeFacetField("price", "Price") };
        }

        public override List<IFacetField> FacetFields { get; }
        public override string IndexProvider => "ExternalIndex";
        public override SortField[] DefaultSort => new[] { new SortField("sortOrder", SortField.INT, false) };

        public override Query BuildBaseQuery(NameValueCollection querystring, string cultureCode)
        {
            if(!ExamineManager.TryGetIndex(IndexProvider, out var index))
            {
                throw new ArgumentNullException(nameof(IndexProvider));
            }

            var query = (LuceneSearchQuery) index.GetSearcher().CreateQuery();
            query.NodeTypeAlias(Product.ModelTypeAlias);
            query.Group(group => group.Field(UmbracoContentIndex.VariesByCultureFieldName, "y").And()
                    .Field($"{UmbracoExamineIndex.PublishedFieldName}_{cultureCode.ToLower()}", "y")).Or()
                .Field(UmbracoContentIndex.VariesByCultureFieldName, "n");
            return query.Query;
        }
    }
}
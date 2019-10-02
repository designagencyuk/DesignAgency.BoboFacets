using System.Collections.Generic;
using DesignAgency.BoboFacets.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.PublishedModels;

namespace DesignAgency.BoboFacets.Example.Models
{
    public class ProductsViewModel : Products
    {
        public ProductsViewModel(IPublishedContent content) : base(content)
        {
        }



        public Dictionary<string, IEnumerable<FacetSelection>> FacetSelection { get; set; }

        public IEnumerable<Product> Results { get; set; }

        public IEnumerable<FacetGroup> FacetGroups { get; set; }
        public int TotalResults { get; internal set; }
        public int TotalDocs { get; internal set; }
        public bool HasNextPage { get; internal set; }
        public IEnumerable<IFacetField> FacetFields { get; set; }
    }
}
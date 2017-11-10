using DesignAgency.BoboFacets.Example.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using Umbraco.Web.PublishedContentModels;

namespace DesignAgency.BoboFacets.Example.Controllers
{
    public class ProductsController : RenderMvcController
    {
        public ActionResult Products(RenderModel model)
        {
            var browser = BrowseManager.Browser<MyBrowser>();
            var viewModel = new ProductsViewModel(model.Content);

            var browserRequest = browser.ConvertBrowseRequest(Request.QueryString.AllKeys.SelectMany(Request.QueryString.GetValues, (k, v) => new KeyValuePair<string, string>(k, v)).ToList());

            viewModel.FacetSelection = browser.ConvertToFacetSelection(browserRequest.GetSelections());

            var results = browser.Browse(browserRequest);

            var products = Umbraco.TypedContent(results.Hits.Select(x => x.StoredFields.Get("id"))).OfType<Product>();

            viewModel.Results = products;
            viewModel.FacetGroups = browser.ConvertToFacetGroups(results.FacetMap);
            viewModel.TotalResults = results.NumHits;
            viewModel.TotalDocs = results.TotalDocs;
            viewModel.HasNextPage = browserRequest.Count < results.NumHits;

            return CurrentTemplate(viewModel);
        }
    }
}
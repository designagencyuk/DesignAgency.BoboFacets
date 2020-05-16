using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using DesignAgency.BoboFacets.Example.Models;
using DesignAgency.BoboFacets.Models;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using Umbraco.Web.PublishedModels;

namespace DesignAgency.BoboFacets.Example.Controllers
{
    public class ProductsController : RenderMvcController
    {
        private readonly IBrowseManager _browseManager;

        public ProductsController(IBrowseManager browseManager)
        {
            _browseManager = browseManager;
        }

        public ActionResult Products(ContentModel model)
        {
            var browser = _browseManager.Browser<ProductBrowser>();
            var viewModel = new ProductsViewModel(model.Content);

            var browserRequest = browser.CreateBrowseRequest(Request.QueryString, Thread.CurrentThread.CurrentUICulture.Name);

            var facetValueLabelLookupDictionary = new Dictionary<string, Func<string, IFacetField, string>>
            {
                {
                    "category", FacetValueLabelLookup
                }
            };

            viewModel.FacetSelection = browser.ConvertToFacetSelection(browserRequest.GetSelections(), Thread.CurrentThread.CurrentUICulture.Name, facetValueLabelLookupDictionary);

            var results = browser.Browse(browserRequest, Thread.CurrentThread.CurrentUICulture.Name);
            
            var products = Umbraco.Content(results.Hits.Select(x => x.StoredFields.Get("id"))).OfType<Product>();
            
            viewModel.FacetFields = browser.FacetFields;
            viewModel.Results = products;
            viewModel.FacetGroups = browser.ConvertToFacetGroups(results.FacetMap, Thread.CurrentThread.CurrentUICulture.Name, facetValueLabelLookupDictionary);
            viewModel.TotalResults = results.NumHits;
            viewModel.TotalDocs = results.TotalDocs;
            viewModel.HasNextPage = browserRequest.Count < results.NumHits;

            return CurrentTemplate(viewModel);
        }

        private string FacetValueLabelLookup(string facetValue, IFacetField field)
        {
            return Umbraco.GetDictionaryValue($"{field.OriginalAlias}_{facetValue}", facetValue);
        }
    }
}
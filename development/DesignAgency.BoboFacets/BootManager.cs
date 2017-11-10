using DesignAgency.BoboFacets.Domain;
using DesignAgency.BoboFacets.Models;
using Examine;
using Examine.LuceneEngine;
using Examine.LuceneEngine.Providers;
using Lucene.Net.Documents;
using System.Collections.Generic;
using Umbraco.Core;

namespace DesignAgency.BoboFacets
{
    public class BootManager : ApplicationEventHandler
    {
        protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            BoboBrowsersResolver.Current = new BoboBrowsersResolver(PluginManager.Current.ResolveBrowsers());
        }

        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            foreach(var browser in BoboBrowsersResolver.Current.Browsers)
            {
                var browserIndexer = (LuceneIndexer)ExamineManager.Instance.IndexProviderCollection[browser.IndexProvider];
                browserIndexer.DocumentWriting += (sender, e) => BrowserIndexer_DocumentWriting(browser.FacetFields, sender, e);
            }
        }

        private void BrowserIndexer_DocumentWriting(IEnumerable<IFacetField> facetFields, object sender, DocumentWritingEventArgs e)
        {
            foreach (var facetField in facetFields)
            {
                if (e.Fields.ContainsKey(facetField.Alias))
                {
                    var facetValue = e.Fields[facetField.Alias];
                    foreach (var value in facetField.PrepareForIndex(facetValue))
                    {
                        e.Document.Add(facetField.CreateIndexField(value));
                    }
                }
            }
        }
    }
}

using Examine;
using Examine.LuceneEngine.Providers;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DesignAgency.BoboFacets.Models;
using Examine.LuceneEngine;
using Lucene.Net.Documents;
using Umbraco.Core.Composing;

namespace DesignAgency.BoboFacets.Runtime.Componets
{
    public class BoboFacetsComponent : IComponent
    {
        private readonly IExamineManager _examineManager;
        private readonly FacetBrowserCollection _facetsBrowserCollection;

        public BoboFacetsComponent(IExamineManager examineManager, FacetBrowserCollection facetsBrowserCollection)
        {
            _examineManager = examineManager;
            _facetsBrowserCollection = facetsBrowserCollection;
        }

        public void Initialize()
        {
            foreach (var browser in _facetsBrowserCollection)
            {
                if (_examineManager.TryGetIndex(browser.IndexProvider, out var browserIndex) && browserIndex is LuceneIndex browserLuceneIndex)
                {
                    browserLuceneIndex.DocumentWriting += (sender, e) => BrowserIndexer_DocumentWriting(browser.FacetFields, sender, e);
                }
            }
        }

        private void BrowserIndexer_DocumentWriting(IEnumerable<IFacetField> facetFields, object sender, DocumentWritingEventArgs e)
        {
            var fields = e.Document.GetFields().ToList();
            foreach (var facetField in facetFields)
            {
                if (facetField.CultureDependant)
                {
                    var cultureFields = fields.Where(x => x.Name.StartsWith(facetField.OriginalAlias));
                    foreach (var cultureField in cultureFields)
                    {
                        var match = Regex.Match(cultureField.Name,
                            $"{facetField.OriginalAlias}_((?:[a-z]{{2}})(?:[-]{{1}}[a-z]{{2}})*)$");
                        if (match.Success)
                        {
                            AddFacetField(e, cultureField, facetField, match.Groups[1].Value);
                        }
                    }
                }
                else
                {
                    var field = e.Document.GetField(facetField.OriginalAlias);
                    AddFacetField(e, field, facetField, string.Empty);
                }
            }
        }

        private void AddFacetField(DocumentWritingEventArgs e, IFieldable field, IFacetField facetField, string cultureCode)
        {
            if (field != null)
            {
                var facetValue = field.StringValue;
                foreach (var value in facetField.PrepareForIndex(facetValue))
                {
                    e.Document.Add(facetField.CreateIndexField(value, cultureCode));
                }
            }
        }

        public void Terminate()
        {
        }
    }
}

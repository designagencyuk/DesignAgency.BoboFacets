using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using BoboBrowse.Net;
using BoboBrowse.Net.Facets;
using DesignAgency.BoboFacets.FacetQueryStringParsers;
using DesignAgency.BoboFacets.Models;
using Examine;
using Examine.LuceneEngine.Providers;
using Lucene.Net.Search;

namespace DesignAgency.BoboFacets.Browsers
{
    public abstract class BaseBrowser : IFacetBrowser
    {
        public abstract List<IFacetField> FacetFields { get; }

        public abstract string IndexProvider { get; }

        public abstract SortField[] DefaultSort { get; }

        protected IFacetQueryStringParser QueryStringParser { get; }

        protected readonly IExamineManager ExamineManager;

        protected BaseBrowser(IExamineManager examineManager, IFacetQueryStringParser queryStringParser)
        {
            QueryStringParser = queryStringParser;
            ExamineManager = examineManager;
        }

        protected BaseBrowser(IExamineManager examineManager) : this(examineManager, new DefaultQueryStringParser())
        {
            
        }
        /// <summary>
        /// Executes the browse request
        /// </summary>
        /// <param name="browseRequest"></param>
        /// <param name="cultureCode"></param>
        /// <param name="facetHandlers"></param>
        /// <param name="addDefaultFacetHandlers"></param>
        /// <returns></returns>
        public virtual BrowseResult Browse(BrowseRequest browseRequest, string cultureCode, List<IFacetHandler> facetHandlers = null, bool addDefaultFacetHandlers = true)
        {
            facetHandlers = facetHandlers ?? new List<IFacetHandler>();

            if (addDefaultFacetHandlers)
            {
                foreach (var facetField in FacetFields)
                {
                    var facetAlias = facetField.CreateFacetFieldAlias(cultureCode);
                    facetHandlers.Add(facetField.CreateFacetHandler(cultureCode));
                    var facetSpec = facetField.CreateFacetSpec();

                    browseRequest.SetFacetSpec(facetAlias, facetSpec);
                }
            }

            if(!ExamineManager.TryGetIndex(IndexProvider, out var index))
            {
                throw new ArgumentNullException(nameof(IndexProvider));
            }

            var luceneSearcher = (LuceneSearcher) index.GetSearcher();
            var searcher = (IndexSearcher)luceneSearcher.GetLuceneSearcher();
            var reader = searcher.IndexReader;
            
            using (var boboReader = BoboIndexReader.GetInstance(reader, facetHandlers))
            {
                using (IBrowsable browser = new BoboBrowser(boboReader))
                {
                    return browser.Browse(browseRequest);
                }
            }
        }

        /// <summary>
        /// Converts the selection to a FacetSelection and optionally looks up a display label for the facet value if a facetDisplayDictionaryValues is passed and has a value for the facet alias
        /// </summary>
        /// <param name="browseSelection"></param>
        /// <param name="cultureCode"></param>
        /// <param name="facetValueLabelLookupDictionary"></param>
        /// <returns></returns>
        public virtual Dictionary<string, IEnumerable<FacetSelection>> ConvertToFacetSelection(BrowseSelection[] browseSelection, string cultureCode, IDictionary<string, Func<string, IFacetField, string>> facetValueLabelLookupDictionary = null)
        {
            var facetSelection = new Dictionary<string, IEnumerable<FacetSelection>>();
            var selections = browseSelection.ToDictionary(x => x.FieldName, x => x.Values);
            if (selections.Any())
            {
                foreach (var selection in selections)
                {
                    var selectedItems = new List<FacetSelection>();

                    foreach (var selectedValue in selection.Value)
                    {
                        var facetName = selectedValue;

                        var facetField = FacetFields.FirstOrDefault(x => selection.Key == x.CreateFacetFieldAlias(cultureCode));
                        var originalAlias = facetField?.OriginalAlias ?? selection.Key;

                        if (facetValueLabelLookupDictionary != null && facetValueLabelLookupDictionary.ContainsKey(originalAlias))
                        {
                            facetName = facetValueLabelLookupDictionary[originalAlias].Invoke(selectedValue, facetField);
                        }
                        else if (facetField != null)
                        {
                            facetName = facetField.CreateValueLabel(selectedValue);
                        }

                        var facetSel = new FacetSelection
                        {
                            Label = facetName,
                            Value = selectedValue
                        };
                        selectedItems.Add(facetSel);
                    }

                    facetSelection.Add(selection.Key, selectedItems);
                }
            }
            return facetSelection;
        }

        /// <summary>
        /// Converts the browser request to facet groups and optionally looks up a display label for the facet value if a facetDisplayDictionaryValues is passed and has a value for the facet alias
        /// </summary>
        /// <param name="facetMap"></param>
        /// <param name="cultureCode"></param>
        /// <param name="facetValueLabelLookupDictionary"></param>
        /// <returns></returns>
        public virtual IEnumerable<FacetGroup> ConvertToFacetGroups(IDictionary<string, IFacetAccessible> facetMap, string cultureCode, IDictionary<string, Func<string, IFacetField, string>> facetValueLabelLookupDictionary = null)
        {
            var facetGroups = new List<FacetGroup>();
            if (facetMap.Any())
            {
                foreach (var map in facetMap)
                {
                    var facetField = FacetFields.FirstOrDefault(x => map.Key == x.CreateFacetFieldAlias(cultureCode));
                    var originalAlias = facetField?.OriginalAlias ?? map.Key;
                    var group = new FacetGroup
                    {
                        Label = facetField?.Label,
                        Alias = map.Key
                    };
                    var sortOrder = 0;
                    foreach (var f in map.Value.GetFacets())
                    {
                        var facetName = f.Value;
                        sortOrder++;
                        if (facetValueLabelLookupDictionary != null && facetValueLabelLookupDictionary.ContainsKey(originalAlias))
                        {
                            facetName = facetValueLabelLookupDictionary[originalAlias].Invoke(f.Value, facetField);
                        }
                        else if(facetField != null)
                        {
                            facetName = facetField.CreateValueLabel(f.Value);
                        }

                        var facet = new Facet
                        {
                            Label = facetName,
                            Value = f.Value,
                            Count = f.FacetValueHitCount,
                            Sort = sortOrder
                        };
                        group.Facets.Add(facet);
                    }

                    facetGroups.Add(group);
                }
            }
            return facetGroups;
        }

        /// <summary>
        /// Builds an optional base query to filter the results
        /// </summary>
        /// <param name="querystring"></param>
        /// <param name="cultureCode"></param>
        /// <returns></returns>
        public virtual Query BuildBaseQuery(NameValueCollection querystring, string cultureCode)
        {
            return null;
        }

        /// <summary>
        /// Determines the sort order of the results. Defaults to the DefaultSort
        /// </summary>
        /// <param name="querystring"></param>
        /// <param name="cultureCode"></param>
        /// <returns></returns>
        public virtual SortField[] DetermineSort(NameValueCollection querystring, string cultureCode)
        {
            return DefaultSort;
        }

        /// <summary>
        /// Creates a browser request from the passed in key value pair query string
        /// </summary>
        /// <param name="querystring"></param>
        /// <param name="cultureCode"></param>
        /// <param name="page"></param>
        /// <param name="useOffset"></param>
        /// <param name="itemsPerPage"></param>
        /// <returns></returns>
        public virtual BrowseRequest CreateBrowseRequest(NameValueCollection querystring, string cultureCode, int? page = null, bool useOffset = false, int itemsPerPage = 10)
        {
            var query = BuildBaseQuery(querystring, cultureCode);
            
            var offset = 0;
            if (!page.HasValue)
            {
                page = 1;
            }
            var count = page.Value * itemsPerPage;

            if (useOffset)
            {
                offset = (page.Value - 1) * itemsPerPage;
                count = itemsPerPage;
            }

            var browseRequest = new BrowseRequest
            {
                Offset = offset,
                Count = count,
                Query = query,
                FetchStoredFields = true,
                Sort = DetermineSort(querystring, cultureCode)
            };

            var facetSelection = QueryStringParser.ParseQueryString(querystring, FacetFields, cultureCode);
            
            foreach (var facetField in facetSelection)
            {
                if (facetField.Value.Any())
                {
                    var sel = new BrowseSelection(facetField.Key.CreateFacetFieldAlias(cultureCode));
                    foreach (var selection in facetField.Value)
                    {
                        if (!string.IsNullOrEmpty(selection))
                        {
                            sel.AddValue(selection);
                        }
                    }
                    sel.SelectionOperation = facetField.Key.SelectionOperation;
                    browseRequest.AddSelection(sel);
                }
            }

            return browseRequest;
        }
    }
}

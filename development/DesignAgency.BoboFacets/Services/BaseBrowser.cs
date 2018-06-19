using BoboBrowse.Api;
using BoboBrowse.Facets;
using DesignAgency.BoboFacets.Domain;
using DesignAgency.BoboFacets.Models;
using Examine;
using Examine.LuceneEngine.Providers;
using Examine.LuceneEngine.SearchCriteria;
using Examine.SearchCriteria;
using Lucene.Net.Search;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DesignAgency.BoboFacets.Services
{
    public abstract class BaseBrowser : IFacetBrowser
    {
        public abstract IEnumerable<IFacetField> FacetFields { get; }

        public abstract string SearchProvider { get; }

        public abstract string IndexProvider { get; }

        public abstract SortField[] DefaultSort { get; }
        
        /// <summary>
        /// Executes the browse request
        /// </summary>
        /// <param name="browseRequest"></param>
        /// <param name="facetHandlers"></param>
        /// <param name="addDefaultFacetHandlers"></param>
        /// <returns></returns>
        public virtual BrowseResult Browse(BrowseRequest browseRequest, List<FacetHandler> facetHandlers = null, bool addDefaultFacetHandlers = true)
        {
            facetHandlers = facetHandlers ?? new List<FacetHandler>();

            if (addDefaultFacetHandlers)
            {
                foreach (var facetField in FacetFields)
                {
                    var facetAlias = facetField.Alias.FacetFieldAlias();
                    facetHandlers.Add(facetField.CreateFacetHandler());
                    var facetSpec = new FacetSpec
                    {
                        OrderBy = facetField.ValueOrderBy,
                        ExpandSelection = facetField.ExpandSelection,
                        MinHitCount = facetField.MinHitCount
                    };

                    browseRequest.SetFacetSpec(facetAlias, facetSpec);
                }
            }

            var searchProvider = ExamineManager.Instance.SearchProviderCollection[SearchProvider] as LuceneSearcher;
            var searcher = (IndexSearcher)searchProvider.GetSearcher();
            var reader = searcher.GetIndexReader();

            var boboReader = BoboIndexReader.GetInstance(reader, facetHandlers);

            IBrowsable browser = new BoboBrowser(boboReader);

            return browser.Browse(browseRequest);
        }

        /// <summary>
        /// Converts the selection to a FacetSelection and optionally looks up a display label for the facet value if a facetDisplayDictionaryValues is passed and has a value for the facet alias
        /// </summary>
        /// <param name="browseSelection"></param>
        /// <param name="facetDisplayDictionaryValues"></param>
        /// <returns></returns>
        public virtual Dictionary<string, IEnumerable<FacetSelection>> ConvertToFacetSelection(BrowseSelection[] browseSelection, Dictionary<string, Dictionary<string, string>> facetDisplayDictionaryValues = null)
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

                        var facetField = FacetFields.FirstOrDefault(x => selection.Key == x.Alias.FacetFieldAlias());

                        if (facetDisplayDictionaryValues != null)
                        {
                            if (facetDisplayDictionaryValues.ContainsKey(selection.Key) && facetDisplayDictionaryValues[selection.Key].ContainsKey(selectedValue))
                            {
                                facetName = facetDisplayDictionaryValues[selection.Key][selectedValue];
                            }
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
        /// <param name="facetDisplayDictionaryValues"></param>
        /// <returns></returns>
        public virtual IEnumerable<FacetGroup> ConvertToFacetGroups(Dictionary<string, IFacetAccessible> facetMap, Dictionary<string, Dictionary<string, string>> facetDisplayDictionaryValues = null)
        {
            var facetGroups = new List<FacetGroup>();
            if (facetMap.Any())
            {
                foreach (var map in facetMap)
                {
                    var facetField = FacetFields.FirstOrDefault(x => map.Key == x.Alias.FacetFieldAlias());
                    var group = new FacetGroup
                    {
                        Label = facetField?.Label,
                        Alias = map.Key
                    };
                    var sortOrder = 0;
                    foreach (var f in map.Value.GetFacets())
                    {
                        var facetName = f.Value.ToString();
                        sortOrder++;
                        if (facetDisplayDictionaryValues != null)
                        {
                            if (facetDisplayDictionaryValues.ContainsKey(map.Key) &&
                                facetDisplayDictionaryValues[map.Key].ContainsKey(f.Value.ToString()))
                            {
                                facetName = facetDisplayDictionaryValues[map.Key][f.Value.ToString()];
                            }
                        }
                        else if(facetField != null)
                        {
                            facetName = facetField.CreateValueLabel(f.Value.ToString());
                        }

                        var facet = new Facet
                        {
                            Label = facetName,
                            Value = f.Value.ToString(),
                            Count = f.HitCount,
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
        /// <returns></returns>
        public virtual ISearchCriteria BuildBaseQuery(List<KeyValuePair<string, string>> querystring)
        {
            return null;
        }

        /// <summary>
        /// Determines the sort order of the results. Defaults to the DefaultSort
        /// </summary>
        /// <param name="querystring"></param>
        /// <returns></returns>
        public virtual SortField[] DetermineSort(List<KeyValuePair<string, string>> querystring)
        {
            return DefaultSort;
        }

        /// <summary>
        /// Creates a browser request from the passed in key value pair query string
        /// </summary>
        /// <param name="querystring"></param>
        /// <param name="page"></param>
        /// <param name="useOffset"></param>
        /// <param name="itemsPerPage"></param>
        /// <returns></returns>
        public virtual BrowseRequest CreateBrowseRequest(List<KeyValuePair<string, string>> querystring, int? page = null, bool useOffset = false, int itemsPerPage = 10)
        {
            Query query = null;
            var compiledQuery = BuildBaseQuery(querystring);
            if(compiledQuery != null)
            {
                var luceneParams = compiledQuery as LuceneSearchCriteria;
                query = luceneParams?.Query ?? throw new ArgumentException("Provided ISearchCriteria dos not match the allowed ISearchCriteria. Ensure you only use an ISearchCriteria created from the current SearcherProvider");
            }
            
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
                Sort = DetermineSort(querystring)
            };
            var queryStringKeys = querystring.Select(x => x.Key);
            foreach (var facetField in FacetFields.Where(x => queryStringKeys.Contains(x.Alias.FacetFieldAlias())))
            {
                var facetKvp = querystring.FirstOrDefault(x => x.Key == facetField.Alias.FacetFieldAlias());
                var facetValue = facetKvp.Value;
                if (!string.IsNullOrWhiteSpace(facetValue))
                {
                    var sel = browseRequest.GetSelection(facetKvp.Key) ?? new BrowseSelection(facetKvp.Key);
                    sel.AddValue(facetValue);
                    sel.SelectionOperation = facetField.SelectionOperation;
                    browseRequest.RemoveSelection(facetKvp.Key);
                    browseRequest.AddSelection(sel);
                }
            }

            return browseRequest;
        }
        /// <summary>
        /// Creates a browser request from the passed in key value pair query string to handle a multirequest(ex. 2x f_category in query)
        /// </summary>
        /// <param name="querystring"></param>
        /// <param name="page"></param>
        /// <param name="useOffset"></param>
        /// <param name="itemsPerPage"></param>
        /// <returns></returns>
        public virtual BrowseRequest CreateMultiBrowseRequest(List<KeyValuePair<string, string>> querystring, int? page = null, bool useOffset = false, int itemsPerPage = 10)
        {
            Query query = null;
            var compiledQuery = BuildBaseQuery(querystring);
            if (compiledQuery != null)
            {
                var luceneParams = compiledQuery as LuceneSearchCriteria;
                query = luceneParams?.Query ?? throw new ArgumentException("Provided ISearchCriteria dos not match the allowed ISearchCriteria. Ensure you only use an ISearchCriteria created from the current SearcherProvider");
            }

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
                Sort = DetermineSort(querystring)
            };
            var queryStringKeys = querystring.Select(x => x.Key);
            foreach (var facetField in FacetFields.Where(x => queryStringKeys.Contains(x.Alias.FacetFieldAlias())))
            {
                var facetKvps = querystring.Where(x => x.Key == facetField.Alias.FacetFieldAlias());
                foreach (var facetKvp in facetKvps)
                {

              
                    var facetValue = facetKvp.Value;
                    if (!string.IsNullOrWhiteSpace(facetValue))
                    {
                        var sel = browseRequest.GetSelection(facetKvp.Key) ?? new BrowseSelection(facetKvp.Key);
                        sel.AddValue(facetValue);
                        sel.SelectionOperation = facetField.SelectionOperation;
                        browseRequest.RemoveSelection(facetKvp.Key);
                        browseRequest.AddSelection(sel);
                    }

                }
            }

            return browseRequest;
        }

    }
}

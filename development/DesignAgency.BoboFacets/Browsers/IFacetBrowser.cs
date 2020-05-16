using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using BoboBrowse.Net;
using BoboBrowse.Net.Facets;
using DesignAgency.BoboFacets.Models;
using Lucene.Net.Search;

namespace DesignAgency.BoboFacets.Browsers
{
    public interface IFacetBrowser
    {
        SortField[] DefaultSort { get; }
        List<IFacetField> FacetFields { get; }
        string IndexProvider { get; }
        /// <summary>
        /// Executes the browse request
        /// </summary>
        /// <param name="browseRequest"></param>
        /// <param name="cultureCode"></param>
        /// <param name="facetHandlers"></param>
        /// <param name="addDefaultFacetHandlers"></param>
        /// <returns></returns>
        BrowseResult Browse(BrowseRequest browseRequest, string cultureCode, List<IFacetHandler> facetHandlers = null, bool addDefaultFacetHandlers = true);
        /// <summary>
        /// Builds an optional base query to filter the results
        /// </summary>
        /// <param name="querystring"></param>
        /// <param name="cultureCode"></param>
        /// <returns></returns>
        Query BuildBaseQuery(NameValueCollection querystring, string cultureCode);
        BrowseRequest CreateBrowseRequest(NameValueCollection querystring, string cultureCode, int? page = default(int?), bool useOffset = false, int itemsPerPage = 10);
        /// <summary>
        /// Converts the browser request to facet groups and optionally looks up a display label for the facet value if a facetDisplayDictionaryValues is passed and has a value for the facet alias
        /// </summary>
        /// <param name="facetMap"></param>
        /// <param name="cultureCode"></param>
        /// <param name="facetValueLabelLookupDictionary"></param>
        /// <returns></returns>
        IEnumerable<FacetGroup> ConvertToFacetGroups(IDictionary<string, IFacetAccessible> facetMap, string cultureCode, IDictionary<string, Func<string, IFacetField, string>> facetValueLabelLookupDictionary = null);
        /// <summary>
        /// Converts the selection to a FacetSelection and optionally looks up a display label for the facet value if a facetDisplayDictionaryValues is passed and has a value for the facet alias
        /// </summary>
        /// <param name="browseSelection"></param>
        /// <param name="cultureCode"></param>
        /// <param name="facetValueLabelLookupDictionary"></param>
        /// <returns></returns>
        Dictionary<string, IEnumerable<FacetSelection>> ConvertToFacetSelection(BrowseSelection[] browseSelection, string cultureCode, IDictionary<string, Func<string, IFacetField, string>> facetValueLabelLookupDictionary = null);
        /// <summary>
        /// Determines the sort order of the results. Defaults to the DefaultSort
        /// </summary>
        /// <param name="querystring"></param>
        /// <param name="cultureCode"></param>
        /// <returns></returns>
        SortField[] DetermineSort(NameValueCollection querystring, string cultureCode);
    }
}

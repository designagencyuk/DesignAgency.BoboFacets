using System.Collections.Generic;
using BoboBrowse.Api;
using BoboBrowse.Facets;
using DesignAgency.BoboFacets.Models;
using Examine.SearchCriteria;
using Lucene.Net.Search;

namespace DesignAgency.BoboFacets.Services
{
    public interface IFacetBrowser
    {
        SortField[] DefaultSort { get; }
        IEnumerable<IFacetField> FacetFields { get; }
        string SearchProvider { get; }
        string IndexProvider { get; }

        BrowseResult Browse(BrowseRequest browseRequest, List<FacetHandler> facetHandlers = null, bool addDefaultFacetHandlers = true);
        ISearchCriteria BuildBaseQuery(List<KeyValuePair<string, string>> querystring);
        BrowseRequest ConvertBrowseRequest(List<KeyValuePair<string, string>> querystring, int? page = default(int?), bool useOffset = false, int itemsPerPage = 10);
        IEnumerable<FacetGroup> ConvertToFacetGroups(Dictionary<string, IFacetAccessible> facetMap, Dictionary<string, Dictionary<string, string>> facetDisplayDictionaryValues = null);
        Dictionary<string, IEnumerable<FacetSelection>> ConvertToFacetSelection(BrowseSelection[] browseSelection, Dictionary<string, Dictionary<string, string>> facetDisplayDictionaryValues = null);
        SortField[] DetermineSort(List<KeyValuePair<string, string>> querystring);
    }
}
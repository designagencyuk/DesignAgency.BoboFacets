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
        BrowseResult Browse(BrowseRequest browseRequest, string cultureCode, List<IFacetHandler> facetHandlers = null, bool addDefaultFacetHandlers = true);
        Query BuildBaseQuery(NameValueCollection querystring);
        BrowseRequest CreateBrowseRequest(NameValueCollection querystring, string cultureCode, int? page = default(int?), bool useOffset = false, int itemsPerPage = 10);
        IEnumerable<FacetGroup> ConvertToFacetGroups(IDictionary<string, IFacetAccessible> facetMap, string cultureCode, IDictionary<string, IFacetLabelLookup> facetValueLabelLookupDictionary = null);
        Dictionary<string, IEnumerable<FacetSelection>> ConvertToFacetSelection(BrowseSelection[] browseSelection, string cultureCode, IDictionary<string, IFacetLabelLookup> facetValueLabelLookupDictionary = null);
        SortField[] DetermineSort(NameValueCollection querystring);
    }
}

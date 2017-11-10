using System.Collections.Generic;
using BoboBrowse.Facets;
using Lucene.Net.Documents;
using static BoboBrowse.Api.BrowseSelection;
using static BoboBrowse.Api.FacetSpec;

namespace DesignAgency.BoboFacets.Models
{
    public interface IFacetField
    {
        /// <summary>
        /// The fields current alias
        /// </summary>
        string Alias { get; }
        /// <summary>
        /// The display label for the field
        /// </summary>
        string Label { get; }
        /// <summary>
        /// Indicates if the field contains multiple selection values
        /// </summary>
        bool Multivalue { get; }
        /// <summary>
        /// The order in which the facet options should be displayed
        /// </summary>
        FacetSortSpec ValueOrderBy { get; }
        /// <summary>
        /// Set this to false when the selection operation is ValueOperationAnd in order to get the correct HitCount on the facets
        /// </summary>
        bool ExpandSelection { get; }
        /// <summary>
        /// Sets the min hit count for the field to be returned as a valid facet
        /// </summary>
        int MinHitCount { get; }
        /// <summary>
        /// Sets the operation to using when selecting multiple options
        /// </summary>
        ValueOperation SelectionOperation { get; }
        /// <summary>
        /// Takes the current field value and prepares it for index a facet option
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        IEnumerable<string> PrepareForIndex(string value);

        /// <summary>
        /// Returns the facet handler for this facet field
        /// </summary>
        /// <returns></returns>
        FacetHandler CreateFacetHandler();

        /// <summary>
        /// Returns the index field
        /// </summary>
        /// <param name="fieldValue"></param>
        /// <returns></returns>
        Fieldable CreateIndexField(string fieldValue);

        /// <summary>
        /// Creates a readable label from the value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        string CreateValueLabel(string value);
    }
}
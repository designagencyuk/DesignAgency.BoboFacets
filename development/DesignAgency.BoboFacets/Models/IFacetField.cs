using System.Collections.Generic;
using BoboBrowse.Net;
using BoboBrowse.Net.Facets;
using Lucene.Net.Documents;

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
        bool MultiValue { get; }
        /// <summary>
        /// Indicates if the field is dependant on the culture
        /// </summary>
        bool CultureDependant { get; set; }
        /// <summary>
        /// The order in which the facet options should be displayed
        /// </summary>
        FacetSpec.FacetSortSpec ValueOrderBy { get; }
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
        BrowseSelection.ValueOperation SelectionOperation { get; }
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
        IFacetHandler CreateFacetHandler(string cultureCode);

        /// <summary>
        /// Returns the index field
        /// </summary>
        /// <param name="fieldValue"></param>
        /// <param name="cultureCode"></param>
        /// <returns></returns>
        IFieldable CreateIndexField(string fieldValue, string cultureCode);

        /// <summary>
        /// Creates a readable label from the value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        string CreateValueLabel(string value);

        string CreateFacetFieldAlias(string cultureCode);
    }
}

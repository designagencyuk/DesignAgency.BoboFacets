using System.Collections.Generic;
using BoboBrowse.Net;
using BoboBrowse.Net.Facets;
using Lucene.Net.Documents;

namespace DesignAgency.BoboFacets.Models
{
    public interface IFacetField
    {
        /// <summary>
        /// The fields alias (before it's indexed by the facet handler)
        /// </summary>
        string OriginalAlias { get; }
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
        /// Sets the operation to use when selecting multiple field values
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
        /// Returns the facet spec for this field
        /// </summary>
        /// <returns></returns>
        FacetSpec CreateFacetSpec();

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

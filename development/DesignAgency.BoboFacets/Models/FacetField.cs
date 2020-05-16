using System;
using System.Collections.Generic;
using System.Linq;
using BoboBrowse.Net;
using BoboBrowse.Net.Facets;
using BoboBrowse.Net.Facets.Impl;
using DesignAgency.BoboFacets.Extensions;
using Lucene.Net.Documents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Core;

namespace DesignAgency.BoboFacets.Models
{
    public class FacetField : IFacetField
    {
        public string OriginalAlias { get; set; }
        public string Label { get; set; }
        public bool MultiValue { get; set; }
        public bool CultureDependant { get; set; }
        public BrowseSelection.ValueOperation SelectionOperation { get; set; }

        protected FacetSpec.FacetSortSpec ValueOrderBy { get; set; }
        protected bool ExpandSelection { get; set; }
        protected int MinHitCount { get; set; }
        protected int MaxCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alias">The field alias (before it's indexed by the facet handler)</param>
        /// <param name="label">The display label for the field</param>
        /// <param name="multiValue">Indicates if the field contains multiple selection values</param>
        /// <param name="cultureDependant">Indicates if the field is dependant on the culture</param>
        /// <param name="valueOrderBy">The order in which the facet values should be displayed</param>
        /// <param name="expandSelection">Set this to false when the selection operation is ValueOperationAnd in order to get the correct HitCount on the facets</param>
        /// <param name="minHitCount">The min hit count for the field to be returned as a valid facet</param>
        /// <param name="maxCount">The maximum number of choices to return. Default = 0 which means all.</param>
        /// <param name="valueOperation">Sets the operation to use when selecting multiple field values</param>
        public FacetField(string alias, string label, bool multiValue, bool cultureDependant = false, FacetSpec.FacetSortSpec valueOrderBy = FacetSpec.FacetSortSpec.OrderHitsDesc, bool expandSelection = true, int minHitCount = 0, int maxCount = 0, BrowseSelection.ValueOperation valueOperation = BrowseSelection.ValueOperation.ValueOperationOr)
        {
            OriginalAlias = alias;
            Label = label;
            MultiValue = multiValue;
            CultureDependant = cultureDependant;
            ValueOrderBy = valueOrderBy;
            ExpandSelection = expandSelection;
            MinHitCount = minHitCount;
            MaxCount = maxCount;
            SelectionOperation = valueOperation;
        }

        /// <summary>
        /// Supports MultiValueFacetHandler & the SimpleFacetHandler
        /// </summary>
        /// <returns></returns>
        // ReSharper disable once InheritdocConsiderUsage
        public virtual IFacetHandler CreateFacetHandler(string cultureCode)
        {
            if (MultiValue)
            {
                return new MultiValueFacetHandler(this.CreateFacetFieldAlias(cultureCode));
            }
            return new SimpleFacetHandler(this.CreateFacetFieldAlias(cultureCode));
        }

        public virtual FacetSpec CreateFacetSpec()
        {
            return new FacetSpec
            {
                OrderBy = ValueOrderBy,
                ExpandSelection = ExpandSelection,
                MinHitCount = MinHitCount,
                MaxCount = MaxCount
            };
        }

        /// <summary>
        /// Supports converting a csv value, Umbraco tags json value and a NuPicker json value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        // ReSharper disable once InheritdocConsiderUsage
        public virtual IEnumerable<string> PrepareForIndex(string value)
        {
            if (!MultiValue)
                return new[] { value };

            IEnumerable<string> values;
            // Values may be stored as json if property is using Umbraco.Tags/Nupickers and set to store as json
            if (value.DetectIsJson())
            {
                // try parse for nupickers json format, failing that parse for umbraco tags format
                try
                {
                    values = JsonConvert.DeserializeObject<JArray>(value).Select(x => x["key"].ToString()).ToList();
                }
                catch (Exception)
                {
                    values = JsonConvert.DeserializeObject<JArray>(value).ToObject<List<string>>();
                }
            }
            else
            {
                values = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            return values;
        }

        public virtual IFieldable CreateIndexField(string fieldValue, string cultureCode)
        {
            return new Field(this.CreateFacetFieldAlias(cultureCode), fieldValue.Trim(), Field.Store.YES, Field.Index.NOT_ANALYZED);
        }

        public virtual string CreateValueLabel(string value)
        {
            return value;
        }

        public virtual string CreateFacetFieldAlias(string cultureCode)
        {
            return this.FacetFieldAlias(cultureCode);
        }
    }
}

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
        public string Alias { get; set; }

        public string Label { get; set; }

        public bool MultiValue { get; set; }
        public bool CultureDependant { get; set; }

        public FacetSpec.FacetSortSpec ValueOrderBy { get; set; }

        public bool ExpandSelection { get; set; }

        public int MinHitCount { get; set; }

        public BrowseSelection.ValueOperation SelectionOperation { get; set; }

        public FacetField(string alias, string label, bool multiValue, bool cultureDependant = false, FacetSpec.FacetSortSpec valueOrderBy = FacetSpec.FacetSortSpec.OrderHitsDesc, bool expandSelection = true, int minHitCount = 0, BrowseSelection.ValueOperation valueOperation = BrowseSelection.ValueOperation.ValueOperationOr)
        {
            Alias = alias;
            Label = label;
            MultiValue = multiValue;
            CultureDependant = cultureDependant;
            ValueOrderBy = valueOrderBy;
            ExpandSelection = expandSelection;
            MinHitCount = minHitCount;
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

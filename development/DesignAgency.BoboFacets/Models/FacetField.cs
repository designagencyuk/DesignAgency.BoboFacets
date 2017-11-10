using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using static BoboBrowse.Api.FacetSpec;
using static BoboBrowse.Api.BrowseSelection;

namespace DesignAgency.BoboFacets.Models
{
    public class FacetField : IFacetField
    {
        public string Alias { get; set; }

        public string Label { get; set; }

        public bool Multivalue { get; set; }

        public FacetSortSpec ValueOrderBy { get; set; }

        public bool ExpandSelection { get; set; }

        public int MinHitCount { get; set; }

        public ValueOperation SelectionOperation { get; set; }

        public FacetField(string alias, string label, bool multiValue, FacetSortSpec valueOrderBy = FacetSortSpec.OrderHitsDesc, bool expandSelection = true, int minHitCount = 0, ValueOperation valueOperation = ValueOperation.ValueOperationOr)
        {
            Alias = alias;
            Label = label;
            Multivalue = multiValue;
            ValueOrderBy = valueOrderBy;
            ExpandSelection = expandSelection;
            MinHitCount = minHitCount;
            SelectionOperation = valueOperation;
        }

        public virtual IEnumerable<string> PrepareForIndex(string value)
        {
            if (!Multivalue)
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
    }
}

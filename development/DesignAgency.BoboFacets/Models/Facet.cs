using umbraco.cms.presentation;

namespace DesignAgency.BoboFacets.Models
{
    public class Facet
    {
        public string Label { get; set; }
        public string Value { get; set; }
        public long Count { get; set; }
        public int Sort { get; set; } 
    }
}

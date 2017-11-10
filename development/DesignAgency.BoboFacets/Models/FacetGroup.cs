using System.Collections.Generic;

namespace DesignAgency.BoboFacets.Models
{
    public class FacetGroup
    {
        public FacetGroup()
        {
            Facets = new List<Facet>();
        }

        public string Label { get; set; }
        public string Alias { get; set; }
        public List<Facet> Facets { get; set; }
    }
}

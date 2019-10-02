namespace DesignAgency.BoboFacets.Models
{
    public interface IFacetLabelLookup
    {
        string LookupLabel(string value, IFacetField facetField);
    }
}

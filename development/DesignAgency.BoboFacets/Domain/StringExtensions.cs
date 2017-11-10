namespace DesignAgency.BoboFacets.Domain
{
    public static class StringExtensions
    {
        public static string FacetFieldAlias(this string fieldAlias)
        {
            return Constants.Conventions.FacetFieldPrefix + fieldAlias;
        }
    }
}

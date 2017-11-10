using DesignAgency.BoboFacets.Services;
using System;
using System.Collections.Generic;
using Umbraco.Core.ObjectResolution;

namespace DesignAgency.BoboFacets
{
    internal class BoboBrowsersResolver : ManyObjectsResolverBase<BoboBrowsersResolver, IFacetBrowser>
    {
	    internal BoboBrowsersResolver(IEnumerable<Type> finders)
			: base(finders)
		{ }
        
        internal BoboBrowsersResolver(params Type[] finders)
            : base(finders)
        { }
        
        internal IEnumerable<IFacetBrowser> Browsers
        {
            get { return Values; }
        }
    }
}

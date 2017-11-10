using DesignAgency.BoboFacets.Services;
using System;
using System.Collections.Generic;
using Umbraco.Core;

namespace DesignAgency.BoboFacets.Domain
{
    public static class PluginManagerExtensions
    {
        internal static IEnumerable<Type> ResolveBrowsers(this PluginManager resolver)
        {
            return resolver.ResolveTypes<IFacetBrowser>();
        }
    }
}

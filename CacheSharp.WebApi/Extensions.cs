using System.Collections.Generic;
using System.Linq;

namespace CacheSharp.WebApi
{
    public static class Extensions
    {
        public static string ToCacheKey(this Dictionary<string, object> arguments, string scope = CacheScope.Application)
        {
            return arguments.Aggregate(scope, (current, arg) => current + ("," + arg.Key + "=" + arg.Value));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwaggerWcf.Support
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> FilterUnique<T>(this IEnumerable<T> list)
        {
            return list.GroupBy(e => e).Select(g => g.Key);
        }
    }
}

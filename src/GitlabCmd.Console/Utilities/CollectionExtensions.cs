using System.Collections.Generic;
using System.Linq;

namespace GitlabCmd.Console.Utilities
{
    public static class CollectionExtensions
    {
        public static bool Contains<T>(this IEnumerable<T> sequence, IEnumerable<T> other)
            => new HashSet<T>(sequence).IsSubsetOf(other);

        public static IReadOnlyCollection<T> SafeEnumerate<T>(this IEnumerable<T> sequence) 
            => sequence == null ? new List<T>() : sequence.ToList();
    }
}

using System.Collections.Generic;
using System.Linq;

namespace GitlabCmd.Utilities
{
    public static class CollectionExtensions
    {
        public static bool Contains<T>(this IEnumerable<T> sequence, IEnumerable<T> other)
            => new HashSet<T>(other).IsSubsetOf(sequence);

        public static IReadOnlyCollection<T> SafeEnumerate<T>(this IEnumerable<T> sequence) 
            => sequence == null ? new List<T>() : sequence.ToList();

        public static IEnumerable<string> NormalizeSpaces(this IEnumerable<string> sequence)
            => sequence.Select(s => s ?? "").Select(s => s.Trim());
    }
}

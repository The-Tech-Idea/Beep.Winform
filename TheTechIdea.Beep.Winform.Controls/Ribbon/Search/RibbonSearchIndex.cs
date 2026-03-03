using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Search
{
    public static class RibbonSearchIndex
    {
        public static List<RibbonSearchMatch> BuildMatches(
            string query,
            IEnumerable<SimpleItem> candidates,
            IReadOnlyDictionary<string, int>? commandUsage = null,
            int maxResults = 12,
            Func<SimpleItem, int>? scoreBoostProvider = null)
        {
            if (string.IsNullOrWhiteSpace(query) || candidates == null)
            {
                return [];
            }

            var matches = new List<RibbonSearchMatch>();
            foreach (var candidate in candidates)
            {
                var match = RibbonSearchRanking.Score(candidate, query, commandUsage, scoreBoostProvider);
                if (match != null)
                {
                    matches.Add(match);
                }
            }

            return matches
                .OrderByDescending(m => m.Score)
                .ThenBy(m => GetDisplayText(m.Item), StringComparer.CurrentCultureIgnoreCase)
                .Take(Math.Max(1, maxResults))
                .ToList();
        }

        public static List<SimpleItem> RankCommands(
            string query,
            IEnumerable<SimpleItem> candidates,
            IReadOnlyDictionary<string, int>? commandUsage = null,
            int maxResults = 12,
            Func<SimpleItem, int>? scoreBoostProvider = null)
        {
            return BuildMatches(query, candidates, commandUsage, maxResults, scoreBoostProvider)
                .Select(m => m.Item)
                .ToList();
        }

        private static string GetDisplayText(SimpleItem item)
        {
            if (!string.IsNullOrWhiteSpace(item.DisplayField))
            {
                return item.DisplayField;
            }

            if (!string.IsNullOrWhiteSpace(item.Text))
            {
                return item.Text;
            }

            return item.Name ?? string.Empty;
        }
    }
}

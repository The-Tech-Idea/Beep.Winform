using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Search
{
    public enum RibbonSearchMatchKind
    {
        Exact,
        StartsWith,
        Contains,
        Fuzzy,
        Tooltip,
        Shortcut,
        Metadata
    }

    public sealed class RibbonSearchMatch
    {
        public required SimpleItem Item { get; init; }
        public required string CommandKey { get; init; }
        public required int Score { get; init; }
        public required RibbonSearchMatchKind MatchKind { get; init; }
    }

    public static class RibbonSearchRanking
    {
        public static RibbonSearchMatch? Score(
            SimpleItem item,
            string query,
            IReadOnlyDictionary<string, int>? commandUsage = null,
            Func<SimpleItem, int>? scoreBoostProvider = null)
        {
            if (item == null || string.IsNullOrWhiteSpace(query))
            {
                return null;
            }

            string q = query.Trim();
            if (q.Length == 0)
            {
                return null;
            }

            string text = GetDisplayText(item);
            string tooltip = item.ToolTip ?? string.Empty;
            string shortcut = item.ShortcutText ?? string.Empty;
            string metadata = BuildMetadata(item);

            int score = 0;
            RibbonSearchMatchKind matchKind = RibbonSearchMatchKind.Metadata;

            if (EqualsIgnoreCase(text, q))
            {
                score = 1000;
                matchKind = RibbonSearchMatchKind.Exact;
            }
            else if (StartsWithIgnoreCase(text, q))
            {
                score = 850;
                matchKind = RibbonSearchMatchKind.StartsWith;
            }
            else if (ContainsIgnoreCase(text, q))
            {
                score = 700;
                matchKind = RibbonSearchMatchKind.Contains;
            }
            else
            {
                int fuzzyScore = GetFuzzyScore(text, q);
                if (fuzzyScore > 0)
                {
                    score = fuzzyScore;
                    matchKind = RibbonSearchMatchKind.Fuzzy;
                }
                else if (ContainsIgnoreCase(tooltip, q))
                {
                    score = 520;
                    matchKind = RibbonSearchMatchKind.Tooltip;
                }
                else if (ContainsIgnoreCase(shortcut, q))
                {
                    score = 460;
                    matchKind = RibbonSearchMatchKind.Shortcut;
                }
                else if (ContainsIgnoreCase(metadata, q))
                {
                    score = 360;
                    matchKind = RibbonSearchMatchKind.Metadata;
                }
                else
                {
                    return null;
                }
            }

            string key = GetCommandKey(item);
            if (commandUsage != null && commandUsage.TryGetValue(key, out int usageScore))
            {
                score += Math.Min(140, usageScore * 10);
            }

            if (scoreBoostProvider != null)
            {
                score += Math.Clamp(scoreBoostProvider(item), -200, 300);
            }

            return new RibbonSearchMatch
            {
                Item = item,
                CommandKey = key,
                Score = score,
                MatchKind = matchKind
            };
        }

        private static string BuildMetadata(SimpleItem item)
        {
            return string.Join(' ',
            [
                item.Name ?? string.Empty,
                item.Category.ToString(),
                item.GroupName ?? string.Empty,
                item.Description ?? string.Empty,
                item.SubText ?? string.Empty,
                item.SubText2 ?? string.Empty,
                item.SubText3 ?? string.Empty
            ]);
        }

        private static bool EqualsIgnoreCase(string value, string query)
        {
            return !string.IsNullOrWhiteSpace(value) &&
                   value.Equals(query, StringComparison.OrdinalIgnoreCase);
        }

        private static bool StartsWithIgnoreCase(string value, string query)
        {
            return !string.IsNullOrWhiteSpace(value) &&
                   value.StartsWith(query, StringComparison.OrdinalIgnoreCase);
        }

        private static bool ContainsIgnoreCase(string value, string query)
        {
            return !string.IsNullOrWhiteSpace(value) &&
                   value.Contains(query, StringComparison.OrdinalIgnoreCase);
        }

        private static int GetFuzzyScore(string value, string query)
        {
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(query))
            {
                return 0;
            }

            string left = NormalizeForFuzzy(value);
            string right = NormalizeForFuzzy(query);
            if (left.Length == 0 || right.Length == 0)
            {
                return 0;
            }

            int limit = right.Length <= 4 ? 1 : right.Length <= 8 ? 2 : 3;
            int lengthDiff = Math.Abs(left.Length - right.Length);
            if (lengthDiff <= limit + 2)
            {
                int distance = LevenshteinDistance(left, right, limit + 1);
                if (distance <= limit)
                {
                    return 640 - (distance * 36);
                }
            }

            foreach (var token in TokenizeForFuzzy(value))
            {
                if (token.Length < 2)
                {
                    continue;
                }

                int tokenLimit = right.Length <= 5 ? 1 : 2;
                int tokenDiff = Math.Abs(token.Length - right.Length);
                if (tokenDiff > tokenLimit + 2)
                {
                    continue;
                }

                int tokenDistance = LevenshteinDistance(token, right, tokenLimit + 1);
                if (tokenDistance <= tokenLimit)
                {
                    return 605 - (tokenDistance * 28);
                }
            }

            if (IsSubsequence(left, right))
            {
                return 560;
            }

            return 0;
        }

        private static string NormalizeForFuzzy(string value)
        {
            var chars = value.Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray();
            return new string(chars);
        }

        private static IEnumerable<string> TokenizeForFuzzy(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                yield break;
            }

            var token = new List<char>(value.Length);
            foreach (char ch in value)
            {
                if (char.IsLetterOrDigit(ch))
                {
                    token.Add(char.ToLowerInvariant(ch));
                    continue;
                }

                if (token.Count > 0)
                {
                    yield return new string(token.ToArray());
                    token.Clear();
                }
            }

            if (token.Count > 0)
            {
                yield return new string(token.ToArray());
            }
        }

        private static bool IsSubsequence(string value, string query)
        {
            if (query.Length > value.Length)
            {
                return false;
            }

            int i = 0;
            int j = 0;
            while (i < value.Length && j < query.Length)
            {
                if (value[i] == query[j])
                {
                    j++;
                }

                i++;
            }

            return j == query.Length;
        }

        private static int LevenshteinDistance(string left, string right, int limit)
        {
            int n = left.Length;
            int m = right.Length;
            if (n == 0) return m;
            if (m == 0) return n;

            var prev = new int[m + 1];
            var curr = new int[m + 1];
            for (int j = 0; j <= m; j++) prev[j] = j;

            for (int i = 1; i <= n; i++)
            {
                curr[0] = i;
                int rowMin = curr[0];
                for (int j = 1; j <= m; j++)
                {
                    int cost = left[i - 1] == right[j - 1] ? 0 : 1;
                    curr[j] = Math.Min(
                        Math.Min(prev[j] + 1, curr[j - 1] + 1),
                        prev[j - 1] + cost);
                    if (curr[j] < rowMin) rowMin = curr[j];
                }

                if (rowMin > limit)
                {
                    return rowMin;
                }

                (prev, curr) = (curr, prev);
            }

            return prev[m];
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

        private static string GetCommandKey(SimpleItem item)
        {
            if (!string.IsNullOrWhiteSpace(item.ActionID)) return item.ActionID;
            if (!string.IsNullOrWhiteSpace(item.ReferenceID)) return item.ReferenceID;
            if (!string.IsNullOrWhiteSpace(item.GuidId)) return item.GuidId;
            if (!string.IsNullOrWhiteSpace(item.Name)) return item.Name;
            if (!string.IsNullOrWhiteSpace(item.Text)) return item.Text;
            return $"{item.MenuID}:{item.MenuName}:{item.ItemType}";
        }
    }
}

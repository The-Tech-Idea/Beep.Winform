using System;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers
{
    /// <summary>
    /// Performs prefix and fuzzy search against popup rows.
    /// Provides filtered results and an index of the best match to highlight.
    /// </summary>
    internal static class ComboBoxSearchEngine
    {
        internal readonly record struct SearchMatch(int Start, int Length, int Score)
        {
            public static SearchMatch None => new SearchMatch(-1, 0, int.MinValue);
            public bool IsMatch => Score > int.MinValue;
        }

        public static (IReadOnlyList<ComboBoxPopupRowModel> Results, int BestMatchIndex) FilterRows(
            IReadOnlyList<ComboBoxPopupRowModel> allRows,
            string searchText,
            bool fuzzyMatch = true)
        {
            if (allRows == null || allRows.Count == 0)
            {
                return (Array.Empty<ComboBoxPopupRowModel>(), -1);
            }

            if (string.IsNullOrWhiteSpace(searchText))
            {
                return (allRows, -1);
            }

            var query = searchText.Trim();
            var results = new List<ComboBoxPopupRowModel>();
            int bestMatchIndex = -1;
            int bestScore = int.MinValue;

            int index = 0;
            while (index < allRows.Count)
            {
                var row = allRows[index];

                // Group block filtering: keep a header only when at least one child row matches.
                if (row.RowKind == ComboBoxPopupRowKind.GroupHeader)
                {
                    int nextHeader = index + 1;
                    while (nextHeader < allRows.Count && allRows[nextHeader].RowKind != ComboBoxPopupRowKind.GroupHeader)
                    {
                        nextHeader++;
                    }

                    var groupRows = new List<ComboBoxPopupRowModel>();
                    for (int i = index + 1; i < nextHeader; i++)
                    {
                        var child = allRows[i];
                        if (child.RowKind == ComboBoxPopupRowKind.Separator)
                        {
                            continue;
                        }

                        var match = MatchRow(child, query, fuzzyMatch);
                        if (!match.IsMatch)
                        {
                            continue;
                        }

                        groupRows.Add(WithMatch(child, match));
                        if (match.Score > bestScore)
                        {
                            bestScore = match.Score;
                        }
                    }

                    if (groupRows.Count > 0)
                    {
                        results.Add(row);
                        foreach (var matched in groupRows)
                        {
                            results.Add(matched);
                            if (matched.MatchLength > 0 && matched.MatchStart >= 0)
                            {
                                if (matched.MatchLength > 0 && bestMatchIndex < 0)
                                {
                                    // Set to first matched row after a group header if we have none yet.
                                    bestMatchIndex = results.Count - 1;
                                }
                            }
                        }
                    }

                    index = nextHeader;
                    continue;
                }

                if (row.RowKind == ComboBoxPopupRowKind.Separator)
                {
                    index++;
                    continue;
                }

                var standaloneMatch = MatchRow(row, query, fuzzyMatch);
                if (standaloneMatch.IsMatch)
                {
                    var matchedRow = WithMatch(row, standaloneMatch);
                    results.Add(matchedRow);

                    if (standaloneMatch.Score > bestScore)
                    {
                        bestScore = standaloneMatch.Score;
                        bestMatchIndex = results.Count - 1;
                    }
                }

                index++;
            }

            // If we found matches but never set bestMatchIndex (group path), fallback to first data row.
            if (bestMatchIndex < 0)
            {
                for (int i = 0; i < results.Count; i++)
                {
                    if (results[i].RowKind != ComboBoxPopupRowKind.GroupHeader &&
                        results[i].RowKind != ComboBoxPopupRowKind.Separator)
                    {
                        bestMatchIndex = i;
                        break;
                    }
                }
            }

            return (results, bestMatchIndex);
        }

        private static ComboBoxPopupRowModel WithMatch(ComboBoxPopupRowModel row, SearchMatch match)
            => new ComboBoxPopupRowModel
            {
                SourceItem = row.SourceItem,
                RowKind = row.RowKind,
                Text = row.Text,
                SubText = row.SubText,
                TrailingText = row.TrailingText,
                TrailingValueText = row.TrailingValueText,
                ImagePath = row.ImagePath,
                GroupName = row.GroupName,
                LayoutPreset = row.LayoutPreset,
                IsSelected = row.IsSelected,
                IsEnabled = row.IsEnabled,
                IsKeyboardFocused = row.IsKeyboardFocused,
                IsCheckable = row.IsCheckable,
                IsChecked = row.IsChecked,
                MatchStart = match.Start,
                MatchLength = match.Length,
                ListIndex = row.ListIndex
            };

        private static SearchMatch MatchRow(ComboBoxPopupRowModel row, string query, bool fuzzy)
        {
            if (row == null || !row.IsEnabled)
            {
                return SearchMatch.None;
            }

            if (row.RowKind is ComboBoxPopupRowKind.GroupHeader or
                ComboBoxPopupRowKind.Separator or
                ComboBoxPopupRowKind.EmptyState or
                ComboBoxPopupRowKind.LoadingState)
            {
                return SearchMatch.None;
            }

            var primary = ScoreText(row.Text, query, fuzzy, baseWeight: 100);
            var secondary = ScoreText(row.SubText, query, fuzzy, baseWeight: 70);

            if (primary.Score >= secondary.Score)
            {
                return primary;
            }

            return secondary;
        }

        private static SearchMatch ScoreText(string text, string query, bool fuzzy, int baseWeight)
        {
            if (string.IsNullOrEmpty(text))
            {
                return SearchMatch.None;
            }

            // Prefix match is best.
            if (text.StartsWith(query, StringComparison.OrdinalIgnoreCase))
            {
                int score = baseWeight + 300 - Math.Max(0, text.Length - query.Length);
                return new SearchMatch(0, query.Length, score);
            }

            // Substring match.
            int idx = text.IndexOf(query, StringComparison.OrdinalIgnoreCase);
            if (idx >= 0)
            {
                int score = baseWeight + 200 - idx;
                return new SearchMatch(idx, query.Length, score);
            }

            // Fuzzy subsequence fallback.
            if (fuzzy)
            {
                if (IsSubsequence(query, text, out int spread, out int firstIndex, out int lastIndex))
                {
                    int score = baseWeight + 100 - spread;
                    int length = Math.Max(1, (lastIndex - firstIndex) + 1);
                    return new SearchMatch(firstIndex, length, score);
                }
            }

            return SearchMatch.None;
        }

        private static bool IsSubsequence(string query, string text, out int spread, out int firstMatch, out int lastMatch)
        {
            spread = 0;
            firstMatch = -1;
            lastMatch = -1;

            if (query.Length > text.Length) return false;

            int qIdx = 0;

            for (int tIdx = 0; tIdx < text.Length && qIdx < query.Length; tIdx++)
            {
                if (char.ToLowerInvariant(text[tIdx]) == char.ToLowerInvariant(query[qIdx]))
                {
                    if (firstMatch == -1) firstMatch = tIdx;
                    lastMatch = tIdx;
                    qIdx++;
                }
            }

            if (qIdx == query.Length)
            {
                spread = lastMatch - firstMatch;
                return true;
            }

            return false;
        }
    }
}

using System;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.CommandPalette
{
    /// <summary>
    /// Subsequence matching with a relevance score, as used by VS Code's Quick Pick, Raycast and
    /// Linear's Cmd-K.
    /// </summary>
    /// <remarks>
    /// The palette previously filtered with <c>string.Contains</c>, which is the difference between
    /// a command palette and a filtered list box: <c>"opfi"</c> finds nothing, and neither does
    /// <c>"newwin"</c> for "New Window". Every reference product matches on subsequence — the query's
    /// characters appearing in order, not necessarily adjacently — and ranks by how well they land.
    /// </remarks>
    internal static class CommandFuzzyMatcher
    {
        /// <summary>
        /// Scores <paramref name="candidate"/> against <paramref name="query"/>, and reports which
        /// characters matched so the caller can highlight them.
        /// </summary>
        /// <returns><see langword="false"/> when the query is not a subsequence of the candidate.</returns>
        public static bool TryMatch(string candidate, string query, out int score, out List<int> matchedIndexes)
        {
            score = 0;
            matchedIndexes = new List<int>();

            if (string.IsNullOrEmpty(candidate)) return false;
            if (string.IsNullOrWhiteSpace(query)) { score = 1; return true; }

            int ci = 0;
            int consecutive = 0;

            for (int qi = 0; qi < query.Length; qi++)
            {
                char q = char.ToLowerInvariant(query[qi]);
                if (char.IsWhiteSpace(q)) continue;

                bool found = false;
                while (ci < candidate.Length)
                {
                    char c = char.ToLowerInvariant(candidate[ci]);
                    if (c == q)
                    {
                        matchedIndexes.Add(ci);

                        // A match at a word boundary is what the user most likely meant: "opfi"
                        // should rank "Open File" above "Loop Filter", where the same letters occur
                        // mid-word. Consecutive matches score higher again, so a literal substring
                        // still beats a scattered subsequence.
                        bool atStart = ci == 0;
                        bool afterSeparator = ci > 0 &&
                            (char.IsWhiteSpace(candidate[ci - 1]) || candidate[ci - 1] is '-' or '_' or '.' or '/');
                        bool camelHump = ci > 0 && char.IsLower(candidate[ci - 1]) && char.IsUpper(candidate[ci]);

                        score += 1;
                        if (atStart) score += 8;
                        else if (afterSeparator || camelHump) score += 5;

                        consecutive = consecutive > 0 ? consecutive + 1 : 1;
                        score += consecutive;

                        ci++;
                        found = true;
                        break;
                    }

                    consecutive = 0;
                    ci++;
                }

                if (!found) { score = 0; matchedIndexes.Clear(); return false; }
            }

            // Shorter candidates win ties: with equal matches, "Open" is a better answer than
            // "Open Recent Workspace From Folder".
            score += Math.Max(0, 20 - candidate.Length / 4);
            return true;
        }
    }
}

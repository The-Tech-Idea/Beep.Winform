// BeepCommandRegistry.cs
// Central registry of BeepCommandEntry instances for BeepDocumentHost.
// Provides registration, fuzzy search, category grouping, and usage tracking.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// Stores and queries all <see cref="BeepCommandEntry"/> instances for a
    /// <see cref="BeepDocumentHost"/>.  Supports fuzzy search and usage-frequency ranking.
    /// </summary>
    public sealed class BeepCommandRegistry
    {
        private readonly Dictionary<string, BeepCommandEntry> _commands =
            new(StringComparer.Ordinal);

        // ── Registration ──────────────────────────────────────────────────────

        /// <summary>Registers a command, replacing any existing entry with the same id.</summary>
        public void Register(BeepCommandEntry entry)
        {
            ArgumentNullException.ThrowIfNull(entry);
            if (string.IsNullOrWhiteSpace(entry.Id))
                throw new ArgumentException("Command id must not be empty.", nameof(entry));
            _commands[entry.Id] = entry;
        }

        /// <summary>Removes the command with the given id.</summary>
        /// <returns><see langword="true"/> if found and removed.</returns>
        public bool Unregister(string id)
        {
            ArgumentNullException.ThrowIfNull(id);
            return _commands.Remove(id);
        }

        // ── Query ─────────────────────────────────────────────────────────────

        /// <summary>Returns the command with the given id, or null if not found.</summary>
        public BeepCommandEntry? FindById(string id)
        {
            ArgumentNullException.ThrowIfNull(id);
            return _commands.TryGetValue(id, out var e) ? e : null;
        }

        /// <summary>Returns all commands in the given category (case-insensitive).</summary>
        public IReadOnlyList<BeepCommandEntry> GetByCategory(string category)
        {
            ArgumentNullException.ThrowIfNull(category);
            return _commands.Values
                .Where(c => string.Equals(c.Category, category, StringComparison.OrdinalIgnoreCase))
                .OrderBy(c => c.Title)
                .ToList();
        }

        /// <summary>Returns all registered commands sorted by category then title.</summary>
        public IReadOnlyList<BeepCommandEntry> GetAll()
            => _commands.Values
                .OrderBy(c => c.Category)
                .ThenBy(c => c.Title)
                .ToList();

        /// <summary>
        /// Fuzzy-searches commands by title.  An empty query returns all commands sorted
        /// by usage frequency.  Results are ranked: prefix match &gt; contains &gt; subsequence.
        /// </summary>
        public IReadOnlyList<BeepCommandEntry> FuzzySearch(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return _commands.Values
                    .OrderByDescending(c => c.UsageCount)
                    .ThenByDescending(c => c.LastUsed ?? DateTime.MinValue)
                    .ThenBy(c => c.Title)
                    .ToList();

            string q = query.Trim();
            var scored = new List<(BeepCommandEntry Entry, int Score)>();
            foreach (var cmd in _commands.Values)
            {
                int score = FuzzyScore(cmd.Title, q);
                if (score > 0)
                    scored.Add((cmd, score));
            }
            return scored
                .OrderByDescending(x => x.Score)
                .ThenByDescending(x => x.Entry.UsageCount)
                .Select(x => x.Entry)
                .ToList();
        }

        /// <summary>Increments the usage counter for the command with the given id.</summary>
        public void RecordUsage(string id)
        {
            if (_commands.TryGetValue(id, out var cmd))
            {
                cmd.UsageCount++;
                cmd.LastUsed = DateTime.UtcNow;
            }
        }

        // ── Fuzzy scoring ─────────────────────────────────────────────────────

        /// <summary>
        /// Returns a positive score when all query characters appear in <paramref name="source"/>
        /// as a subsequence (case-insensitive).  Returns 0 when the query is not matched.
        /// Higher score = better match.
        /// </summary>
        private static int FuzzyScore(string source, string query)
        {
            if (string.IsNullOrEmpty(source)) return 0;

            if (source.StartsWith(query, StringComparison.OrdinalIgnoreCase))
                return 1000 + (100 - Math.Min(source.Length, 100));

            int ci = source.IndexOf(query, StringComparison.OrdinalIgnoreCase);
            if (ci >= 0)
                return 500 + (100 - Math.Min(ci, 100));

            // Subsequence match
            int qi = 0, si = 0, gaps = 0;
            string sl = source.ToLowerInvariant();
            string ql = query.ToLowerInvariant();
            while (si < sl.Length && qi < ql.Length)
            {
                if (sl[si] == ql[qi]) qi++;
                else gaps++;
                si++;
            }
            return qi < ql.Length ? 0 : Math.Max(1, 200 - gaps);
        }
    }
}

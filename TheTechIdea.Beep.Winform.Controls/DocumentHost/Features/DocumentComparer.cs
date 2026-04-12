// DocumentComparer.cs
// Line-by-line text diff engine for DiffViewPanel.
// Uses the Myers O(ND) diff algorithm to produce a list of DiffLine entries
// (Added / Removed / Unchanged) suitable for rendering by DiffHighlightPainter.
// No external dependencies — pure System.Collections.Generic logic.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost.Features
{
    /// <summary>Classification of a single diff line.</summary>
    public enum DiffLineKind { Unchanged, Added, Removed }

    /// <summary>A single line in a diff result.</summary>
    public sealed record DiffLine(DiffLineKind Kind, int LineA, int LineB, string Text);

    /// <summary>
    /// Summary statistics returned alongside the diff.
    /// </summary>
    public sealed record DiffStats(int LinesAdded, int LinesRemoved, int LinesChanged, int LinesTotal);

    /// <summary>
    /// Result of comparing two text documents.
    /// </summary>
    public sealed class DiffResult
    {
        /// <summary>Line-by-line merge sequence.</summary>
        public IReadOnlyList<DiffLine> Lines { get; init; } = Array.Empty<DiffLine>();

        /// <summary>Summary statistics.</summary>
        public DiffStats Stats { get; init; } = new DiffStats(0, 0, 0, 0);

        /// <summary><c>true</c> when the two texts are identical.</summary>
        public bool IsIdentical => Stats.LinesAdded == 0 && Stats.LinesRemoved == 0;
    }

    /// <summary>
    /// Static engine that diffs two multi-line strings and returns a
    /// <see cref="DiffResult"/> ready for rendering.
    /// </summary>
    public static class DocumentComparer
    {
        /// <summary>
        /// Compares <paramref name="textA"/> (left/old) with <paramref name="textB"/> (right/new).
        /// </summary>
        public static DiffResult Compare(string textA, string textB,
            StringComparison comparison = StringComparison.Ordinal)
        {
            var linesA = SplitLines(textA);
            var linesB = SplitLines(textB);

            var script = ComputeEditScript(linesA, linesB, comparison);
            var result = new List<DiffLine>(linesA.Count + linesB.Count);

            int ia = 0, ib = 0;
            int added = 0, removed = 0;

            foreach (var op in script)
            {
                switch (op)
                {
                    case EditOp.Keep:
                        result.Add(new DiffLine(DiffLineKind.Unchanged, ia, ib, linesA[ia]));
                        ia++; ib++;
                        break;
                    case EditOp.Delete:
                        result.Add(new DiffLine(DiffLineKind.Removed, ia, -1, linesA[ia]));
                        ia++; removed++;
                        break;
                    case EditOp.Insert:
                        result.Add(new DiffLine(DiffLineKind.Added, -1, ib, linesB[ib]));
                        ib++; added++;
                        break;
                }
            }

            int changed = Math.Min(added, removed);
            var stats = new DiffStats(added, removed, changed, result.Count);
            return new DiffResult { Lines = result, Stats = stats };
        }

        // ── Myers diff (simplified linear-space shortest-edit-script) ─────────

        private enum EditOp { Keep, Delete, Insert }

        private static List<EditOp> ComputeEditScript(
            IList<string> a, IList<string> b, StringComparison cmp)
        {
            int n = a.Count, m = b.Count;
            int max = n + m;
            if (max == 0) return new List<EditOp>();

            var v   = new int[2 * max + 1];
            var trace = new List<int[]>();

            for (int d = 0; d <= max; d++)
            {
                var snap = (int[])v.Clone();
                trace.Add(snap);

                for (int k = -d; k <= d; k += 2)
                {
                    int x;
                    if (k == -d || (k != d && v[k - 1 + max] < v[k + 1 + max]))
                        x = v[k + 1 + max];
                    else
                        x = v[k - 1 + max] + 1;

                    int y = x - k;
                    while (x < n && y < m &&
                           string.Equals(a[x], b[y], cmp))
                    { x++; y++; }

                    v[k + max] = x;
                    if (x >= n && y >= m)
                        return Backtrack(trace, a, b, cmp, max);
                }
            }

            // Fallback: treat everything as replace
            var ops = new List<EditOp>(n + m);
            for (int i = 0; i < n; i++) ops.Add(EditOp.Delete);
            for (int j = 0; j < m; j++) ops.Add(EditOp.Insert);
            return ops;
        }

        private static List<EditOp> Backtrack(
            List<int[]> trace, IList<string> a, IList<string> b,
            StringComparison cmp, int max)
        {
            int x = a.Count, y = b.Count;
            var ops = new List<EditOp>();

            for (int d = trace.Count - 1; d >= 0; d--)
            {
                var v = trace[d];
                int k = x - y;

                int prevK;
                if (k == -d || (k != d && v[k - 1 + max] < v[k + 1 + max]))
                    prevK = k + 1;
                else
                    prevK = k - 1;

                int prevX = v[prevK + max];
                int prevY = prevX - prevK;

                while (x > prevX && y > prevY)
                { ops.Add(EditOp.Keep); x--; y--; }

                if (d > 0)
                {
                    if (x == prevX) ops.Add(EditOp.Insert);
                    else            ops.Add(EditOp.Delete);
                }

                x = prevX; y = prevY;
            }

            ops.Reverse();
            return ops;
        }

        private static List<string> SplitLines(string text)
        {
            if (string.IsNullOrEmpty(text)) return new List<string>();
            return new List<string>(text.Split('\n'));
        }
    }
}

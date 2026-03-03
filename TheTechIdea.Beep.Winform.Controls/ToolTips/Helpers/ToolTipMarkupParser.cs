using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers
{
    /// <summary>
    /// Sprint 2 — Lightweight markup parser for tooltip text.
    /// Supported syntax:
    ///   **bold**        — bold weight
    ///   *italic*        — italic
    ///   `code`          — monospace + tinted code background
    ///   [label](target) — clickable hyperlink
    /// </summary>
    public static class ToolTipMarkupParser
    {
        // ──────────────────────────────────────────────────────────────────────
        // Regex patterns
        // ──────────────────────────────────────────────────────────────────────
        private static readonly Regex BoldPattern    = new Regex(@"\*\*(.+?)\*\*",  RegexOptions.Compiled);
        private static readonly Regex ItalicPattern  = new Regex(@"\*(.+?)\*",      RegexOptions.Compiled);
        private static readonly Regex CodePattern    = new Regex(@"`(.+?)`",         RegexOptions.Compiled);
        private static readonly Regex LinkPattern    = new Regex(@"\[(.+?)\]\((.+?)\)", RegexOptions.Compiled);

        // ──────────────────────────────────────────────────────────────────────
        // Public API
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Parse a single markup string into an ordered list of <see cref="MarkupSpan"/> objects
        /// ready for rendering with individual font/color overrides.
        /// </summary>
        public static List<MarkupSpan> Parse(string markup)
        {
            if (string.IsNullOrEmpty(markup))
                return new List<MarkupSpan>();

            // Tokenise from left to right using the first match each iteration.
            var spans  = new List<MarkupSpan>();
            int cursor = 0;

            while (cursor < markup.Length)
            {
                // Find the earliest match of any pattern
                Match best    = null;
                SpanKind kind = SpanKind.Normal;

                TryBetter(BoldPattern.Match(markup, cursor),   SpanKind.Bold,   ref best, ref kind);
                TryBetter(ItalicPattern.Match(markup, cursor), SpanKind.Italic, ref best, ref kind);
                TryBetter(CodePattern.Match(markup, cursor),   SpanKind.Code,   ref best, ref kind);
                TryBetter(LinkPattern.Match(markup, cursor),   SpanKind.Link,   ref best, ref kind);

                if (best == null || !best.Success)
                {
                    // No more markup — emit the rest as plain text
                    spans.Add(new MarkupSpan(markup.Substring(cursor), SpanKind.Normal));
                    break;
                }

                // Emit any plain text before this match
                if (best.Index > cursor)
                    spans.Add(new MarkupSpan(markup.Substring(cursor, best.Index - cursor), SpanKind.Normal));

                // Emit the matched span
                string text   = best.Groups[1].Value;
                string target = kind == SpanKind.Link ? best.Groups[2].Value : null;
                spans.Add(new MarkupSpan(text, kind, target));

                cursor = best.Index + best.Length;
            }

            return spans;
        }

        private static void TryBetter(Match candidate, SpanKind candidateKind, ref Match best, ref SpanKind bestKind)
        {
            if (!candidate.Success) return;
            if (best == null || !best.Success || candidate.Index < best.Index)
            {
                best     = candidate;
                bestKind = candidateKind;
            }
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Supporting types
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>Type of a parsed markup span.</summary>
    public enum SpanKind
    {
        Normal,
        Bold,
        Italic,
        Code,
        Link
    }

    /// <summary>
    /// A single styled piece of text produced by <see cref="ToolTipMarkupParser"/>.
    /// </summary>
    public class MarkupSpan
    {
        public string   Text        { get; }
        public SpanKind Kind        { get; }
        /// <summary>Non-null for <see cref="SpanKind.Link"/> — URL or command string.</summary>
        public string   LinkTarget  { get; }

        public MarkupSpan(string text, SpanKind kind, string linkTarget = null)
        {
            Text       = text ?? string.Empty;
            Kind       = kind;
            LinkTarget = linkTarget;
        }

        /// <summary>Returns the <see cref="FontStyle"/> that should be applied for this span.</summary>
        public FontStyle GetFontStyle(FontStyle baseStyle = FontStyle.Regular)
        {
            return Kind switch
            {
                SpanKind.Bold   => baseStyle | FontStyle.Bold,
                SpanKind.Italic => baseStyle | FontStyle.Italic,
                SpanKind.Link   => baseStyle | FontStyle.Underline,
                _               => baseStyle
            };
        }

        /// <summary>Returns true if this span needs a tinted background (code blocks).</summary>
        public bool HasBackground => Kind == SpanKind.Code;

        public override string ToString() => $"[{Kind}] \"{Text}\"";
    }
}

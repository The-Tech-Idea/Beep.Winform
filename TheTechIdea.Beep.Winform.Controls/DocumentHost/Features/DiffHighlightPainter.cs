// DiffHighlightPainter.cs
// Renders a single DiffLine row inside DiffViewPanel.
// Added  lines  → green-tinted background  + "+" gutter indicator
// Removed lines → red-tinted  background  + "-" gutter indicator
// Unchanged     → transparent background
// Uses only System.Drawing — no extra dependencies.
// ─────────────────────────────────────────────────────────────────────────────
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost.Features
{
    /// <summary>
    /// Paints one <see cref="DiffLine"/> row into a given <see cref="Graphics"/> context.
    /// Callers drive the row layout; this class owns only the per-row drawing.
    /// </summary>
    public sealed class DiffHighlightPainter
    {
        // ── Configurable colours ──────────────────────────────────────────────

        public Color AddedBackground    { get; set; } = Color.FromArgb(40, 0, 180, 0);
        public Color RemovedBackground  { get; set; } = Color.FromArgb(40, 220, 0, 0);
        public Color AddedGutterColor   { get; set; } = Color.FromArgb(0, 180, 0);
        public Color RemovedGutterColor { get; set; } = Color.FromArgb(220, 0, 0);
        public Color GutterTextColor    { get; set; } = Color.White;
        public Color LineTextColor      { get; set; } = ColorUtils.MapSystemColor(SystemColors.WindowText);
        public Color LineNumberColor    { get; set; } = ColorUtils.MapSystemColor(SystemColors.GrayText);
        private bool _isDark;
        public bool IsDarkMode
        {
            get => _isDark;
            set { _isDark = value; InvalidateColours(); }
        }
        public Font  LineFont           { get; set; } = new Font("Cascadia Code", 9f, FontStyle.Regular);

        public int GutterWidth    { get; set; } = 28;
        public int LineNumWidth   { get; set; } = 44;
        public int RowHeight      { get; set; } = 20;
        public int TextLeftPad    { get; set; } = 4;

        // ── Painting ──────────────────────────────────────────────────────────

        /// <summary>
        /// Draws a single diff row at vertical offset <paramref name="y"/>.
        /// </summary>
        public void PaintRow(Graphics g, DiffLine line, int y, int totalWidth,
            bool isSelected = false)
        {
            var rowRect = new Rectangle(0, y, totalWidth, RowHeight);

            // Background
            Color back = line.Kind switch
            {
                DiffLineKind.Added   => AddedBackground,
                DiffLineKind.Removed => RemovedBackground,
                _                    => Color.Transparent
            };

            if (isSelected)
                back = _isDark ? Color.FromArgb(0, 120, 215) : ColorUtils.MapSystemColor(SystemColors.Highlight);

            if (back != Color.Transparent)
                using (var br = new SolidBrush(back))
                    g.FillRectangle(br, rowRect);

            // Gutter (+/-)
            var gutterRect = new Rectangle(0, y, GutterWidth, RowHeight);
            if (line.Kind != DiffLineKind.Unchanged)
            {
                var gutterColor = line.Kind == DiffLineKind.Added
                    ? AddedGutterColor : RemovedGutterColor;
                using var gb = new SolidBrush(gutterColor);
                g.FillRectangle(gb, gutterRect);

                string marker = line.Kind == DiffLineKind.Added ? "+" : "−";
                using var fb = new SolidBrush(GutterTextColor);
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                var gutterStr = new StringFormat { Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center };
                g.DrawString(marker, LineFont, fb, gutterRect, gutterStr);
            }

            // Line numbers
            var sf = new StringFormat { Alignment = StringAlignment.Far,
                LineAlignment = StringAlignment.Center, FormatFlags = StringFormatFlags.NoWrap };
            using var lnBrush = new SolidBrush(isSelected ? (_isDark ? Color.White : ColorUtils.MapSystemColor(SystemColors.HighlightText)) : LineNumberColor);

            var lineNumRect = new Rectangle(GutterWidth, y, LineNumWidth, RowHeight);
            if (line.LineA >= 0)
                g.DrawString((line.LineA + 1).ToString(), LineFont, lnBrush, lineNumRect, sf);

            // Text content
            var textRect = new Rectangle(
                GutterWidth + LineNumWidth + TextLeftPad,
                y,
                totalWidth - GutterWidth - LineNumWidth - TextLeftPad,
                RowHeight);

            using var textBrush = new SolidBrush(isSelected ? (_isDark ? Color.White : ColorUtils.MapSystemColor(SystemColors.HighlightText)) : LineTextColor);
            var textSf = new StringFormat { FormatFlags = StringFormatFlags.NoWrap,
                LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
            g.DrawString(line.Text ?? string.Empty, LineFont, textBrush, textRect, textSf);
        }

        /// <summary>Paints a divider between the two panes of a side-by-side view.</summary>
        public void PaintSplitDivider(Graphics g, int x, int height)
        {
            using var pen = new Pen(_isDark ? Color.FromArgb(70, 70, 75) : ColorUtils.MapSystemColor(SystemColors.ControlDark), 1f);
            g.DrawLine(pen, x, 0, x, height);
        }
        private void InvalidateColours() { }
    }
}

// BeepDocumentBreadcrumb.cs
// Clickable path bar shown above the document content area.
// Shows: Workspace > Group > DocumentTitle  with overflow collapse.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// Horizontal breadcrumb strip rendered above the content area.
    /// Segments are clickable (raises <see cref="SegmentClicked"/>).
    /// Supports keyboard left/right navigation with <see cref="KeyboardShortcutsEnabled"/>.
    /// </summary>
    internal sealed class BeepDocumentBreadcrumb : Control
    {
        // ── Constants ─────────────────────────────────────────────────────────

        private const int StripHeight  = 26;
        private const int Pad          = 8;
        private const int SepWidth     = 14;
        private const int EllipsisW    = 22;

        // ── State ─────────────────────────────────────────────────────────────

        private readonly List<BreadcrumbSegment> _segments = new();
        private int      _hoveredIndex = -1;
        private int      _focusedIndex = -1;
        private IBeepTheme? _theme;

        // ── Events ────────────────────────────────────────────────────────────

        /// <summary>Raised when the user clicks a breadcrumb segment.</summary>
        internal event EventHandler<BreadcrumbSegmentEventArgs>? SegmentClicked;

        // ── Properties ────────────────────────────────────────────────────────

        /// <summary>When true, left/right arrow keys cycle keyboard focus across segments.</summary>
        internal bool KeyboardShortcutsEnabled { get; set; } = true;

        // ── Constructor ───────────────────────────────────────────────────────

        internal BeepDocumentBreadcrumb()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.UserPaint            |
                ControlStyles.ResizeRedraw         |
                ControlStyles.Selectable,
                true);

            Height   = StripHeight;
            TabStop  = true;
        }

        // ── Theme ─────────────────────────────────────────────────────────────

        internal void ApplyTheme(IBeepTheme? theme)
        {
            _theme    = theme;
            BackColor = BreadcrumbThemeHelpers.ThemeAwareColor(_theme?.PanelBackColor, SystemColors.Control);
            ForeColor = BreadcrumbThemeHelpers.ThemeAwareColor(_theme?.ForeColor, SystemColors.ControlText);
            Invalidate();
        }

        // ── Segments ──────────────────────────────────────────────────────────

        internal void SetSegments(IEnumerable<string> labels, IEnumerable<string?> tags)
        {
            _segments.Clear();
            var lArr = new List<string>(labels);
            var tArr = new List<string?>(tags);
            for (int i = 0; i < lArr.Count; i++)
                _segments.Add(new BreadcrumbSegment(lArr[i], i < tArr.Count ? tArr[i] : null));
            _focusedIndex = _segments.Count - 1;
            Invalidate();
        }

        internal void SetActiveDocument(string? workspaceName, string? groupName, string? documentTitle)
        {
            var labels = new List<string>();
            var tags   = new List<string?>();

            if (!string.IsNullOrEmpty(workspaceName))
            { labels.Add(workspaceName!); tags.Add("workspace:" + workspaceName); }

            if (!string.IsNullOrEmpty(groupName))
            { labels.Add(groupName!); tags.Add("group:" + groupName); }

            if (!string.IsNullOrEmpty(documentTitle))
            { labels.Add(documentTitle!); tags.Add("document:" + documentTitle); }

            SetSegments(labels, tags);
        }

        // ── Painting ──────────────────────────────────────────────────────────

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            Color back   = BreadcrumbThemeHelpers.ThemeAwareColor(_theme?.PanelBackColor, SystemColors.Control);
            Color fore   = BreadcrumbThemeHelpers.ThemeAwareColor(_theme?.ForeColor, SystemColors.ControlText);
            Color hover  = BreadcrumbThemeHelpers.ThemeAwareColor(_theme?.PrimaryColor, SystemColors.Highlight);
            Color dim    = BreadcrumbThemeHelpers.ThemeAwareGrayText(_theme?.PanelBackColor);
            Color accent = BreadcrumbThemeHelpers.ThemeAwareColor(_theme?.PrimaryColor, SystemColors.Highlight);

            using var backBr = new SolidBrush(back);
            g.FillRectangle(backBr, ClientRectangle);

            // Bottom border
            using var borderPen = new Pen(_theme?.BorderColor ?? Color.FromArgb(30, fore));
            g.DrawLine(borderPen, 0, Height - 1, Width, Height - 1);

            if (_segments.Count == 0) return;

            var font      = BeepFontManager.GetCachedFont("Segoe UI", 9.5f, FontStyle.Regular);
            var boldFont  = BeepFontManager.GetCachedFont("Segoe UI", 9.5f, FontStyle.Bold);
            int x         = Pad;
            bool overflow = false;

            // Measure all segments to detect overflow
            var widths = new float[_segments.Count];
            float total = Pad;
            for (int i = 0; i < _segments.Count; i++)
            {
                widths[i] = g.MeasureString(_segments[i].Label, font).Width + Pad * 2;
                total += widths[i] + SepWidth;
            }
            overflow = total > Width;

            // When overflowing: hide leading segments and show ellipsis
            int startIdx = 0;
            if (overflow)
            {
                float avail = Width - EllipsisW - Pad;
                float used  = 0;
                startIdx = _segments.Count - 1;
                while (startIdx > 0 && used + widths[startIdx] + SepWidth < avail)
                {
                    used += widths[startIdx] + SepWidth;
                    startIdx--;
                }
                startIdx = Math.Min(startIdx + 1, _segments.Count - 1);

                // Draw ellipsis button
                var ellipRect = new Rectangle(x, (Height - 18) / 2, EllipsisW, 18);
                using var ellipBr = new SolidBrush(dim);
                g.DrawString("…", font, ellipBr, ellipRect);
                x += EllipsisW + 4;
            }

            for (int i = startIdx; i < _segments.Count; i++)
            {
                var seg  = _segments[i];
                bool last = i == _segments.Count - 1;
                bool hov  = i == _hoveredIndex;
                bool foc  = i == _focusedIndex;

                float sw   = widths[i];
                var segRect = new RectangleF(x, 2, sw, Height - 4);
                seg.Bounds  = new Rectangle((int)segRect.X, (int)segRect.Y,
                                            (int)segRect.Width, (int)segRect.Height);

                if (hov || foc)
                {
                    using var hlBr = new SolidBrush(Color.FromArgb(25, accent));
                    g.FillRectangle(hlBr, segRect);
                }

                var textBr = new SolidBrush(last ? fore : dim);
                var f      = last ? boldFont : font;
                g.DrawString(seg.Label, f, textBr,
                    new RectangleF(x + Pad / 2f, (Height - f.Height) / 2f, sw, Height));
                textBr.Dispose();

                x += (int)sw;

                if (!last)
                {
                    using var sepBr = new SolidBrush(dim);
                    g.DrawString("›", font, sepBr,
                        new PointF(x, (Height - font.Height) / 2f));
                    x += SepWidth;
                }
            }
        }

        // ── Mouse ─────────────────────────────────────────────────────────────

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            int idx = HitTest(e.Location);
            if (idx != _hoveredIndex)
            {
                _hoveredIndex = idx;
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hoveredIndex = -1;
            Invalidate();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            int idx = HitTest(e.Location);
            if (idx >= 0)
            {
                _focusedIndex = idx;
                SegmentClicked?.Invoke(this, new BreadcrumbSegmentEventArgs(_segments[idx]));
                Invalidate();
            }
        }

        private int HitTest(Point p)
        {
            for (int i = 0; i < _segments.Count; i++)
                if (_segments[i].Bounds.Contains(p)) return i;
            return -1;
        }

        // ── Keyboard ──────────────────────────────────────────────────────────

        protected override bool IsInputKey(Keys keyData)
            => keyData is Keys.Left or Keys.Right || base.IsInputKey(keyData);

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!KeyboardShortcutsEnabled || _segments.Count == 0) return;

            if (e.KeyCode == Keys.Left && _focusedIndex > 0)
            {
                _focusedIndex--;
                Invalidate();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Right && _focusedIndex < _segments.Count - 1)
            {
                _focusedIndex++;
                Invalidate();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Enter && _focusedIndex >= 0)
            {
                SegmentClicked?.Invoke(this, new BreadcrumbSegmentEventArgs(_segments[_focusedIndex]));
                e.Handled = true;
            }
        }

        protected override void OnGotFocus(EventArgs e)  { base.OnGotFocus(e);  Invalidate(); }
        protected override void OnLostFocus(EventArgs e) { base.OnLostFocus(e); Invalidate(); }
    }

    // ── Supporting types ──────────────────────────────────────────────────────

    internal sealed class BreadcrumbSegment
    {
        internal string  Label  { get; }
        internal string? Tag    { get; }
        internal Rectangle Bounds { get; set; }

        internal BreadcrumbSegment(string label, string? tag)
        {
            Label = label;
            Tag   = tag;
        }
    }

    internal sealed class BreadcrumbSegmentEventArgs : EventArgs
    {
        internal BreadcrumbSegment Segment { get; }
        internal BreadcrumbSegmentEventArgs(BreadcrumbSegment segment) => Segment = segment;
    }

    // ── Theme-aware color fallbacks ──────────────────────────────────────

    internal static class BreadcrumbThemeHelpers
    {
        internal static Color ThemeAwareColor(Color? themeColor, Color lightColor)
        {
            if (themeColor.HasValue && themeColor.Value != Color.Empty)
                return themeColor.Value;
            return Sc(lightColor);
        }

        internal static Color ThemeAwareGrayText(Color? refColor)
        {
            if (refColor.HasValue && IsDarkBackground(refColor.Value))
                return Color.FromArgb(150, 150, 155);
            return SystemColors.GrayText;
        }

        private static bool IsDarkBackground(Color c) => c.GetBrightness() < 0.5;

        private static Color Sc(Color lightColor)
        {
            return ColorUtils.MapSystemColor(lightColor);
        }
    }
}

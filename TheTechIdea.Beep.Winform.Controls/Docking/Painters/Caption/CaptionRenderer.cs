using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Docking.Helpers;
using TheTechIdea.Beep.Winform.Controls.Docking.Layoutmanagers;
using TheTechIdea.Beep.Winform.Controls.FontManagement;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Painters.Caption
{
    /// <summary>
    /// The single renderer for caption/header strips, shared by <c>DockPanel</c> and
    /// <c>BeepDockspace</c>. Paints the strip background, tabs (icon + title + dirty marker +
    /// border), trailing caption buttons, and the bottom separator using the resolved
    /// <see cref="DockingPainterContext"/> palette, <see cref="BeepFontManager"/> fonts, and
    /// <see cref="DockingCaptionPainter"/> (SVG via <c>StyledImagePainter</c>) for icons.
    /// Geometry and interaction state come from <see cref="CaptionLayoutManager"/> — this class
    /// only paints.
    /// </summary>
    internal sealed class CaptionRenderer : System.IDisposable
    {
        private static Font SafeFont => BeepFontManager.DefaultFont ?? SystemFonts.DefaultFont;
        private readonly PaintResourceCache _cache = new PaintResourceCache();

        public void Dispose() => _cache.Dispose();

        public void Paint(Graphics g, DockingPainterContext ctx, CaptionLayoutManager layout, IReadOnlyList<CaptionButtonKind> buttons)
        {
            if (g == null || ctx == null || layout == null)
                return;

            var colors = ctx.Colors;
            var flavor = ctx.Flavor;
            Rectangle strip = ctx.Bounds;

            PaintHeaderBackground(g, strip, colors.HeaderBackColor, flavor);

            PaintTabs(g, ctx, layout);
            PaintButtons(g, colors.HeaderButtonForeColor, layout, buttons);

            if (layout.HasOverflow)
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle chevron = layout.OverflowButtonRect;
                string icon = DockingCaptionPainter.CaptionIcons.DropDown;
                if (!string.IsNullOrEmpty(icon))
                    DockingCaptionPainter.PaintIcon(g, chevron, icon, colors.HeaderButtonForeColor);
                DockingCaptionPainter.PaintDropDownFallback(g, chevron, colors.HeaderButtonForeColor);
            }

            if (flavor.TabBorderStyle == DockingTabBorderStyle.Rectangle)
            {
                using (var pen = _cache.GetPen(colors.TabBorderColor))
                {
                    switch (ctx.TabStyle)
                    {
                        case Models.TabStyle.VsCode:
                            // Thin accent line below the active tab only — no full separator.
                            break;
                        case Models.TabStyle.VsIde2022:
                            // Thick 2px separator with gradient edge.
                            pen.Width = 2;
                            g.DrawLine(pen, strip.Left, strip.Bottom - 2, strip.Right - 1, strip.Bottom - 2);
                            break;
                        case Models.TabStyle.Browser:
                            // Top-border style separator.
                            g.DrawLine(pen, strip.Left, 0, strip.Right - 1, 0);
                            break;
                        default:
                            g.DrawLine(pen, strip.Left, strip.Bottom - 1, strip.Right - 1, strip.Bottom - 1);
                            break;
                    }
                }
            }
        }

        private void PaintHeaderBackground(Graphics g, Rectangle strip, Color back, DockingStyleFlavor flavor)
        {
            if (flavor.HeaderCornerRadius <= 0 && flavor.HeaderElevationBlend <= 0f)
            {
                using var brush = _cache.GetBrush(back);
                g.FillRectangle(brush, strip);
                return;
            }

            // Rounded header (Fluent2 / Material3 / MacOS) with a subtle elevation tint.
            Color tinted = flavor.HeaderElevationBlend > 0f
                ? BlendTowards(back, ControlPaint.Light(back, 0.05f), flavor.HeaderElevationBlend)
                : back;
            using (var brush = _cache.GetBrush(tinted))
            {
                if (flavor.HeaderCornerRadius > 0)
                {
                    using var path = RoundedRectPath(strip, flavor.HeaderCornerRadius);
                    g.FillPath(brush, path);
                }
                else
                {
                    g.FillRectangle(brush, strip);
                }
            }
        }

        private static Color BlendTowards(Color a, Color b, float t)
        {
            t = System.Math.Max(0f, System.Math.Min(1f, t));
            return Color.FromArgb(
                a.A,
                (int)(a.R + (b.R - a.R) * t),
                (int)(a.G + (b.G - a.G) * t),
                (int)(a.B + (b.B - a.B) * t));
        }

        internal static GraphicsPath RoundedRectPath(Rectangle r, int radius)
        {
            var path = new GraphicsPath();
            int d = radius * 2;
            if (d > r.Width) d = r.Width;
            if (d > r.Height) d = r.Height;
            if (d <= 0) { path.AddRectangle(r); return path; }
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void PaintTabs(Graphics g, DockingPainterContext ctx, CaptionLayoutManager layout)
        {
            if (layout.IsVertical)
            {
                PaintVerticalTabs(g, ctx, layout);
                return;
            }

            PaintHorizontalTabs(g, ctx, layout);
        }

        private void PaintHorizontalTabs(Graphics g, DockingPainterContext ctx, CaptionLayoutManager layout)
        {
            switch (ctx.TabStyle)
            {
                case Models.TabStyle.VsCode:     PaintVsCodeTabs(g, ctx, layout);    return;
                case Models.TabStyle.VsIde2022:  PaintVsIde2022Tabs(g, ctx, layout); return;
                case Models.TabStyle.JetBrains:  PaintJetBrainsTabs(g, ctx, layout); return;
                case Models.TabStyle.Browser:    PaintBrowserTabs(g, ctx, layout);   return;
                default:                         PaintDefaultTabs(g, ctx, layout);   return;
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  Default — flat square tabs with separator line
        // ════════════════════════════════════════════════════════════════════
        private void PaintDefaultTabs(Graphics g, DockingPainterContext ctx, CaptionLayoutManager layout)
        {
            var colors = ctx.Colors;
            Font font = SafeFont;
            using var sf = TabStringFormat();
            foreach (var kv in layout.TabRects)
            {
                var tab = kv.Key;
                var r = kv.Value;
                bool active = tab.IsActive;
                Color back = active ? colors.ActiveTabBackColor : colors.InactiveTabBackColor;
                Color fore = active ? colors.ActiveTabForeColor : colors.InactiveTabForeColor;

                // Hover — lighten the background
                if (!active && tab == layout.HoveredTab)
                    back = ControlPaint.Light(back, 0.10f);

                using (var brush = _cache.GetBrush(back))
                    g.FillRectangle(brush, r);

                // Border
                using (var pen = _cache.GetPen(colors.TabBorderColor))
                    g.DrawRectangle(pen, r.X, r.Y, r.Width - 1, r.Height - 1);

                // Active accent bar (bottom)
                if (active)
                {
                    var accent = new Rectangle(r.Left + 1, r.Bottom - 2, r.Width - 2, 2);
                    using var ab = _cache.GetBrush(colors.AccentColor);
                    g.FillRectangle(ab, accent);
                }

                DrawTabContent(g, r, tab, fore, font, sf, colors);
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  VsCode — pill tabs, no borders, hover fill, bottom accent
        // ════════════════════════════════════════════════════════════════════
        private void PaintVsCodeTabs(Graphics g, DockingPainterContext ctx, CaptionLayoutManager layout)
        {
            var colors = ctx.Colors;
            Font font = SafeFont;
            using var sf = TabStringFormat();
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            foreach (var kv in layout.TabRects)
            {
                var tab = kv.Key;
                var r = kv.Value;
                bool active = tab.IsActive;
                bool hover = tab == layout.HoveredTab;
                Color back = active ? colors.ActiveTabBackColor : colors.InactiveTabBackColor;
                Color fore = active ? colors.ActiveTabForeColor : colors.InactiveTabForeColor;

                // Inactive + hover: subtle fill
                if (hover && !active)
                    back = ControlPaint.Light(back, 0.08f);

                // Pill shape
                var pillRect = new Rectangle(r.X + 2, r.Y + 3, r.Width - 4, r.Height - 6);
                if (pillRect.Width < 4 || pillRect.Height < 4) pillRect = r;

                using (var path = RoundedRectPath(pillRect, pillRect.Height / 2))
                using (var brush = _cache.GetBrush(back))
                    g.FillPath(brush, path);

                // Active: bottom accent bar
                if (active)
                {
                    var accentRect = new Rectangle(pillRect.Left + 4, pillRect.Bottom - 2, pillRect.Width - 8, 2);
                    using (var path = RoundedRectPath(accentRect, 1))
                    using (var ab = _cache.GetBrush(colors.AccentColor))
                        g.FillPath(ab, path);
                }

                DrawTabContent(g, pillRect, tab, fore, font, sf, colors);
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  VsIde2022 — sharp tabs, gradient header, thick accent, hover glow
        // ════════════════════════════════════════════════════════════════════
        private void PaintVsIde2022Tabs(Graphics g, DockingPainterContext ctx, CaptionLayoutManager layout)
        {
            var colors = ctx.Colors;
            Font font = SafeFont;
            using var sf = TabStringFormat();

            foreach (var kv in layout.TabRects)
            {
                var tab = kv.Key;
                var r = kv.Value;
                bool active = tab.IsActive;
                bool hover = tab == layout.HoveredTab;
                Color back = active ? colors.ActiveTabBackColor : colors.InactiveTabBackColor;
                Color fore = active ? colors.ActiveTabForeColor : colors.InactiveTabForeColor;

                // Active tab bleeds into the header (top edge fully colored)
                if (active) r = new Rectangle(r.X, 0, r.Width, r.Height + r.Y);

                // Hover: glow effect via lighter fill
                if (hover && !active)
                    back = ControlPaint.Light(back, 0.12f);

                // Gradient background for active tab
                if (active)
                {
                    var dark = ControlPaint.Dark(back, 0.08f);
                    using var gradient = new System.Drawing.Drawing2D.LinearGradientBrush(
                        r, back, dark, System.Drawing.Drawing2D.LinearGradientMode.Vertical);
                    g.FillRectangle(gradient, r);
                }
                else
                {
                    using var brush = _cache.GetBrush(back);
                    g.FillRectangle(brush, r);
                }

                // Border (no left/right border for adjacent tabs)
                using (var pen = _cache.GetPen(colors.TabBorderColor))
                {
                    g.DrawLine(pen, r.Left, r.Bottom - 1, r.Right - 1, r.Bottom - 1);
                    if (active)
                        g.DrawLine(pen, r.Left, r.Top, r.Right - 1, r.Top);
                }

                // Thick accent bar on active tab
                if (active)
                {
                    var accent = new Rectangle(r.Left, r.Bottom - 3, r.Width, 3);
                    using var ab = _cache.GetBrush(colors.AccentColor);
                    g.FillRectangle(ab, accent);
                }

                // Hover underline for inactive
                if (hover && !active)
                {
                    var ul = new Rectangle(r.Left + 4, r.Bottom - 1, r.Width - 8, 1);
                    using var hb = _cache.GetBrush(colors.AccentColor);
                    g.FillRectangle(hb, ul);
                }

                DrawTabContent(g, r, tab, fore, font, sf, colors);
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  JetBrains — clean rectangular, top accent, subtle hover
        // ════════════════════════════════════════════════════════════════════
        private void PaintJetBrainsTabs(Graphics g, DockingPainterContext ctx, CaptionLayoutManager layout)
        {
            var colors = ctx.Colors;
            Font font = SafeFont;
            using var sf = TabStringFormat();

            foreach (var kv in layout.TabRects)
            {
                var tab = kv.Key;
                var r = kv.Value;
                bool active = tab.IsActive;
                bool hover = tab == layout.HoveredTab;
                Color back = active ? colors.ActiveTabBackColor : colors.InactiveTabBackColor;
                Color fore = active ? colors.ActiveTabForeColor : colors.InactiveTabForeColor;

                // Hover: very subtle fill
                if (hover && !active)
                    back = ControlPaint.Light(back, 0.06f);

                // Background with subtle rounding
                int rnd = 2;
                using (var path = RoundedRectPath(r, rnd))
                using (var brush = _cache.GetBrush(back))
                    g.FillPath(brush, path);

                // Active: top accent bar
                if (active)
                {
                    var accent = new Rectangle(r.Left, r.Top, r.Width, 2);
                    using (var path = RoundedRectPath(accent, 1))
                    using (var ab = _cache.GetBrush(colors.AccentColor))
                        g.FillPath(ab, path);
                }
                // Hover: top dot indicator
                else if (hover)
                {
                    var dot = new Rectangle(r.Left + r.Width / 2 - 3, r.Top, 6, 2);
                    using var hb = _cache.GetBrush(colors.AccentColor);
                    g.FillRectangle(hb, dot);
                }

                DrawTabContent(g, r, tab, fore, font, sf, colors);
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  Browser — trapezoid tabs (rounded top, flat bottom), dark separator
        // ════════════════════════════════════════════════════════════════════
        private void PaintBrowserTabs(Graphics g, DockingPainterContext ctx, CaptionLayoutManager layout)
        {
            var colors = ctx.Colors;
            Font font = SafeFont;
            using var sf = TabStringFormat();
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            foreach (var kv in layout.TabRects)
            {
                var tab = kv.Key;
                var r = kv.Value;
                bool active = tab.IsActive;
                bool hover = tab == layout.HoveredTab;
                Color back = active ? colors.ActiveTabBackColor : colors.InactiveTabBackColor;
                Color fore = active ? colors.ActiveTabForeColor : colors.InactiveTabForeColor;

                // Hover: warm fill
                if (hover && !active)
                    back = ControlPaint.Light(back, 0.15f);

                // Trapezoid: top-rounded, flat bottom
                int rnd = 6;
                using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    path.AddArc(r.X, r.Y, rnd * 2, rnd * 2, 180, 90);
                    path.AddArc(r.Right - rnd * 2, r.Y, rnd * 2, rnd * 2, 270, 90);
                    path.AddLine(r.Right - 1, r.Bottom - 1, r.X + 1, r.Bottom - 1);
                    path.CloseFigure();
                    using var brush = _cache.GetBrush(back);
                    g.FillPath(brush, path);
                }

                // Bottom separator line (thick, dark)
                using (var pen = _cache.GetPen(colors.TabBorderColor, 1.5f))
                    g.DrawLine(pen, 0, r.Bottom - 1, r.Right, r.Bottom - 1);

                // Active tab: accent tint at top
                if (active)
                {
                    var tintRect = new Rectangle(r.X + 2, r.Y, r.Width - 4, 3);
                    using var tb = _cache.GetBrush(colors.AccentColor);
                    g.FillRectangle(tb, tintRect);
                }

                DrawTabContent(g, r, tab, fore, font, sf, colors);
            }
        }

        private static StringFormat TabStringFormat() => new StringFormat
        {
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.EllipsisCharacter,
            FormatFlags = StringFormatFlags.NoWrap
        };

        private void DrawTabContent(Graphics g, Rectangle r, CaptionTabModel tab, Color fore, Font font, StringFormat sf, DockingThemeColors colors)
        {
            bool showIcon = DockingCaptionPainter.HasTabIcon(tab.IconPath);
            if (showIcon)
                DockingCaptionPainter.PaintTabIcon(g, r, tab.IconPath, fore);

            int textLeft = r.Left + DockingCaptionPainter.GetTabContentLeft(showIcon);
            var textRect = new Rectangle(
                textLeft, r.Top,
                Math.Max(0, r.Right - textLeft - DockingCaptionPainter.TabTextPadding),
                r.Height);
            using (var brush = _cache.GetBrush(fore))
                g.DrawString(tab.Title ?? "Panel", font, brush, textRect, sf);

            if (tab.IsDirty)
            {
                var dot = new Rectangle(r.Right - r.Height / 2 - 3, r.Top + r.Height / 2 - 2, 5, 5);
                using var dirtyBrush = _cache.GetBrush(colors.AccentColor);
                g.FillEllipse(dirtyBrush, dot);
            }
        }

        private void PaintVerticalTabs(Graphics g, DockingPainterContext ctx, CaptionLayoutManager layout)
        {
            var colors = ctx.Colors;
            var flavor = ctx.Flavor;
            Font font = SafeFont;

            using var sf = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter,
                FormatFlags = StringFormatFlags.NoWrap
            };

            float angle = layout.IsFlipped ? 90f : -90f;

            foreach (var kv in layout.TabRects)
            {
                CaptionTabModel tab = kv.Key;
                Rectangle tabRect = kv.Value;

                Color tabBack = tab.IsActive ? colors.ActiveTabBackColor : colors.InactiveTabBackColor;
                Color tabFore = tab.IsActive ? colors.ActiveTabForeColor : colors.InactiveTabForeColor;

                PaintTabBackground(g, tabRect, tabBack, flavor);

                if (flavor.TabBorderStyle == DockingTabBorderStyle.Rectangle)
                {
                    using var pen = _cache.GetPen(colors.TabBorderColor);
                    g.DrawRectangle(pen, tabRect.X, tabRect.Y, Math.Max(0, tabRect.Width - 1), Math.Max(0, tabRect.Height - 1));
                }

                if (tab.IsActive && flavor.ActiveTabAccentWidth > 0)
                {
                    int accentW = flavor.ActiveTabAccentWidth;
                    var accentRect = new Rectangle(
                        tabRect.Right - accentW, tabRect.Top + 2,
                        accentW, Math.Max(0, tabRect.Height - 4));
                    using var accentBrush = _cache.GetBrush(colors.AccentColor);
                    g.FillRectangle(accentBrush, accentRect);
                }

                // Paint title text rotated 90° (or -90° for Left).
                var state = g.Save();
                float cx = tabRect.Left + tabRect.Width / 2f;
                float cy = tabRect.Top + tabRect.Height / 2f;
                g.TranslateTransform(cx, cy);
                g.RotateTransform(angle);

                // After rotation, the "width" is the height of the original rect, and the
                // "height" is the width (since we rotated 90°). Draw from centre.
                int textW = tabRect.Height - 2;
                int textH = tabRect.Width - 2;
                var rotatedRect = new RectangleF(-textW / 2f, -textH / 2f, textW, textH);

                using var brush = _cache.GetBrush(tabFore);
                g.DrawString(tab.Title ?? "Panel", font, brush, rotatedRect, sf);
                g.Restore(state);
            }
        }

        private void PaintTabBackground(Graphics g, Rectangle tabRect, Color tabBack, DockingStyleFlavor flavor)
        {
            using var brush = _cache.GetBrush(tabBack);
            if (flavor.TabCornerRadius > 0)
            {
                using var path = RoundedRectPath(tabRect, flavor.TabCornerRadius);
                g.FillPath(brush, path);
            }
            else
            {
                g.FillRectangle(brush, tabRect);
            }
        }

        private void PaintButtons(Graphics g, Color tint, CaptionLayoutManager layout, IReadOnlyList<CaptionButtonKind> buttons)
        {
            if (buttons == null)
                return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            foreach (var kind in buttons)
            {
                Rectangle r = layout.GetButtonRect(kind);
                if (r.IsEmpty)
                    continue;

                string icon = IconFor(kind);
                if (!string.IsNullOrEmpty(icon))
                    DockingCaptionPainter.PaintIcon(g, r, icon, tint);

                switch (kind)
                {
                    case CaptionButtonKind.Close:
                        DockingCaptionPainter.PaintCloseFallback(g, r, tint);
                        break;
                    case CaptionButtonKind.Float:
                        DockingCaptionPainter.PaintFloatFallback(g, r, tint);
                        break;
                    case CaptionButtonKind.AutoHide:
                    case CaptionButtonKind.Pin:
                        DockingCaptionPainter.PaintPinFallback(g, r, tint);
                        break;
                    case CaptionButtonKind.DropDown:
                        DockingCaptionPainter.PaintDropDownFallback(g, r, tint);
                        break;
                }
            }
        }

        private static string IconFor(CaptionButtonKind kind) => kind switch
        {
            CaptionButtonKind.Close => DockingCaptionPainter.CaptionIcons.Close,
            CaptionButtonKind.Float => DockingCaptionPainter.CaptionIcons.Float,
            CaptionButtonKind.AutoHide => DockingCaptionPainter.CaptionIcons.Pin,
            CaptionButtonKind.Pin => DockingCaptionPainter.CaptionIcons.Pin,
            CaptionButtonKind.DropDown => DockingCaptionPainter.CaptionIcons.DropDown,
            _ => string.Empty
        };
    }
}

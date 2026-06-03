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
                    g.DrawLine(pen, strip.Left, strip.Bottom - 1, strip.Right - 1, strip.Bottom - 1);
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
            var colors = ctx.Colors;
            var flavor = ctx.Flavor;
            Font font = SafeFont;

            using var sf = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter,
                FormatFlags = StringFormatFlags.NoWrap
            };

            foreach (var kv in layout.TabRects)
            {
                CaptionTabModel tab = kv.Key;
                Rectangle tabRect = kv.Value;

                Color tabBack = tab.IsActive ? colors.ActiveTabBackColor : colors.InactiveTabBackColor;
                Color tabFore = tab.IsActive ? colors.ActiveTabForeColor : colors.InactiveTabForeColor;
                bool showIcon = DockingCaptionPainter.HasTabIcon(tab.IconPath);

                PaintTabBackground(g, tabRect, tabBack, flavor);

                if (flavor.TabBorderStyle == DockingTabBorderStyle.Rectangle)
                {
                    using var pen = _cache.GetPen(colors.TabBorderColor);
                    if (flavor.TabCornerRadius > 0)
                    {
                        using var path = RoundedRectPath(tabRect, flavor.TabCornerRadius);
                        g.DrawPath(pen, path);
                    }
                    else
                    {
                        g.DrawRectangle(pen, tabRect.X, tabRect.Y, System.Math.Max(0, tabRect.Width - 1), System.Math.Max(0, tabRect.Height - 1));
                    }
                }

                if (tab.IsActive)
                {
                    if (flavor.UseTonalElevation)
                    {
                        var tint = BlendTowards(tabBack, ControlPaint.Light(tabBack, 0.18f), 0.35f);
                        using var elevationBrush = _cache.GetBrush(Color.FromArgb(40, tint));
                        if (flavor.TabCornerRadius > 0)
                        {
                            using var p = RoundedRectPath(tabRect, flavor.TabCornerRadius);
                            g.FillPath(elevationBrush, p);
                        }
                        else
                        {
                            g.FillRectangle(elevationBrush, tabRect);
                        }
                    }
                    if (flavor.ActiveTabAccentWidth > 0)
                    {
                        int accentH = flavor.ActiveTabAccentWidth;
                        var accentRect = new Rectangle(
                            tabRect.Left + 2,
                            tabRect.Bottom - accentH,
                            System.Math.Max(0, tabRect.Width - 4),
                            accentH);
                        using var accentBrush = _cache.GetBrush(colors.AccentColor);
                        g.FillRectangle(accentBrush, accentRect);
                    }
                }

                if (showIcon)
                    DockingCaptionPainter.PaintTabIcon(g, tabRect, tab.IconPath, tabFore);

                int textLeft = tabRect.Left + DockingCaptionPainter.GetTabContentLeft(showIcon);
                var textRect = new Rectangle(
                    textLeft,
                    tabRect.Top,
                    System.Math.Max(0, tabRect.Right - textLeft - DockingCaptionPainter.TabTextPadding),
                    tabRect.Height);

                using (var brush = _cache.GetBrush(tabFore))
                    g.DrawString(tab.Title ?? "Panel", font, brush, textRect, sf);

                if (tab.IsDirty)
                {
                    var dot = new Rectangle(tabRect.Right - 8, tabRect.Top + 6, 5, 5);
                    using var dirtyBrush = _cache.GetBrush(colors.ActiveTabBackColor);
                    g.FillEllipse(dirtyBrush, dot);
                }

                if (ctx.IsDesignTime && tab.IsActive)
                {
                    using var selectPen = new Pen(colors.ActiveTabBackColor)
                    {
                        DashStyle = DashStyle.Dot,
                        Width = 1
                    };
                    g.DrawRectangle(selectPen,
                        tabRect.X + 1, tabRect.Y + 1,
                        System.Math.Max(0, tabRect.Width - 3),
                        System.Math.Max(0, tabRect.Height - 3));
                }
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

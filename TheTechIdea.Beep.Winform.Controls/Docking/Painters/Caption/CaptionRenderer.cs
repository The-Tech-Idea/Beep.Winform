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
    internal sealed class CaptionRenderer
    {
        /// <summary>
        /// Paints the caption strip described by <paramref name="layout"/>.
        /// </summary>
        /// <param name="g">Target graphics.</param>
        /// <param name="ctx">Render context. <c>ctx.Bounds</c> is the full strip rectangle.</param>
        /// <param name="layout">Pre-computed tab/button geometry for this strip.</param>
        /// <param name="buttons">Buttons to paint (right-to-left), matching the layout's compute call.</param>
        public void Paint(Graphics g, DockingPainterContext ctx, CaptionLayoutManager layout, IReadOnlyList<CaptionButtonKind> buttons)
        {
            if (g == null || ctx == null || layout == null)
                return;

            var colors = ctx.Colors;
            Rectangle strip = ctx.Bounds;

            using (var brush = new SolidBrush(colors.HeaderBackColor))
                g.FillRectangle(brush, strip);

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

            using (var pen = new Pen(colors.TabBorderColor))
                g.DrawLine(pen, strip.Left, strip.Bottom - 1, strip.Right - 1, strip.Bottom - 1);
        }

        private static void PaintTabs(Graphics g, DockingPainterContext ctx, CaptionLayoutManager layout)
        {
            var colors = ctx.Colors;
            Font font = BeepFontManager.DefaultFont;

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

                using (var brush = new SolidBrush(tabBack))
                    g.FillRectangle(brush, tabRect);

                using (var pen = new Pen(colors.TabBorderColor))
                    g.DrawRectangle(pen, tabRect.X, tabRect.Y, System.Math.Max(0, tabRect.Width - 1), System.Math.Max(0, tabRect.Height - 1));

                if (showIcon)
                    DockingCaptionPainter.PaintTabIcon(g, tabRect, tab.IconPath, tabFore);

                int textLeft = tabRect.Left + DockingCaptionPainter.GetTabContentLeft(showIcon);
                var textRect = new Rectangle(
                    textLeft,
                    tabRect.Top,
                    System.Math.Max(0, tabRect.Right - textLeft - DockingCaptionPainter.TabTextPadding),
                    tabRect.Height);

                using (var brush = new SolidBrush(tabFore))
                    g.DrawString(tab.Title ?? "Panel", font, brush, textRect, sf);

                if (tab.IsDirty)
                {
                    var dot = new Rectangle(tabRect.Right - 8, tabRect.Top + 6, 5, 5);
                    using var dirtyBrush = new SolidBrush(colors.ActiveTabBackColor);
                    g.FillEllipse(dirtyBrush, dot);
                }
            }
        }

        private static void PaintButtons(Graphics g, Color tint, CaptionLayoutManager layout, IReadOnlyList<CaptionButtonKind> buttons)
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

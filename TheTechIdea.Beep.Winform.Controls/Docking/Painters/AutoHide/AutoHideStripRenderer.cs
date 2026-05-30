using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Docking.Helpers;
using TheTechIdea.Beep.Winform.Controls.Docking.Layoutmanagers;
using TheTechIdea.Beep.Winform.Controls.FontManagement;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Painters.AutoHide
{
    /// <summary>
    /// The single renderer for auto-hide edge strips. Paints the strip background and each tab
    /// (background + border + icon + title, rotated on vertical edges) from the resolved
    /// <see cref="DockingPainterContext"/> palette, <see cref="BeepFontManager"/> fonts, and
    /// <see cref="DockingCaptionPainter"/> (SVG via <c>StyledImagePainter</c>) for icons. Geometry
    /// and hit-testing live in <see cref="AutoHideStripLayoutManager"/> — this class only paints.
    /// </summary>
    internal sealed class AutoHideStripRenderer
    {
        /// <summary>
        /// Paints the auto-hide strip described by <paramref name="layout"/>.
        /// </summary>
        /// <param name="g">Target graphics.</param>
        /// <param name="ctx">Render context. <c>ctx.Bounds</c> is the full strip rectangle.</param>
        /// <param name="layout">Pre-computed tab geometry for this strip.</param>
        public void Paint(Graphics g, DockingPainterContext ctx, AutoHideStripLayoutManager layout)
        {
            if (g == null || ctx == null || layout == null)
                return;

            var colors = ctx.Colors;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var bg = new SolidBrush(colors.AutoHideStripBackColor))
                g.FillRectangle(bg, ctx.Bounds);

            Font font = BeepFontManager.StatusBarFont;

            foreach (var kv in layout.TabRects)
            {
                AutoHideTabModel tab = kv.Key;
                Rectangle rect = kv.Value;

                Color tabBack = tab.IsActive
                    ? colors.AutoHideActiveTabBackColor
                    : colors.AutoHideTabBackColor;
                Color tabFore = tab.IsActive
                    ? colors.ActiveTabForeColor
                    : colors.InactiveTabForeColor;

                using (var brush = new SolidBrush(tabBack))
                    g.FillRectangle(brush, rect);

                using (var pen = new Pen(colors.TabBorderColor))
                    g.DrawRectangle(pen, rect.X, rect.Y, Math.Max(0, rect.Width - 1), Math.Max(0, rect.Height - 1));

                PaintTab(g, rect, tab, tabFore, font, layout);
            }
        }

        private static void PaintTab(Graphics g, Rectangle rect, AutoHideTabModel tab, Color fore, Font font, AutoHideStripLayoutManager layout)
        {
            string iconPath = DockingCaptionPainter.ResolveTabIconPath(tab.IconPath);

            using var sf = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            };

            if (layout.Horizontal)
            {
                var iconRect = new Rectangle(
                    rect.Left + layout.TabPadding,
                    rect.Top + (rect.Height - layout.IconSize) / 2,
                    layout.IconSize, layout.IconSize);
                DockingCaptionPainter.PaintIcon(g, iconRect, iconPath, fore);

                int textLeft = iconRect.Right + layout.IconGap;
                var textRect = new Rectangle(
                    textLeft, rect.Top,
                    Math.Max(0, rect.Right - layout.TabPadding - textLeft), rect.Height);

                using var brush = new SolidBrush(fore);
                g.DrawString(tab.Title, font, brush, textRect, sf);
                return;
            }

            // Vertical edges (Left / Right): icon near the top, title rotated below it.
            var vIconRect = new Rectangle(
                rect.Left + (rect.Width - layout.IconSize) / 2,
                rect.Top + layout.TabPadding,
                layout.IconSize, layout.IconSize);
            DockingCaptionPainter.PaintIcon(g, vIconRect, iconPath, fore);

            int textStart = vIconRect.Bottom + layout.IconGap;
            int textLen = Math.Max(0, rect.Bottom - layout.TabPadding - textStart);

            var state = g.Save();
            g.TranslateTransform(rect.X + rect.Width / 2f, textStart + textLen / 2f);
            g.RotateTransform(-90f);
            var rotRect = new RectangleF(-textLen / 2f, -rect.Width / 2f, textLen, rect.Width);
            using (var brush = new SolidBrush(fore))
                g.DrawString(tab.Title, font, brush, rotRect, sf);
            g.Restore(state);
        }
    }
}

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.BaseImage;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.SideBar.Painters
{
    /// <summary>
    /// PillRail style: narrow vertical rail with centered circular icons
    /// Inspired by pill rail images in attachments (small rail with bottom controls and toggle)
    /// Uses theme colors (if requested) and ControlStyle where possible
    /// </summary>
    public sealed class PillRailSideBarPainter : BaseSideBarPainter
    {
        public override string Name => "PillRail";

        public override void Paint(ISideBarPainterContext context)
        {
            var g = context.Graphics;
            var bounds = context.DrawingRect;

            // Use theme colors if available
            Color backColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.SideMenuBackColor
                : Color.FromArgb(245, 247, 250);

            using (var brush = new SolidBrush(backColor))
            {
                g.FillRectangle(brush, bounds);
            }

            // Draw outer subtle border
            Color borderColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.BorderColor
                : Color.FromArgb(220, 220, 220);

            using (var pen = new Pen(borderColor, 1f))
            {
                g.DrawLine(pen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom);
            }

            int padding = 8;
            int currentY = bounds.Top + padding;

            if (context.ShowToggleButton)
            {
                Rectangle toggleRect = new Rectangle(bounds.Left + padding, currentY, bounds.Width - padding * 2, context.ItemHeight);
                PaintToggleButton(g, toggleRect, context);
                currentY += context.ItemHeight + padding;
            }

            // Center icons vertically in rail
            int railHeight = bounds.Height - (padding * 2) - (context.ShowToggleButton ? context.ItemHeight + padding : 0);
            int totalItems = context.Items?.Count ?? 0;
            int startIndex = 0;

            if (totalItems > 0)
            {
                int totalIconArea = totalItems * context.ItemHeight + (totalItems - 1) * 6;
                int startY = bounds.Top + (bounds.Height - totalIconArea) / 2;
                currentY = Math.Max(currentY, startY);
            }

            // Paint icons only (text hidden) - center each icon
            int iconSize = 20;

            foreach (var item in context.Items)
            {
                Rectangle itemRect = new Rectangle(bounds.Left + padding, currentY, bounds.Width - padding * 2, context.ItemHeight);

                // Draw hover background as small circle behind icon
                if (item == context.HoveredItem)
                {
                    var circle = new Rectangle(itemRect.X + (itemRect.Width - iconSize) / 2 - 4,
                                               itemRect.Y + (itemRect.Height - iconSize) / 2 - 4,
                                               iconSize + 8, iconSize + 8);
                    using (var path = CreateRoundedPath(circle, circle.Width / 2))
                    using (var brush = new SolidBrush(Color.FromArgb(20, 0, 0, 0)))
                    {
                        g.FillPath(brush, path);
                    }
                }

                // Draw selected accent circle
                if (item == context.SelectedItem)
                {
                    var selCircle = new Rectangle(itemRect.X + (itemRect.Width - iconSize) / 2 - 6,
                                                  itemRect.Y + (itemRect.Height - iconSize) / 2 - 6,
                                                  iconSize + 12, iconSize + 12);
                    Color accent = context.UseThemeColors && context.Theme != null ? context.Theme.PrimaryColor : context.AccentColor;
                    using (var path = CreateRoundedPath(selCircle, selCircle.Width / 2))
                    using (var brush = new SolidBrush(accent))
                    {
                        g.FillPath(brush, path);
                    }
                }

                // Draw the icon using cached approach
                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    Rectangle iconRect = new Rectangle(itemRect.X + (itemRect.Width - iconSize) / 2, itemRect.Y + (itemRect.Height - iconSize) / 2, iconSize, iconSize);
                    PaintMenuItemIcon(g, item, iconRect, context);
                }

                currentY += context.ItemHeight + 6;
            }
        }

        public override void PaintToggleButton(Graphics g, Rectangle toggleRect, ISideBarPainterContext context)
        {
            // small pill toggle button
            Color buttonColor = context.UseThemeColors && context.Theme != null ? context.Theme.PrimaryColor : context.AccentColor;
            using (var path = CreateRoundedPath(toggleRect, toggleRect.Height / 2))
            using (var brush = new SolidBrush(buttonColor))
            {
                g.FillPath(brush, path);
            }
        }

        public override void PaintSelection(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            // Selection handled as small accent circle, but keep method for base compatibility
            Color accent = context.UseThemeColors && context.Theme != null ? context.Theme.PrimaryColor : context.AccentColor;
            using (var brush = new SolidBrush(Color.FromArgb(255, accent.R, accent.G, accent.B)))
            {
                g.FillRectangle(brush, itemRect);
            }
        }

        public override void PaintHover(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            // Hover handled in Paint; provide fallback
            using (var brush = new SolidBrush(Color.FromArgb(15, 0, 0, 0)))
            {
                g.FillRectangle(brush, itemRect);
            }
        }
    }
}

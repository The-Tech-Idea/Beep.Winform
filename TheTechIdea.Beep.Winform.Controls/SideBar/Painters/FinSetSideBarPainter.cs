using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.BaseImage;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.SideBar.Painters
{
    /// <summary>
    /// FinSet style: expanded sidebar with pill-shaped selection, rounded corners, and generous spacing.
    /// Uses BeepStyling + Theme where possible.
    /// </summary>
    public sealed class FinSetSideBarPainter : BaseSideBarPainter
    {
        private static readonly ImagePainter _imagePainter = new ImagePainter();
        public override string Name => "FinSet";

        public override void Paint(ISideBarPainterContext context)
        {
            var g = context.Graphics;
            var bounds = context.DrawingRect;

            // Use control style painting for a consistent background
            var controlStyle = BeepStyling.GetControlStyle();
            var path = BeepStyling.CreateControlStylePath(bounds, controlStyle);
            BeepStyling.PaintStyleBackground(g, path, controlStyle);

            // Slight outer border
            Color borderColor = context.UseThemeColors && context.Theme != null ? context.Theme.BorderColor : Color.FromArgb(230, 230, 230);
            using (var pen = new Pen(borderColor, 1f))
            {
                g.DrawPath(pen, path);
            }

            int padding = 16;
            int currentY = bounds.Top + padding;

            if (context.ShowToggleButton)
            {
                Rectangle toggleRect = new Rectangle(bounds.Left + padding, currentY, bounds.Width - padding * 2, context.ItemHeight);
                PaintToggleButton(g, toggleRect, context);
                currentY += context.ItemHeight + 12;
            }

            PaintMenuItems(g, bounds, context, ref currentY);
        }

        public override void PaintToggleButton(Graphics g, Rectangle toggleRect, ISideBarPainterContext context)
        {
            // big rounded toggle, responsive to theme
            Color buttonColor = context.UseThemeColors && context.Theme != null ? context.Theme.PrimaryColor : context.AccentColor;
            using (var path = CreateRoundedPath(toggleRect, 8))
            using (var brush = new SolidBrush(buttonColor))
            {
                g.FillPath(brush, path);
            }
        }

        public override void PaintSelection(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            // Pill-shaped selection
            Color selectColor = context.UseThemeColors && context.Theme != null ? context.Theme.AccentColor : Color.FromArgb(102, 80, 255);

            var rect = new Rectangle(itemRect.Left + 6, itemRect.Top + 4, itemRect.Width - 12, itemRect.Height - 8);
            using (var path = CreateRoundedPath(rect, rect.Height / 2))
            using (var brush = new LinearGradientBrush(rect, Color.FromArgb(220, selectColor), selectColor, LinearGradientMode.Vertical))
            {
                g.FillPath(brush, path);
            }

            // Subtle accent left bar
            Color accent = context.UseThemeColors && context.Theme != null ? context.Theme.PrimaryColor : context.AccentColor;
            Rectangle accentBar = new Rectangle(rect.Left + 6, rect.Top + 8, 6, rect.Height - 16);
            using (var brush = new SolidBrush(accent))
            {
                g.FillRectangle(brush, accentBar);
            }
        }

        public override void PaintHover(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            Color hover = context.UseThemeColors && context.Theme != null ? Color.FromArgb(20, context.Theme.PrimaryColor) : Color.FromArgb(15, 0, 0, 0);
            var rect = new Rectangle(itemRect.Left + 6, itemRect.Top + 4, itemRect.Width - 12, itemRect.Height - 8);
            using (var path = CreateRoundedPath(rect, rect.Height / 2))
            using (var brush = new SolidBrush(hover))
            {
                g.FillPath(brush, path);
            }
        }

        private void PaintMenuItems(Graphics g, Rectangle bounds, ISideBarPainterContext context, ref int currentY)
        {
            if (context.Items == null || context.Items.Count == 0) return;

            int padding = 16;
            int iconSize = 20;
            int iconPadding = 12;

            foreach (var item in context.Items)
            {
                Rectangle itemRect = new Rectangle(bounds.Left + padding, currentY, bounds.Width - padding * 2, context.ItemHeight);

                if (item == context.HoveredItem) PaintHover(g, itemRect, context);
                if (item == context.SelectedItem) PaintSelection(g, itemRect, context);

                int x = itemRect.X + 10;

                // Draw icon
                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    Rectangle iconRect = new Rectangle(x, itemRect.Y + (itemRect.Height - iconSize) / 2, iconSize, iconSize);
                    _imagePainter.ImagePath = GetIconPath(item, context);
                    if (context.Theme != null && context.UseThemeColors)
                    {
                        _imagePainter.CurrentTheme = context.Theme;
                        _imagePainter.ApplyThemeOnImage = true;
                        _imagePainter.ImageEmbededin = ImageEmbededin.SideBar;
                    }
                    _imagePainter.DrawImage(g, iconRect);
                    x += iconSize + iconPadding;
                }

                // Draw text
                if (!context.IsCollapsed)
                {
                    Color textColor = context.UseThemeColors && context.Theme != null ? context.Theme.SideMenuForeColor : Color.FromArgb(16, 24, 32);
                    using (var font = new Font("Inter", 14f, item == context.SelectedItem ? FontStyle.Bold : FontStyle.Regular))
                    using (var brush = new SolidBrush(textColor))
                    {
                        Rectangle textRect = new Rectangle(x, itemRect.Y, itemRect.Right - x - 8, itemRect.Height);
                        StringFormat format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
                        g.DrawString(item.Text, font, brush, textRect, format);
                    }
                }

                currentY += context.ItemHeight + 8;
            }
        }
    }
}

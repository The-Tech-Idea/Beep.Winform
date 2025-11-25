using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.BaseImage;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.SideBar.Painters
{
    public sealed class Fluent2SideBarPainter : BaseSideBarPainter
    {
        public override string Name => "Fluent2";

        public override void Paint(ISideBarPainterContext context)
        {
            var g = context.Graphics;
            var bounds = context.DrawingRect;

            // Fluent 2 layer color with acrylic effect
            Color backColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.SideMenuBackColor
                : Color.FromArgb(243, 242, 241);

            using (var brush = new SolidBrush(backColor))
            {
                g.FillRectangle(brush, bounds);
            }

            // Fluent 2 divider color
            Color dividerColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.BorderColor
                : Color.FromArgb(237, 235, 233);

            using (var pen = new Pen(dividerColor, 1f))
            {
                g.DrawLine(pen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom);
            }

            int padding = 8;
            int currentY = bounds.Top + padding;

            if (context.ShowToggleButton)
            {
                Rectangle toggleRect = new Rectangle(bounds.Left + padding, currentY, bounds.Width - padding * 2, context.ItemHeight);
                PaintToggleButton(g, toggleRect, context);
                currentY += context.ItemHeight + 8;
            }

            PaintMenuItems(g, bounds, context, ref currentY);
        }

        public override void PaintToggleButton(Graphics g, Rectangle toggleRect, ISideBarPainterContext context)
        {
            // Fluent 2 accent color (blue)
            Color buttonColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.PrimaryColor
                : Color.FromArgb(0, 120, 212);

            using (var path = CreateRoundedPath(toggleRect, 4))
            using (var brush = new SolidBrush(buttonColor))
            {
                g.FillPath(brush, path);
            }

            // Fluent 2 white on accent - use shared DrawHamburgerIcon helper
            Color iconColor = Color.White;
            int iconW = Math.Min(22, Math.Max(12, toggleRect.Width - 12));
            int iconH = Math.Min(14, Math.Max(10, toggleRect.Height - 8));
            var iconRect = new Rectangle(toggleRect.X + (toggleRect.Width - iconW) / 2, toggleRect.Y + (toggleRect.Height - iconH) / 2, iconW, iconH);
            DrawHamburgerIcon(g, iconRect, iconColor);
        }

        public override void PaintSelection(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            // Fluent 2 subtle selection with left accent bar
            Color selectionColor = context.UseThemeColors && context.Theme != null
                ? Color.FromArgb(243, context.Theme.PrimaryColor.R, context.Theme.PrimaryColor.G, context.Theme.PrimaryColor.B)
                : Color.FromArgb(243, 242, 241);

            using (var brush = new SolidBrush(selectionColor))
            {
                g.FillRectangle(brush, itemRect);
            }

            // Left accent bar (Fluent signature)
            Color accentColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.PrimaryColor
                : Color.FromArgb(0, 120, 212);

            Rectangle accentBar = new Rectangle(itemRect.X, itemRect.Y, 3, itemRect.Height);
            using (var brush = new SolidBrush(accentColor))
            {
                g.FillRectangle(brush, accentBar);
            }
        }

        public override void PaintHover(Graphics g, Rectangle itemRect, ISideBarPainterContext context)
        {
            // Fluent 2 hover state
            Color hoverColor = context.UseThemeColors && context.Theme != null
                ? Color.FromArgb(10, context.Theme.PrimaryColor.R, context.Theme.PrimaryColor.G, context.Theme.PrimaryColor.B)
                : Color.FromArgb(10, 0, 120, 212);

            using (var brush = new SolidBrush(hoverColor))
            {
                g.FillRectangle(brush, itemRect);
            }
        }

        private void PaintMenuItems(Graphics g, Rectangle bounds, ISideBarPainterContext context, ref int currentY)
        {
            if (context.Items == null || context.Items.Count == 0) return;

            int padding = 8;
            int iconSize = GetTopLevelIconSize(context);
            int iconPadding = GetIconPadding(context);

            foreach (var item in context.Items)
            {
                Rectangle itemRect = new Rectangle(bounds.Left + padding, currentY, bounds.Width - padding * 2, context.ItemHeight);

                if (item == context.HoveredItem) PaintHover(g, itemRect, context);
                if (item == context.SelectedItem) PaintSelection(g, itemRect, context);

                int x = itemRect.X + (item == context.SelectedItem ? 8 : 6); // Offset for accent bar

                // Draw icon using the base painter's cached approach
                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    Rectangle iconRect = new Rectangle(x, itemRect.Y + (itemRect.Height - iconSize) / 2, iconSize, iconSize);
                    PaintMenuItemIcon(g, item, iconRect, context);
                    x += iconSize + iconPadding;
                }

                // Draw text
                if (!context.IsCollapsed)
                {
                    // Fluent 2 text color
                    Color textColor = context.UseThemeColors && context.Theme != null
                        ? (item == context.SelectedItem ? Color.FromArgb(0, 120, 212) : context.Theme.SideMenuForeColor)
                        : (item == context.SelectedItem ? Color.FromArgb(0, 120, 212) : Color.FromArgb(50, 49, 48));

                    var font = BeepFontManager.GetCachedFont("Segoe UI", 14f, item == context.SelectedItem ? FontStyle.Bold : FontStyle.Regular);
                    using (var brush = new SolidBrush(textColor))
                    {
                        Rectangle textRect = new Rectangle(x, itemRect.Y, Math.Max(0, itemRect.Right - x - 12), itemRect.Height);
                        StringFormat format = new StringFormat
                        {
                            Alignment = StringAlignment.Near,
                            LineAlignment = StringAlignment.Center,
                            Trimming = StringTrimming.EllipsisCharacter
                        };

                        g.DrawString(item.Text, font, brush, textRect, format);
                    }
                }

                currentY += context.ItemHeight + 2;
            }
        }
    }
}

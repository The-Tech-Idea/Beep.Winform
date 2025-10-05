using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.BaseImage;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.SideBar
{
    /// <summary>
    /// Base class for BeepSideBar painters with common helper methods
    /// </summary>
    public abstract class BaseSideBarPainter : ISideBarPainter
    {
        // Reusable ImagePainter instance - DO NOT create new instances in loops!
        private static readonly ImagePainter _sharedImagePainter = new ImagePainter();

        public abstract string Name { get; }

        public abstract void Paint(ISideBarPainterContext context);

        public virtual void PaintMenuItem(Graphics g, SimpleItem item, Rectangle itemRect, ISideBarPainterContext context)
        {
            int padding = 8;
            int iconSize = 24;
            int x = itemRect.X + padding;

            // Draw icon if available
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                var iconRect = new Rectangle(x, itemRect.Y + (itemRect.Height - iconSize) / 2, iconSize, iconSize);
                PaintMenuItemIcon(g, item, iconRect, context);
                x += iconSize + padding;
            }

            // Draw text if not collapsed
            if (!context.IsCollapsed)
            {
                var textRect = new Rectangle(x, itemRect.Y, itemRect.Right - x - padding, itemRect.Height);
                PaintMenuItemText(g, item, textRect, context);
            }
        }

        public abstract void PaintSelection(Graphics g, Rectangle itemRect, ISideBarPainterContext context);

        public abstract void PaintHover(Graphics g, Rectangle itemRect, ISideBarPainterContext context);

        public abstract void PaintToggleButton(Graphics g, Rectangle toggleRect, ISideBarPainterContext context);

        public virtual void PaintChildItem(Graphics g, SimpleItem childItem, Rectangle childRect, ISideBarPainterContext context, int indentLevel)
        {
            int padding = 4;
            int iconSize = 18; // Smaller for children
            int indent = context.IndentationWidth * indentLevel;
            int x = childRect.X + indent + padding;

            // Draw connector line
            PaintConnectorLine(g, childRect, context, indent);

            // Draw icon if available
            if (!string.IsNullOrEmpty(childItem.ImagePath))
            {
                var iconRect = new Rectangle(x, childRect.Y + (childRect.Height - iconSize) / 2, iconSize, iconSize);
                PaintMenuItemIcon(g, childItem, iconRect, context);
                x += iconSize + padding;
            }

            // Draw text if not collapsed
            if (!context.IsCollapsed)
            {
                var textRect = new Rectangle(x, childRect.Y, childRect.Right - x - padding, childRect.Height);
                PaintChildItemText(g, childItem, textRect, context);
            }
        }

        /// <summary>
        /// Helper method to paint menu item icon using ImagePainter with theme support
        /// REUSES shared ImagePainter instance - do NOT create new instances!
        /// </summary>
        protected virtual void PaintMenuItemIcon(Graphics g, SimpleItem item, Rectangle iconRect, ISideBarPainterContext context)
        {
            try
            {
                // Reuse the shared ImagePainter instance
                _sharedImagePainter.ImagePath = item.ImagePath;

                // Apply theme if available
                if (context.Theme != null)
                {
                    _sharedImagePainter.CurrentTheme = context.Theme;
                    _sharedImagePainter.ApplyThemeOnImage = true;
                    _sharedImagePainter.ImageEmbededin = ImageEmbededin.SideBar;
                }
                else
                {
                    _sharedImagePainter.ApplyThemeOnImage = false;
                }

                // Draw using ImagePainter's DrawImage method
                _sharedImagePainter.DrawImage(g, iconRect);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Icon drawing error: {ex.Message}");
                // Fallback: draw placeholder
                using (var pen = new Pen(Color.Gray, 1f))
                {
                    g.DrawRectangle(pen, iconRect);
                }
            }
        }

        /// <summary>
        /// Helper method to paint menu item text with theme colors
        /// Uses TextRenderer for better text quality and automatic ellipsis
        /// </summary>
        protected virtual void PaintMenuItemText(Graphics g, SimpleItem item, Rectangle textRect, ISideBarPainterContext context)
        {
            if (string.IsNullOrEmpty(item.Text) || textRect.Width <= 0 || textRect.Height <= 0)
                return;

            // Use theme colors if available
            Color textColor;
            if (context.Theme != null && context.UseThemeColors)
            {
                textColor = item.IsEnabled ?
                    context.Theme.SideMenuForeColor :
                    context.Theme.DisabledForeColor;
            }
            else
            {
                textColor = item.IsEnabled ? Color.FromArgb(240, 240, 240) : Color.Gray;
            }

            // Use theme font if available
            Font textFont = context.Theme != null ?
                Vis.Modules.Managers.BeepThemesManager.ToFont(context.Theme.GetAnswerFont()) :
                new Font("Segoe UI", 9f);

            // Use TextRenderer for better quality
            var textFormat = System.Windows.Forms.TextFormatFlags.Left |
                            System.Windows.Forms.TextFormatFlags.VerticalCenter |
                            System.Windows.Forms.TextFormatFlags.EndEllipsis |
                            System.Windows.Forms.TextFormatFlags.NoPrefix |
                            System.Windows.Forms.TextFormatFlags.SingleLine;

            System.Windows.Forms.TextRenderer.DrawText(g, item.Text, textFont, textRect, textColor, textFormat);
        }

        /// <summary>
        /// Helper method to paint child item text (smaller font)
        /// Uses TextRenderer for better text quality and automatic ellipsis
        /// </summary>
        protected virtual void PaintChildItemText(Graphics g, SimpleItem childItem, Rectangle textRect, ISideBarPainterContext context)
        {
            if (string.IsNullOrEmpty(childItem.Text) || textRect.Width <= 0 || textRect.Height <= 0)
                return;

            // Use theme colors if available
            Color textColor;
            if (context.Theme != null && context.UseThemeColors)
            {
                textColor = childItem.IsEnabled ?
                    context.Theme.SideMenuForeColor :
                    context.Theme.DisabledForeColor;
            }
            else
            {
                textColor = childItem.IsEnabled ? Color.FromArgb(220, 220, 220) : Color.Gray;
            }

            // Use smaller font for children
            Font textFont = context.Theme != null ?
                Vis.Modules.Managers.BeepThemesManager.ToFont(context.Theme.GetAnswerFont()) :
                new Font("Segoe UI", 8.5f);

            // Make it slightly smaller
            if (textFont.Size > 7)
            {
                textFont = new Font(textFont.FontFamily, textFont.Size - 0.5f, textFont.Style);
            }

            // Use TextRenderer for better quality
            var textFormat = System.Windows.Forms.TextFormatFlags.Left |
                            System.Windows.Forms.TextFormatFlags.VerticalCenter |
                            System.Windows.Forms.TextFormatFlags.EndEllipsis |
                            System.Windows.Forms.TextFormatFlags.NoPrefix |
                            System.Windows.Forms.TextFormatFlags.SingleLine;

            System.Windows.Forms.TextRenderer.DrawText(g, childItem.Text, textFont, textRect, textColor, textFormat);
        }

        /// <summary>
        /// Helper method to draw connector line from parent to child
        /// </summary>
        protected virtual void PaintConnectorLine(Graphics g, Rectangle childRect, ISideBarPainterContext context, int indent)
        {
            Color lineColor = context.Theme != null && context.UseThemeColors ?
                Color.FromArgb(50, context.Theme.SideMenuForeColor) :
                Color.FromArgb(50, 180, 180, 180);

            using (var pen = new Pen(lineColor, 1f))
            {
                int lineX = childRect.X + (indent / 2);
                int lineY = childRect.Y + childRect.Height / 2;

                // Draw horizontal line to child
                g.DrawLine(pen, lineX, lineY, childRect.X + indent, lineY);
            }
        }

        /// <summary>
        /// Helper method to create rounded rectangle path
        /// </summary>
        protected GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int diameter = radius * 2;
            var arc = new Rectangle(rect.Location, new Size(diameter, diameter));

            // Top left arc
            path.AddArc(arc, 180, 90);

            // Top right arc
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);

            // Bottom right arc
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // Bottom left arc
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Helper method to draw expand/collapse icon
        /// </summary>
        protected virtual void PaintExpandCollapseIcon(Graphics g, Rectangle iconRect, bool isExpanded, ISideBarPainterContext context)
        {
            Color iconColor = context.Theme != null && context.UseThemeColors ?
                context.Theme.SideMenuForeColor :
                Color.FromArgb(200, 200, 200);

            using (var pen = new Pen(iconColor, 2f))
            {
                int centerX = iconRect.X + iconRect.Width / 2;
                int centerY = iconRect.Y + iconRect.Height / 2;

                // Draw horizontal line
                g.DrawLine(pen, centerX - 4, centerY, centerX + 4, centerY);

                // Draw vertical line if not expanded (making a plus)
                if (!isExpanded)
                {
                    g.DrawLine(pen, centerX, centerY - 4, centerX, centerY + 4);
                }
            }
        }

        /// <summary>
        /// Helper method to get effective color based on UseThemeColors setting
        /// </summary>
        protected Color GetEffectiveColor(ISideBarPainterContext context, Color themeColor, Color fallbackColor)
        {
            return context.UseThemeColors && context.Theme != null ? themeColor : fallbackColor;
        }

        /// <summary>
        /// Calculate minimum required size for a sidebar item based on text and icon
        /// Helps prevent text clipping by ensuring adequate space
        /// Sidebar is always vertical: icon left, text right
        /// </summary>
        protected static Size CalculateMinimumItemSize(Graphics g, SimpleItem item,
            string fontFamily = "Segoe UI", float fontSize = 9f, int iconSize = 24, int padding = 12)
        {
            if (item == null)
                return new Size(200, 44); // Default minimum

            bool hasIcon = !string.IsNullOrEmpty(item.ImagePath);
            bool hasText = !string.IsNullOrEmpty(item.Text);

            if (!hasIcon && !hasText)
                return new Size(200, 44); // Default minimum

            Size textSize = Size.Empty;

            if (hasText)
            {
                using (var font = new Font(fontFamily, fontSize, FontStyle.Regular))
                {
                    textSize = System.Windows.Forms.TextRenderer.MeasureText(g, item.Text, font,
                        new Size(int.MaxValue, int.MaxValue),
                        System.Windows.Forms.TextFormatFlags.NoPadding |
                        System.Windows.Forms.TextFormatFlags.SingleLine);
                }
            }

            // Sidebar: icon left, text right, side by side
            int width, height;

            if (hasIcon && hasText)
            {
                // Both icon and text
                width = iconSize + textSize.Width + padding * 3; // icon + spacing + text + padding
                height = Math.Max(iconSize, textSize.Height) + padding * 2;
            }
            else if (hasIcon)
            {
                // Only icon
                width = iconSize + padding * 2;
                height = iconSize + padding * 2;
            }
            else
            {
                // Only text
                width = textSize.Width + padding * 2;
                height = textSize.Height + padding * 2;
            }

            return new Size(Math.Max(width, 200), Math.Max(height, 44));
        }
    }
}

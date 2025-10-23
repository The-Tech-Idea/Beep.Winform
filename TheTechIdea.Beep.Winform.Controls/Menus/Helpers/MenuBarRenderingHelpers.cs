using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Menus.Helpers
{
    /// <summary>
    /// Static helper class providing common rendering utilities for all menu bar painters
    /// </summary>
    public static class MenuBarRenderingHelpers
    {
        #region Path Creation
        /// <summary>
        /// Creates a rounded rectangle graphics path
        /// </summary>
        public static GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0 || rect.IsEmpty)
            {
                path.AddRectangle(rect);
                return path;
            }

            int d = radius * 2;
            path.AddArc(rect.Left, rect.Top, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Top, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.Left, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Creates a tab-shaped path
        /// </summary>
        public static GraphicsPath CreateTabPath(Rectangle rect, int radius, bool isActive = false)
        {
            var path = new GraphicsPath();
            if (rect.IsEmpty) return path;

            if (isActive)
            {
                // Active tab - no bottom border
                path.AddArc(rect.Left, rect.Top, radius * 2, radius * 2, 180, 90);
                path.AddArc(rect.Right - radius * 2, rect.Top, radius * 2, radius * 2, 270, 90);
                path.AddLine(rect.Right, rect.Top + radius, rect.Right, rect.Bottom);
                path.AddLine(rect.Right, rect.Bottom, rect.Left, rect.Bottom);
                path.AddLine(rect.Left, rect.Bottom, rect.Left, rect.Top + radius);
            }
            else
            {
                // Inactive tab - full border
                path.AddArc(rect.Left, rect.Top + 2, radius * 2, radius * 2, 180, 90);
                path.AddArc(rect.Right - radius * 2, rect.Top + 2, radius * 2, radius * 2, 270, 90);
                path.AddLine(rect.Right, rect.Top + radius + 2, rect.Right, rect.Bottom - 2);
                path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2 - 2, radius * 2, radius * 2, 0, 90);
                path.AddArc(rect.Left, rect.Bottom - radius * 2 - 2, radius * 2, radius * 2, 90, 90);
                path.AddLine(rect.Left, rect.Bottom - radius - 2, rect.Left, rect.Top + radius + 2);
            }
            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Creates a breadcrumb item path with chevron
        /// </summary>
        public static GraphicsPath CreateBreadcrumbPath(Rectangle rect, int chevronWidth = 12)
        {
            var path = new GraphicsPath();
            if (rect.IsEmpty) return path;

            // Create breadcrumb shape: rectangle with chevron on right side
            var points = new Point[]
            {
                new Point(rect.Left, rect.Top),
                new Point(rect.Right - chevronWidth, rect.Top),
                new Point(rect.Right, rect.Top + rect.Height / 2),
                new Point(rect.Right - chevronWidth, rect.Bottom),
                new Point(rect.Left, rect.Bottom)
            };

            path.AddPolygon(points);
            return path;
        }
        #endregion

        #region Menu Item Drawing
        /// <summary>
        /// Draws a menu item with text and optional icon
        /// </summary>
        public static void DrawMenuItem(Graphics g, Rectangle rect, SimpleItem item, Font font, 
            Color textColor, Color backgroundColor, Color borderColor, Size iconSize, 
            bool showIcon = true, bool showDropdownIndicator = false, int cornerRadius = 0)
        {
            if (g == null || rect.IsEmpty || item == null) return;

            // Draw background
            DrawMenuItemBackground(g, rect, backgroundColor, borderColor, cornerRadius);

            // Calculate layout
            var layout = CalculateMenuItemLayout(rect, item, font, iconSize, showIcon, showDropdownIndicator);

            // Draw icon if available and requested
            if (showIcon && !string.IsNullOrEmpty(item.ImagePath) && !layout.IconRect.IsEmpty)
            {
                DrawMenuItemIcon(g, layout.IconRect, item.ImagePath, textColor);
            }

            // Draw text
            if (!string.IsNullOrEmpty(item.Text) && !layout.TextRect.IsEmpty)
            {
                DrawMenuItemText(g, layout.TextRect, item.Text, font, textColor);
            }

            // Draw dropdown indicator
            if (showDropdownIndicator && item.Children?.Count > 0 && !layout.DropdownRect.IsEmpty)
            {
                DrawDropdownIndicator(g, layout.DropdownRect, textColor);
            }
        }

        /// <summary>
        /// Draws menu item background
        /// </summary>
        public static void DrawMenuItemBackground(Graphics g, Rectangle rect, Color backgroundColor, 
            Color borderColor, int cornerRadius = 0)
        {
            if (g == null || rect.IsEmpty) return;

            // Fill background
            if (backgroundColor != Color.Transparent)
            {
                using var brush = new SolidBrush(backgroundColor);
                if (cornerRadius > 0)
                {
                    using var path = CreateRoundedPath(rect, cornerRadius);
                    g.FillPath(brush, path);
                }
                else
                {
                    g.FillRectangle(brush, rect);
                }
            }

            // Draw border
            if (borderColor != Color.Transparent)
            {
                using var pen = new Pen(borderColor);
                if (cornerRadius > 0)
                {
                    using var path = CreateRoundedPath(rect, cornerRadius);
                    g.DrawPath(pen, path);
                }
                else
                {
                    g.DrawRectangle(pen, rect);
                }
            }
        }

        /// <summary>
        /// Draws menu item icon
        /// </summary>
        public static void DrawMenuItemIcon(Graphics g, Rectangle iconRect, string iconPath, Color color)
        {
            if (g == null || iconRect.IsEmpty || string.IsNullOrEmpty(iconPath)) return;

            try
            {
                // Use ImagePainter for proper image rendering
                using var imagePainter = new BaseImage.ImagePainter(iconPath);
                imagePainter.ApplyThemeOnImage = false; // Don't apply theme color by default
                imagePainter.DrawImage(g, iconRect);
            }
            catch
            {
                // Fallback to placeholder if image loading fails
                using var brush = new SolidBrush(Color.FromArgb(100, color));
                g.FillRectangle(brush, iconRect);
                
                using var pen = new Pen(color);
                g.DrawRectangle(pen, iconRect);
            }
        }

        /// <summary>
        /// Draws menu item text
        /// </summary>
        public static void DrawMenuItemText(Graphics g, Rectangle textRect, string text, Font font, Color color)
        {
            if (g == null || textRect.IsEmpty || string.IsNullOrEmpty(text) || font == null) return;

            using var brush = new SolidBrush(color);
            var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            };

          //  g.DrawString(text, font, brush, textRect, format);
            TextRenderer.DrawText(g, text, font, textRect, color, 
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        /// <summary>
        /// Draws dropdown indicator (chevron down)
        /// </summary>
        public static void DrawDropdownIndicator(Graphics g, Rectangle rect, Color color)
        {
            if (g == null || rect.IsEmpty) return;

            using var brush = new SolidBrush(color);
            var points = new Point[]
            {
                new Point(rect.X + 2, rect.Y + rect.Height / 3),
                new Point(rect.X + rect.Width / 2, rect.Y + 2 * rect.Height / 3),
                new Point(rect.Right - 2, rect.Y + rect.Height / 3)
            };
            g.FillPolygon(brush, points);
        }

        /// <summary>
        /// Calculates layout rectangles for menu item components
        /// </summary>
        public static MenuItemLayout CalculateMenuItemLayout(Rectangle itemRect, SimpleItem item, Font font, 
            Size iconSize, bool showIcon, bool showDropdownIndicator)
        {
            var layout = new MenuItemLayout();
            if (itemRect.IsEmpty) return layout;

            int padding = 8;
            int iconTextSpacing = 4;
            int dropdownWidth = 12;

            // Start with full item rectangle
            var availableRect = Rectangle.Inflate(itemRect, -padding, -padding);
            int currentX = availableRect.X;

            // Icon area
            if (showIcon && !string.IsNullOrEmpty(item?.ImagePath))
            {
                layout.IconRect = new Rectangle(
                    currentX,
                    availableRect.Y + (availableRect.Height - iconSize.Height) / 2,
                    iconSize.Width,
                    iconSize.Height
                );
                currentX += iconSize.Width + iconTextSpacing;
            }

            // Dropdown indicator area
            if (showDropdownIndicator && item?.Children?.Count > 0)
            {
                layout.DropdownRect = new Rectangle(
                    availableRect.Right - dropdownWidth,
                    availableRect.Y + (availableRect.Height - 8) / 2,
                    dropdownWidth,
                    8
                );
            }

            // Text area (remaining space)
            int textWidth = availableRect.Right - currentX;
            if (showDropdownIndicator && item?.Children?.Count > 0)
            {
                textWidth -= dropdownWidth + iconTextSpacing;
            }

            if (textWidth > 0)
            {
                layout.TextRect = new Rectangle(
                    currentX,
                    availableRect.Y,
                    textWidth,
                    availableRect.Height
                );
            }

            return layout;
        }
        #endregion

        #region Special Effects
        /// <summary>
        /// Draws a hover effect
        /// </summary>
        public static void DrawHoverEffect(Graphics g, Rectangle rect, Color hoverColor, int cornerRadius = 0)
        {
            if (g == null || rect.IsEmpty) return;

            using var brush = new SolidBrush(Color.FromArgb(50, hoverColor));
            if (cornerRadius > 0)
            {
                using var path = CreateRoundedPath(rect, cornerRadius);
                g.FillPath(brush, path);
            }
            else
            {
                g.FillRectangle(brush, rect);
            }
        }

        /// <summary>
        /// Draws a selection indicator
        /// </summary>
        public static void DrawSelectionIndicator(Graphics g, Rectangle rect, Color selectionColor, 
            SelectionIndicatorStyle style = SelectionIndicatorStyle.Background)
        {
            if (g == null || rect.IsEmpty) return;

            using var brush = new SolidBrush(selectionColor);
            using var pen = new Pen(selectionColor, 2);

            switch (style)
            {
                case SelectionIndicatorStyle.Background:
                    g.FillRectangle(brush, rect);
                    break;
                case SelectionIndicatorStyle.BottomLine:
                    g.DrawLine(pen, rect.Left, rect.Bottom - 1, rect.Right, rect.Bottom - 1);
                    break;
                case SelectionIndicatorStyle.LeftLine:
                    g.DrawLine(pen, rect.Left, rect.Top, rect.Left, rect.Bottom);
                    break;
                case SelectionIndicatorStyle.Border:
                    g.DrawRectangle(pen, rect);
                    break;
                case SelectionIndicatorStyle.Dot:
                    var dotRect = new Rectangle(
                        rect.X + rect.Width / 2 - 3,
                        rect.Bottom - 8,
                        6, 6
                    );
                    g.FillEllipse(brush, dotRect);
                    break;
            }
        }

        /// <summary>
        /// Draws a breadcrumb separator (chevron)
        /// </summary>
        public static void DrawBreadcrumbSeparator(Graphics g, Rectangle rect, Color color)
        {
            if (g == null || rect.IsEmpty) return;

            using var brush = new SolidBrush(color);
            var points = new Point[]
            {
                new Point(rect.X + 2, rect.Y + 2),
                new Point(rect.X + rect.Width - 2, rect.Y + rect.Height / 2),
                new Point(rect.X + 2, rect.Bottom - 2)
            };
            g.FillPolygon(brush, points);
        }

        /// <summary>
        /// Draws menu bar title/logo
        /// </summary>
        public static void DrawMenuBarTitle(Graphics g, Rectangle rect, string title, string iconPath, 
            Font font, Color textColor, Size iconSize)
        {
            if (g == null || rect.IsEmpty) return;

            int padding = 8;
            int iconTextSpacing = 8;
            int currentX = rect.X + padding;

            // Draw icon if available
            if (!string.IsNullOrEmpty(iconPath))
            {
                var iconRect = new Rectangle(
                    currentX,
                    rect.Y + (rect.Height - iconSize.Height) / 2,
                    iconSize.Width,
                    iconSize.Height
                );
                DrawMenuItemIcon(g, iconRect, iconPath, textColor);
                currentX += iconSize.Width + iconTextSpacing;
            }

            // Draw title text
            if (!string.IsNullOrEmpty(title))
            {
                var textRect = new Rectangle(
                    currentX,
                    rect.Y,
                    rect.Right - currentX - padding,
                    rect.Height
                );
                DrawMenuItemText(g, textRect, title, font, textColor);
            }
        }
        #endregion

        #region Layout Helpers
        /// <summary>
        /// Calculates menu item rectangles for horizontal layout
        /// </summary>
        public static List<Rectangle> CalculateHorizontalMenuItemRects(Rectangle contentRect, 
            List<SimpleItem> items, int itemHeight, int itemSpacing, int minItemWidth = 60)
        {
            var rects = new List<Rectangle>();
            if (contentRect.IsEmpty || items == null || items.Count == 0) return rects;

            int currentX = contentRect.X;
            int itemY = contentRect.Y + (contentRect.Height - itemHeight) / 2;

            foreach (var item in items)
            {
                // Calculate item width based on content
                int itemWidth = Math.Max(minItemWidth, CalculateMenuItemWidth(item, itemHeight));
                
                // Ensure item fits in available space
                if (currentX + itemWidth > contentRect.Right)
                {
                    itemWidth = contentRect.Right - currentX;
                    if (itemWidth < minItemWidth / 2) break; // Not enough space
                }

                var itemRect = new Rectangle(currentX, itemY, itemWidth, itemHeight);
                rects.Add(itemRect);

                currentX += itemWidth + itemSpacing;
                if (currentX >= contentRect.Right) break;
            }

            return rects;
        }

        /// <summary>
        /// Calculates preferred width for a menu item
        /// </summary>
        public static int CalculateMenuItemWidth(SimpleItem item, int itemHeight, Font font = null)
        {
            if (item == null) return 60;

            int width = 20; // Base padding
            
            // Add icon width
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                width += 16 + 4; // Icon + spacing
            }

            // Add text width
            if (!string.IsNullOrEmpty(item.Text) && font != null)
            {
                using var tempBitmap = new Bitmap(1, 1);
                using var g = Graphics.FromImage(tempBitmap);
                var textSize = TextUtils.MeasureText(g,item.Text, font);
                width += (int)textSize.Width + 4; // Text + spacing
            }
            else if (!string.IsNullOrEmpty(item.Text))
            {
                // Rough estimation without font
                width += item.Text.Length * 8;
            }

            // Add dropdown indicator width
            if (item.Children?.Count > 0)
            {
                width += 12 + 4; // Dropdown arrow + spacing
            }

            return Math.Max(width, 60); // Minimum width
        }
        #endregion
    }

    /// <summary>
    /// Layout information for menu item components
    /// </summary>
    public struct MenuItemLayout
    {
        public Rectangle IconRect;
        public Rectangle TextRect;
        public Rectangle DropdownRect;
    }

    /// <summary>
    /// Selection indicator styles
    /// </summary>
    public enum SelectionIndicatorStyle
    {
        Background,
        BottomLine,
        LeftLine,
        Border,
        Dot
    }

    /// <summary>
    /// Notification types for icon drawing
    /// </summary>
    public enum NotificationType
    {
        None,
        NewMessage,
        FriendRequest,
        Tag,
        Event,
        Reminder,
        Custom
    }
}
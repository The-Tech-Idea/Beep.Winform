using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.BaseImage;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.NavBars.Painters
{
    /// <summary>
    /// Base class with helpers used by concrete NavBar painters.
    /// Provides common rendering utilities and default implementations.
    /// </summary>
    public abstract class BaseNavBarPainter : INavBarPainter
    {
        // Reusable ImagePainter instance - DO NOT create new instances in loops!
        private static readonly ImagePainter _sharedImagePainter = new ImagePainter();
        
        public abstract string Name { get; }
        public abstract void Draw(Graphics g, INavBarPainterContext context, Rectangle bounds);

        /// <summary>Gets DPI-scaled value using context's owner control.</summary>
        protected static int ScaleValue(int value, INavBarPainterContext ctx) =>
            ctx?.OwnerControl != null ? DpiScalingHelper.ScaleValue(value, ctx.OwnerControl) : value;
        /// <summary>Text font from context.</summary>
        protected static Font TextFont(INavBarPainterContext ctx) => ctx?.TextFont;

        public virtual void DrawSelection(Graphics g, INavBarPainterContext context, Rectangle selectedRect)
        {
            // Default: subtle pill highlight using theme accent
            using var path = CreateRoundedPath(selectedRect, 10);
            Color accentColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.AccentColor
                : context.AccentColor;
            using var br = new SolidBrush(Color.FromArgb(28, accentColor));
            using var pen = new Pen(Color.FromArgb(120, accentColor), 1f) { Alignment = PenAlignment.Inset };
            g.FillPath(br, path);
            g.DrawPath(pen, path);
        }

        public virtual void DrawHover(Graphics g, INavBarPainterContext context, Rectangle hoverRect)
        {
            // Default: subtle highlight on hover
            using var path = CreateRoundedPath(hoverRect, 8);
            Color accentColor = context.UseThemeColors && context.Theme != null
                ? context.Theme.AccentColor
                : context.AccentColor;
            using var br = new SolidBrush(Color.FromArgb(15, accentColor));
            g.FillPath(br, path);
        }

        public virtual void UpdateHitAreas(INavBarPainterContext context, Rectangle bounds, Action<string, Rectangle, Action> registerHitArea)
        {
            // Default: register hit areas for each nav item
            if (context.Items == null) return;

            bool isVertical = context.Orientation == NavBarOrientation.Vertical;
            int itemCount = context.Items.Count;
            
            if (itemCount == 0) return;

            int padding = ScaleValue(10, context);
            int itemGap = ScaleValue(4, context);
            
            if (isVertical)
            {
                // Vertical layout (side navigation)
                int itemHeight = context.ItemHeight;
                int currentY = bounds.Top + padding;

                for (int i = 0; i < itemCount; i++)
                {
                    var item = context.Items[i];
                    var itemRect = new Rectangle(bounds.Left + padding, currentY, bounds.Width - padding * 2, itemHeight);
                    int index = i; // Capture for lambda
                    registerHitArea($"NavItem_{i}", itemRect, () => context.SelectItemByIndex(index));
                    currentY += itemHeight + itemGap;
                }
            }
            else
            {
                // Horizontal layout (top/bottom navigation)
                int itemWidth = context.ItemWidth;
                if (itemWidth <= 0)
                    itemWidth = (bounds.Width - padding * 2) / itemCount;
                int itemW = Math.Max(ScaleValue(80, context), itemWidth - ScaleValue(4, context));
                int itemH = bounds.Height - padding * 2;
                int currentX = bounds.Left + padding;

                for (int i = 0; i < itemCount; i++)
                {
                    var item = context.Items[i];
                    var itemRect = new Rectangle(currentX, bounds.Top + padding, itemW, itemH);
                    int index = i; // Capture for lambda
                    registerHitArea($"NavItem_{i}", itemRect, () => context.SelectItemByIndex(index));
                    currentX += itemWidth;
                }
            }
        }

        #region Helper Methods

        /// <summary>
        /// Helper method to draw all nav items with icons and text
        /// </summary>
        protected void DrawNavItems(Graphics g, INavBarPainterContext context, Rectangle bounds)
        {
            if (context.Items == null || context.Items.Count == 0) return;

            bool isVertical = context.Orientation == NavBarOrientation.Vertical;
            int itemCount = context.Items.Count;
            int padding = ScaleValue(10, context);
            int itemGap = ScaleValue(4, context);
            int hoveredIndex = context.HoveredItemIndex;
            var selectedItem = context.SelectedItem;

            if (isVertical)
            {
                // Vertical layout
                int itemHeight = context.ItemHeight;
                int currentY = bounds.Top + padding;

                for (int i = 0; i < itemCount; i++)
                {
                    var item = context.Items[i];
                    var itemRect = new Rectangle(bounds.Left + padding, currentY, bounds.Width - padding * 2, itemHeight);

                    // Draw hover effect if this item is hovered
                    if (i == hoveredIndex)
                    {
                        DrawHover(g, context, itemRect);
                    }

                    // Draw selection if this is the selected item
                    if (item == selectedItem)
                    {
                        DrawSelection(g, context, itemRect);
                    }

                    // Draw item content (icon and text)
                    DrawNavItem(g, context, item, itemRect, false);

                    currentY += itemHeight + itemGap;
                }
            }
            else
            {
                // Horizontal layout
                int itemWidth = context.ItemWidth;
                if (itemWidth <= 0)
                    itemWidth = (bounds.Width - padding * 2) / itemCount;
                int itemW = Math.Max(ScaleValue(80, context), itemWidth - ScaleValue(4, context));
                int itemH = bounds.Height - padding * 2;
                int currentX = bounds.Left + padding;

                for (int i = 0; i < itemCount; i++)
                {
                    var item = context.Items[i];
                    var itemRect = new Rectangle(currentX, bounds.Top + padding, itemW, itemH);

                    // Draw hover effect
                    if (i == hoveredIndex)
                    {
                        DrawHover(g, context, itemRect);
                    }

                    // Draw selection
                    if (item == selectedItem)
                    {
                        DrawSelection(g, context, itemRect);
                    }

                    // Draw item content
                    DrawNavItem(g, context, item, itemRect, true);

                    currentX += itemWidth;
                }
            }
        }

        /// <summary>
        /// Helper method to draw a single nav item (icon + text)
        /// </summary>
        protected virtual void DrawNavItem(Graphics g, INavBarPainterContext context, SimpleItem item, Rectangle itemRect, bool isHorizontal)
        {
            int padding = ScaleValue(10, context);
            int iconSize = ScaleValue(24, context);
            int textOffsetY = ScaleValue(8, context);
            int textOffsetX = ScaleValue(12, context);

            if (isHorizontal)
            {
                // Horizontal: icon on top, text on bottom (centered)
                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    var iconRect = new Rectangle(
                        itemRect.X + (itemRect.Width - iconSize) / 2,
                        itemRect.Y + ScaleValue(4, context),
                        iconSize, iconSize);
                    DrawNavItemIcon(g, context, item, iconRect);
                }

                if (!string.IsNullOrEmpty(item.Text))
                {
                    var textRect = new Rectangle(
                        itemRect.X + padding,
                        itemRect.Y + iconSize + textOffsetY,
                        itemRect.Width - padding * 2,
                        itemRect.Height - iconSize - textOffsetX);
                    DrawNavItemText(g, context, item, textRect, StringAlignment.Center);
                }
            }
            else
            {
                // Vertical: icon on left, text on right
                int x = itemRect.X + padding;

                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    var iconRect = new Rectangle(x, itemRect.Y + (itemRect.Height - iconSize) / 2, iconSize, iconSize);
                    DrawNavItemIcon(g, context, item, iconRect);
                    x += iconSize + textOffsetX;
                }

                if (!string.IsNullOrEmpty(item.Text))
                {
                    var textRect = new Rectangle(x, itemRect.Y, itemRect.Right - x - padding, itemRect.Height);
                    DrawNavItemText(g, context, item, textRect, StringAlignment.Near);
                }
            }
        }

        /// <summary>
        /// Helper method to draw nav item icon using ImagePainter with theme support
        /// REUSES shared ImagePainter instance - do NOT create new instances!
        /// </summary>
        protected virtual void DrawNavItemIcon(Graphics g, INavBarPainterContext context, SimpleItem item, Rectangle iconRect)
        {
            try
            {
                _sharedImagePainter.ImagePath = item.ImagePath;
                
                if (context.Theme != null)
                {
                    _sharedImagePainter.CurrentTheme = context.Theme;
                    _sharedImagePainter.ApplyThemeOnImage = true;
                }
                else
                {
                    _sharedImagePainter.ApplyThemeOnImage = false;
                }
                
                _sharedImagePainter.DrawImage(g, iconRect);
            }
            catch
            {
                // Fallback: draw placeholder
                using (var pen = new Pen(Color.Gray, 1f))
                {
                    g.DrawRectangle(pen, iconRect);
                }
            }
        }

        /// <summary>
        /// Helper method to draw nav item text with theme colors
        /// Uses TextRenderer for better text quality and clipping
        /// </summary>
        protected virtual void DrawNavItemText(Graphics g, INavBarPainterContext context, SimpleItem item, Rectangle textRect, StringAlignment alignment)
        {
            if (string.IsNullOrEmpty(item.Text) || textRect.Width <= 0 || textRect.Height <= 0)
                return;

            Color textColor;
            if (context.Theme != null && context.UseThemeColors)
            {
                textColor = item.IsEnabled ? 
                    context.Theme.ForeColor : 
                    context.Theme.DisabledForeColor;
            }
            else
            {
                textColor = item.IsEnabled ? Color.FromArgb(60, 60, 67) : Color.Gray;
            }

            var fontToUse = TextFont(context) ?? BeepFontManager.DefaultFont;
            bool needDispose = false;
            Font font = fontToUse;
            int scaled40 = ScaleValue(40, context);
            float desiredSize = textRect.Height >= scaled40 
                ? Math.Max(9f, fontToUse.Size) 
                : Math.Max(8f, fontToUse.Size - 1f);
            if (Math.Abs(fontToUse.Size - desiredSize) > 0.1f)
            {
                font = new Font(fontToUse.FontFamily, desiredSize, fontToUse.Style);
                needDispose = true;
            }
            try
            {
                var textFormat = System.Windows.Forms.TextFormatFlags.EndEllipsis | 
                                System.Windows.Forms.TextFormatFlags.VerticalCenter | 
                                System.Windows.Forms.TextFormatFlags.NoPrefix;
                if (alignment == StringAlignment.Center)
                    textFormat |= System.Windows.Forms.TextFormatFlags.HorizontalCenter;
                else if (alignment == StringAlignment.Near)
                    textFormat |= System.Windows.Forms.TextFormatFlags.Left;
                else
                    textFormat |= System.Windows.Forms.TextFormatFlags.Right;
                System.Windows.Forms.TextRenderer.DrawText(g, item.Text, font, textRect, textColor, textFormat);
            }
            finally
            {
                if (needDispose) font?.Dispose();
            }
        }

        /// <summary>
        /// Overload for custom font and size
        /// </summary>
        protected virtual void DrawNavItemText(Graphics g, INavBarPainterContext context, SimpleItem item, Rectangle textRect, 
            StringAlignment alignment, string fontFamily, float fontSize, FontStyle fontStyle = FontStyle.Regular)
        {
            if (string.IsNullOrEmpty(item.Text) || textRect.Width <= 0 || textRect.Height <= 0)
                return;

            Color textColor;
            if (context.Theme != null && context.UseThemeColors)
            {
                textColor = item.IsEnabled ? 
                    context.Theme.ForeColor : 
                    context.Theme.DisabledForeColor;
            }
            else
            {
                textColor = item.IsEnabled ? Color.FromArgb(60, 60, 67) : Color.Gray;
            }

            using (var font = new Font(fontFamily, fontSize, fontStyle))
            {
                var textFormat = System.Windows.Forms.TextFormatFlags.EndEllipsis | 
                                System.Windows.Forms.TextFormatFlags.VerticalCenter | 
                                System.Windows.Forms.TextFormatFlags.NoPrefix;
                
                if (alignment == StringAlignment.Center)
                    textFormat |= System.Windows.Forms.TextFormatFlags.HorizontalCenter;
                else if (alignment == StringAlignment.Near)
                    textFormat |= System.Windows.Forms.TextFormatFlags.Left;
                else
                    textFormat |= System.Windows.Forms.TextFormatFlags.Right;

                System.Windows.Forms.TextRenderer.DrawText(g, item.Text, font, textRect, textColor, textFormat);
            }
        }

        /// <summary>
        /// Calculate minimum required size for a nav item based on text and icon
        /// Helps prevent text clipping by ensuring adequate space
        /// </summary>
        protected static Size CalculateMinimumItemSize(Graphics g, SimpleItem item, bool isHorizontal, 
            string fontFamily = "Segoe UI", float fontSize = 9f, int iconSize = 24, int padding = 12)
        {
            if (item == null)
                return new Size(80, 48); // Default minimum

            bool hasIcon = !string.IsNullOrEmpty(item.ImagePath);
            bool hasText = !string.IsNullOrEmpty(item.Text);

            if (!hasIcon && !hasText)
                return new Size(80, 48); // Default minimum

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

            if (isHorizontal)
            {
                // Horizontal: icon above text, stacked vertically
                int width, height;

                if (hasIcon && hasText)
                {
                    // Both icon and text
                    width = Math.Max(iconSize, textSize.Width) + padding * 2;
                    height = iconSize + textSize.Height + padding * 3; // icon + spacing + text + padding
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

                return new Size(Math.Max(width, 80), Math.Max(height, 48));
            }
            else
            {
                // Vertical: icon left, text right, side by side
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

                return new Size(Math.Max(width, 80), Math.Max(height, 48));
            }
        }

        protected static void FillRoundedRect(Graphics g, Rectangle rect, int radius, Color color)
        {
            using var path = CreateRoundedPath(rect, radius);
            using var br = new SolidBrush(color);
            g.FillPath(br, path);
        }

        protected static void StrokeRoundedRect(Graphics g, Rectangle rect, int radius, Color color, float width = 1f)
        {
            using var path = CreateRoundedPath(rect, radius);
            using var pen = new Pen(color, width) { Alignment = PenAlignment.Inset };
            g.DrawPath(pen, path);
        }

        protected static GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int d = Math.Max(0, Math.Min(radius * 2, Math.Min(rect.Width, rect.Height)));
            if (d <= 1) { path.AddRectangle(rect); return path; }
            var arc = new Rectangle(rect.X, rect.Y, d, d);
            path.AddArc(arc, 180, 90);
            arc.X = rect.Right - d; path.AddArc(arc, 270, 90);
            arc.Y = rect.Bottom - d; path.AddArc(arc, 0, 90);
            arc.X = rect.Left; path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }

        #endregion
    }
}

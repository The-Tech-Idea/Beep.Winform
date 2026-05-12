using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Tokens;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Simple clean category list with left selection indicator and distinct styling
    /// </summary>
    internal class SimpleListPainter : MinimalListBoxPainter
    {
        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            if (g == null || itemRect.IsEmpty || item == null) return;

            // Clear and prepare item background first
            DrawItemBackgroundEx(g, itemRect, item, isHovered, isSelected);

            // Calculate layout
            var padding = GetPreferredPadding();
            var contentRect = Rectangle.Inflate(itemRect, -padding.Left, -padding.Top);

            // Draw text
            Color textColor = isSelected ? Color.White : (_theme?.ListItemForeColor ?? Color.Black);
            DrawItemText(g, contentRect, item.Text, textColor, _owner.TextFont);

            // Draw selection indicator on left
            if (isSelected)
            {
                using (var brush = new SolidBrush(_theme?.PrimaryColor ?? Color.DodgerBlue))
                {
                    Rectangle indicator = new Rectangle(itemRect.Left, itemRect.Top, Scale(4), itemRect.Height);
                    g.FillRectangle(brush, indicator);
                }
            }
        }
        
        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            if (g == null || itemRect.IsEmpty) return;

            using (var path = GraphicsExtensions.CreateRoundedRectanglePath(itemRect, Scale(ListBoxTokens.CornerRadiusSmall)))
            {
                if (isSelected)
                {
                    var selColor = _theme?.PrimaryColor ?? Color.DodgerBlue;
                    
                    // Subtle background fill
                    using (var brush = new SolidBrush(Color.FromArgb(15, selColor.R, selColor.G, selColor.B)))
                    {
                        g.FillPath(brush, path);
                    }

                    // Selection border
                    using (var pen = new Pen(selColor, 2f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
                else if (isHovered)
                {
                    // Hover effect with subtle gradient
                    var hoverBase = _theme?.ListItemHoverBackColor ?? _theme?.BackgroundColor ?? Color.White;
                    using (var hoverBrush = new LinearGradientBrush(itemRect,
                        Color.FromArgb(ListBoxTokens.HoverOverlayAlpha, hoverBase),
                        Color.Transparent,
                        LinearGradientMode.Vertical))
                    {
                        g.FillPath(hoverBrush, path);
                    }

                    using (var pen = new Pen(_theme?.AccentColor ?? Color.LightGray, 1f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
                else
                {
                    // Normal state - subtle border only
                    using (var brush = new SolidBrush(_theme?.BackgroundColor ?? Color.White))
                    {
                        g.FillPath(brush, path);
                    }

                    using (var pen = new Pen(_theme?.BorderColor ?? Color.FromArgb(230, 230, 230), 1f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
        }
    }
}

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
                Rectangle indicator = new Rectangle(itemRect.Left, itemRect.Top, Scale(4), itemRect.Height);
                g.FillRectangle(GetBrush(_theme?.PrimaryColor ?? Color.DodgerBlue), indicator);
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
                    g.FillPath(GetBrush(Color.FromArgb(15, selColor.R, selColor.G, selColor.B)), path);

                    // Selection border
                    g.DrawPath(GetPen(selColor, 2f), path);
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

                    g.DrawPath(GetPen(_theme?.AccentColor ?? Color.LightGray, 1f), path);
                }
                else
                {
                    // Normal state - subtle border only
                    g.FillPath(GetBrush(_theme?.BackgroundColor ?? Color.White), path);

                    g.DrawPath(GetPen(_theme?.BorderColor ?? Color.FromArgb(230, 230, 230), 1f), path);
                }
            }
        }
    }
}

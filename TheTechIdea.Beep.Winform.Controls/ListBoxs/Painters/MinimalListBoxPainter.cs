using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Tokens;
using TheTechIdea.Beep.Winform.Controls.Styling.PathPainters;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Minimal list box painter - subtle styling with understated borders and backgrounds
    /// </summary>
    internal class MinimalListBoxPainter : StandardListBoxPainter
    {
        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            if (g == null || itemRect.IsEmpty) return;

            // Minimal: flat background with subtle hover
            if (isSelected)
            {
                // Subtle selection highlight
                g.FillRectangle(GetBrush(PathPainterHelpers.WithAlphaIfNotEmpty(_theme?.PrimaryColor ?? Color.Empty, 25)), itemRect);

                // Very subtle selection indicator on left
                g.FillRectangle(GetBrush(_theme?.PrimaryColor ?? Color.Empty), itemRect.Left, itemRect.Top, Scale(2), itemRect.Height);
            }
            else if (isHovered)
            {
                // Very subtle hover background
                g.FillRectangle(GetBrush(Color.FromArgb(8, Color.Black)), itemRect);
            }
            // No background for normal state - minimal approach
        }
        
        public override int GetPreferredItemHeight()
        {
            return Scale(ListBoxTokens.ItemHeightDense); // 28 logical px
        }
    }
}

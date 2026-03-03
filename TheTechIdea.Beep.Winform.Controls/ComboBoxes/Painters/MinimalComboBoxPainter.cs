using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.PathPainters;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters
{
    /// <summary>
    /// Minimal combo box painter - Style #1
    /// Simple rectangular dropdown with minimal border
    /// </summary>
    internal class MinimalComboBoxPainter : BaseComboBoxPainter
    {
     
        
        protected override void DrawBorder(Graphics g, Rectangle rect)
        {
            // Very subtle border for minimal Style
            Color borderColor = PathPainterHelpers.WithAlphaIfNotEmpty(_theme?.BorderColor ?? Color.Empty, 200);
            var pen = PaintersFactory.GetPen(borderColor,1f);
            g.DrawRectangle(pen, rect.X, rect.Y, rect.Width -1, rect.Height -1);
        }
        
        protected override void DrawDropdownButton(Graphics g, Rectangle buttonRect)
        {
            if (buttonRect.IsEmpty) return;

            // Subtle hover fill for interactive feedback (no separator for minimal style)
            if (_owner.IsButtonHovered && _owner.Enabled)
            {
                Color hoverFill = PathPainterHelpers.WithAlphaIfNotEmpty(
                    _theme?.ComboBoxHoverBackColor != Color.Empty
                        ? _theme.ComboBoxHoverBackColor
                        : (_theme?.PrimaryColor ?? Color.Empty), 50);
                if (hoverFill != Color.Empty && hoverFill.A > 0)
                {
                    using (var path = GetRoundedRectPath(Rectangle.Inflate(buttonRect, -ScaleX(1), -ScaleY(1)), ScaleX(4)))
                        g.FillPath(PaintersFactory.GetSolidBrush(hoverFill), path);
                }
            }

            DrawDropdownArrow(g, buttonRect, GetArrowColor(), _owner.IsDropdownOpen);
        }
        
        public override Padding GetPreferredPadding()
        {
            return new System.Windows.Forms.Padding(8,4,8,4); // Slightly more padding for minimal Style
        }
    }
}

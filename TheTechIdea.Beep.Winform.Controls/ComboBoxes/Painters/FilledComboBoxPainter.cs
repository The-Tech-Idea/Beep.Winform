using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.PathPainters;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters
{
    /// <summary>
    /// Filled combo box painter - Style #5
    /// Filled background with subtle shadow elevation (Material Design filled variant)
    /// </summary>
    internal class FilledComboBoxPainter : BaseComboBoxPainter
    {
        private const int BorderRadius = 4;
        private const int TextYOffsetLogical = 1;
        private const int ChevronYOffsetLogical = -1;
        
     
        
        protected override void DrawBorder(Graphics g, Rectangle rect)
        {
            // Filled Style has bottom border only (Material Design pattern)
            Color borderColor = _owner.Focused 
                ? _theme?.PrimaryColor ?? Color.Blue
                : (_owner.HasError ? Color.Red : (_theme?.BorderColor ?? Color.Gray));
            
            float borderWidth = _owner.Focused ? 2f : 1f;
            
            var pen = PaintersFactory.GetPen(borderColor, borderWidth);
            // Draw bottom border only
            g.DrawLine(pen, rect.Left, rect.Bottom - 1, rect.Right, rect.Bottom - 1);
        }
        
        protected override void DrawBackground(Graphics g, Rectangle rect)
        {
            // Solid combobox background first
            base.DrawBackground(g, rect);
            // Material "Filled" variant: add subtle tint over the solid background
            Color bgBase = _theme?.ComboBoxBackColor ?? _theme?.BackColor ?? SystemColors.Window;
            if (bgBase == Color.Empty) bgBase = SystemColors.Window;
            Color fillColor = Color.FromArgb(_owner.Focused ? 22 : 14,
                bgBase.R, bgBase.G, bgBase.B);
            using (var path = GetFilledShapePath(rect, BorderRadius))
            {
                var brush = PaintersFactory.GetSolidBrush(fillColor);
                g.FillPath(brush, path);
            }
        }
        
        protected override void DrawDropdownButton(Graphics g, Rectangle buttonRect)
        {
            if (buttonRect.IsEmpty) return;

            // Subtle hover fill (no separator for filled style)
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

            // Filled style: no separator + slightly upward chevron to compensate
            // for the strong bottom-border visual anchor.
            Color arrowColor = GetArrowColor();
            int iconSize = Math.Min(ScaleX(15), Math.Min(buttonRect.Width, buttonRect.Height) - ScaleX(8));
            if (iconSize > 4)
            {
                int x = buttonRect.X + (buttonRect.Width - iconSize) / 2;
                int y = buttonRect.Y + (buttonRect.Height - iconSize) / 2 + ScaleY(ChevronYOffsetLogical);
                var iconRect = new Rectangle(x, y, iconSize, iconSize);
                DrawSvgIcon(g, iconRect, SvgsUI.ChevronDown, arrowColor, rotationDeg: _owner?.ChevronAngle ?? 0f);
                return;
            }

            DrawDropdownArrow(g, buttonRect, arrowColor, _owner.IsDropdownOpen);
        }

        protected override void DrawText(Graphics g, Rectangle textAreaRect)
        {
            if (textAreaRect.IsEmpty)
            {
                return;
            }

            // Filled variant benefits from a slight downward text nudge so the label
            // sits optically centered against the filled background slab.
            var adjustedRect = new Rectangle(
                textAreaRect.X,
                textAreaRect.Y + ScaleY(TextYOffsetLogical),
                textAreaRect.Width,
                Math.Max(1, textAreaRect.Height - ScaleY(TextYOffsetLogical)));

            base.DrawText(g, adjustedRect);
        }
        
        /// <summary>
        /// Rounded-top / flat-bottom path for the Material Filled variant.
        /// Kept private because it differs from the standard full-rounded path
        /// in <see cref="BaseComboBoxPainter.GetRoundedRectPath"/>.
        /// </summary>
        private GraphicsPath GetFilledShapePath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;
            
            // Top corners rounded, bottom corners sharp for filled Style
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter - 1, rect.Y, diameter, diameter, 270, 90);
            path.AddLine(rect.Right - 1, rect.Y + radius, rect.Right - 1, rect.Bottom - 1);
            path.AddLine(rect.Right - 1, rect.Bottom - 1, rect.X, rect.Bottom - 1);
            path.AddLine(rect.X, rect.Bottom - 1, rect.X, rect.Y + radius);
            path.CloseFigure();
            
            return path;
        }
        
        public override System.Windows.Forms.Padding GetPreferredPadding()
        {
            return new System.Windows.Forms.Padding(12, 8, 8, 8);
        }

        protected override bool ShowButtonSeparator => false;
    }
}

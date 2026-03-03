using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.PathPainters;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters
{
    /// <summary>
    /// Material Design Outlined combo box painter - Style #4
    /// Material Design outlined variant with floating label support
    /// </summary>
    internal class MaterialOutlinedComboBoxPainter : BaseComboBoxPainter
    {
        private const int BorderRadius = 4;
        private const int TextYOffsetLogical = 1;
        private const int ChevronYOffsetLogical = -1;
        
       
        
        protected override void DrawBorder(Graphics g, Rectangle rect)
        {
            Color borderColor = _owner.Focused 
                ? _theme?.PrimaryColor ?? Color.Empty
                : (_owner.HasError ? Color.Red : PathPainterHelpers.WithAlphaIfNotEmpty(_theme?.BorderColor ?? Color.Empty, 180));
            
            float borderWidth = _owner.Focused ? 2f : 1f;
            
            var pen = PaintersFactory.GetPen(borderColor, borderWidth);
            using (var path = GetRoundedRectPath(rect, BorderRadius))
            {
                // Pen alignment requires a clone if modification needed; here we don't modify
                g.DrawPath(pen, path);
            }
            
            // Draw floating label if present and focused or has value
            if (!string.IsNullOrEmpty(_owner.LabelText))
            {
                DrawFloatingLabel(g, rect, borderColor);
            }
        }
        
        protected override void DrawDropdownButton(Graphics g, Rectangle buttonRect)
        {
            if (buttonRect.IsEmpty) return;

            // Material outlined: no separator + slight upward chevron alignment to
            // match the floating-label visual weight.
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

            // Material outlined has a floating label; nudging text down improves
            // optical centering between label notch and bottom border.
            var adjustedRect = new Rectangle(
                textAreaRect.X,
                textAreaRect.Y + ScaleY(TextYOffsetLogical),
                textAreaRect.Width,
                Math.Max(1, textAreaRect.Height - ScaleY(TextYOffsetLogical)));

            base.DrawText(g, adjustedRect);
        }

        protected override void DrawDecorations(Graphics g, Rectangle drawingRect)
        {
            if (!string.IsNullOrEmpty(_owner.LabelText))
            {
                Color labelColor = _owner.Focused
                    ? (_theme?.PrimaryColor ?? Color.Empty)
                    : (_owner.HasError
                        ? Color.Red
                        : PathPainterHelpers.WithAlphaIfNotEmpty(_theme?.BorderColor ?? Color.Empty, 180));
                DrawFloatingLabel(g, drawingRect, labelColor);
            }
        }
        
        private void DrawFloatingLabel(Graphics g, Rectangle rect, Color labelColor)
        {
            // Position label at top-left with gap in border
            Font labelFont;
            // _theme.LabelFont is a TypographyStyle in this project
            if (_theme?.LabelFont != null)
            {
                var tf = _theme.LabelFont; // TypographyStyle
                var family = string.IsNullOrEmpty(tf.FontFamily) ? "Segoe UI" : tf.FontFamily;
                var size = tf.FontSize > 0 ? tf.FontSize : 8f;
                var style = tf.FontStyle;
                labelFont = PaintersFactory.GetFont(family, size, style);
            }
            else
            {
                labelFont = PaintersFactory.GetFont("Segoe UI", 8f, FontStyle.Regular);
            }
            
            string labelText = _owner.LabelText;
            SizeF labelSizeF = TextUtils.MeasureText(labelText, labelFont, int.MaxValue);
            var labelSize = new Size((int)labelSizeF.Width, (int)labelSizeF.Height);
            
            // Position label
            int labelX = rect.X + 12;
            int labelY = rect.Y - (labelSize.Height / 2);
            
            // Clear background behind label — use theme colour directly,
            // NOT _helper.GetBackgroundColor() which reads the wrong BackColor.
            Color bgColor = _theme?.ComboBoxBackColor ?? _theme?.BackColor ?? SystemColors.Window;
            if (bgColor == Color.Empty) bgColor = SystemColors.Window;
            var bgBrush = PaintersFactory.GetSolidBrush(bgColor);
            var clearRect = new Rectangle(labelX - 4, labelY, labelSize.Width + 8, labelSize.Height);
            g.FillRectangle(bgBrush, clearRect);
            
            // Draw label text
            TextRenderer.DrawText(g, labelText, labelFont, 
                new Point(labelX, labelY), labelColor, bgColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
        }
        
        public override Padding GetPreferredPadding()
        {
            return new Padding(12, 12, 8, 12); // More top padding for floating label
        }

        protected override bool ShowButtonSeparator => false;
    }
}

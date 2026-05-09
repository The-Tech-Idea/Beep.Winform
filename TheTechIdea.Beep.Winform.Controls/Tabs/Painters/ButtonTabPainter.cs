using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Tabs.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Painters
{
    public class ButtonTabPainter : BaseTabPainter
    {
        public ButtonTabPainter(BeepTabs tabControl) : base(tabControl) { }

        public override void PaintTab(Graphics g, RectangleF tabRect, int index, bool isSelected, bool isHovered, float alpha = 1.0f)
        {
            // Shrink rect slightly to create spacing between buttons
            RectangleF buttonRect = new RectangleF(tabRect.X + 2, tabRect.Y + 2, tabRect.Width - 4, tabRect.Height - 4);

            Color fillColor = isSelected ? (Theme?.PrimaryColor ?? SystemColors.Highlight) : (Theme?.ButtonBackColor ?? SystemColors.Control);
            Color foreColor = isSelected ? GetContrastColor(fillColor) : (Theme?.ButtonForeColor ?? SystemColors.ControlText);
            
            if (isHovered && !isSelected)
            {
                fillColor = Theme?.ButtonHoverBackColor ?? SystemColors.ButtonHighlight;
            }

            using (GraphicsPath path = GetRoundedRect(buttonRect, 4))
            {
                using (var brush = PaintersFactory.GetSolidBrush(fillColor))
                {
                    g.FillPath(brush, path);
                }
                
                // Optional border
                if (!isSelected)
                {
                    using (var pen = PaintersFactory.GetPen(Theme?.BorderColor ?? SystemColors.ButtonShadow))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }

            bool vertical = (TabControl.HeaderPosition == TabHeaderPosition.Left || TabControl.HeaderPosition == TabHeaderPosition.Right);
            
            if (TabControl.ShouldShowTabText(index))
            {
                // Custom text drawing to handle specific button colors
                using (Font font = TabFontHelpers.CreateDerivedSafeFont(TextFont ?? TabControl.Font, isSelected ? FontStyle.Bold : FontStyle.Regular, TabControl))
                {
                    var textBrush = PaintersFactory.GetSolidBrush(foreColor);
                    string text = TabControl.GetTabTitle(index);
                    
                    if (!vertical)
                    {
                        SizeF textSize = TextUtils.MeasureText(g, text, font);
                        PointF textPoint = new PointF(buttonRect.X + GetScaledTextPadding(), buttonRect.Y + (buttonRect.Height - textSize.Height) / 2);
                        g.DrawString(text, font, textBrush, textPoint);
                    }
                    else
                    {
                        GraphicsState state = g.Save();
                        g.TranslateTransform(buttonRect.X + buttonRect.Width / 2, buttonRect.Y + buttonRect.Height / 2);
                        g.RotateTransform(90);
                        SizeF textSize = TextUtils.MeasureText(g, text, font);
                        PointF textPoint = new PointF(-textSize.Width / 2, -textSize.Height / 2);
                        g.DrawString(text, font, textBrush, textPoint);
                        g.Restore(state);
                    }
                }
            }

            if (TabControl.ShowCloseButtons)
            {
                DrawCloseButton(g, buttonRect, vertical);
            }
        }

        private static Color GetContrastColor(Color background)
        {
            float luminance = (0.299f * background.R + 0.587f * background.G + 0.114f * background.B) / 255f;
            return luminance > 0.5f ? Color.FromArgb(28, 27, 31) : Color.White;
        }

        public override void PaintTabItem(Graphics g, Tabs.Models.BeepTabHeaderItemLayout itemLayout, float alpha = 1.0f)
        {
            RectangleF buttonRect = new RectangleF(itemLayout.Bounds.X + 2, itemLayout.Bounds.Y + 2, itemLayout.Bounds.Width - 4, itemLayout.Bounds.Height - 4);
            Color fillColor = itemLayout.Item.IsSelected ? (Theme?.PrimaryColor ?? SystemColors.Highlight) : (Theme?.ButtonBackColor ?? SystemColors.Control);
            Color foreColor = itemLayout.Item.IsSelected ? GetContrastColor(fillColor) : (Theme?.ButtonForeColor ?? SystemColors.ControlText);

            if (itemLayout.Item.IsHovered && !itemLayout.Item.IsSelected)
            {
                fillColor = Theme?.ButtonHoverBackColor ?? SystemColors.ButtonHighlight;
            }

            using (GraphicsPath path = GetRoundedRect(buttonRect, 4))
            {
                using (var brush = PaintersFactory.GetSolidBrush(fillColor))
                {
                    g.FillPath(brush, path);
                }

                if (!itemLayout.Item.IsSelected)
                {
                    using var pen = PaintersFactory.GetPen(Theme?.BorderColor ?? SystemColors.ButtonShadow);
                    g.DrawPath(pen, path);
                }
            }

            var contentLayout = new Tabs.Models.BeepTabHeaderItemLayout
            {
                Item = itemLayout.Item,
                Bounds = Rectangle.Ceiling(buttonRect),
                HasCloseButton = itemLayout.HasCloseButton
            };

            DrawTabItemContent(g, contentLayout, alpha, foreColor);
        }
    }
}

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Tabs.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Painters
{
    public class ButtonTabPainter : BaseTabPainter
    {
        /// <summary>Gap between the tab slot and the button drawn inside it (design pixels).</summary>
        private const int ButtonInset = 2;

        public ButtonTabPainter(BeepTabs tabControl) : base(tabControl) { }

        /// <summary>
        /// Adds this style's inset to the measured width.
        /// </summary>
        /// <remarks>
        /// The button is drawn inside the tab slot with an inset on each side, and the caption is
        /// then laid out inside the *button*, not the slot — so it had four fewer pixels than
        /// <see cref="BaseTabPainter.MeasureTab"/> had reserved, and captions clipped on this style
        /// alone. That is the measure/draw divergence again, in geometry rather than in fonts:
        /// whatever a painter subtracts before drawing content, it has to add when measuring.
        /// </remarks>
        public override SizeF MeasureTab(Graphics g, int index, Font font)
        {
            SizeF size = base.MeasureTab(g, index, font);
            if (size.IsEmpty) return size;

            int inset = Scale(ButtonInset) * 2;
            return new SizeF(size.Width + inset, size.Height + inset);
        }

        private static Color GetContrastColor(Color background)
        {
            float luminance = (0.299f * background.R + 0.587f * background.G + 0.114f * background.B) / 255f;
            return luminance > 0.5f ? Color.FromArgb(28, 27, 31) : Color.White;
        }

        public override void PaintTabItem(Graphics g, Tabs.Models.BeepTabHeaderItemLayout itemLayout, float alpha = 1.0f)
        {
            int inset = Scale(ButtonInset);
            RectangleF buttonRect = RectangleF.Inflate(itemLayout.Bounds, -inset, -inset);
            if (buttonRect.Width <= 0 || buttonRect.Height <= 0) return;
            // Resolved through the colour seam: reading Theme.ButtonBackColor directly filled
            // *unselected* buttons with the primary colour under MaterialDesignTheme.
            Color fillColor = TheTechIdea.Beep.Winform.Controls.Tabs.Helpers.TabThemeHelpers
                .GetTabBackgroundColor(Theme, itemLayout.Item.IsSelected, itemLayout.Item.IsHovered);
            Color foreColor = GetContrastColor(fillColor);

            using (GraphicsPath path = GetRoundedRect(buttonRect, Scale(4)))
            {
                var brush = PaintersFactory.GetSolidBrush(fillColor);
                g.FillPath(brush, path);

                if (!itemLayout.Item.IsSelected)
                {
                    var pen = PaintersFactory.GetPen(TheTechIdea.Beep.Winform.Controls.Tabs.Helpers
                        .TabThemeHelpers.GetTabBorderColor(Theme, false, itemLayout.Item.IsHovered));
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

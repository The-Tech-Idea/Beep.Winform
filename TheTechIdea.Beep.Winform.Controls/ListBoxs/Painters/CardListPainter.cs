using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Card-Style list with elevated items (taller than standard filled)
    /// </summary>
    internal class CardListPainter : FilledListBoxPainter
    {
        public override int GetPreferredItemHeight()
        {
            return 60; // Taller for card appearance
        }

        public override System.Windows.Forms.Padding GetPreferredPadding()
        {
            return new System.Windows.Forms.Padding(16, 8, 16, 8);
        }

        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            if (g == null || itemRect.IsEmpty) return;

            // Add shadow for card appearance
            var shadowRect = Rectangle.Inflate(itemRect, 6, 6);
            using (var shadowBrush = new LinearGradientBrush(shadowRect, Color.FromArgb(30, Color.Black), Color.Transparent, 90f))
            {
                g.FillRectangle(shadowBrush, shadowRect);
            }

            // Draw card background with rounded corners
            using (var path = GraphicsExtensions.CreateRoundedRectanglePath(itemRect, 8))
            {
                Color backgroundColor = Color.White;

                if (isSelected)
                {
                    backgroundColor = BeepStyling.CurrentTheme?.PrimaryColor ?? Color.LightBlue;
                }
                else if (isHovered)
                {
                    backgroundColor = Color.FromArgb(220, 220, 220);
                }

                using (var brush = new SolidBrush(backgroundColor))
                {
                    g.FillPath(brush, path);
                }

                // Draw border for selected cards
                if (isSelected)
                {
                    using (var pen = new Pen(BeepStyling.CurrentTheme?.AccentColor ?? Color.Blue, 3f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
        }

        // Renamed DrawItemContent to DrawItem to match the base class method
        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            base.DrawItem(g, itemRect, item, isHovered, isSelected);

            // Render icon/image using StyledImagePainter
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                var imageBounds = new Rectangle(itemRect.X + 8, itemRect.Y + 8, 44, 44);
                StyledImagePainter.Paint(g, imageBounds, item.ImagePath);
            }
        }
    }
}

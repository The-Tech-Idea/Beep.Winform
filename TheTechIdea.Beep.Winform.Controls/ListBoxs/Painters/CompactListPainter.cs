using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Compact list with smaller items for high-density display
    /// </summary>
    internal class CompactListPainter : MinimalListBoxPainter
    {
        public override int GetPreferredItemHeight()
        {
            return 24; // Smaller height for compact display
        }
        
        public override System.Windows.Forms.Padding GetPreferredPadding()
        {
            return new System.Windows.Forms.Padding(6, 2, 6, 2);
        }

        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            if (g == null || itemRect.IsEmpty) return;

            Color backgroundColor = Color.White;

            if (isSelected)
            {
                backgroundColor = Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.PrimaryColor ?? Color.LightBlue;
            }
            else if (isHovered)
            {
                backgroundColor = Color.FromArgb(240, 240, 240);
            }

            using (var brush = new SolidBrush(backgroundColor))
            {
                g.FillRectangle(brush, itemRect);
            }

            if (isHovered || isSelected)
            {
                using (var pen = new Pen(Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.AccentColor ?? Color.Gray, 1.5f))
                {
                    g.DrawRectangle(pen, itemRect.X, itemRect.Y, itemRect.Width - 1, itemRect.Height - 1);
                }
            }
        }

        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            DrawItemBackground(g, itemRect, isHovered, isSelected);

            var padding = GetPreferredPadding();
            var textRect = new Rectangle(
                itemRect.X + padding.Left,
                itemRect.Y + padding.Top,
                itemRect.Width - padding.Left - padding.Right,
                itemRect.Height - padding.Top - padding.Bottom
            );

            Color textColor = isSelected
                ? Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.OnPrimaryColor ?? Color.White
                : Color.FromArgb(40, 40, 40);

            using (var font = new Font(_owner.TextFont.FontFamily, _owner.TextFont.Size, FontStyle.Regular))
            {
                System.Windows.Forms.TextRenderer.DrawText(g, item.Text, font, textRect, textColor,
                    System.Windows.Forms.TextFormatFlags.Left | System.Windows.Forms.TextFormatFlags.VerticalCenter);
            }
        }
    }
}

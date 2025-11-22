using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Compact list with smaller items for high-density display with distinct styling
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

            if (isSelected)
            {
                var selColor = Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.PrimaryColor ?? Color.LightBlue;
                
                // Filled background for selected
                using (var brush = new SolidBrush(Color.FromArgb(40, selColor.R, selColor.G, selColor.B)))
                {
                    g.FillRectangle(brush, itemRect);
                }

                // Selection border on left side (compact style)
                using (var brush = new SolidBrush(selColor))
                {
                    g.FillRectangle(brush, itemRect.Left, itemRect.Top, 3, itemRect.Height);
                }

                // Right subtle border
                using (var pen = new Pen(selColor, 1f))
                {
                    g.DrawLine(pen, itemRect.Left + 1, itemRect.Top, itemRect.Left + 1, itemRect.Bottom);
                }
            }
            else if (isHovered)
            {
                using (var brush = new SolidBrush(Color.FromArgb(245, 245, 245)))
                {
                    g.FillRectangle(brush, itemRect);
                }

                using (var pen = new Pen(Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.AccentColor ?? Color.Gray, 0.5f))
                {
                    g.DrawRectangle(pen, itemRect.X, itemRect.Y, itemRect.Width - 1, itemRect.Height - 1);
                }
            }
            else
            {
                // Normal compact style - minimal background
                using (var brush = new SolidBrush(Color.White))
                {
                    g.FillRectangle(brush, itemRect);
                }
            }
        }
    }
}

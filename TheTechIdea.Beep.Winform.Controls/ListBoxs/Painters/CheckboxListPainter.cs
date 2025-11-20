using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// List with checkboxes for multi-select (from images 1, 3)
    /// </summary>
    internal class CheckboxListPainter : OutlinedListBoxPainter
    {
        public override bool SupportsCheckboxes() => true;
        
        public override System.Windows.Forms.Padding GetPreferredPadding()
        {
            return new System.Windows.Forms.Padding(12, 6, 12, 6);
        }

        public override int GetPreferredItemHeight()
        {
            // Slightly taller for better checkbox targeting
            return 32;
        }

        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            if (g == null || item == null || itemRect.IsEmpty) return;

            // Draw item background
            DrawItemBackgroundEx(g, itemRect, item, isHovered, isSelected);

            // Calculate checkbox rectangle
            var padding = GetPreferredPadding();
            var checkboxRect = new Rectangle(
                itemRect.X + padding.Left,
                itemRect.Y + (itemRect.Height - 16) / 2,
                16,
                16
            );

            // Draw checkbox
            DrawCheckbox(g, checkboxRect, item.IsChecked, isHovered);

            // Draw text next to checkbox
            var textRect = new Rectangle(
                checkboxRect.Right + 8,
                itemRect.Y,
                itemRect.Width - checkboxRect.Right - 8 - padding.Right,
                itemRect.Height
            );

            Color textColor = isSelected
                ? Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.PrimaryColor ?? Color.Black
                : Color.FromArgb(26, 32, 44);

            DrawItemText(g, textRect, item.Text, textColor, _owner.Font);
        }

        private void DrawCheckbox(Graphics g, Rectangle checkboxRect, bool isChecked, bool isHovered)
        {
            using (var path = CreateRoundedRectangle(checkboxRect, 3))
            {
                Color borderColor = isHovered
                    ? Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.AccentColor ?? Color.Gray
                    : Color.FromArgb(200, 200, 200);

                using (var borderPen = new Pen(borderColor, 1.5f))
                {
                    g.DrawPath(borderPen, path);
                }

                if (isChecked)
                {
                    Color fillColor = Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.PrimaryColor ?? Color.FromArgb(33, 150, 243);
                    using (var fillBrush = new SolidBrush(fillColor))
                    {
                        g.FillPath(fillBrush, path);
                    }

                    using (var checkPen = new Pen(Color.White, 2))
                    {
                        checkPen.StartCap = LineCap.Round;
                        checkPen.EndCap = LineCap.Round;

                        var centerX = checkboxRect.X + checkboxRect.Width / 2;
                        var centerY = checkboxRect.Y + checkboxRect.Height / 2;
                        var size = 4;

                        g.DrawLine(checkPen,
                            centerX - size, centerY,
                            centerX - 1, centerY + size);
                        g.DrawLine(checkPen,
                            centerX - 1, centerY + size,
                            centerX + size + 1, centerY - size);
                    }
                }

                if (isHovered)
                {
                    using (var hoverBrush = new SolidBrush(Color.FromArgb(30, borderColor)))
                    {
                        g.FillPath(hoverBrush, path);
                    }
                }
            }
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;

            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }
    }
}

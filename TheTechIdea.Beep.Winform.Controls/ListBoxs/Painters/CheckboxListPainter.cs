using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// List with checkboxes for multi-select with distinct styling
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
                ? Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.OnPrimaryColor ?? Color.White
                : Color.FromArgb(26, 32, 44);

            DrawItemText(g, textRect, item.Text, textColor, _owner.Font);
        }

        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            if (g == null || itemRect.IsEmpty) return;

            using (var path = GraphicsExtensions.CreateRoundedRectanglePath(itemRect, 3))
            {
                if (isSelected)
                {
                    var selColor = Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.PrimaryColor ?? Color.LightBlue;

                    // Selected background with subtle gradient
                    using (var brush = new LinearGradientBrush(itemRect,
                        Color.FromArgb(30, selColor.R, selColor.G, selColor.B),
                        Color.FromArgb(10, selColor.R, selColor.G, selColor.B),
                        LinearGradientMode.Vertical))
                    {
                        g.FillPath(brush, path);
                    }

                    // Selection border
                    using (var pen = new Pen(selColor, 2f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
                else if (isHovered)
                {
                    // Hover background
                    using (var brush = new SolidBrush(Color.FromArgb(248, 248, 248)))
                    {
                        g.FillPath(brush, path);
                    }

                    // Hover border
                    using (var pen = new Pen(Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.AccentColor ?? Color.Gray, 1.5f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
                else
                {
                    // Normal state
                    using (var brush = new SolidBrush(Color.White))
                    {
                        g.FillPath(brush, path);
                    }

                    // Normal border
                    using (var pen = new Pen(Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.BorderColor ?? Color.FromArgb(200, 200, 200), 0.5f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }

            // Draw subtle divider
            using (var pen = new Pen(Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.BorderColor ?? Color.FromArgb(220, 220, 220), 0.5f))
            {
                g.DrawLine(pen, itemRect.Left + 8, itemRect.Bottom - 1, itemRect.Right - 8, itemRect.Bottom - 1);
            }
        }
    }
}

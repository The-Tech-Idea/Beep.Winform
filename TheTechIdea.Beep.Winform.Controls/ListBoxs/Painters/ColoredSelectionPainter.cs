using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Tokens;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Selected items with full width colored backgrounds (from image 4)
    /// Gray/Green colored full-width selection backgrounds with descriptions
    /// </summary>
    internal class ColoredSelectionPainter : BaseListBoxPainter
    {
        public override bool SupportsCheckboxes() => true;

        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
                DrawItemBackgroundEx(g, itemRect, item, isHovered, isSelected);

            int currentX = itemRect.Left + Scale(16);

            // Draw checkbox
            if (_owner.ShowCheckBox && SupportsCheckboxes())
            {
                int cbSize = Scale(18);
                Rectangle checkRect = new Rectangle(currentX, itemRect.Y + (itemRect.Height - cbSize) / 2, cbSize, cbSize);
                bool isChecked = _owner.SelectedItems?.Contains(item) == true;
                DrawColoredCheckbox(g, checkRect, isChecked, GetSelectionColor(item), isHovered);
                currentX += Scale(26);
            }

            // Draw main text
            int textPad = Scale(16);
            Rectangle textRect = new Rectangle(currentX, itemRect.Y + Scale(8), itemRect.Width - currentX - textPad, itemRect.Height / 2);
            Color textColor = _owner.IsItemSelected(item)
                ? Color.White
                : Color.FromArgb(40, 40, 40);

            Font boldFont = new Font(_owner.TextFont, FontStyle.Bold);
            DrawItemText(g, textRect, item.Text, textColor, boldFont);

            // Draw description
            if (!string.IsNullOrEmpty(item.Description))
            {
                Font smallFont = BeepFontManager.GetFont(_owner.TextFont.Name, _owner.TextFont.Size - 1);
                Rectangle descRect = new Rectangle(currentX, itemRect.Y + itemRect.Height / 2, itemRect.Width - currentX - textPad, itemRect.Height / 2 - Scale(8));
                Color descColor = _owner.IsItemSelected(item)
                    ? Color.FromArgb(200, 200, 200)
                    : Color.FromArgb(120, 120, 120);

                System.Windows.Forms.TextRenderer.DrawText(g, item.Description, smallFont, descRect, descColor,
                    System.Windows.Forms.TextFormatFlags.Left | System.Windows.Forms.TextFormatFlags.Top);
            }
        }

        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            // Use BeepStyling for ColoredSelection background, border, and shadow
            using (var path = Beep.Winform.Controls.Styling.BeepStyling.CreateControlStylePath(itemRect, Style))
            {
                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBackground(g, path, Style);

                if (isHovered)
                {
                    using (var hoverBrush = new SolidBrush(Color.FromArgb(30, _theme?.AccentColor ?? Color.Gray)))
                    {
                        g.FillPath(hoverBrush, path);
                    }
                }

                Beep.Winform.Controls.Styling.BeepStyling.PaintStyleBorder(g, path, false, Style);
            }
        }

        private Color GetSelectionColor(SimpleItem item)
        {
            // Determine checkbox color based on item
            if (item.Text?.ToLower().Contains("custom") == true)
                return _theme?.PrimaryColor ?? Color.FromArgb(76, 175, 80); // Green
            else
                return Color.FromArgb(120, 120, 120); // Gray
        }

        private void DrawColoredCheckbox(Graphics g, Rectangle checkboxRect, bool isChecked, Color checkColor, bool isHovered)
        {
            using (var path = GraphicsExtensions.CreateRoundedRectanglePath(checkboxRect, Scale(3)))
            {
                // Draw background with hover effect
                Color bgColor = isChecked
                    ? checkColor
                    : (isHovered ? Color.FromArgb(240, 240, 240) : Color.White);

                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillPath(brush, path);
                }

                // Draw border
                using (var pen = new Pen(checkColor, 1.5f))
                {
                    g.DrawPath(pen, path);
                }

                // Draw checkmark if checked
                if (isChecked)
                {
                    using (var pen = new Pen(Color.White, 2f))
                    {
                        Point[] checkPoints = new Point[]
                        {
                            new Point(checkboxRect.Left + Scale(3), checkboxRect.Top + checkboxRect.Height / 2),
                            new Point(checkboxRect.Left + checkboxRect.Width / 2 - 1, checkboxRect.Bottom - Scale(4)),
                            new Point(checkboxRect.Right - Scale(3), checkboxRect.Top + Scale(3))
                        };
                        g.DrawLines(pen, checkPoints);
                    }
                }
            }
        }

        public override int GetPreferredItemHeight()
        {
            return Scale(64);
        }
    }
}

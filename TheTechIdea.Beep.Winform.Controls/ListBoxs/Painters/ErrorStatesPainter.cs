using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Tokens;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Job type list with error states (from image 2 - bottom left "Job Type")
    /// Shows items with error badges and prohibited states
    /// </summary>
    internal class ErrorStatesPainter : BaseListBoxPainter
    {
        public override bool SupportsCheckboxes() => true;

        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            DrawItemBackgroundEx(g, itemRect, item, isHovered, isSelected);

            int currentX = itemRect.Left + Scale(16);

            // Draw checkbox
            if (_owner.ShowCheckBox && SupportsCheckboxes())
            {
                int cbSize = Scale(20);
                Rectangle checkRect = new Rectangle(currentX, itemRect.Y + (itemRect.Height - cbSize) / 2, cbSize, cbSize);
                bool isChecked = _owner.SelectedItems?.Contains(item) == true;
                bool hasError = HasError(item);
                DrawErrorCheckbox(g, checkRect, isChecked, hasError, isHovered);
                currentX += Scale(28);
            }

            // Draw main text
            Rectangle textRect = new Rectangle(currentX, itemRect.Y + Scale(6), itemRect.Width - currentX - Scale(100), itemRect.Height / 2);
            Color textColor = GetTextColor(item);
            DrawItemText(g, textRect, item.Text, textColor, _owner.TextFont);

            // Draw description/error text
            if (!string.IsNullOrEmpty(item.Description) || HasError(item))
            {
                string errorText = HasError(item) ? "Option now prohibited" : item.Description;
                Font smallFont = BeepFontManager.GetFont(_owner.TextFont.Name, _owner.TextFont.Size - 1);
                Rectangle descRect = new Rectangle(currentX, itemRect.Y + itemRect.Height / 2, itemRect.Width - currentX - Scale(100), itemRect.Height / 2 - Scale(6));
                Color descColor = HasError(item)
                    ? _theme?.ErrorColor ?? Color.FromArgb(180, 50, 50)
                    : Color.FromArgb(120, 120, 120);
                System.Windows.Forms.TextRenderer.DrawText(g, errorText, smallFont, descRect, descColor,
                    System.Windows.Forms.TextFormatFlags.Left | System.Windows.Forms.TextFormatFlags.Top);
            }

            // Draw error badge on right
            if (HasError(item))
            {
                DrawErrorBadge(g, itemRect, isHovered);
            }
        }

        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            // Use BeepStyling for ErrorStates background, border, and shadow
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

        private bool HasError(SimpleItem item)
        {
            return item.Text?.ToLower().Contains("part-time") == true ||
                   item.Description?.ToLower().Contains("prohibited") == true;
        }

        private Color GetTextColor(SimpleItem item)
        {
            return HasError(item)
                ? _theme?.ErrorColor ?? Color.FromArgb(180, 60, 60)
                : Color.FromArgb(40, 40, 40);
        }

        private void DrawErrorCheckbox(Graphics g, Rectangle checkboxRect, bool isChecked, bool hasError, bool isHovered)
        {
            Color checkColor = hasError
                ? _theme?.ErrorColor ?? Color.FromArgb(220, 53, 69)
                : Color.FromArgb(108, 117, 125);

            using (var path = GraphicsExtensions.CreateRoundedRectanglePath(checkboxRect, Scale(4)))
            {
                Color bgColor = isChecked ? checkColor : (isHovered ? Color.FromArgb(240, 240, 240) : Color.White);
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillPath(brush, path);
                }

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
                            new Point(checkboxRect.Left + Scale(4), checkboxRect.Top + checkboxRect.Height / 2),
                            new Point(checkboxRect.Left + checkboxRect.Width / 2, checkboxRect.Bottom - Scale(5)),
                            new Point(checkboxRect.Right - Scale(4), checkboxRect.Top + Scale(4))
                        };
                        g.DrawLines(pen, checkPoints);
                    }
                }
            }
        }

        private void DrawErrorBadge(Graphics g, Rectangle itemRect, bool isHovered)
        {
            string badgeText = "Error state!";
            Font badgeFont = BeepFontManager.GetFont(_owner.TextFont.Name, _owner.TextFont.Size - 1);
            SizeF textSizeF = TextUtils.MeasureText(g, badgeText, badgeFont);
            var textSize = new Size((int)textSizeF.Width, (int)textSizeF.Height);

            int badgeWidth = textSize.Width + Scale(16);
            int badgeHeight = Scale(22);

            Rectangle badgeRect = new Rectangle(
                itemRect.Right - badgeWidth - Scale(16),
                itemRect.Y + (itemRect.Height - badgeHeight) / 2,
                badgeWidth,
                badgeHeight);

            Color badgeColor = Color.FromArgb(255, 243, 205);
            using (var brush = new LinearGradientBrush(badgeRect, badgeColor, Color.FromArgb(255, 230, 180), LinearGradientMode.Vertical))
            using (var path = GraphicsExtensions.CreateRoundedRectanglePath(badgeRect, 11))
            {
                g.FillPath(brush, path);
            }

            using (var pen = new Pen(Color.FromArgb(255, 193, 7), 1f))
            using (var path = GraphicsExtensions.CreateRoundedRectanglePath(badgeRect, 11))
            {
                g.DrawPath(pen, path);
            }

            // Draw hover effect for badge
            if (isHovered)
            {
                using (var hoverBrush = new SolidBrush(Color.FromArgb(30, Color.Black)))
                {
                    g.FillPath(hoverBrush, GraphicsExtensions.CreateRoundedRectanglePath(badgeRect, 11));
                }
            }

            Color badgeTextColor = Color.FromArgb(120, 80, 0);
            System.Windows.Forms.TextRenderer.DrawText(g, badgeText, badgeFont, badgeRect, badgeTextColor,
                System.Windows.Forms.TextFormatFlags.HorizontalCenter | System.Windows.Forms.TextFormatFlags.VerticalCenter);
        }

        public override int GetPreferredItemHeight()
        {
            return Scale(64);
        }
    }
}

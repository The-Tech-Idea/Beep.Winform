using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Dates
{
    public partial class BeepDateDropDown
    {
        private Rectangle GetContentRectForDrawing()
        {
            UpdateDrawingRect();
            return DrawingRect;
        }

        private Rectangle GetButtonRect(Rectangle contentRect)
        {
            int x = contentRect.Right - _buttonWidth - _padding;
            int y = contentRect.Y + _padding;
            int h = Math.Max(0, contentRect.Height - (_padding * 2));
            return new Rectangle(x, y, _buttonWidth, h);
        }

        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            if (_currentTheme == null) return;

            var content = GetContentRectForDrawing();

            string text = _isEditing ? _inputText : (_selectedDateTime == DateTime.MinValue ? string.Empty : _selectedDateTime.ToString("d"));
            Rectangle textRect = new Rectangle(content.X + _padding, content.Y, Math.Max(0, content.Width - (_showDropDown ? (_buttonWidth + _padding * 2) : _padding)), content.Height);

            if (!string.IsNullOrEmpty(text))
            {
                TextRenderer.DrawText(g, text, _textFont, textRect, _currentTheme.ComboBoxForeColor != Color.Empty ? _currentTheme.ComboBoxForeColor : ForeColor,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
            }
            else
            {
                string placeholder = "dd/MM/yyyy";
                Color placeholderColor = _currentTheme.TextBoxPlaceholderColor != Color.Empty ? _currentTheme.TextBoxPlaceholderColor : Color.FromArgb(150, ForeColor);
                TextRenderer.DrawText(g, placeholder, _textFont, textRect, placeholderColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
            }

            if (_showDropDown)
            {
                var btn = GetButtonRect(content);
                int dividerX = btn.Left - _padding;
                using (var pen = new Pen(Color.FromArgb(60, _currentTheme.ComboBoxBorderColor != Color.Empty ? _currentTheme.ComboBoxBorderColor : BorderColor), 1))
                {
                    g.DrawLine(pen, new Point(dividerX, content.Y + _padding), new Point(dividerX, content.Bottom - _padding));
                }

                // draw icon using StyledImagePainter with tint from theme
                if (!string.IsNullOrEmpty(_calendarIconPath))
                {
                    Color iconColor = _currentTheme?.PrimaryTextColor != Color.Empty ? _currentTheme.PrimaryTextColor : ForeColor;
                    int cornerRadius = Math.Max(0, this.BorderRadius);
                    StyledImagePainter.PaintWithTint(g, btn, _calendarIconPath, iconColor, 1f, cornerRadius);
                }
                else
                {
                    DrawDropdownArrow(g, btn);
                }
            }
        }

        private void DrawDropdownArrow(Graphics g, Rectangle workingRect)
        {
            int arrowSize = Math.Min(12, Math.Min(_buttonWidth - (_padding * 2), workingRect.Height - (_padding * 2)));
            int arrowX = workingRect.Left + (workingRect.Width - arrowSize) / 2;
            int arrowY = workingRect.Top + (workingRect.Height - arrowSize) / 2;
            Rectangle arrowRect = new Rectangle(arrowX, arrowY, arrowSize, arrowSize);
            Point[] points = new Point[]
            {
                new Point(arrowRect.Left + arrowSize/4, arrowRect.Top + arrowSize/3),
                new Point(arrowRect.Left + arrowSize/2, arrowRect.Top + (2*arrowSize)/3),
                new Point(arrowRect.Left + (3*arrowSize)/4, arrowRect.Top + arrowSize/3)
            };
            using (SolidBrush b = new SolidBrush(ForeColor)) g.FillPolygon(b, points);
        }

        private void UpdateMinimumSize()
        {
            try
            {
                string sample = "00/00/0000";
                var measured = TextRenderer.MeasureText(sample + "  ", _textFont);
                int buttonSpace = _showDropDown ? _buttonWidth + (_padding * 2) : 0;
                this.MinimumSize = new Size(Math.Max(120, measured.Width + buttonSpace + 12), Math.Max(22, measured.Height + 6));
                if (Height < MinimumSize.Height) Height = MinimumSize.Height;
            }
            catch { this.MinimumSize = new Size(120, 24); }
        }
    }
}

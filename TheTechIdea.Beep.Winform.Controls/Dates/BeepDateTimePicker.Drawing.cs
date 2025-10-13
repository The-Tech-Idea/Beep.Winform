using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;
using TheTechIdea.Beep.Winform.Controls.Dates.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Dates
{
    /// <summary>
    /// BeepDateTimePicker - Drawing partial: DrawContent override and painting delegation
    /// </summary>
    public partial class BeepDateTimePicker
    {
        protected override void DrawContent(Graphics g)
        {
            if (_currentPainter == null)
            {
                InitializePainter();
                if (_currentPainter == null) return;
            }

            // Get the drawing rectangle (from BaseControl)
            var drawingRect = DrawingRect;
            if (drawingRect.Width <= 0 || drawingRect.Height <= 0) return;

            // Apply theme colors
            var textColor = UseThemeColors ? _currentTheme?.ForeColor ?? ForeColor : ForeColor;
            var backColor = UseThemeColors ? _currentTheme?.BackgroundColor ?? BackColor : BackColor;

            // Paint the text box/input area
            PaintInputArea(g, drawingRect, textColor, backColor);

            // Paint dropdown button
            PaintDropdownButton(g, _dropdownButtonRect);

            // Paint clear button if needed
            if (_allowClear && _selectedDate.HasValue && !_clearButtonRect.IsEmpty)
            {
                PaintClearButton(g, _clearButtonRect);
            }
        }

        private void PaintInputArea(Graphics g, Rectangle bounds, Color textColor, Color backColor)
        {
            // Fill background
            using (var brush = new SolidBrush(backColor))
            {
                g.FillRectangle(brush, bounds);
            }

            // Draw border
            var borderColor = UseThemeColors ? _currentTheme?.AccentColor ?? Color.Gray : Color.Gray;
            if (Focused)
            {
                borderColor = UseThemeColors ? _currentTheme?.AccentColor ?? Color.DodgerBlue : Color.DodgerBlue;
            }

            using (var pen = new Pen(borderColor, 1))
            {
                g.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
            }

            // Draw text
            var displayText = GetFormattedText();
            if (!string.IsNullOrEmpty(displayText))
            {
                var font = _useThemeFont && BeepThemesManager.ToFont(_currentTheme?.LabelFont) != null ? BeepThemesManager.ToFont(_currentTheme?.LabelFont) : _textFont;
                
                using (var brush = new SolidBrush(textColor))
                {
                    var format = new StringFormat
                    {
                        Alignment = StringAlignment.Near,
                        LineAlignment = StringAlignment.Center,
                        Trimming = StringTrimming.EllipsisCharacter
                    };

                    g.DrawString(displayText, font, brush, _textDisplayRect, format);
                }
            }
            else
            {
                // Draw placeholder
                var placeholderText = "Select date...";
                var font = _useThemeFont && BeepThemesManager.ToFont(_currentTheme?.LabelFont) != null ? BeepThemesManager.ToFont(_currentTheme?.LabelFont) : _textFont;
                var placeholderColor = Color.FromArgb(128, textColor);

                using (var brush = new SolidBrush(placeholderColor))
                {
                    var format = new StringFormat
                    {
                        Alignment = StringAlignment.Near,
                        LineAlignment = StringAlignment.Center
                    };

                    g.DrawString(placeholderText, font, brush, _textDisplayRect, format);
                }
            }
        }

        private void PaintDropdownButton(Graphics g, Rectangle buttonRect)
        {
            if (buttonRect.IsEmpty) return;

            var buttonColor = UseThemeColors ? _currentTheme?.ButtonBackColor ?? Color.LightGray : Color.LightGray;
            var isHovered = _hoverState.IsAreaHovered(DateTimePickerHitArea.DropdownButton);
            var isPressed = _hoverState.IsAreaPressed(DateTimePickerHitArea.DropdownButton);

            if (isPressed)
            {
                buttonColor = ControlPaint.Dark(buttonColor, 0.2f);
            }
            else if (isHovered)
            {
                buttonColor = ControlPaint.Light(buttonColor, 0.1f);
            }

            // Fill button
            using (var brush = new SolidBrush(buttonColor))
            {
                g.FillRectangle(brush, buttonRect);
            }

            // Draw border
            var borderColor = UseThemeColors ? _currentTheme?.AccentColor ?? Color.Gray : Color.Gray;
            using (var pen = new Pen(borderColor, 1))
            {
                g.DrawRectangle(pen, buttonRect.X, buttonRect.Y, buttonRect.Width - 1, buttonRect.Height - 1);
            }

            // Draw calendar icon (simple calendar grid)
            var iconRect = new Rectangle(
                buttonRect.X + (buttonRect.Width - 16) / 2,
                buttonRect.Y + (buttonRect.Height - 16) / 2,
                16, 16
            );

            var iconColor = UseThemeColors ? _currentTheme?.ForeColor ?? Color.Black : Color.Black;
            using (var pen = new Pen(iconColor, 1.5f))
            {
                // Calendar frame
                g.DrawRectangle(pen, iconRect.X + 2, iconRect.Y + 3, 11, 10);
                // Top bar
                g.DrawLine(pen, iconRect.X + 2, iconRect.Y + 6, iconRect.X + 13, iconRect.Y + 6);
                // Grid lines
                g.DrawLine(pen, iconRect.X + 6, iconRect.Y + 6, iconRect.X + 6, iconRect.Y + 13);
                g.DrawLine(pen, iconRect.X + 10, iconRect.Y + 6, iconRect.X + 10, iconRect.Y + 13);
                g.DrawLine(pen, iconRect.X + 2, iconRect.Y + 9, iconRect.X + 13, iconRect.Y + 9);
            }
        }

        private void PaintClearButton(Graphics g, Rectangle buttonRect)
        {
            if (buttonRect.IsEmpty) return;

            var buttonColor = UseThemeColors ? _currentTheme?.ButtonBackColor ?? Color.LightGray : Color.LightGray;
            var isHovered = _hoverState.IsAreaHovered(DateTimePickerHitArea.ClearButton);
            var isPressed = _hoverState.IsAreaPressed(DateTimePickerHitArea.ClearButton);

            if (isPressed)
            {
                buttonColor = ControlPaint.Dark(buttonColor, 0.2f);
            }
            else if (isHovered)
            {
                buttonColor = Color.FromArgb(255, 220, 220);
            }

            // Fill button
            using (var brush = new SolidBrush(buttonColor))
            {
                g.FillRectangle(brush, buttonRect);
            }

            // Draw border
            var borderColor = UseThemeColors ? _currentTheme?.AccentColor ?? Color.Gray : Color.Gray;
            using (var pen = new Pen(borderColor, 1))
            {
                g.DrawRectangle(pen, buttonRect.X, buttonRect.Y, buttonRect.Width - 1, buttonRect.Height - 1);
            }

            // Draw X icon
            var iconRect = new Rectangle(
                buttonRect.X + (buttonRect.Width - 12) / 2,
                buttonRect.Y + (buttonRect.Height - 12) / 2,
                12, 12
            );

            var iconColor = isHovered ? Color.DarkRed : (UseThemeColors ? _currentTheme?.ForeColor ?? Color.Black : Color.Black);
            using (var pen = new Pen(iconColor, 2f))
            {
                pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                
                g.DrawLine(pen, iconRect.Left, iconRect.Top, iconRect.Right, iconRect.Bottom);
                g.DrawLine(pen, iconRect.Right, iconRect.Top, iconRect.Left, iconRect.Bottom);
            }
        }
    }
}

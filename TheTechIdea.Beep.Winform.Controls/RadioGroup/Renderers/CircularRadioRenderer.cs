using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.RadioGroup.Renderers
{
    /// <summary>
    /// Traditional circular radio button renderer
    /// </summary>
    public class CircularRadioRenderer : IRadioGroupRenderer, IImageAwareRenderer
    {
        private BaseControl _owner;
        private IBeepTheme _theme;
        private BeepImage _imageRenderer;
        private Font _textFont;
        private Size _maxImageSize = new Size(24, 24);

        #region Properties
        public string StyleName => "Circular";
        public string DisplayName => "Traditional Circular";
        public bool SupportsMultipleSelection => false; // Traditional radio buttons are single-selection

        public Size MaxImageSize 
        { 
            get => _maxImageSize; 
            set => _maxImageSize = value; 
        }

        // Traditional radio specifications
        private const int RadioSize = 16;
        private int IconSize => Math.Min(_maxImageSize.Width, _maxImageSize.Height);
        private const int MinItemHeight = 28;
        private const int ItemPadding = 8;
        private const int ComponentSpacing = 8;
        #endregion

        #region Initialization
        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _theme = theme;
            _imageRenderer = new BeepImage();
            UpdateTheme(theme);
        }

        public void UpdateTheme(IBeepTheme theme)
        {
            _theme = theme;
            _textFont = _owner?.Font ?? (_theme?.LabelMedium != null
                ? new Font(_theme.LabelMedium.FontFamily, _theme.LabelMedium.FontSize)
                : new Font("Segoe UI", 12f));
        }
        #endregion

        #region Rendering
        public void RenderItem(Graphics graphics, SimpleItem item, Rectangle rectangle, RadioItemState state)
        {
            if (graphics == null || item == null) return;

            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Get colors based on state
            var colors = GetStateColors(state);

            // Draw hover background if needed
            if (state.IsHovered)
            {
                DrawHoverBackground(graphics, rectangle, colors);
            }

            // Calculate layout areas
            var contentArea = GetContentArea(rectangle);
            var radioArea = GetSelectorArea(rectangle);
            
            // Draw radio button
            DrawRadioButton(graphics, radioArea, state, colors);

            // Draw content (icon + text)
            DrawContent(graphics, item, contentArea, radioArea, state, colors);

            // Draw focus indicator
            if (state.IsFocused)
            {
                DrawFocusIndicator(graphics, rectangle, colors);
            }
        }

        private void DrawHoverBackground(Graphics graphics, Rectangle rectangle, RadioColors colors)
        {
            using (var brush = new SolidBrush(colors.HoverBackground))
            {
                graphics.FillRectangle(brush, rectangle);
            }
        }

        private void DrawRadioButton(Graphics graphics, Rectangle radioArea, RadioItemState state, RadioColors colors)
        {
            var center = new Point(
                radioArea.X + radioArea.Width / 2,
                radioArea.Y + radioArea.Height / 2
            );

            int outerRadius = RadioSize / 2;
            
            // Draw outer circle
            Color borderColor = state.IsEnabled ? 
                (state.IsSelected ? colors.SelectedBorder : colors.Border) : 
                colors.DisabledBorder;

            using (var pen = new Pen(borderColor, 2f))
            {
                var outerRect = new Rectangle(
                    center.X - outerRadius,
                    center.Y - outerRadius,
                    outerRadius * 2,
                    outerRadius * 2
                );
                graphics.DrawEllipse(pen, outerRect);
            }

            // Draw inner circle (when selected)
            if (state.IsSelected && state.IsEnabled)
            {
                int innerRadius = outerRadius - 4;
                using (var brush = new SolidBrush(colors.SelectedFill))
                {
                    var innerRect = new Rectangle(
                        center.X - innerRadius,
                        center.Y - innerRadius,
                        innerRadius * 2,
                        innerRadius * 2
                    );
                    graphics.FillEllipse(brush, innerRect);
                }
            }
        }

        private void DrawContent(Graphics graphics, SimpleItem item, Rectangle contentArea, Rectangle radioArea, RadioItemState state, RadioColors colors)
        {
            int currentX = radioArea.Right + ComponentSpacing;
            
            // Draw icon if present
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                var iconRect = new Rectangle(
                    currentX,
                    contentArea.Y + (contentArea.Height - IconSize) / 2,
                    IconSize,
                    IconSize
                );

                _imageRenderer.ImagePath = item.ImagePath;
                _imageRenderer.Draw(graphics, iconRect);
                
                currentX += IconSize + ComponentSpacing;
            }

            // Draw text
            if (!string.IsNullOrEmpty(item.Text))
            {
                var textRect = new Rectangle(
                    currentX,
                    contentArea.Y,
                    Math.Max(0, contentArea.Right - currentX),
                    contentArea.Height
                );

                Color textColor = state.IsEnabled ? 
                    (state.IsSelected ? colors.SelectedText : colors.Text) : 
                    colors.DisabledText;
                
                using (var brush = new SolidBrush(textColor))
                {
                    var stringFormat = new StringFormat
                    {
                        Alignment = StringAlignment.Near,
                        LineAlignment = StringAlignment.Center,
                        Trimming = StringTrimming.EllipsisCharacter
                    };

                    graphics.DrawString(item.Text, _textFont, brush, textRect, stringFormat);
                }
            }
        }

        private void DrawFocusIndicator(Graphics graphics, Rectangle rectangle, RadioColors colors)
        {
            using var pen = new Pen(colors.FocusBorder, 1f) { DashStyle = DashStyle.Dot };
            var focusRect = Rectangle.Inflate(rectangle, -2, -2);
            graphics.DrawRectangle(pen, focusRect);
        }
        #endregion

        #region Measurement
        public Size MeasureItem(SimpleItem item, Graphics graphics)
        {
            if (item == null) return new Size(100, MinItemHeight);

            int width = ItemPadding; // Left padding
            int height = MinItemHeight;

            // Radio button width
            width += RadioSize + ComponentSpacing;

            // Icon width - account for image if present
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                width += IconSize + ComponentSpacing;
                // Ensure minimum height accommodates the image
                height = Math.Max(height, IconSize + ItemPadding);
            }

            // Text width
            if (!string.IsNullOrEmpty(item.Text))
            {
                var textSize = graphics.MeasureString(item.Text, _textFont);
                width += (int)Math.Ceiling(textSize.Width);
                height = Math.Max(height, (int)Math.Ceiling(textSize.Height) + ItemPadding);
            }

            width += ItemPadding; // Right padding

            return new Size(width, height);
        }

        public Rectangle GetContentArea(Rectangle itemRectangle)
        {
            return new Rectangle(
                itemRectangle.X + ItemPadding / 2,
                itemRectangle.Y + ItemPadding / 2,
                itemRectangle.Width - ItemPadding,
                itemRectangle.Height - ItemPadding
            );
        }

        public Rectangle GetSelectorArea(Rectangle itemRectangle)
        {
            var contentArea = GetContentArea(itemRectangle);
            return new Rectangle(
                contentArea.X,
                contentArea.Y + (contentArea.Height - RadioSize) / 2,
                RadioSize,
                RadioSize
            );
        }
        #endregion

        #region Group Decorations
        public void RenderGroupDecorations(Graphics graphics, Rectangle groupRectangle, List<SimpleItem> items, List<Rectangle> itemRectangles, List<RadioItemState> states)
        {
            // Traditional radio buttons typically don't have group decorations
        }
        #endregion

        #region Helper Methods
        private RadioColors GetStateColors(RadioItemState state)
        {
            if (_theme == null)
            {
                return new RadioColors
                {
                    Border = Color.FromArgb(118, 118, 118),
                    SelectedBorder = Color.FromArgb(0, 120, 215),
                    DisabledBorder = Color.FromArgb(200, 200, 200),
                    SelectedFill = Color.FromArgb(0, 120, 215),
                    Text = Color.Black,
                    SelectedText = Color.Black,
                    DisabledText = Color.FromArgb(109, 109, 109),
                    HoverBackground = Color.FromArgb(240, 240, 240),
                    FocusBorder = Color.FromArgb(0, 120, 215)
                };
            }

            return new RadioColors
            {
                Border = _theme.BorderColor,
                SelectedBorder = _theme.PrimaryColor,
                DisabledBorder = _theme.DisabledBorderColor,
                SelectedFill = _theme.PrimaryColor,
                Text = _theme.ForeColor,
                SelectedText = _theme.ForeColor,
                DisabledText = _theme.DisabledForeColor,
                HoverBackground = _theme.ButtonHoverBackColor,
                FocusBorder = _theme.BorderColor
            };
        }

        private class RadioColors
        {
            public Color Border { get; set; }
            public Color SelectedBorder { get; set; }
            public Color DisabledBorder { get; set; }
            public Color SelectedFill { get; set; }
            public Color Text { get; set; }
            public Color SelectedText { get; set; }
            public Color DisabledText { get; set; }
            public Color HoverBackground { get; set; }
            public Color FocusBorder { get; set; }
        }
        #endregion
    }
}
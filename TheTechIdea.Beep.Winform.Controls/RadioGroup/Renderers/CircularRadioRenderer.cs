using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.RadioGroup.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;

namespace TheTechIdea.Beep.Winform.Controls.RadioGroup.Renderers
{
    /// <summary>
    /// Traditional circular radio button renderer with StyleColors support
    /// </summary>
    public class CircularRadioRenderer : IRadioGroupRenderer, IImageAwareRenderer
    {
        private BaseControl _owner;
        private IBeepTheme _theme;
        private Font _textFont;
        private Size _maxImageSize = new Size(24, 24);
        private BeepControlStyle _controlStyle = BeepControlStyle.Minimal;
        private bool _useThemeColors = true;

        #region Properties
        public string StyleName => "Circular";
        public string DisplayName => "Traditional Circular";
        public bool SupportsMultipleSelection => false; // Traditional radio buttons are single-selection
        public bool AllowMultipleSelection { get; set; }
        
        public BeepControlStyle ControlStyle
        {
            get => _controlStyle;
            set => _controlStyle = value;
        }
        
        public bool UseThemeColors
        {
            get => _useThemeColors;
            set => _useThemeColors = value;
        }

        public Size MaxImageSize 
        { 
            get => _maxImageSize; 
            set => _maxImageSize = value; 
        }

        // Traditional radio specifications
        private const int RadioSize = 20;
        private int IconSize => Math.Min(_maxImageSize.Width, _maxImageSize.Height);
        private const int MinItemHeight = 40;
        private const int ItemPadding = 12;
        private const int ComponentSpacing = 12;
        #endregion

        #region Initialization
        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _theme = theme;
            UpdateTheme(theme);
        }

        public void UpdateTheme(IBeepTheme theme)
        {
            _theme = theme;
            _textFont = _owner?.Font ?? RadioGroupFontHelpers.GetItemFont(_controlStyle, isSelected: false, theme);
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
            if (state.IsEnabled && state.IsHovered)
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
            if (state.IsSelected)
            {
                int innerRadius = outerRadius - 4;
                var fillColor = state.IsEnabled ? colors.SelectedFill : colors.DisabledBorder;
                using (var brush = new SolidBrush(fillColor))
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
            int currentX = Math.Max(radioArea.Right + ComponentSpacing, contentArea.Left);
            
            // Draw icon if present
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                int iconSize = Math.Min(IconSize, Math.Max(12, contentArea.Height - 6));
                var iconRect = new Rectangle(
                    currentX,
                    contentArea.Y + (contentArea.Height - iconSize) / 2,
                    iconSize,
                    iconSize
                );
                var iconPath = RadioGroupIconHelpers.GetItemIconPath(item.ImagePath);
                var iconColor = RadioGroupIconHelpers.GetIconColor(_theme, _useThemeColors, state.IsSelected, !state.IsEnabled);
                RadioGroupIconHelpers.PaintIcon(graphics, iconRect, iconPath, iconColor, _theme, _useThemeColors, _controlStyle);
                
                currentX += iconSize + ComponentSpacing;
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
            using var pen = new Pen(colors.FocusBorder, 2f) { DashStyle = DashStyle.Dot };
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
                var textSize =  TextUtils.MeasureText(graphics,item.Text, _textFont);
                width += (int)Math.Ceiling(textSize.Width);
                height = Math.Max(height, (int)Math.Ceiling(textSize.Height) + ItemPadding);
            }

            width += ItemPadding; // Right padding

            return new Size(width, height);
        }

        public Rectangle GetContentArea(Rectangle itemRectangle)
        {
            return new Rectangle(
                itemRectangle.X + Math.Max(4, ItemPadding / 2),
                itemRectangle.Y + Math.Max(2, ItemPadding / 2),
                Math.Max(0, itemRectangle.Width - Math.Max(8, ItemPadding)),
                Math.Max(0, itemRectangle.Height - Math.Max(4, ItemPadding))
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

        public void Cleanup()
        {
            // No cached GDI+ resources — fonts are owned by the control
        }
        #endregion

        #region Helper Methods
        private RadioColors GetStateColors(RadioItemState state)
        {
            if (!_useThemeColors || _theme == null)
            {
                return new RadioColors
                {
                    Border = StyleColors.GetBorder(_controlStyle),
                    SelectedBorder = StyleColors.GetPrimary(_controlStyle),
                    DisabledBorder = Color.FromArgb(160, StyleColors.GetBorder(_controlStyle)),
                    SelectedFill = StyleColors.GetPrimary(_controlStyle),
                    Text = StyleColors.GetForeground(_controlStyle),
                    SelectedText = StyleColors.GetForeground(_controlStyle),
                    DisabledText = Color.FromArgb(150, StyleColors.GetForeground(_controlStyle)),
                    HoverBackground = StyleColors.GetHover(_controlStyle),
                    FocusBorder = StyleColors.GetPrimary(_controlStyle)
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
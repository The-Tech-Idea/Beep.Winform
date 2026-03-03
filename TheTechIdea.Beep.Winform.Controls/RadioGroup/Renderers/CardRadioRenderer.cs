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
    /// Card-Style renderer with elevation and modern aesthetics with StyleColors support
    /// </summary>
    public class CardRadioRenderer : IRadioGroupRenderer, IImageAwareRenderer
    {
        private BaseControl _owner;
        private IBeepTheme _theme;
        private Font _textFont;
        private Size _maxImageSize = new Size(24, 24);
        private BeepControlStyle _controlStyle = BeepControlStyle.TailwindCard;
        private bool _useThemeColors = true;

        #region Properties
        public string StyleName => "Card";
        public string DisplayName => "Material Card Style";
        public bool SupportsMultipleSelection => true;
        
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

        // Card design specifications
        private const int SelectorSize = 18;
        private int IconSize => Math.Min(_maxImageSize.Width, _maxImageSize.Height);
        private const int MinItemHeight = 48;
        private const int ItemPadding = 16;
        private const int ComponentSpacing = 12;
        private const int CornerRadius = 8;
        private const int CardElevation = 2;
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

            // Get card area (with margin)
            var cardRect = GetCardRectangle(rectangle);
            
            // Get colors based on state
            var colors = GetStateColors(state);

            // Draw shadow
            if (state.IsEnabled && (state.IsHovered || state.IsSelected))
            {
                DrawCardShadow(graphics, cardRect, state);
            }

            // Draw card background
            DrawCardBackground(graphics, cardRect, state, colors);

            // Draw card border
            DrawCardBorder(graphics, cardRect, state, colors);

            // Draw content
            DrawContent(graphics, item, cardRect, GetSelectorArea(rectangle), state, colors);

            // Draw selector indicator
            DrawCardSelector(graphics, cardRect, state, colors);
        }

        private Rectangle GetCardRectangle(Rectangle itemRectangle)
        {
            return new Rectangle(
                itemRectangle.X,
                itemRectangle.Y,
                itemRectangle.Width,
                itemRectangle.Height
            );
        }

        private void DrawCardShadow(Graphics graphics, Rectangle cardRect, RadioItemState state)
        {
            int shadowIntensity = state.IsSelected ? 8 : (state.IsHovered ? 4 : 2);
            Color shadowColor = Color.FromArgb(20, Color.Black);

            for (int i = 0; i < shadowIntensity; i++)
            {
                var shadowRect = new Rectangle(
                    cardRect.X + i,
                    cardRect.Y + i,
                    cardRect.Width,
                    cardRect.Height
                );

                using (var brush = new SolidBrush(Color.FromArgb(shadowIntensity - i, shadowColor)))
                using (var path = CreateRoundedRectanglePath(shadowRect, CornerRadius))
                {
                    graphics.FillPath(brush, path);
                }
            }
        }

        private void DrawCardBackground(Graphics graphics, Rectangle cardRect, RadioItemState state, CardColors colors)
        {
            Color backgroundColor;
            if (!state.IsEnabled)
            {
                backgroundColor = colors.DisabledBackground;
            }
            else if (state.IsSelected)
            {
                backgroundColor = colors.SelectedBackground;
            }
            else if (state.IsHovered)
            {
                backgroundColor = colors.HoverBackground;
            }
            else
            {
                backgroundColor = colors.Background;
            }

            // Add gradient for card Style to make it more distinct
            if (state.IsSelected)
            {
                using (var brush = new LinearGradientBrush(cardRect, backgroundColor, 
                    Color.FromArgb(Math.Max(0, backgroundColor.R - 20), 
                                   Math.Max(0, backgroundColor.G - 20), 
                                   Math.Max(0, backgroundColor.B - 20)), 
                    LinearGradientMode.Vertical))
                using (var path = CreateRoundedRectanglePath(cardRect, CornerRadius))
                {
                    graphics.FillPath(brush, path);
                }
            }
            else
            {
                using (var brush = new SolidBrush(backgroundColor))
                using (var path = CreateRoundedRectanglePath(cardRect, CornerRadius))
                {
                    graphics.FillPath(brush, path);
                }
            }
        }

        private void DrawCardBorder(Graphics graphics, Rectangle cardRect, RadioItemState state, CardColors colors)
        {
            Color borderColor;
            float borderWidth;
            if (!state.IsEnabled)
            {
                borderColor = colors.DisabledBorder;
                borderWidth = 1f;
            }
            else if (state.IsSelected)
            {
                borderColor = colors.SelectedBorder;
                borderWidth = 2f;
            }
            else if (state.IsFocused)
            {
                borderColor = colors.FocusBorder;
                borderWidth = 2f;
            }
            else if (state.IsHovered)
            {
                borderColor = colors.HoverBorder;
                borderWidth = 1.5f;
            }
            else
            {
                borderColor = colors.Border;
                borderWidth = 1f;
            }

            using (var pen = new Pen(borderColor, borderWidth))
            using (var path = CreateRoundedRectanglePath(cardRect, CornerRadius))
            {
                graphics.DrawPath(pen, path);
            }
        }

        private void DrawContent(Graphics graphics, SimpleItem item, Rectangle contentArea, Rectangle selectorArea, RadioItemState state, CardColors colors)
        {
            int left = contentArea.X + ItemPadding;
            int rightCap = Math.Max(left, selectorArea.X - ComponentSpacing);
            int currentX = left;

            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                int sz = Math.Min(IconSize, Math.Max(12, contentArea.Height - 6));
                var iconRect = new Rectangle(currentX, contentArea.Y + (contentArea.Height - sz) / 2, sz, sz);
                var iconPath = RadioGroupIconHelpers.GetItemIconPath(item.ImagePath);
                var iconColor = RadioGroupIconHelpers.GetIconColor(_theme, _useThemeColors, state.IsSelected, !state.IsEnabled);
                RadioGroupIconHelpers.PaintIcon(graphics, iconRect, iconPath, iconColor, _theme, _useThemeColors, _controlStyle);
                currentX += sz + ComponentSpacing;
            }

            if (!string.IsNullOrEmpty(item.Text))
            {
                var textRect = new Rectangle(currentX, contentArea.Y, Math.Max(0, rightCap - currentX), contentArea.Height);
                using var brush = new SolidBrush(state.IsEnabled ? colors.Text : colors.DisabledText);
                var fmt = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
                graphics.DrawString(item.Text, _textFont, brush, textRect, fmt);
            }
        }

        private void DrawCardSelector(Graphics graphics, Rectangle cardRect, RadioItemState state, CardColors colors)
        {
            int selectorSize = Math.Min(SelectorSize, Math.Max(0, cardRect.Height - (ItemPadding * 2)));
            var selectorRect = new Rectangle(
                Math.Max(cardRect.X + ItemPadding, cardRect.Right - ItemPadding - selectorSize),
                cardRect.Y + (cardRect.Height - selectorSize) / 2,
                selectorSize,
                selectorSize
            );
            if (selectorRect.Width <= 0 || selectorRect.Height <= 0) return;

            var center = new Point(
                selectorRect.X + selectorRect.Width / 2,
                selectorRect.Y + selectorRect.Height / 2
            );

            if (_owner.GetType().GetProperty("AllowMultipleSelection")?.GetValue(_owner) as bool? == true)
            {
                // Draw checkbox
                DrawCardCheckbox(graphics, selectorRect, state, colors);
            }
            else
            {
                // Draw radio button
                DrawCardRadio(graphics, center, state, colors);
            }
        }

        private void DrawCardRadio(Graphics graphics, Point center, RadioItemState state, CardColors colors)
        {
            int radius = SelectorSize / 2 - 1;

            // Outer circle
            Color borderColor = !state.IsEnabled
                ? colors.DisabledBorder
                : (state.IsSelected ? colors.SelectedBorder : colors.Border);
            using (var pen = new Pen(borderColor, 2f))
            {
                var outerRect = new Rectangle(
                    center.X - radius,
                    center.Y - radius,
                    radius * 2,
                    radius * 2
                );
                graphics.DrawEllipse(pen, outerRect);
            }

            // Inner filled circle (when selected)
            if (state.IsSelected)
            {
                int fillRadius = radius - 4;
                var fillColor = state.IsEnabled ? colors.SelectedBorder : colors.DisabledBorder;
                using (var brush = new SolidBrush(fillColor))
                {
                    var innerRect = new Rectangle(
                        center.X - fillRadius,
                        center.Y - fillRadius,
                        fillRadius * 2,
                        fillRadius * 2
                    );
                    graphics.FillEllipse(brush, innerRect);
                }
            }
        }

        private void DrawCardCheckbox(Graphics graphics, Rectangle selectorRect, RadioItemState state, CardColors colors)
        {
            var checkboxRect = new Rectangle(
                selectorRect.X + 2,
                selectorRect.Y + 2,
                selectorRect.Width - 4,
                selectorRect.Height - 4
            );

            if (state.IsSelected)
            {
                // Filled checkbox
                var fillColor = state.IsEnabled ? colors.SelectedBorder : colors.DisabledBorder;
                using (var brush = new SolidBrush(fillColor))
                using (var path = CreateRoundedRectanglePath(checkboxRect, 3))
                {
                    graphics.FillPath(brush, path);
                }

                // Checkmark
                DrawCheckmark(graphics, checkboxRect, colors.Background);
            }
            else
            {
                // Outlined checkbox
                var borderColor = state.IsEnabled ? colors.Border : colors.DisabledBorder;
                using (var pen = new Pen(borderColor, 2f))
                using (var path = CreateRoundedRectanglePath(checkboxRect, 3))
                {
                    graphics.DrawPath(pen, path);
                }
            }
        }

        private void DrawCheckmark(Graphics graphics, Rectangle rect, Color color)
        {
            using (var pen = new Pen(color, 2f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                var points = new PointF[]
                {
                    new PointF(rect.X + 3, rect.Y + rect.Height / 2),
                    new PointF(rect.X + rect.Width / 2 - 1, rect.Y + rect.Height - 5),
                    new PointF(rect.X + rect.Width - 3, rect.Y + 3)
                };

                graphics.DrawLines(pen, points);
            }
        }
        #endregion

        #region Measurement
        public Size MeasureItem(SimpleItem item, Graphics graphics)
        {
            if (item == null) return new Size(150, MinItemHeight);

            int width = ItemPadding; // Left padding
            int height = MinItemHeight;

            // Selector width
            width += SelectorSize + ComponentSpacing;

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
                var textSize = TextUtils.MeasureText(graphics,item.Text, _textFont);
                width += (int)Math.Ceiling(textSize.Width);
                height = Math.Max(height, (int)Math.Ceiling(textSize.Height) + ItemPadding);
            }

            width += ItemPadding; // Right padding

            return new Size(width, height);
        }

        public Rectangle GetContentArea(Rectangle itemRectangle)
        {
            var cardRect = GetCardRectangle(itemRectangle);
            return new Rectangle(
                cardRect.X + ItemPadding,
                cardRect.Y + ItemPadding,
                Math.Max(0, cardRect.Width - (ItemPadding * 2)),
                Math.Max(0, cardRect.Height - (ItemPadding * 2))
            );
        }

        public Rectangle GetSelectorArea(Rectangle itemRectangle)
        {
            var cardRect = GetCardRectangle(itemRectangle);
            int selectorSize = Math.Min(SelectorSize, Math.Max(0, cardRect.Height - (ItemPadding * 2)));
            int selectorY = cardRect.Y + (cardRect.Height - selectorSize) / 2;
            return new Rectangle(
                Math.Max(cardRect.X + ItemPadding, cardRect.Right - ItemPadding - selectorSize),
                selectorY,
                selectorSize,
                selectorSize
            );
        }
        #endregion

        #region Group Decorations
        public void RenderGroupDecorations(Graphics graphics, Rectangle groupRectangle, List<SimpleItem> items, List<Rectangle> itemRectangles, List<RadioItemState> states)
        {
            // Cards typically don't need group decorations
            // Could add a subtle background or title here if needed
        }
        #endregion

        #region Helper Methods
        private CardColors GetStateColors(RadioItemState state)
        {
            if (!_useThemeColors || _theme == null)
            {
                var primary = StyleColors.GetPrimary(_controlStyle);
                var background = StyleColors.GetBackground(_controlStyle);
                var foreground = StyleColors.GetForeground(_controlStyle);
                var border = StyleColors.GetBorder(_controlStyle);
                var hover = StyleColors.GetHover(_controlStyle);
                var selection = StyleColors.GetSelection(_controlStyle);
                var secondary = StyleColors.GetSecondary(_controlStyle);

                return new CardColors
                {
                    Background = background,
                    HoverBackground = hover,
                    SelectedBackground = selection,
                    Border = border,
                    HoverBorder = Color.FromArgb(180, primary),
                    SelectedBorder = primary,
                    Text = foreground,
                    SubText = Color.FromArgb(170, foreground),
                    FocusBorder = primary,
                    DisabledBorder = Color.FromArgb(150, border),
                    DisabledBackground = Color.FromArgb(200, secondary),
                    DisabledText = Color.FromArgb(128, foreground)
                };
            }

            return new CardColors
            {
                Background = _theme.BackgroundColor,
                HoverBackground = Color.FromArgb(248, _theme.BackgroundColor),
                SelectedBackground = Color.FromArgb(240, _theme.PrimaryColor),
                Border = _theme.BorderColor,
                HoverBorder = _theme.ButtonHoverBorderColor,
                SelectedBorder = _theme.PrimaryColor,
                Text = _theme.ForeColor,
                SubText = _theme.SecondaryTextColor,
                FocusBorder = _theme.PrimaryColor,
                DisabledBorder = _theme.DisabledBorderColor,
                DisabledBackground = _theme.DisabledBackColor,
                DisabledText = _theme.DisabledForeColor
            };
        }

        private GraphicsPath CreateRoundedRectanglePath(Rectangle rectangle, int cornerRadius)
        {
            var path = new GraphicsPath();
            
            if (cornerRadius <= 0)
            {
                path.AddRectangle(rectangle);
                return path;
            }

            int diameter = cornerRadius * 2;
            var arc = new Rectangle(rectangle.Location, new Size(diameter, diameter));

            // Top left arc
            path.AddArc(arc, 180, 90);

            // Top right arc
            arc.X = rectangle.Right - diameter;
            path.AddArc(arc, 270, 90);

            // Bottom right arc
            arc.Y = rectangle.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // Bottom left arc
            arc.X = rectangle.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        private class CardColors
        {
            public Color Background { get; set; }
            public Color HoverBackground { get; set; }
            public Color SelectedBackground { get; set; }
            public Color Border { get; set; }
            public Color HoverBorder { get; set; }
            public Color SelectedBorder { get; set; }
            public Color Text { get; set; }
            public Color SubText { get; set; }
            public Color FocusBorder { get; set; }
            public Color DisabledBorder { get; set; }
            public Color DisabledBackground { get; set; }
            public Color DisabledText { get; set; }
        }
        #endregion
    }
}
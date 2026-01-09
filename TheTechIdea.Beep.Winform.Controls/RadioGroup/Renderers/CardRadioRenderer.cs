using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Images;

namespace TheTechIdea.Beep.Winform.Controls.RadioGroup.Renderers
{
    /// <summary>
    /// Card-Style renderer with elevation and modern aesthetics with StyleColors support
    /// </summary>
    public class CardRadioRenderer : IRadioGroupRenderer, IImageAwareRenderer
    {
        private BaseControl _owner;
        private IBeepTheme _theme;
        private BeepImage _imageRenderer;
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
            _imageRenderer = new BeepImage();
            UpdateTheme(theme);
        }

        public void UpdateTheme(IBeepTheme theme)
        {
            _theme = theme;
            _textFont = _owner?.Font ?? (_theme?.BodySmall != null
                ? new Font(_theme.BodySmall.FontFamily, _theme.BodySmall.FontSize)
                : new Font("Segoe UI", 10f));
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
            if (state.IsHovered || state.IsSelected)
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
            Color backgroundColor = state.IsSelected ? colors.SelectedBackground : 
                                  state.IsHovered ? colors.HoverBackground : 
                                  colors.Background;

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
            Color borderColor = state.IsSelected ? colors.SelectedBorder :
                              state.IsHovered ? colors.HoverBorder :
                              colors.Border;

            float borderWidth = state.IsSelected ? 2f : 1f;

            using (var pen = new Pen(borderColor, borderWidth))
            using (var path = CreateRoundedRectanglePath(cardRect, CornerRadius))
            {
                graphics.DrawPath(pen, path);
            }
        }

        private void DrawContent(Graphics graphics, SimpleItem item, Rectangle contentArea, Rectangle selectorArea, RadioItemState state, CardColors colors)
        {
            int currentX = selectorArea.Right + ComponentSpacing;
            currentX = Math.Max(currentX, contentArea.Left);
            int rightCap = contentArea.Right;

            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                int sz = Math.Min(IconSize, Math.Max(12, contentArea.Height - 6));
                var iconRect = new Rectangle(currentX, contentArea.Y + (contentArea.Height - sz) / 2, sz, sz);
                _imageRenderer.ImagePath = item.ImagePath;
                _imageRenderer.Draw(graphics, iconRect);
                currentX += sz + ComponentSpacing;
            }

            if (!string.IsNullOrEmpty(item.Text))
            {
                var textRect = new Rectangle(currentX, contentArea.Y, Math.Max(0, rightCap - currentX), contentArea.Height);
                using var brush = new SolidBrush(state.IsEnabled ? colors.Text : _theme.DisabledForeColor);
                var fmt = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
                graphics.DrawString(item.Text, _textFont, brush, textRect, fmt);
            }
        }

        private void DrawCardSelector(Graphics graphics, Rectangle cardRect, RadioItemState state, CardColors colors)
        {
            // Position selector in top-right corner
            var selectorRect = new Rectangle(
                cardRect.Right - SelectorSize - ItemPadding,
                cardRect.Y + ItemPadding,
                SelectorSize,
                SelectorSize
            );

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
            Color borderColor = state.IsSelected ? colors.SelectedBorder : colors.Border;
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
                using (var brush = new SolidBrush(colors.SelectedBorder))
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
                using (var brush = new SolidBrush(colors.SelectedBorder))
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
                using (var pen = new Pen(colors.Border, 2f))
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
                cardRect.Width - (ItemPadding * 2),
                cardRect.Height - (ItemPadding * 2)
            );
        }

        public Rectangle GetSelectorArea(Rectangle itemRectangle)
        {
            var cardRect = GetCardRectangle(itemRectangle);
            return new Rectangle(
                cardRect.Right - ItemPadding - SelectorSize,
                cardRect.Y + ItemPadding,
                SelectorSize,
                SelectorSize
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
            if (_theme == null)
            {
                return new CardColors
                {
                    Background = Color.White,
                    HoverBackground = Color.FromArgb(248, 250, 252),
                    SelectedBackground = Color.FromArgb(240, 247, 255),
                    Border = Color.FromArgb(226, 232, 240),
                    HoverBorder = Color.FromArgb(148, 163, 184),
                    SelectedBorder = Color.FromArgb(59, 130, 246),
                    Text = Color.FromArgb(15, 23, 42),
                    SubText = Color.FromArgb(100, 116, 139)
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
                SubText = _theme.SecondaryTextColor
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
        }
        #endregion
    }
}
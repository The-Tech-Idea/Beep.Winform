using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.RadioGroup.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;

namespace TheTechIdea.Beep.Winform.Controls.RadioGroup.Renderers
{
    /// <summary>
    /// Modern flat design renderer for radio group items with StyleColors support
    /// </summary>
    public class FlatRadioRenderer : IRadioGroupRenderer, IImageAwareRenderer
    {
        private BaseControl _owner;
        private IBeepTheme _theme;
        private Font _textFont;
        private Size _maxImageSize = new Size(24, 24);
        private BeepControlStyle _controlStyle = BeepControlStyle.Minimal;
        private bool _useThemeColors = true;

        #region Properties
        public string StyleName => "Flat";
        public string DisplayName => "Modern Flat Design";
        public bool SupportsMultipleSelection => true;
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

        // Flat design specifications
        private const int SelectorSize = 20;
        private int IconSize => Math.Min(_maxImageSize.Width, _maxImageSize.Height);
        private const int MinItemHeight = 40;
        private const int ItemPadding = 12;
        private const int ComponentSpacing = 12;
        private const int CornerRadius = 4;
        private int S(int value) => _owner == null ? value : DpiScalingHelper.ScaleValue(value, _owner);
        private float SF(float value) => _owner == null ? value : DpiScalingHelper.ScaleValue(value, _owner);
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

            // Draw background
            DrawItemBackground(graphics, rectangle, state, colors);

            // Calculate layout areas
            var contentArea = GetContentArea(rectangle);
            var selectorArea = GetSelectorArea(rectangle);
            
            // Draw selector (radio button or checkbox)
            DrawSelector(graphics, selectorArea, state, colors);

            // Draw content (icon + text)
            DrawContent(graphics, item, contentArea, selectorArea, state, colors);

            // Draw focus indicator
            if (state.IsFocused)
            {
                DrawFocusIndicator(graphics, rectangle, colors);
            }
        }

        private void DrawItemBackground(Graphics graphics, Rectangle rectangle, RadioItemState state, FlatColors colors)
        {
            Color backgroundColor = Color.Transparent;
            
            if (!state.IsEnabled)
            {
                backgroundColor = state.IsSelected ? colors.DisabledBackground : Color.Transparent;
            }
            else if (state.IsSelected)
            {
                backgroundColor = colors.SelectedBackground;
            }
            else if (state.IsHovered)
            {
                backgroundColor = colors.HoverBackground;
            }

            if (backgroundColor != Color.Transparent)
            {
                using (var brush = new SolidBrush(backgroundColor))
                using (var path = CreateRoundedRectanglePath(rectangle, S(CornerRadius)))
                {
                    graphics.FillPath(brush, path);
                }
            }
        }

        private void DrawSelector(Graphics graphics, Rectangle selectorArea, RadioItemState state, FlatColors colors)
        {
            var center = new Point(
                selectorArea.X + selectorArea.Width / 2,
                selectorArea.Y + selectorArea.Height / 2
            );

            if (AllowMultipleSelection)
            {
                // Draw flat checkbox
                DrawFlatCheckbox(graphics, selectorArea, state, colors);
            }
            else
            {
                // Draw flat radio button
                DrawFlatRadio(graphics, center, state, colors);
            }
        }

        private void DrawFlatRadio(Graphics graphics, Point center, RadioItemState state, FlatColors colors)
        {
            int radius = Math.Max(2, S(SelectorSize) / 2 - S(1));

            // Outer circle - flat design with subtle border
            Color borderColor = !state.IsEnabled
                ? colors.DisabledBorder
                : (state.IsSelected ? colors.SelectedBorder : colors.Border);
            Color fillColor = !state.IsEnabled
                ? colors.DisabledBackground
                : (state.IsSelected ? colors.SelectedBorder : Color.Transparent);

            var outerRect = new Rectangle(
                center.X - radius,
                center.Y - radius,
                radius * 2,
                radius * 2
            );

            // Fill background if selected
            if (state.IsSelected)
            {
                using (var brush = new SolidBrush(fillColor))
                {
                    graphics.FillEllipse(brush, outerRect);
                }
            }

            // Draw border
            using (var pen = new Pen(borderColor, state.IsSelected ? SF(2f) : SF(1.5f)))
            {
                graphics.DrawEllipse(pen, outerRect);
            }

            // Draw inner dot if selected
            if (state.IsSelected)
            {
                int innerRadius = Math.Max(2, radius - S(6));
                var innerRect = new Rectangle(
                    center.X - innerRadius,
                    center.Y - innerRadius,
                    innerRadius * 2,
                    innerRadius * 2
                );

                using (var brush = new SolidBrush(state.IsEnabled ? colors.SelectedIndicator : colors.DisabledText))
                {
                    graphics.FillEllipse(brush, innerRect);
                }
            }
        }

        private void DrawFlatCheckbox(Graphics graphics, Rectangle selectorArea, RadioItemState state, FlatColors colors)
        {
            var checkboxRect = new Rectangle(
                selectorArea.X + S(2),
                selectorArea.Y + S(2),
                selectorArea.Width - S(4),
                selectorArea.Height - S(4)
            );

            if (state.IsSelected)
            {
                // Filled checkbox with rounded corners
                var checkFill = state.IsEnabled ? colors.SelectedBorder : colors.DisabledBackground;
                using (var brush = new SolidBrush(checkFill))
                using (var path = CreateRoundedRectanglePath(checkboxRect, S(3)))
                {
                    graphics.FillPath(brush, path);
                }

                // Checkmark
                DrawCheckmark(graphics, checkboxRect, state.IsEnabled ? colors.SelectedIndicator : colors.DisabledText);
            }
            else
            {
                // Outlined checkbox
                var borderColor = state.IsEnabled ? colors.Border : colors.DisabledBorder;
                using (var pen = new Pen(borderColor, SF(1.5f)))
                using (var path = CreateRoundedRectanglePath(checkboxRect, S(3)))
                {
                    graphics.DrawPath(pen, path);
                }
            }
        }

        private void DrawCheckmark(Graphics graphics, Rectangle rect, Color color)
        {
            using (var pen = new Pen(color, SF(2f)))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                // Draw checkmark path
                var points = new PointF[]
                {
                    new PointF(rect.X + 4, rect.Y + rect.Height / 2),
                    new PointF(rect.X + rect.Width / 2 - 1, rect.Y + rect.Height - 6),
                    new PointF(rect.X + rect.Width - 4, rect.Y + 4)
                };

                graphics.DrawLines(pen, points);
            }
        }

        private void DrawContent(Graphics graphics, SimpleItem item, Rectangle contentArea, Rectangle selectorArea, RadioItemState state, FlatColors colors)
        {
            int currentX = Math.Max(selectorArea.Right + ComponentSpacing, contentArea.Left);

            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                int sz = Math.Min(IconSize, Math.Max(12, contentArea.Height - 6));
                var iconRect = new Rectangle(currentX, contentArea.Y + (contentArea.Height - sz) / 2, sz, sz);
                var iconPath = RadioGroupIconHelpers.GetItemIconPath(item.ImagePath);
                var iconColor = RadioGroupIconHelpers.GetIconColor(_theme, _useThemeColors, state.IsSelected, !state.IsEnabled);
                RadioGroupIconHelpers.PaintIcon(graphics, iconRect, iconPath, iconColor, _theme, _useThemeColors, _controlStyle);
                currentX += sz + ComponentSpacing;
            }

            bool hasSubtitle = !string.IsNullOrEmpty(item.SubText);
            var subtitleFont = hasSubtitle ? RadioGroupFontHelpers.GetSubtextFont(_controlStyle, _theme) : null;
            if (!string.IsNullOrEmpty(item.Text))
            {
                int textWidth = Math.Max(0, contentArea.Right - currentX);
                int titleHeight = contentArea.Height;
                if (hasSubtitle)
                {
                    var titleSize = TextUtils.MeasureText(graphics, item.Text, _textFont);
                    titleHeight = Math.Max(1, Math.Min(contentArea.Height - 1, (int)Math.Ceiling(titleSize.Height)));
                }

                var textRect = new Rectangle(currentX, contentArea.Y, textWidth, titleHeight);
                using var brush = new SolidBrush(state.IsEnabled ? colors.Text : colors.DisabledText);
                var fmt = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
                graphics.DrawString(item.Text, _textFont, brush, textRect, fmt);
            }

            if (hasSubtitle)
            {
                int textWidth = Math.Max(0, contentArea.Right - currentX);
                var titleSize = TextUtils.MeasureText(graphics, item.Text ?? string.Empty, _textFont);
                int titleHeight = Math.Max(1, Math.Min(contentArea.Height - 1, (int)Math.Ceiling(titleSize.Height)));
                int subtitleHeight = Math.Max(0, contentArea.Height - titleHeight);
                var subtitleRect = new Rectangle(currentX, contentArea.Y + titleHeight, textWidth, subtitleHeight);
                using var brush = new SolidBrush(state.IsEnabled ? colors.SubtitleText : colors.DisabledText);
                var fmt = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
                graphics.DrawString(item.SubText, subtitleFont, brush, subtitleRect, fmt);
                subtitleFont?.Dispose();
            }
        }

        private void DrawFocusIndicator(Graphics graphics, Rectangle rectangle, FlatColors colors)
        {
            using (var pen = new Pen(colors.FocusBorder, SF(2f)))
            using (var path = CreateRoundedRectanglePath(Rectangle.Inflate(rectangle, S(1), S(1)), S(CornerRadius + 1)))
            {
                graphics.DrawPath(pen, path);
            }
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
        #endregion

        #region Measurement
        public Size MeasureItem(SimpleItem item, Graphics graphics)
        {
            if (item == null) return new Size(120, MinItemHeight);

            int width = S(ItemPadding); // Left padding
            int height = S(MinItemHeight);

            // Selector width
            width += S(SelectorSize) + S(ComponentSpacing);

            // Icon width - account for image if present
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                width += IconSize + ComponentSpacing;
                // Ensure minimum height accommodates the image
                height = Math.Max(height, IconSize + S(ItemPadding));
            }

            // Text width
            if (!string.IsNullOrEmpty(item.Text))
            {
                var textSize =  TextUtils.MeasureText(graphics,item.Text, _textFont);
                width += (int)Math.Ceiling(textSize.Width);
                
                // Account for subtitle
                if (!string.IsNullOrEmpty(item.SubText))
                {
                    using var subtitleFont = RadioGroupFontHelpers.GetSubtextFont(_controlStyle, _theme);
                    var subtitleSize = TextUtils.MeasureText(graphics, item.SubText, subtitleFont);
                    height = Math.Max(height, (int)Math.Ceiling(textSize.Height + subtitleSize.Height) + S(ItemPadding));
                }
                else
                {
                    height = Math.Max(height, (int)Math.Ceiling(textSize.Height) + S(ItemPadding));
                }
            }

            width += S(ItemPadding); // Right padding

            return new Size(width, height);
        }

        public Rectangle GetContentArea(Rectangle itemRectangle)
        {
            return new Rectangle(
                itemRectangle.X + S(ItemPadding) / 2,
                itemRectangle.Y + S(ItemPadding) / 2,
                Math.Max(0, itemRectangle.Width - S(ItemPadding)),
                Math.Max(0, itemRectangle.Height - S(ItemPadding))
            );
        }

        public Rectangle GetSelectorArea(Rectangle itemRectangle)
        {
            var contentArea = GetContentArea(itemRectangle);
            int selectorSize = Math.Min(S(SelectorSize), Math.Max(0, contentArea.Height));
            return new Rectangle(
                contentArea.X,
                contentArea.Y + (contentArea.Height - selectorSize) / 2,
                selectorSize,
                selectorSize
            );
        }
        #endregion

        #region Group Decorations
        public void RenderGroupDecorations(Graphics graphics, Rectangle groupRectangle, List<SimpleItem> items, List<Rectangle> itemRectangles, List<RadioItemState> states)
        {
            // Flat design typically doesn't need group decorations
        }

        public void Cleanup()
        {
            // No cached GDI+ resources — fonts are owned by the control
        }
        #endregion

        #region Helper Methods
        private FlatColors GetStateColors(RadioItemState state)
        {
            if (!_useThemeColors || _theme == null)
            {
                return new FlatColors
                {
                    Border = StyleColors.GetBorder(_controlStyle),
                    SelectedBorder = StyleColors.GetPrimary(_controlStyle),
                    SelectedIndicator = Color.White,
                    Text = StyleColors.GetForeground(_controlStyle),
                    SubtitleText = Color.FromArgb(190, StyleColors.GetForeground(_controlStyle)),
                    DisabledText = Color.FromArgb(150, StyleColors.GetForeground(_controlStyle)),
                    DisabledBorder = Color.FromArgb(140, StyleColors.GetBorder(_controlStyle)),
                    HoverBackground = StyleColors.GetHover(_controlStyle),
                    SelectedBackground = StyleColors.GetSelection(_controlStyle),
                    DisabledBackground = Color.FromArgb(80, StyleColors.GetBackground(_controlStyle)),
                    FocusBorder = StyleColors.GetPrimary(_controlStyle)
                };
            }

            return new FlatColors
            {
                Border = _theme.BorderColor,
                SelectedBorder = _theme.PrimaryColor,
                SelectedIndicator = _theme.ButtonForeColor,
                Text = _theme.ForeColor,
                SubtitleText = _theme.LabelForeColor,
                DisabledText = _theme.DisabledForeColor,
                DisabledBorder = _theme.DisabledBorderColor,
                HoverBackground = _theme.ButtonHoverBackColor,
                SelectedBackground = _theme.SelectedRowBackColor,
                DisabledBackground = _theme.DisabledBackColor,
                FocusBorder = _theme.PrimaryColor
            };
        }

        private class FlatColors
        {
            public Color Border { get; set; }
            public Color SelectedBorder { get; set; }
            public Color SelectedIndicator { get; set; }
            public Color Text { get; set; }
            public Color SubtitleText { get; set; }
            public Color DisabledText { get; set; }
            public Color DisabledBorder { get; set; }
            public Color HoverBackground { get; set; }
            public Color SelectedBackground { get; set; }
            public Color DisabledBackground { get; set; }
            public Color FocusBorder { get; set; }
        }
        #endregion
    }
}
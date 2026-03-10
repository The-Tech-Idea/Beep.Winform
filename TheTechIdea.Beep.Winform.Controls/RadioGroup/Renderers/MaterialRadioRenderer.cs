using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.RadioGroup.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.RadioGroup.Renderers
{
    /// <summary>
    /// Material Design 3 compliant radio group renderer with StyleColors support
    /// </summary>
    public class MaterialRadioRenderer : IRadioGroupRenderer, IImageAwareRenderer
    {
        private BaseControl _owner;
        private IBeepTheme _theme;
        private Font _textFont;
        private Size _maxImageSize = new Size(24, 24);
        private BeepControlStyle _controlStyle = BeepControlStyle.Material3;
        private bool _useThemeColors = true;

        #region Properties
        public string StyleName => "Material";
        public string DisplayName => "Material Design 3";
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

        // Material Design specifications
        private const int RadioSize = 20;
        private int IconSize => Math.Min(_maxImageSize.Width, _maxImageSize.Height);
        private const int MinItemHeight = 40;
        private const int ItemPadding = 16;
        private const int ComponentSpacing = 12;
        private const int StateLayerSize = 40;
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

            // Note: BeepImage theme will be handled when drawing
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

            // Draw state layer (hover/focus background)
            if (state.IsEnabled && (state.IsHovered || state.IsFocused))
            {
                DrawStateLayer(graphics, rectangle, colors, state.IsFocused);
            }

            // Calculate layout areas
            var contentArea = GetContentArea(rectangle);
            var radioArea = GetRadioArea(rectangle);
            
            // Draw radio button
            DrawRadioButton(graphics, radioArea, state, colors);

            // Draw content (icon + text)
            DrawContent(graphics, item, contentArea, radioArea, state, colors);

            // Draw ripple effect if pressed
            if (state.IsEnabled && state.IsPressed)
            {
                DrawRippleEffect(graphics, rectangle, colors);
            }

            if (state.IsFocused)
            {
                DrawFocusIndicator(graphics, rectangle, colors);
            }
        }

        private void DrawStateLayer(Graphics graphics, Rectangle rectangle, MaterialColors colors, bool isFocused)
        {
            Color stateColor = Color.FromArgb(isFocused ? 20 : 12, colors.Primary);
            
            using (var brush = new SolidBrush(stateColor))
            {
                // Create rounded rectangle for state layer
                using (var path = CreateRoundedRectanglePath(rectangle, S(4)))
                {
                    graphics.FillPath(brush, path);
                }
            }
        }

        private void DrawRadioButton(Graphics graphics, Rectangle radioArea, RadioItemState state, MaterialColors colors)
        {
            var center = new Point(
                radioArea.X + radioArea.Width / 2,
                radioArea.Y + radioArea.Height / 2
            );

            // Check if multiple selection is enabled
            if (AllowMultipleSelection)
            {
                // Draw Material checkbox
                DrawMaterialCheckbox(graphics, radioArea, state, colors);
            }
            else
            {
                // Draw Material radio button
                DrawMaterialRadio(graphics, center, state, colors);
            }
        }

        private void DrawMaterialRadio(Graphics graphics, Point center, RadioItemState state, MaterialColors colors)
        {
            int outerRadius = S(RadioSize) / 2;
            int innerRadius = Math.Max(2, outerRadius - S(2));

            // Outer circle
            Color borderColor = state.IsEnabled
                ? (state.IsSelected ? colors.Primary : colors.Outline)
                : colors.Disabled;

            using (var pen = new Pen(borderColor, SF(2f)))
            {
                var outerRect = new Rectangle(
                    center.X - outerRadius,
                    center.Y - outerRadius,
                    outerRadius * 2,
                    outerRadius * 2
                );
                graphics.DrawEllipse(pen, outerRect);
            }

            // Inner filled circle (when selected)
            if (state.IsSelected)
            {
                var fillColor = state.IsEnabled ? colors.Primary : colors.Disabled;
                using (var brush = new SolidBrush(fillColor))
                {
                    int fillRadius = Math.Max(2, innerRadius - S(4));
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

        private void DrawMaterialCheckbox(Graphics graphics, Rectangle checkboxArea, RadioItemState state, MaterialColors colors)
        {
            var checkboxRect = new Rectangle(
                checkboxArea.X + S(2),
                checkboxArea.Y + S(2),
                checkboxArea.Width - S(4),
                checkboxArea.Height - S(4)
            );

            if (state.IsSelected)
            {
                // Filled checkbox with Material Design styling
                var fillColor = state.IsEnabled ? colors.Primary : colors.Disabled;
                using (var brush = new SolidBrush(fillColor))
                {
                    graphics.FillRectangle(brush, checkboxRect);
                }

                // Material Design checkmark
                DrawMaterialCheckmark(graphics, checkboxRect, colors.OnPrimary);
            }
            else
            {
                // Outlined checkbox
                using (var pen = new Pen(colors.Outline, SF(2f)))
                {
                    graphics.DrawRectangle(pen, checkboxRect);
                }
            }
        }

        private void DrawMaterialCheckmark(Graphics graphics, Rectangle rect, Color color)
        {
            using (var pen = new Pen(color, SF(2f)))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                // Material Design checkmark proportions
                var points = new PointF[]
                {
                    new PointF(rect.X + rect.Width * 0.2f, rect.Y + rect.Height * 0.5f),
                    new PointF(rect.X + rect.Width * 0.45f, rect.Y + rect.Height * 0.7f),
                    new PointF(rect.X + rect.Width * 0.8f, rect.Y + rect.Height * 0.3f)
                };

                graphics.DrawLines(pen, points);
            }
        }

        private void DrawContent(Graphics graphics, SimpleItem item, Rectangle contentArea, Rectangle radioArea, RadioItemState state, MaterialColors colors)
        {
            int currentX = radioArea.Right + ComponentSpacing;
            currentX = Math.Max(currentX, contentArea.Left);
            // Draw icon if present
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                int sz = Math.Min(IconSize, Math.Max(12, contentArea.Height - 4));
                var iconRect = new Rectangle(
                    currentX,
                    contentArea.Y + (contentArea.Height - sz) / 2,
                    sz,
                    sz
                );
                var iconPath = RadioGroupIconHelpers.GetItemIconPath(item.ImagePath);
                var iconColor = RadioGroupIconHelpers.GetIconColor(_theme, _useThemeColors, state.IsSelected, !state.IsEnabled);
                RadioGroupIconHelpers.PaintIcon(graphics, iconRect, iconPath, iconColor, _theme, _useThemeColors, _controlStyle);
                currentX += sz + ComponentSpacing;
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

                var textColor = state.IsEnabled ? colors.OnSurface : colors.Disabled;
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

        private void DrawRippleEffect(Graphics graphics, Rectangle rectangle, MaterialColors colors)
        {
            // Simple ripple effect - could be enhanced with animation
            Color rippleColor = Color.FromArgb(24, colors.Primary);
            
            using (var brush = new SolidBrush(rippleColor))
            using (var path = CreateRoundedRectanglePath(rectangle, S(StateLayerSize)))
            {
                graphics.FillPath(brush, path);
            }
        }

        private void DrawFocusIndicator(Graphics graphics, Rectangle rectangle, MaterialColors colors)
        {
            var focusRect = Rectangle.Inflate(rectangle, -1, -1);
            using (var path = CreateRoundedRectanglePath(focusRect, S(6)))
            using (var pen = new Pen(colors.Primary, SF(2f)))
            {
                graphics.DrawPath(pen, path);
            }
        }
        #endregion

        #region Measurement
        public Size MeasureItem(SimpleItem item, Graphics graphics)
        {
            if (item == null) return new Size(120, MinItemHeight);

            int width = S(ItemPadding); // Left padding
            int height = S(MinItemHeight);

            // Radio button width (includes state layer)
            width += S(StateLayerSize) + S(ComponentSpacing);

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
                height = Math.Max(height, (int)Math.Ceiling(textSize.Height) + S(ItemPadding));
            }

            width += S(ItemPadding); // Right padding

            return new Size(width, height);
        }

        public Rectangle GetContentArea(Rectangle itemRectangle)
        {
            var left = itemRectangle.X + Math.Max(S(4), S(ItemPadding) / 2);
            var top = itemRectangle.Y + Math.Max(S(2), S(ItemPadding) / 2);
            var width = Math.Max(0, itemRectangle.Width - Math.Max(S(8), S(ItemPadding)));
            var height = Math.Max(0, itemRectangle.Height - Math.Max(S(4), S(ItemPadding)));
            return new Rectangle(left, top, width, height);
        }

        public Rectangle GetRadioArea(Rectangle itemRectangle)
        {
            var contentArea = GetContentArea(itemRectangle);
            return new Rectangle(
                contentArea.X,
                contentArea.Y + (contentArea.Height - S(RadioSize)) / 2,
                S(RadioSize),
                S(RadioSize)
            );
        }
        
        public Rectangle GetSelectorArea(Rectangle itemRectangle)
        {
            var contentArea = GetContentArea(itemRectangle);
            return new Rectangle(
                contentArea.X,
                contentArea.Y + (contentArea.Height - S(StateLayerSize)) / 2,
                S(StateLayerSize),
                S(StateLayerSize)
            );
        }
        #endregion

        #region Group Decorations
        public void RenderGroupDecorations(Graphics graphics, Rectangle groupRectangle, List<SimpleItem> items, List<Rectangle> itemRectangles, List<RadioItemState> states)
        {
            // Material Design groups typically don't have additional decorations
            // Could add subtle borders or backgrounds here if needed
        }
        #endregion

        #region Helper Methods
        private MaterialColors GetStateColors(RadioItemState state)
        {
            // Use StyleColors if not using theme colors or no theme available
            if (!_useThemeColors || _theme == null)
            {
                return new MaterialColors
                {
                    Primary = StyleColors.GetPrimary(_controlStyle),
                    OnPrimary = Color.White,
                    Surface = StyleColors.GetBackground(_controlStyle),
                    OnSurface = StyleColors.GetForeground(_controlStyle),
                    OnSurfaceVariant = Color.FromArgb(128, StyleColors.GetForeground(_controlStyle)),
                    Outline = StyleColors.GetBorder(_controlStyle),
                    Hover = StyleColors.GetHover(_controlStyle),
                    Selection = StyleColors.GetSelection(_controlStyle),
                    Disabled = Color.FromArgb(150, StyleColors.GetForeground(_controlStyle))
                };
            }

            return new MaterialColors
            {
                Primary = _theme.PrimaryColor,
                OnPrimary = _theme.ButtonForeColor,
                Surface = _theme.BackgroundColor,
                OnSurface = _theme.ForeColor,
                OnSurfaceVariant = _theme.SecondaryTextColor,
                Outline = _theme.BorderColor,
                Hover = _theme.ButtonHoverBackColor,
                Selection = _theme.SelectedRowBackColor,
                Disabled = _theme.DisabledForeColor
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

        private class MaterialColors
        {
            public Color Primary { get; set; }
            public Color OnPrimary { get; set; }
            public Color Surface { get; set; }
            public Color OnSurface { get; set; }
            public Color OnSurfaceVariant { get; set; }
            public Color Outline { get; set; }
            public Color Hover { get; set; }
            public Color Selection { get; set; }
            public Color Disabled { get; set; }
        }
        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;

namespace TheTechIdea.Beep.Winform.Controls.RadioGroup.Renderers
{
    /// <summary>
    /// Material Design 3 compliant radio group renderer with StyleColors support
    /// </summary>
    public class MaterialRadioRenderer : IRadioGroupRenderer, IImageAwareRenderer
    {
        private BaseControl _owner;
        private IBeepTheme _theme;
        private BeepImage _imageRenderer;
        private Font _textFont;
        private Size _maxImageSize = new Size(24, 24);
        private BeepControlStyle _controlStyle = BeepControlStyle.Material3;
        private bool _useThemeColors = true;

        #region Properties
        public string StyleName => "Material";
        public string DisplayName => "Material Design 3";
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

        // Material Design specifications
        private const int RadioSize = 20;
        private int IconSize => Math.Min(_maxImageSize.Width, _maxImageSize.Height);
        private const int MinItemHeight = 40;
        private const int ItemPadding = 16;
        private const int ComponentSpacing = 12;
        private const int StateLayerSize = 40;
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
            // Prefer owner's current font for consistency with control
            if (_owner != null && _owner.Font != null)
            {
                _textFont = _owner.Font;
            }
            else if (_theme != null && _theme.BodyMedium != null)
            {
                _textFont = new Font(_theme.BodyMedium.FontFamily, _theme.BodyMedium.FontSize);
            }
            else
            {
                _textFont = new Font("Segoe UI", 12f);
            }

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
            if (state.IsHovered || state.IsFocused)
            {
                DrawStateLayer(graphics, rectangle, colors);
            }

            // Calculate layout areas
            var contentArea = GetContentArea(rectangle);
            var radioArea = GetRadioArea(rectangle);
            
            // Draw radio button
            DrawRadioButton(graphics, radioArea, state, colors);

            // Draw content (icon + text)
            DrawContent(graphics, item, contentArea, radioArea, state, colors);

            // Draw ripple effect if pressed
            if (state.IsPressed)
            {
                DrawRippleEffect(graphics, rectangle, colors);
            }
        }

        private void DrawStateLayer(Graphics graphics, Rectangle rectangle, MaterialColors colors)
        {
            Color stateColor = Color.FromArgb(12, colors.Primary); // 12% opacity
            
            using (var brush = new SolidBrush(stateColor))
            {
                // Create rounded rectangle for state layer
                using (var path = CreateRoundedRectanglePath(rectangle, 4))
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
            bool isMultipleSelection = _owner.GetType().GetProperty("AllowMultipleSelection")?.GetValue(_owner) as bool? == true;

            if (isMultipleSelection)
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
            int outerRadius = RadioSize / 2;
            int innerRadius = outerRadius - 2;

            // Outer circle
            using (var pen = new Pen(state.IsSelected ? colors.Primary : colors.Outline, 2f))
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
                using (var brush = new SolidBrush(colors.Primary))
                {
                    int fillRadius = innerRadius - 4;
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
                checkboxArea.X + 2,
                checkboxArea.Y + 2,
                checkboxArea.Width - 4,
                checkboxArea.Height - 4
            );

            if (state.IsSelected)
            {
                // Filled checkbox with Material Design styling
                using (var brush = new SolidBrush(colors.Primary))
                {
                    graphics.FillRectangle(brush, checkboxRect);
                }

                // Material Design checkmark
                DrawMaterialCheckmark(graphics, checkboxRect, colors.OnPrimary);
            }
            else
            {
                // Outlined checkbox
                using (var pen = new Pen(colors.Outline, 2f))
                {
                    graphics.DrawRectangle(pen, checkboxRect);
                }
            }
        }

        private void DrawMaterialCheckmark(Graphics graphics, Rectangle rect, Color color)
        {
            using (var pen = new Pen(color, 2f))
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

                _imageRenderer.ImagePath = item.ImagePath;
                _imageRenderer.Draw(graphics, iconRect);
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

                var textColor = state.IsEnabled ? colors.OnSurface : colors.OnSurfaceVariant;
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
            using (var path = CreateRoundedRectanglePath(rectangle, StateLayerSize))
            {
                graphics.FillPath(brush, path);
            }
        }
        #endregion

        #region Measurement
        public Size MeasureItem(SimpleItem item, Graphics graphics)
        {
            if (item == null) return new Size(120, MinItemHeight);

            int width = ItemPadding; // Left padding
            int height = MinItemHeight;

            // Radio button width (includes state layer)
            width += StateLayerSize + ComponentSpacing;

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
            var left = itemRectangle.X + Math.Max(4, ItemPadding / 2);
            var top = itemRectangle.Y + Math.Max(2, ItemPadding / 2);
            var width = Math.Max(0, itemRectangle.Width - Math.Max(8, ItemPadding));
            var height = Math.Max(0, itemRectangle.Height - Math.Max(4, ItemPadding));
            return new Rectangle(left, top, width, height);
        }

        public Rectangle GetRadioArea(Rectangle itemRectangle)
        {
            var contentArea = GetContentArea(itemRectangle);
            return new Rectangle(
                contentArea.X,
                contentArea.Y + (contentArea.Height - RadioSize) / 2,
                RadioSize,
                RadioSize
            );
        }
        
        public Rectangle GetSelectorArea(Rectangle itemRectangle)
        {
            var contentArea = GetContentArea(itemRectangle);
            return new Rectangle(
                contentArea.X,
                contentArea.Y + (contentArea.Height - StateLayerSize) / 2,
                StateLayerSize,
                StateLayerSize
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
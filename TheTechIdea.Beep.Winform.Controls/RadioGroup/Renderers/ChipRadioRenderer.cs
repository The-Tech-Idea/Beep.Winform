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
    /// Chip/pill-Style renderer for modern tag-like selection with StyleColors support
    /// </summary>
    public class ChipRadioRenderer : IRadioGroupRenderer, IImageAwareRenderer
    {
        private BaseControl _owner;
        private IBeepTheme _theme;
        private Font _textFont;
        private Size _maxImageSize = new Size(24, 24);
        private BeepControlStyle _controlStyle = BeepControlStyle.PillRail;
        private bool _useThemeColors = true;

        #region Properties
        public string StyleName => "Chip";
        public string DisplayName => "Chip/Pill Style";
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

        // Chip design specifications
        private int IconSize => Math.Min(_maxImageSize.Width, _maxImageSize.Height);
        private const int MinItemHeight = 36;
        private const int ItemPadding = 12;
        private const int ComponentSpacing = 8;
        private const int ChipRadius = 16;
        private const int CloseButtonSize = 12;
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

            // Get chip area (centered vertically)
            var chipRect = GetChipRectangle(rectangle);
            
            // Get colors based on state
            var colors = GetStateColors(state);

            // Draw chip background
            DrawChipBackground(graphics, chipRect, state, colors);

            // Draw chip border
            DrawChipBorder(graphics, chipRect, state, colors);

            // Draw chip content
            DrawChipContent(graphics, item, chipRect, state, colors);
        }

        private Rectangle GetChipRectangle(Rectangle itemRectangle)
        {
            var chipWidth = CalculateChipWidth(itemRectangle.Width);
            int chipHeight = Math.Max(0, itemRectangle.Height);
            return new Rectangle(itemRectangle.X, itemRectangle.Y, chipWidth, chipHeight);
        }

        private int CalculateChipWidth(int maxWidth)
        {
            return Math.Max(0, maxWidth);
        }

        private void DrawChipBackground(Graphics graphics, Rectangle chipRect, RadioItemState state, ChipColors colors)
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

            using (var brush = new SolidBrush(backgroundColor))
            using (var path = CreateChipPath(chipRect))
            {
                graphics.FillPath(brush, path);
            }
        }

        private void DrawChipBorder(Graphics graphics, Rectangle chipRect, RadioItemState state, ChipColors colors)
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
            using (var path = CreateChipPath(chipRect))
            {
                graphics.DrawPath(pen, path);
            }
        }

        private void DrawChipContent(Graphics graphics, SimpleItem item, Rectangle chipRect, RadioItemState state, ChipColors colors)
        {
            var contentRect = new Rectangle(
                chipRect.X + ItemPadding,
                chipRect.Y,
                Math.Max(0, chipRect.Width - (ItemPadding * 2)),
                chipRect.Height
            );

            int reserveRight = (state.IsSelected && AllowsDeselection()) ? (CloseButtonSize + 6) : 0;
            var contentClip = new Rectangle(contentRect.X, contentRect.Y, Math.Max(0, contentRect.Width - reserveRight), contentRect.Height);
            DrawContent(graphics, item, contentClip, state, colors);

            if (state.IsSelected && AllowsDeselection())
            {
                var closeRect = new Rectangle(
                    contentRect.Right - CloseButtonSize,
                    contentRect.Y + (contentRect.Height - CloseButtonSize) / 2,
                    CloseButtonSize,
                    CloseButtonSize
                );
                DrawCloseButton(graphics, closeRect, colors.SelectedText);
            }
        }

        private void DrawContent(Graphics graphics, SimpleItem item, Rectangle contentArea, RadioItemState state, ChipColors colors)
        {
            int currentX = contentArea.X;
            int maxRight = contentArea.Right;
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                int sz = Math.Min(IconSize, Math.Max(12, contentArea.Height - 6));
                sz = Math.Min(sz, Math.Max(0, maxRight - currentX));
                var iconRect = new Rectangle(currentX, contentArea.Y + (contentArea.Height - sz) / 2, sz, sz);
                var iconPath = RadioGroupIconHelpers.GetItemIconPath(item.ImagePath);
                var iconColor = RadioGroupIconHelpers.GetIconColor(_theme, _useThemeColors, state.IsSelected, !state.IsEnabled);
                RadioGroupIconHelpers.PaintIcon(graphics, iconRect, iconPath, iconColor, _theme, _useThemeColors, _controlStyle);
                currentX += sz + ComponentSpacing;
            }

            if (!string.IsNullOrEmpty(item.Text))
            {
                var textRect = new Rectangle(currentX, contentArea.Y, Math.Max(0, contentArea.Right - currentX), contentArea.Height);
                var color = state.IsEnabled ? (state.IsSelected ? colors.SelectedText : colors.Text) : colors.DisabledText;
                using var brush = new SolidBrush(color);
                var fmt = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
                graphics.DrawString(item.Text, _textFont, brush, textRect, fmt);
            }
        }

        private void DrawCloseButton(Graphics graphics, Rectangle rect, Color color)
        {
            using (var pen = new Pen(color, 1.5f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                // Draw X
                graphics.DrawLine(pen, 
                    rect.X + 4, rect.Y + 4, 
                    rect.Right - 4, rect.Bottom - 4);
                graphics.DrawLine(pen, 
                    rect.Right - 4, rect.Y + 4, 
                    rect.X + 4, rect.Bottom - 4);
            }
        }

        private GraphicsPath CreateChipPath(Rectangle rect)
        {
            var path = new GraphicsPath();
            if (rect.Width <= 1 || rect.Height <= 1)
            {
                path.AddRectangle(rect);
                return path;
            }
            int radius = rect.Height / 2; // Perfect circle ends

            // Left arc
            path.AddArc(rect.X, rect.Y, rect.Height, rect.Height, 90, 180);
            
            // Top line
            path.AddLine(rect.X + radius, rect.Y, rect.Right - radius, rect.Y);
            
            // Right arc
            path.AddArc(rect.Right - rect.Height, rect.Y, rect.Height, rect.Height, 270, 180);
            
            // Bottom line
            path.AddLine(rect.Right - radius, rect.Bottom, rect.X + radius, rect.Bottom);
            
            path.CloseFigure();
            return path;
        }

        private bool AllowsDeselection()
        {
            // Check if multiple selection is enabled
            return _owner.GetType().GetProperty("AllowMultipleSelection")?.GetValue(_owner) as bool? == true;
        }
        #endregion

        #region Measurement
        public Size MeasureItem(SimpleItem item, Graphics graphics)
        {
            if (item == null) return new Size(80, MinItemHeight);

            int width = ItemPadding * 2; // Left and right padding
            int height = MinItemHeight;

            // Icon width - account for image if present
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                width += IconSize + ComponentSpacing;
                // Ensure minimum height accommodates the image
                height = Math.Max(height, IconSize + 8); // 8px padding around icon
            }

            // Text width
            if (!string.IsNullOrEmpty(item.Text))
            {
                var textSize = TextUtils.MeasureText(graphics,item.Text, _textFont);
                width += (int)Math.Ceiling(textSize.Width);
                height = Math.Max(height, (int)Math.Ceiling(textSize.Height) + 8);
            }

            return new Size(width, height);
        }

        public Rectangle GetContentArea(Rectangle itemRectangle)
        {
            var chipRect = GetChipRectangle(itemRectangle);
            return new Rectangle(
                chipRect.X + ItemPadding,
                chipRect.Y,
                Math.Max(0, chipRect.Width - (ItemPadding * 2)),
                chipRect.Height
            );
        }

        public Rectangle GetSelectorArea(Rectangle itemRectangle)
        {
            // For chips, the entire chip is the selector
            return GetChipRectangle(itemRectangle);
        }
        #endregion

        #region Group Decorations
        public void RenderGroupDecorations(Graphics graphics, Rectangle groupRectangle, List<SimpleItem> items, List<Rectangle> itemRectangles, List<RadioItemState> states)
        {
            // Chips typically don't need group decorations
            // Could add a subtle background or container here if needed
        }
        #endregion

        #region Helper Methods
        private ChipColors GetStateColors(RadioItemState state)
        {
            // Use StyleColors if not using theme colors or no theme available
            if (!_useThemeColors || _theme == null)
            {
                var primary = StyleColors.GetPrimary(_controlStyle);
                var background = StyleColors.GetBackground(_controlStyle);
                var foreground = StyleColors.GetForeground(_controlStyle);
                var border = StyleColors.GetBorder(_controlStyle);
                var hover = StyleColors.GetHover(_controlStyle);
                var selection = StyleColors.GetSelection(_controlStyle);
                
                return new ChipColors
                {
                    Background = StyleColors.GetSecondary(_controlStyle),
                    HoverBackground = hover,
                    SelectedBackground = primary,
                    DisabledBackground = Color.FromArgb(200, background),
                    Border = border,
                    HoverBorder = Color.FromArgb(180, primary),
                    SelectedBorder = primary,
                    FocusBorder = primary,
                    DisabledBorder = Color.FromArgb(150, border),
                    Text = foreground,
                    SelectedText = Color.White,
                    DisabledText = Color.FromArgb(128, foreground)
                };
            }

            return new ChipColors
            {
                Background = _theme.PanelBackColor,
                HoverBackground = _theme.ButtonHoverBackColor,
                SelectedBackground = _theme.PrimaryColor,
                DisabledBackground = _theme.DisabledBackColor,
                Border = _theme.BorderColor,
                HoverBorder = _theme.ButtonHoverBorderColor,
                SelectedBorder = _theme.PrimaryColor,
                FocusBorder = _theme.PrimaryColor,
                DisabledBorder = _theme.DisabledBorderColor,
                Text = _theme.ForeColor,
                SelectedText = _theme.ButtonForeColor,
                DisabledText = _theme.DisabledForeColor
            };
        }

        private class ChipColors
        {
            public Color Background { get; set; }
            public Color HoverBackground { get; set; }
            public Color SelectedBackground { get; set; }
            public Color DisabledBackground { get; set; }
            public Color Border { get; set; }
            public Color HoverBorder { get; set; }
            public Color SelectedBorder { get; set; }
            public Color FocusBorder { get; set; }
            public Color DisabledBorder { get; set; }
            public Color Text { get; set; }
            public Color SelectedText { get; set; }
            public Color DisabledText { get; set; }
        }
        #endregion
    }
}
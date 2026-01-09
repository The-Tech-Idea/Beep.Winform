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
    /// Pill-shaped button renderer - fully rounded capsule buttons
    /// </summary>
    public class PillRadioRenderer : IRadioGroupRenderer, IImageAwareRenderer
    {
        private BaseControl _owner;
        private IBeepTheme _theme;
        private BeepImage _imageRenderer;
        private Font _textFont;
        private Size _maxImageSize = new Size(18, 18);
        private BeepControlStyle _controlStyle = BeepControlStyle.PillRail;
        private bool _useThemeColors = true;

        #region Properties
        public string StyleName => "Pill";
        public string DisplayName => "Pill Buttons";
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

        // Pill design specifications
        private int IconSize => Math.Min(_maxImageSize.Width, _maxImageSize.Height);
        private const int MinPillHeight = 36;
        private const int MinPillWidth = 80;
        private const int PillPadding = 16;
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
                ? new Font(_theme.LabelMedium.FontFamily, _theme.LabelMedium.FontSize, FontStyle.Regular)
                : new Font("Segoe UI", 11f));
        }
        #endregion

        #region Rendering
        public void RenderItem(Graphics graphics, SimpleItem item, Rectangle rectangle, RadioItemState state)
        {
            if (graphics == null || item == null) return;

            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var colors = GetStateColors(state);
            var pillRect = GetPillRectangle(rectangle);

            // Draw pill background
            DrawPillBackground(graphics, pillRect, state, colors);

            // Draw pill border
            DrawPillBorder(graphics, pillRect, state, colors);

            // Draw content (icon + text)
            DrawContent(graphics, item, pillRect, state, colors);
        }

        private Rectangle GetPillRectangle(Rectangle itemRectangle)
        {
            // Center the pill vertically within the item rectangle
            return new Rectangle(
                itemRectangle.X,
                itemRectangle.Y + (itemRectangle.Height - MinPillHeight) / 2,
                itemRectangle.Width,
                MinPillHeight
            );
        }

        private void DrawPillBackground(Graphics graphics, Rectangle pillRect, RadioItemState state, PillColors colors)
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

            using (var path = CreatePillPath(pillRect))
            using (var brush = new SolidBrush(backgroundColor))
            {
                graphics.FillPath(brush, path);
            }
        }

        private void DrawPillBorder(Graphics graphics, Rectangle pillRect, RadioItemState state, PillColors colors)
        {
            Color borderColor;
            float borderWidth;
            
            if (state.IsSelected)
            {
                borderColor = colors.SelectedBorder;
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

            using (var path = CreatePillPath(pillRect))
            using (var pen = new Pen(borderColor, borderWidth))
            {
                graphics.DrawPath(pen, path);
            }
        }

        private void DrawContent(Graphics graphics, SimpleItem item, Rectangle pillRect, RadioItemState state, PillColors colors)
        {
            var contentRect = new Rectangle(
                pillRect.X + PillPadding,
                pillRect.Y,
                pillRect.Width - (PillPadding * 2),
                pillRect.Height
            );

            int totalContentWidth = 0;
            
            // Calculate total content width for centering
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                totalContentWidth += IconSize + ComponentSpacing;
            }
            if (!string.IsNullOrEmpty(item.Text))
            {
                var textSize = TextUtils.MeasureText(graphics, item.Text, _textFont);
                totalContentWidth += (int)Math.Ceiling(textSize.Width);
            }
            
            // Center content
            int currentX = contentRect.X + (contentRect.Width - totalContentWidth) / 2;

            // Draw icon if present
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                int sz = Math.Min(IconSize, Math.Max(12, contentRect.Height - 8));
                var iconRect = new Rectangle(
                    currentX,
                    contentRect.Y + (contentRect.Height - sz) / 2,
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
                Color textColor;
                if (!state.IsEnabled)
                {
                    textColor = colors.DisabledText;
                }
                else if (state.IsSelected)
                {
                    textColor = colors.SelectedText;
                }
                else
                {
                    textColor = colors.Text;
                }
                
                using (var brush = new SolidBrush(textColor))
                {
                    var stringFormat = new StringFormat
                    {
                        Alignment = StringAlignment.Near,
                        LineAlignment = StringAlignment.Center,
                        Trimming = StringTrimming.EllipsisCharacter
                    };
                    
                    var textRect = new Rectangle(currentX, contentRect.Y, contentRect.Right - currentX, contentRect.Height);
                    graphics.DrawString(item.Text, _textFont, brush, textRect, stringFormat);
                }
            }
        }

        private GraphicsPath CreatePillPath(Rectangle rect)
        {
            var path = new GraphicsPath();
            int radius = rect.Height / 2; // Perfect circle ends for pill shape

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
        #endregion

        #region Measurement
        public Size MeasureItem(SimpleItem item, Graphics graphics)
        {
            if (item == null) return new Size(MinPillWidth, MinPillHeight);

            int width = PillPadding * 2;
            int height = MinPillHeight;

            // Icon width
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                width += IconSize + ComponentSpacing;
            }

            // Text width
            if (!string.IsNullOrEmpty(item.Text))
            {
                var textSize = TextUtils.MeasureText(graphics, item.Text, _textFont);
                width += (int)Math.Ceiling(textSize.Width);
            }

            // Ensure minimum width for pill shape aesthetics
            width = Math.Max(width, MinPillWidth);

            return new Size(width, height);
        }

        public Rectangle GetContentArea(Rectangle itemRectangle)
        {
            var pillRect = GetPillRectangle(itemRectangle);
            return new Rectangle(
                pillRect.X + PillPadding,
                pillRect.Y,
                pillRect.Width - (PillPadding * 2),
                pillRect.Height
            );
        }

        public Rectangle GetSelectorArea(Rectangle itemRectangle)
        {
            // For pill buttons, the entire pill is the selector
            return GetPillRectangle(itemRectangle);
        }
        #endregion

        #region Group Decorations
        public void RenderGroupDecorations(Graphics graphics, Rectangle groupRectangle, List<SimpleItem> items, List<Rectangle> itemRectangles, List<RadioItemState> states)
        {
            // Pill buttons typically don't need group decorations
            // They stand alone as individual pills
        }
        #endregion

        #region Helper Methods
        private PillColors GetStateColors(RadioItemState state)
        {
            if (!_useThemeColors || _theme == null)
            {
                var primary = StyleColors.GetPrimary(_controlStyle);
                var background = StyleColors.GetBackground(_controlStyle);
                var foreground = StyleColors.GetForeground(_controlStyle);
                var border = StyleColors.GetBorder(_controlStyle);
                var secondary = StyleColors.GetSecondary(_controlStyle);
                var hover = StyleColors.GetHover(_controlStyle);
                
                return new PillColors
                {
                    Background = secondary,
                    HoverBackground = hover,
                    SelectedBackground = primary,
                    DisabledBackground = Color.FromArgb(200, secondary),
                    Border = border,
                    HoverBorder = Color.FromArgb(180, primary),
                    SelectedBorder = primary,
                    Text = foreground,
                    SelectedText = Color.White,
                    DisabledText = Color.FromArgb(128, foreground)
                };
            }

            return new PillColors
            {
                Background = Color.FromArgb(245, _theme.BackgroundColor),
                HoverBackground = _theme.ButtonHoverBackColor,
                SelectedBackground = _theme.PrimaryColor,
                DisabledBackground = _theme.DisabledBackColor,
                Border = _theme.BorderColor,
                HoverBorder = _theme.ButtonHoverBorderColor,
                SelectedBorder = _theme.PrimaryColor,
                Text = _theme.ForeColor,
                SelectedText = _theme.ButtonForeColor,
                DisabledText = _theme.DisabledForeColor
            };
        }

        private class PillColors
        {
            public Color Background { get; set; }
            public Color HoverBackground { get; set; }
            public Color SelectedBackground { get; set; }
            public Color DisabledBackground { get; set; }
            public Color Border { get; set; }
            public Color HoverBorder { get; set; }
            public Color SelectedBorder { get; set; }
            public Color Text { get; set; }
            public Color SelectedText { get; set; }
            public Color DisabledText { get; set; }
        }
        #endregion
    }
}


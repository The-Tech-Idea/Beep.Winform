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
    /// iOS-style segmented control renderer - connected buttons with sliding selection
    /// </summary>
    public class SegmentedRadioRenderer : IRadioGroupRenderer, IImageAwareRenderer
    {
        private BaseControl _owner;
        private IBeepTheme _theme;
        private BeepImage _imageRenderer;
        private Font _textFont;
        private Size _maxImageSize = new Size(20, 20);
        private BeepControlStyle _controlStyle = BeepControlStyle.iOS15;
        private bool _useThemeColors = true;

        #region Properties
        public string StyleName => "Segmented";
        public string DisplayName => "iOS Segmented Control";
        public bool SupportsMultipleSelection => false; // Segmented controls are single-selection
        
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

        // Segmented control specifications
        private int IconSize => Math.Min(_maxImageSize.Width, _maxImageSize.Height);
        private const int MinSegmentHeight = 32;
        private const int MinSegmentWidth = 60;
        private const int SegmentPadding = 12;
        private const int ComponentSpacing = 6;
        private const int CornerRadius = 8;
        private const int DividerWidth = 1;
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
                : new Font("Segoe UI Semibold", 11f));
        }
        #endregion

        #region Rendering
        public void RenderItem(Graphics graphics, SimpleItem item, Rectangle rectangle, RadioItemState state)
        {
            if (graphics == null || item == null) return;

            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var colors = GetStateColors(state);

            // Draw segment background (selected segments get the selection indicator)
            if (state.IsSelected)
            {
                DrawSelectedSegment(graphics, rectangle, colors);
            }
            else if (state.IsHovered)
            {
                DrawHoveredSegment(graphics, rectangle, colors);
            }

            // Draw content (icon + text)
            DrawContent(graphics, item, rectangle, state, colors);
        }

        private void DrawSelectedSegment(Graphics graphics, Rectangle rect, SegmentedColors colors)
        {
            // iOS-style selected segment with rounded corners and shadow
            var segmentRect = new Rectangle(rect.X + 2, rect.Y + 2, rect.Width - 4, rect.Height - 4);
            
            // Draw shadow
            using (var shadowPath = CreateRoundedRectanglePath(new Rectangle(segmentRect.X + 1, segmentRect.Y + 1, segmentRect.Width, segmentRect.Height), CornerRadius - 2))
            using (var shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
            {
                graphics.FillPath(shadowBrush, shadowPath);
            }
            
            // Draw selected segment background
            using (var path = CreateRoundedRectanglePath(segmentRect, CornerRadius - 2))
            using (var brush = new SolidBrush(colors.SelectedBackground))
            {
                graphics.FillPath(brush, path);
            }
        }

        private void DrawHoveredSegment(Graphics graphics, Rectangle rect, SegmentedColors colors)
        {
            var segmentRect = new Rectangle(rect.X + 2, rect.Y + 2, rect.Width - 4, rect.Height - 4);
            
            using (var path = CreateRoundedRectanglePath(segmentRect, CornerRadius - 2))
            using (var brush = new SolidBrush(colors.HoverBackground))
            {
                graphics.FillPath(brush, path);
            }
        }

        private void DrawContent(Graphics graphics, SimpleItem item, Rectangle rect, RadioItemState state, SegmentedColors colors)
        {
            var contentRect = new Rectangle(
                rect.X + SegmentPadding,
                rect.Y,
                rect.Width - (SegmentPadding * 2),
                rect.Height
            );

            int currentX = contentRect.X;
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
            currentX = contentRect.X + (contentRect.Width - totalContentWidth) / 2;

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
                var textColor = state.IsEnabled 
                    ? (state.IsSelected ? colors.SelectedText : colors.Text) 
                    : colors.DisabledText;
                
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
        #endregion

        #region Measurement
        public Size MeasureItem(SimpleItem item, Graphics graphics)
        {
            if (item == null) return new Size(MinSegmentWidth, MinSegmentHeight);

            int width = SegmentPadding * 2;
            int height = MinSegmentHeight;

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

            return new Size(Math.Max(width, MinSegmentWidth), height);
        }

        public Rectangle GetContentArea(Rectangle itemRectangle)
        {
            return new Rectangle(
                itemRectangle.X + SegmentPadding,
                itemRectangle.Y,
                itemRectangle.Width - (SegmentPadding * 2),
                itemRectangle.Height
            );
        }

        public Rectangle GetSelectorArea(Rectangle itemRectangle)
        {
            // For segmented controls, the entire segment is the selector
            return itemRectangle;
        }
        #endregion

        #region Group Decorations
        public void RenderGroupDecorations(Graphics graphics, Rectangle groupRectangle, List<SimpleItem> items, List<Rectangle> itemRectangles, List<RadioItemState> states)
        {
            if (items == null || items.Count == 0 || itemRectangles.Count == 0) return;

            var colors = GetStateColors(new RadioItemState());

            // Calculate the full segmented control bounds
            var firstRect = itemRectangles[0];
            var lastRect = itemRectangles[itemRectangles.Count - 1];
            var fullRect = new Rectangle(
                firstRect.X,
                firstRect.Y,
                lastRect.Right - firstRect.X,
                firstRect.Height
            );

            // Draw the outer container (rounded rectangle background)
            using (var path = CreateRoundedRectanglePath(fullRect, CornerRadius))
            {
                // Background
                using (var brush = new SolidBrush(colors.Background))
                {
                    graphics.FillPath(brush, path);
                }
                
                // Border
                using (var pen = new Pen(colors.Border, 1f))
                {
                    graphics.DrawPath(pen, path);
                }
            }

            // Draw dividers between segments (but not next to selected segment)
            for (int i = 0; i < itemRectangles.Count - 1; i++)
            {
                bool currentSelected = states[i].IsSelected;
                bool nextSelected = states[i + 1].IsSelected;
                
                // Skip divider if either adjacent segment is selected
                if (currentSelected || nextSelected) continue;

                var dividerX = itemRectangles[i].Right;
                var dividerTop = fullRect.Y + 6;
                var dividerBottom = fullRect.Bottom - 6;

                using (var pen = new Pen(colors.Divider, DividerWidth))
                {
                    graphics.DrawLine(pen, dividerX, dividerTop, dividerX, dividerBottom);
                }
            }
        }
        #endregion

        #region Helper Methods
        private SegmentedColors GetStateColors(RadioItemState state)
        {
            if (!_useThemeColors || _theme == null)
            {
                var primary = StyleColors.GetPrimary(_controlStyle);
                var background = StyleColors.GetBackground(_controlStyle);
                var foreground = StyleColors.GetForeground(_controlStyle);
                var border = StyleColors.GetBorder(_controlStyle);
                var secondary = StyleColors.GetSecondary(_controlStyle);
                
                return new SegmentedColors
                {
                    Background = secondary,
                    SelectedBackground = Color.White,
                    HoverBackground = Color.FromArgb(40, primary),
                    Border = border,
                    Divider = Color.FromArgb(100, border),
                    Text = foreground,
                    SelectedText = foreground,
                    DisabledText = Color.FromArgb(128, foreground)
                };
            }

            return new SegmentedColors
            {
                Background = Color.FromArgb(240, _theme.BackgroundColor),
                SelectedBackground = _theme.PanelBackColor,
                HoverBackground = Color.FromArgb(40, _theme.PrimaryColor),
                Border = _theme.BorderColor,
                Divider = Color.FromArgb(100, _theme.BorderColor),
                Text = _theme.ForeColor,
                SelectedText = _theme.ForeColor,
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

        private class SegmentedColors
        {
            public Color Background { get; set; }
            public Color SelectedBackground { get; set; }
            public Color HoverBackground { get; set; }
            public Color Border { get; set; }
            public Color Divider { get; set; }
            public Color Text { get; set; }
            public Color SelectedText { get; set; }
            public Color DisabledText { get; set; }
        }
        #endregion
    }
}


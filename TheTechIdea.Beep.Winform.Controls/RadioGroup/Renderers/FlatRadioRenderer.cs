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
    /// Modern flat design renderer for radio group items (inspired by your design examples)
    /// </summary>
    public class FlatRadioRenderer : IRadioGroupRenderer, IImageAwareRenderer
    {
        private BaseControl _owner;
        private IBeepTheme _theme;
        private BeepImage _imageRenderer;
        private Font _textFont;
        private Size _maxImageSize = new Size(24, 24);

        #region Properties
        public string StyleName => "Flat";
        public string DisplayName => "Modern Flat Design";
        public bool SupportsMultipleSelection => true;

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
            
            if (state.IsSelected)
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
                using (var path = CreateRoundedRectanglePath(rectangle, CornerRadius))
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

            if (_owner.GetType().GetProperty("AllowMultipleSelection")?.GetValue(_owner) as bool? == true)
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
            int radius = SelectorSize / 2 - 1;

            // Outer circle - flat design with subtle border
            Color borderColor = state.IsSelected ? colors.SelectedBorder : colors.Border;
            Color fillColor = state.IsSelected ? colors.SelectedBorder : Color.Transparent;

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
            using (var pen = new Pen(borderColor, state.IsSelected ? 2f : 1.5f))
            {
                graphics.DrawEllipse(pen, outerRect);
            }

            // Draw inner dot if selected
            if (state.IsSelected)
            {
                int innerRadius = radius - 6;
                var innerRect = new Rectangle(
                    center.X - innerRadius,
                    center.Y - innerRadius,
                    innerRadius * 2,
                    innerRadius * 2
                );

                using (var brush = new SolidBrush(colors.SelectedIndicator))
                {
                    graphics.FillEllipse(brush, innerRect);
                }
            }
        }

        private void DrawFlatCheckbox(Graphics graphics, Rectangle selectorArea, RadioItemState state, FlatColors colors)
        {
            var checkboxRect = new Rectangle(
                selectorArea.X + 2,
                selectorArea.Y + 2,
                selectorArea.Width - 4,
                selectorArea.Height - 4
            );

            if (state.IsSelected)
            {
                // Filled checkbox with rounded corners
                using (var brush = new SolidBrush(colors.SelectedBorder))
                using (var path = CreateRoundedRectanglePath(checkboxRect, 3))
                {
                    graphics.FillPath(brush, path);
                }

                // Checkmark
                DrawCheckmark(graphics, checkboxRect, colors.SelectedIndicator);
            }
            else
            {
                // Outlined checkbox
                using (var pen = new Pen(colors.Border, 1.5f))
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
                _imageRenderer.ImagePath = item.ImagePath;
                _imageRenderer.Draw(graphics, iconRect);
                currentX += sz + ComponentSpacing;
            }

            var half = contentArea.Height / 2;
            if (!string.IsNullOrEmpty(item.Text))
            {
                var textRect = new Rectangle(currentX, contentArea.Y, Math.Max(0, contentArea.Right - currentX), half);
                using var brush = new SolidBrush(state.IsEnabled ? colors.Text : colors.DisabledText);
                var fmt = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
                graphics.DrawString(item.Text, _textFont, brush, textRect, fmt);
            }

            if (!string.IsNullOrEmpty(item.SubText))
            {
                using var subtitleFont = new Font(_textFont.FontFamily, Math.Max(6f, _textFont.Size * 0.85f));
                var subtitleRect = new Rectangle(currentX, contentArea.Y + half, Math.Max(0, contentArea.Right - currentX), half);
                using var brush = new SolidBrush(colors.SubtitleText);
                var fmt = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
                graphics.DrawString(item.SubText, subtitleFont, brush, subtitleRect, fmt);
            }
        }

        private void DrawFocusIndicator(Graphics graphics, Rectangle rectangle, FlatColors colors)
        {
            using (var pen = new Pen(colors.FocusBorder, 2f))
            using (var path = CreateRoundedRectanglePath(Rectangle.Inflate(rectangle, 1, 1), CornerRadius + 1))
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
                var textSize = graphics.MeasureString(item.Text, _textFont);
                width += (int)Math.Ceiling(textSize.Width);
                
                // Account for subtitle
                if (!string.IsNullOrEmpty(item.SubText))
                {
                    height = Math.Max(height, (int)(textSize.Height * 1.8f) + ItemPadding);
                }
                else
                {
                    height = Math.Max(height, (int)textSize.Height + ItemPadding);
                }
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
                contentArea.Y + (contentArea.Height - SelectorSize) / 2,
                SelectorSize,
                SelectorSize
            );
        }
        #endregion

        #region Group Decorations
        public void RenderGroupDecorations(Graphics graphics, Rectangle groupRectangle, List<SimpleItem> items, List<Rectangle> itemRectangles, List<RadioItemState> states)
        {
            // Flat design typically doesn't need group decorations
            // Could add subtle separation lines between groups if needed
        }
        #endregion

        #region Helper Methods
        private FlatColors GetStateColors(RadioItemState state)
        {
            if (_theme == null)
            {
                return new FlatColors
                {
                    Border = Color.FromArgb(209, 213, 219),
                    SelectedBorder = Color.FromArgb(34, 197, 94), // Green instead of blue
                    SelectedIndicator = Color.White,
                    Text = Color.FromArgb(17, 24, 39),
                    SubtitleText = Color.FromArgb(107, 114, 128),
                    DisabledText = Color.FromArgb(156, 163, 175),
                    HoverBackground = Color.FromArgb(240, 253, 244), // Light green
                    SelectedBackground = Color.FromArgb(220, 252, 231), // Light green
                    FocusBorder = Color.FromArgb(34, 197, 94) // Green focus
                };
            }

            return new FlatColors
            {
                Border = _theme.BorderColor,
                SelectedBorder = Color.FromArgb(34, 197, 94), // Force green theme
                SelectedIndicator = _theme.ButtonForeColor,
                Text = _theme.ForeColor,
                SubtitleText = _theme.LabelForeColor,
                DisabledText = _theme.DisabledForeColor,
                HoverBackground = Color.FromArgb(240, 253, 244),
                SelectedBackground = Color.FromArgb(220, 252, 231),
                FocusBorder = Color.FromArgb(34, 197, 94)
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
            public Color HoverBackground { get; set; }
            public Color SelectedBackground { get; set; }
            public Color FocusBorder { get; set; }
        }
        #endregion
    }
}
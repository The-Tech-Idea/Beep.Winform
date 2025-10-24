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
    /// Chip/pill-Style renderer for modern tag-like selection
    /// </summary>
    public class ChipRadioRenderer : IRadioGroupRenderer, IImageAwareRenderer
    {
        private BaseControl _owner;
        private IBeepTheme _theme;
        private BeepImage _imageRenderer;
        private Font _textFont;
        private Size _maxImageSize = new Size(24, 24);

        #region Properties
        public string StyleName => "Chip";
        public string DisplayName => "Chip/Pill ProgressBarStyle";
        public bool SupportsMultipleSelection => true;

        public Size MaxImageSize 
        { 
            get => _maxImageSize; 
            set => _maxImageSize = value; 
        }

        // Chip design specifications
        private int IconSize => Math.Min(_maxImageSize.Width, _maxImageSize.Height);
        private const int MinItemHeight = 32;
        private const int ItemPadding = 12;
        private const int ComponentSpacing = 8;
        private const int ChipRadius = 16;
        private const int CloseButtonSize = 12; // Add missing constant
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
                : new Font("Segoe UI", 11f));
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
            // Calculate chip width based on content
            var chipWidth = CalculateChipWidth(itemRectangle.Width);
            
            return new Rectangle(
                itemRectangle.X,
                itemRectangle.Y + (itemRectangle.Height - MinItemHeight) / 2,
                chipWidth,
                MinItemHeight
            );
        }

        private int CalculateChipWidth(int maxWidth)
        {
            // For chip Style, we want to auto-size to content
            // This is a simplified calculation - in real use, measure the actual content
            return Math.Min(maxWidth, 200); // Default reasonable width
        }

        private void DrawChipBackground(Graphics graphics, Rectangle chipRect, RadioItemState state, ChipColors colors)
        {
            Color backgroundColor = state.IsSelected ? colors.SelectedBackground : 
                                  state.IsHovered ? colors.HoverBackground : 
                                  colors.Background;

            using (var brush = new SolidBrush(backgroundColor))
            using (var path = CreateChipPath(chipRect))
            {
                graphics.FillPath(brush, path);
            }
        }

        private void DrawChipBorder(Graphics graphics, Rectangle chipRect, RadioItemState state, ChipColors colors)
        {
            Color borderColor = state.IsSelected ? colors.SelectedBorder :
                              state.IsHovered ? colors.HoverBorder :
                              colors.Border;

            float borderWidth = state.IsSelected ? 2f : 1f;

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
                chipRect.Width - (ItemPadding * 2),
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
                chipRect.Width - (ItemPadding * 2),
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
            if (_theme == null)
            {
                return new ChipColors
                {
                    Background = Color.FromArgb(245, 245, 245),
                    HoverBackground = Color.FromArgb(229, 231, 235),
                    SelectedBackground = Color.FromArgb(147, 51, 234), // Purple instead of blue
                    Border = Color.FromArgb(209, 213, 219),
                    HoverBorder = Color.FromArgb(156, 163, 175),
                    SelectedBorder = Color.FromArgb(124, 58, 237), // Purple
                    Text = Color.FromArgb(55, 65, 81),
                    SelectedText = Color.White,
                    DisabledText = Color.FromArgb(156, 163, 175)
                };
            }

            return new ChipColors
            {
                Background = Color.FromArgb(240, _theme.BackgroundColor),
                HoverBackground = Color.FromArgb(230, _theme.BackgroundColor),
                SelectedBackground = Color.FromArgb(147, 51, 234), // Force purple
                Border = _theme.BorderColor,
                HoverBorder = _theme.ButtonHoverBorderColor,
                SelectedBorder = Color.FromArgb(124, 58, 237), // Purple
                Text = _theme.ForeColor,
                SelectedText = Color.White,
                DisabledText = _theme.DisabledForeColor
            };
        }

        private class ChipColors
        {
            public Color Background { get; set; }
            public Color HoverBackground { get; set; }
            public Color SelectedBackground { get; set; }
            public Color Border { get; set; }
            public Color HoverBorder { get; set; }
            public Color SelectedBorder { get; set; }
            public Color Text { get; set; }
            public Color SelectedText { get; set; }
            public Color DisabledText { get; set; } // Add missing property
        }
        #endregion
    }
}
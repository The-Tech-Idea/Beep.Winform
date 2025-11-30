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
    /// Large touch-friendly tile renderer - Windows 8/10 style tiles with icon and text
    /// </summary>
    public class TileRadioRenderer : IRadioGroupRenderer, IImageAwareRenderer
    {
        private BaseControl _owner;
        private IBeepTheme _theme;
        private BeepImage _imageRenderer;
        private Font _titleFont;
        private Font _subtitleFont;
        private Size _maxImageSize = new Size(48, 48);
        private BeepControlStyle _controlStyle = BeepControlStyle.Metro;
        private bool _useThemeColors = true;

        #region Properties
        public string StyleName => "Tile";
        public string DisplayName => "Large Tiles";
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

        // Tile design specifications
        private int IconSize => Math.Min(_maxImageSize.Width, _maxImageSize.Height);
        private const int MinTileWidth = 120;
        private const int MinTileHeight = 100;
        private const int TilePadding = 16;
        private const int ComponentSpacing = 8;
        private const int CornerRadius = 4;
        private const int SelectionIndicatorSize = 24;
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
            _titleFont = new Font("Segoe UI Semibold", 12f);
            _subtitleFont = new Font("Segoe UI", 9f);
        }
        #endregion

        #region Rendering
        public void RenderItem(Graphics graphics, SimpleItem item, Rectangle rectangle, RadioItemState state)
        {
            if (graphics == null || item == null) return;

            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var colors = GetStateColors(state);
            var tileRect = GetTileRectangle(rectangle);

            // Draw tile background
            DrawTileBackground(graphics, tileRect, state, colors);

            // Draw tile border
            DrawTileBorder(graphics, tileRect, state, colors);

            // Draw selection indicator (checkmark in corner)
            if (state.IsSelected)
            {
                DrawSelectionIndicator(graphics, tileRect, colors);
            }

            // Draw content (icon + text)
            DrawContent(graphics, item, tileRect, state, colors);
        }

        private Rectangle GetTileRectangle(Rectangle itemRectangle)
        {
            // Use full item rectangle but ensure minimum size
            return new Rectangle(
                itemRectangle.X,
                itemRectangle.Y,
                Math.Max(itemRectangle.Width, MinTileWidth),
                Math.Max(itemRectangle.Height, MinTileHeight)
            );
        }

        private void DrawTileBackground(Graphics graphics, Rectangle tileRect, RadioItemState state, TileColors colors)
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

            using (var path = CreateRoundedRectanglePath(tileRect, CornerRadius))
            using (var brush = new SolidBrush(backgroundColor))
            {
                graphics.FillPath(brush, path);
            }
        }

        private void DrawTileBorder(Graphics graphics, Rectangle tileRect, RadioItemState state, TileColors colors)
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
            else if (state.IsFocused)
            {
                borderColor = colors.FocusBorder;
                borderWidth = 2f;
            }
            else
            {
                borderColor = colors.Border;
                borderWidth = 1f;
            }

            using (var path = CreateRoundedRectanglePath(tileRect, CornerRadius))
            using (var pen = new Pen(borderColor, borderWidth))
            {
                graphics.DrawPath(pen, path);
            }
        }

        private void DrawSelectionIndicator(Graphics graphics, Rectangle tileRect, TileColors colors)
        {
            // Draw checkmark circle in top-right corner
            var indicatorRect = new Rectangle(
                tileRect.Right - SelectionIndicatorSize - 8,
                tileRect.Y + 8,
                SelectionIndicatorSize,
                SelectionIndicatorSize
            );

            // Draw circle background
            using (var brush = new SolidBrush(colors.SelectionIndicator))
            {
                graphics.FillEllipse(brush, indicatorRect);
            }

            // Draw checkmark
            using (var pen = new Pen(Color.White, 2f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                var points = new PointF[]
                {
                    new PointF(indicatorRect.X + indicatorRect.Width * 0.25f, indicatorRect.Y + indicatorRect.Height * 0.5f),
                    new PointF(indicatorRect.X + indicatorRect.Width * 0.42f, indicatorRect.Y + indicatorRect.Height * 0.68f),
                    new PointF(indicatorRect.X + indicatorRect.Width * 0.75f, indicatorRect.Y + indicatorRect.Height * 0.32f)
                };

                graphics.DrawLines(pen, points);
            }
        }

        private void DrawContent(Graphics graphics, SimpleItem item, Rectangle tileRect, RadioItemState state, TileColors colors)
        {
            var contentRect = new Rectangle(
                tileRect.X + TilePadding,
                tileRect.Y + TilePadding,
                tileRect.Width - (TilePadding * 2),
                tileRect.Height - (TilePadding * 2)
            );

            int currentY = contentRect.Y;

            // Draw icon centered at top
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                int sz = Math.Min(IconSize, Math.Max(24, Math.Min(contentRect.Width, contentRect.Height - 40)));
                var iconRect = new Rectangle(
                    contentRect.X + (contentRect.Width - sz) / 2,
                    currentY,
                    sz,
                    sz
                );

                _imageRenderer.ImagePath = item.ImagePath;
                _imageRenderer.Draw(graphics, iconRect);
                currentY = iconRect.Bottom + ComponentSpacing;
            }

            // Draw title text centered
            if (!string.IsNullOrEmpty(item.Text))
            {
                Color textColor = state.IsEnabled 
                    ? (state.IsSelected ? colors.SelectedText : colors.Text) 
                    : colors.DisabledText;
                
                using (var brush = new SolidBrush(textColor))
                {
                    var stringFormat = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Near,
                        Trimming = StringTrimming.EllipsisCharacter
                    };
                    
                    var textRect = new Rectangle(contentRect.X, currentY, contentRect.Width, 24);
                    graphics.DrawString(item.Text, _titleFont, brush, textRect, stringFormat);
                    currentY += 24;
                }
            }

            // Draw subtitle text centered (if available)
            if (!string.IsNullOrEmpty(item.SubText))
            {
                Color subtitleColor = state.IsEnabled 
                    ? colors.SubtitleText 
                    : colors.DisabledText;
                
                using (var brush = new SolidBrush(subtitleColor))
                {
                    var stringFormat = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Near,
                        Trimming = StringTrimming.EllipsisCharacter
                    };
                    
                    var textRect = new Rectangle(contentRect.X, currentY, contentRect.Width, 18);
                    graphics.DrawString(item.SubText, _subtitleFont, brush, textRect, stringFormat);
                }
            }
        }
        #endregion

        #region Measurement
        public Size MeasureItem(SimpleItem item, Graphics graphics)
        {
            if (item == null) return new Size(MinTileWidth, MinTileHeight);

            int width = TilePadding * 2;
            int height = TilePadding * 2;

            // Icon height
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                height += IconSize + ComponentSpacing;
            }

            // Title text
            if (!string.IsNullOrEmpty(item.Text))
            {
                var textSize = TextUtils.MeasureText(graphics, item.Text, _titleFont);
                width = Math.Max(width, (int)Math.Ceiling(textSize.Width) + TilePadding * 2);
                height += 24;
            }

            // Subtitle text
            if (!string.IsNullOrEmpty(item.SubText))
            {
                var textSize = TextUtils.MeasureText(graphics, item.SubText, _subtitleFont);
                width = Math.Max(width, (int)Math.Ceiling(textSize.Width) + TilePadding * 2);
                height += 18;
            }

            return new Size(Math.Max(width, MinTileWidth), Math.Max(height, MinTileHeight));
        }

        public Rectangle GetContentArea(Rectangle itemRectangle)
        {
            var tileRect = GetTileRectangle(itemRectangle);
            return new Rectangle(
                tileRect.X + TilePadding,
                tileRect.Y + TilePadding,
                tileRect.Width - (TilePadding * 2),
                tileRect.Height - (TilePadding * 2)
            );
        }

        public Rectangle GetSelectorArea(Rectangle itemRectangle)
        {
            // For tiles, the entire tile is the selector
            return GetTileRectangle(itemRectangle);
        }
        #endregion

        #region Group Decorations
        public void RenderGroupDecorations(Graphics graphics, Rectangle groupRectangle, List<SimpleItem> items, List<Rectangle> itemRectangles, List<RadioItemState> states)
        {
            // Tiles typically don't need group decorations
            // They stand alone as individual tiles in a grid
        }
        #endregion

        #region Helper Methods
        private TileColors GetStateColors(RadioItemState state)
        {
            if (!_useThemeColors || _theme == null)
            {
                var primary = StyleColors.GetPrimary(_controlStyle);
                var background = StyleColors.GetBackground(_controlStyle);
                var foreground = StyleColors.GetForeground(_controlStyle);
                var border = StyleColors.GetBorder(_controlStyle);
                var secondary = StyleColors.GetSecondary(_controlStyle);
                var hover = StyleColors.GetHover(_controlStyle);
                var selection = StyleColors.GetSelection(_controlStyle);
                
                return new TileColors
                {
                    Background = background,
                    HoverBackground = hover,
                    SelectedBackground = Color.FromArgb(20, primary),
                    DisabledBackground = Color.FromArgb(200, secondary),
                    Border = border,
                    HoverBorder = Color.FromArgb(180, primary),
                    SelectedBorder = primary,
                    FocusBorder = primary,
                    Text = foreground,
                    SelectedText = foreground,
                    SubtitleText = Color.FromArgb(160, foreground),
                    DisabledText = Color.FromArgb(128, foreground),
                    SelectionIndicator = primary
                };
            }

            return new TileColors
            {
                Background = _theme.BackgroundColor,
                HoverBackground = _theme.ButtonHoverBackColor,
                SelectedBackground = Color.FromArgb(20, _theme.PrimaryColor),
                DisabledBackground = _theme.DisabledBackColor,
                Border = _theme.BorderColor,
                HoverBorder = _theme.ButtonHoverBorderColor,
                SelectedBorder = _theme.PrimaryColor,
                FocusBorder = _theme.PrimaryColor,
                Text = _theme.ForeColor,
                SelectedText = _theme.ForeColor,
                SubtitleText = _theme.SecondaryTextColor,
                DisabledText = _theme.DisabledForeColor,
                SelectionIndicator = _theme.PrimaryColor
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

        private class TileColors
        {
            public Color Background { get; set; }
            public Color HoverBackground { get; set; }
            public Color SelectedBackground { get; set; }
            public Color DisabledBackground { get; set; }
            public Color Border { get; set; }
            public Color HoverBorder { get; set; }
            public Color SelectedBorder { get; set; }
            public Color FocusBorder { get; set; }
            public Color Text { get; set; }
            public Color SelectedText { get; set; }
            public Color SubtitleText { get; set; }
            public Color DisabledText { get; set; }
            public Color SelectionIndicator { get; set; }
        }
        #endregion
    }
}


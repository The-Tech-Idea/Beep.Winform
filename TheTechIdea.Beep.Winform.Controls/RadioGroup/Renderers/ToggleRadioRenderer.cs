using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.RadioGroup.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;

namespace TheTechIdea.Beep.Winform.Controls.RadioGroup.Renderers
{
    /// <summary>
    /// Toggle switch Style renderer with StyleColors support.
    /// </summary>
    public class ToggleRadioRenderer : IRadioGroupRenderer, IImageAwareRenderer
    {
        private BaseControl _owner;
        private IBeepTheme _theme;
        private Font _textFont;
        private Size _maxImageSize = new Size(24, 24);
        private int IconSize => Math.Min(_maxImageSize.Width, _maxImageSize.Height);
        private BeepControlStyle _controlStyle = BeepControlStyle.iOS15;
        private bool _useThemeColors = true;
        private const int HorizontalPadding = 12;
        private const int VerticalPadding = 6;
        private const int ComponentSpacing = 8;
        private const int TrackWidth = 40;
        private const int TrackHeight = 20;

        public string StyleName => "Toggle";
        public string DisplayName => "Toggle Switch Style";
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

        public Size MaxImageSize { get => _maxImageSize; set => _maxImageSize = value; }

        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner;
            _theme = theme;
            _textFont = RadioGroupFontHelpers.GetItemFont(_controlStyle, isSelected: false, theme);
        }

        public void UpdateTheme(IBeepTheme theme)
        {
            _theme = theme;
            _textFont = _owner?.Font ?? RadioGroupFontHelpers.GetItemFont(_controlStyle, isSelected: false, theme);
        }

        public void RenderItem(Graphics g, SimpleItem item, Rectangle rect, RadioItemState state)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var colors = GetColors();

            // Layout: [Label .....] [Toggle]
            var content = GetContentArea(rect);
            var toggle = GetSelectorArea(rect);

            int reserveRight = toggle.Width + ComponentSpacing;
            int availableRight = Math.Max(content.X, content.Right - reserveRight);
            int currentX = content.X;

            if (!string.IsNullOrEmpty(item.ImagePath) && currentX < availableRight)
            {
                int sz = Math.Min(IconSize, Math.Max(10, content.Height - 4));
                sz = Math.Min(sz, Math.Max(0, availableRight - currentX));
                if (sz > 0)
                {
                    var iconRect = new Rectangle(currentX, content.Y + (content.Height - sz) / 2, sz, sz);
                    var iconPath = RadioGroupIconHelpers.GetItemIconPath(item.ImagePath);
                    var iconColor = RadioGroupIconHelpers.GetIconColor(_theme, _useThemeColors, state.IsSelected, !state.IsEnabled);
                    RadioGroupIconHelpers.PaintIcon(g, iconRect, iconPath, iconColor, _theme, _useThemeColors, _controlStyle);
                    currentX += sz + ComponentSpacing;
                }
            }

            // Text
            if (!string.IsNullOrEmpty(item.Text) && currentX < availableRight)
            {
                using var br = new SolidBrush(state.IsEnabled ? colors.Text : colors.DisabledText);
                var fmt = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
                g.DrawString(item.Text, _textFont, br, new Rectangle(currentX, content.Y, availableRight - currentX, content.Height), fmt);
            }

            // Toggle track
            int actualTrackWidth = Math.Max(8, Math.Min(TrackWidth, Math.Max(0, toggle.Width)));
            int actualTrackHeight = Math.Max(8, Math.Min(TrackHeight, Math.Max(0, toggle.Height)));
            var trackRect = new Rectangle(toggle.Right - actualTrackWidth, toggle.Y + (toggle.Height - actualTrackHeight) / 2, actualTrackWidth, actualTrackHeight);
            int radius = actualTrackHeight / 2;
            var trackColor = !state.IsEnabled
                ? colors.DisabledTrack
                : (state.IsSelected ? colors.PrimaryLite : (state.IsHovered ? colors.HoverTrack : colors.Track));

            using (var path = RoundedRect(trackRect, radius))
            using (var trackBr = new SolidBrush(trackColor))
            {
                g.FillPath(trackBr, path);
            }

            // Thumb
            int thumb = Math.Max(4, actualTrackHeight - 4);
            int cx = state.IsSelected ? trackRect.Right - (thumb / 2) - 2 : trackRect.Left + (thumb / 2) + 2;
            var thumbRect = new Rectangle(cx - thumb / 2, trackRect.Y + 2, thumb, thumb);
            using var thumbBr = new SolidBrush(state.IsEnabled ? colors.Thumb : colors.DisabledThumb);
            g.FillEllipse(thumbBr, thumbRect);

            if (state.IsFocused)
            {
                using var focusPen = new Pen(colors.FocusBorder, 2f);
                var focusRect = Rectangle.Inflate(trackRect, 2, 2);
                using var focusPath = RoundedRect(focusRect, radius + 2);
                g.DrawPath(focusPen, focusPath);
            }
        }

        public Size MeasureItem(SimpleItem item, Graphics g)
        {
            int h = Math.Max(36, TrackHeight + VerticalPadding * 2);
            int w = HorizontalPadding * 2 + TrackWidth;

            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                w += IconSize + ComponentSpacing;
                h = Math.Max(h, IconSize + VerticalPadding * 2);
            }

            if (!string.IsNullOrEmpty(item.Text))
            {
                var s = TextUtils.MeasureText(g,item.Text, _textFont);
                w += (int)Math.Ceiling(s.Width) + ComponentSpacing;
                h = Math.Max(h, (int)Math.Ceiling(s.Height) + VerticalPadding * 2);
            }
            return new Size(w, h);
        }

        public Rectangle GetContentArea(Rectangle itemRectangle)
        {
            return new Rectangle(
                itemRectangle.X + HorizontalPadding,
                itemRectangle.Y + VerticalPadding,
                Math.Max(0, itemRectangle.Width - (HorizontalPadding * 2)),
                Math.Max(0, itemRectangle.Height - (VerticalPadding * 2)));
        }

        public Rectangle GetSelectorArea(Rectangle itemRectangle)
        {
            var content = GetContentArea(itemRectangle);
            int selectorWidth = TrackWidth + HorizontalPadding;
            return new Rectangle(
                Math.Max(content.X, content.Right - selectorWidth),
                content.Y,
                Math.Min(selectorWidth, content.Width),
                content.Height);
        }

        public void RenderGroupDecorations(Graphics graphics, Rectangle groupRectangle, List<SimpleItem> items, List<Rectangle> itemRectangles, List<RadioItemState> states)
        {
        }

        private GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            var path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private (Color Primary, Color PrimaryLite, Color Text, Color DisabledText, Color Track, Color HoverTrack, Color DisabledTrack, Color Thumb, Color DisabledThumb, Color FocusBorder) GetColors()
        {
            if (!_useThemeColors || _theme == null)
            {
                var primary = StyleColors.GetPrimary(_controlStyle);
                var foreground = StyleColors.GetForeground(_controlStyle);
                var border = StyleColors.GetBorder(_controlStyle);
                var hover = StyleColors.GetHover(_controlStyle);
                var selection = StyleColors.GetSelection(_controlStyle);
                var background = StyleColors.GetBackground(_controlStyle);

                return (
                    primary,
                    selection,
                    foreground,
                    Color.FromArgb(150, foreground),
                    border,
                    hover,
                    Color.FromArgb(120, background),
                    Color.White,
                    Color.FromArgb(150, border),
                    primary);
            }

            return (
                _theme.PrimaryColor,
                Color.FromArgb(200, _theme.PrimaryColor),
                _theme.ForeColor,
                _theme.DisabledForeColor,
                _theme.BorderColor,
                _theme.ButtonHoverBackColor,
                _theme.DisabledBackColor,
                _theme.ButtonForeColor,
                _theme.DisabledBorderColor,
                _theme.PrimaryColor);
        }
    }
}

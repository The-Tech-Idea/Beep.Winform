using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.RadioGroup.Renderers
{
    /// <summary>
    /// Toggle switch style renderer.
    /// </summary>
    public class ToggleRadioRenderer : IRadioGroupRenderer, IImageAwareRenderer
    {
        private BaseControl _owner;
        private IBeepTheme _theme;
        private Font _textFont;
        private Size _maxImageSize = new Size(24, 24);

        public string StyleName => "Toggle";
        public string DisplayName => "Toggle Switch Style";
        public bool SupportsMultipleSelection => true;

        public Size MaxImageSize { get => _maxImageSize; set => _maxImageSize = value; }

        public void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner;
            _theme = theme;
            _textFont = new Font("Segoe UI", 11f);
        }

        public void UpdateTheme(IBeepTheme theme)
        {
            _theme = theme;
        }

        public void RenderItem(Graphics g, SimpleItem item, Rectangle rect, RadioItemState state)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var colors = GetColors();

            // Layout: [Label .....] [Toggle]
            var content = GetContentArea(rect);
            var toggle = GetSelectorArea(rect);

            // Text
            if (!string.IsNullOrEmpty(item.Text))
            {
                using var br = new SolidBrush(colors.Text);
                var fmt = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
                g.DrawString(item.Text, _textFont, br, new Rectangle(content.X, content.Y, content.Width - toggle.Width - 8, content.Height), fmt);
            }

            // Toggle track
            int trackH = 20;
            var trackRect = new Rectangle(toggle.X, toggle.Y + (toggle.Height - trackH) / 2, 40, trackH);
            int radius = trackH / 2;
            using (var path = RoundedRect(trackRect, radius))
            using (var trackBr = new SolidBrush(state.IsSelected ? colors.PrimaryLite : colors.Track))
            {
                g.FillPath(trackBr, path);
            }

            // Thumb
            int thumb = trackH - 4;
            int cx = state.IsSelected ? trackRect.Right - (thumb / 2) - 2 : trackRect.Left + (thumb / 2) + 2;
            var thumbRect = new Rectangle(cx - thumb / 2, trackRect.Y + 2, thumb, thumb);
            using var thumbBr = new SolidBrush(colors.Thumb);
            g.FillEllipse(thumbBr, thumbRect);
        }

        public Size MeasureItem(SimpleItem item, Graphics g)
        {
            int h = 28;
            int w = 100;
            if (!string.IsNullOrEmpty(item.Text))
            {
                var s = g.MeasureString(item.Text, _textFont);
                w = (int)s.Width + 70;
                h = Math.Max(h, (int)s.Height + 8);
            }
            return new Size(w, h);
        }

        public Rectangle GetContentArea(Rectangle itemRectangle)
        {
            return new Rectangle(itemRectangle.X + 8, itemRectangle.Y + 4, itemRectangle.Width - 16, itemRectangle.Height - 8);
        }

        public Rectangle GetSelectorArea(Rectangle itemRectangle)
        {
            var content = GetContentArea(itemRectangle);
            return new Rectangle(content.Right - 50, content.Y, 50, content.Height);
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

        private (Color Primary, Color PrimaryLite, Color Text, Color Track, Color Thumb) GetColors()
        {
            if (_theme == null)
                return (Color.FromArgb(59, 130, 246), Color.FromArgb(199, 230, 255), Color.FromArgb(17, 24, 39), Color.FromArgb(229, 231, 235), Color.White);
            return (_theme.PrimaryColor, Color.FromArgb(200, _theme.PrimaryColor), _theme.ForeColor, _theme.BorderColor, _theme.ButtonForeColor);
        }
    }
}

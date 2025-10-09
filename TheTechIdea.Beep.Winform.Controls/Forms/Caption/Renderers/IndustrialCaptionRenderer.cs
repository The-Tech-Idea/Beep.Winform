using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Caption.Renderers
{
    /// <summary>
    /// Industrial-style caption renderer with metallic appearance and thick borders.
    /// </summary>
    internal sealed class IndustrialCaptionRenderer : ICaptionRenderer
    {
        private Form _host;
        private Func<IBeepTheme> _theme;
        private Func<int> _captionHeight;
        private bool _showButtons = true;

        private Rectangle _closeRect, _maxRect, _minRect;
        private bool _hoverClose, _hoverMax, _hoverMin;

        public void UpdateHost(Form host, Func<IBeepTheme> themeProvider, Func<int> captionHeightProvider)
        { _host = host; _theme = themeProvider; _captionHeight = captionHeightProvider; }
        public void UpdateTheme(IBeepTheme theme) { }
        public void SetShowSystemButtons(bool show) => _showButtons = show;

        public void GetTitleInsets(Rectangle captionBounds, float scale, out int leftInset, out int rightInset)
        {
            int btn = _showButtons ? Math.Max(30, (int)(_captionHeight() - DpiScalingHelper.ScaleValue(4, scale))) : 0;
            int pad = DpiScalingHelper.ScaleValue(12, scale);
            rightInset = _showButtons ? (btn * 3 + pad * 4) : pad;
            leftInset = pad;
        }

        public void Paint(Graphics g, Rectangle captionBounds, float scale, IBeepTheme theme, FormWindowState windowState, out Rectangle invalidatedArea)
        {
            invalidatedArea = Rectangle.Empty;
            if (!_showButtons) { _closeRect = _maxRect = _minRect = Rectangle.Empty; return; }
            
            int btn = Math.Max(30, (int)(_captionHeight() - DpiScalingHelper.ScaleValue(4, scale)));
            int pad = DpiScalingHelper.ScaleValue(12, scale);
            int top = captionBounds.Top + Math.Max(2, (captionBounds.Height - btn) / 2);
            _closeRect = new Rectangle(captionBounds.Right - btn - pad, top, btn, btn);
            _maxRect   = new Rectangle(_closeRect.Left - btn - pad, top, btn, btn);
            _minRect   = new Rectangle(_maxRect.Left - btn - pad, top, btn, btn);

            // Enhanced industrial metal palette with copper accents
            Color metallic = Color.FromArgb(140, 145, 155);      // Brushed steel
            Color highlight = Color.FromArgb(200, 205, 215);     // Bright metal
            Color shadow = Color.FromArgb(70, 75, 85);           // Deep shadow
            Color copper = Color.FromArgb(184, 115, 51);         // Copper accent
            
            // Draw industrial-style buttons
            DrawIndustrialButton(g, _minRect, metallic, highlight, shadow, copper, _hoverMin, scale, "minimize");
            DrawIndustrialButton(g, _maxRect, metallic, highlight, shadow, copper, _hoverMax, scale, windowState == FormWindowState.Maximized ? "restore" : "maximize");
            DrawIndustrialButton(g, _closeRect, Color.FromArgb(160, 70, 70), Color.FromArgb(220, 110, 110), Color.FromArgb(100, 35, 35), Color.FromArgb(200, 80, 60), _hoverClose, scale, "close");
        }

        private void DrawIndustrialButton(Graphics g, Rectangle rect, Color baseColor, Color highlight, Color shadow, Color accentColor, bool isHover, float scale, string type)
        {
            if (rect.IsEmpty) return;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            
            // Drop shadow for depth
            using (var shadowBrush = new SolidBrush(Color.FromArgb(40, shadow)))
            {
                var shadowRect = new Rectangle(rect.X + 2, rect.Y + 2, rect.Width, rect.Height);
                g.FillRectangle(shadowBrush, shadowRect);
            }
            
            // Enhanced metallic gradient with multi-stop blend
            Color topColor = isHover ? highlight : Color.FromArgb((highlight.R + baseColor.R) / 2, (highlight.G + baseColor.G) / 2, (highlight.B + baseColor.B) / 2);
            Color midColor = baseColor;
            Color bottomColor = isHover ? baseColor : Color.FromArgb(Math.Max(0, shadow.R + 30), Math.Max(0, shadow.G + 30), Math.Max(0, shadow.B + 30));
            
            using (var brush = new LinearGradientBrush(rect, topColor, bottomColor, LinearGradientMode.Vertical))
            {
                var blend = new ColorBlend(3);
                blend.Colors = new[] { topColor, midColor, bottomColor };
                blend.Positions = new[] { 0f, 0.5f, 1f };
                brush.InterpolationColors = blend;
                g.FillRectangle(brush, rect);
            }
            
            // Copper accent strip (industrial detail)
            if (!isHover)
            {
                using var accentBrush = new LinearGradientBrush(
                    new Rectangle(rect.X, rect.Y, rect.Width, 3),
                    Color.FromArgb(60, accentColor),
                    Color.FromArgb(20, accentColor),
                    LinearGradientMode.Horizontal);
                g.FillRectangle(accentBrush, rect.X, rect.Y + 2, rect.Width, 2);
            }

            // Thick industrial border with beveled edge
            using var borderPen = new Pen(shadow, 3f * scale);
            g.DrawRectangle(borderPen, rect);
            
            // Beveled highlight edges (top and left)
            using var highlightPen = new Pen(highlight, 1.5f * scale);
            g.DrawLine(highlightPen, rect.Left + 2, rect.Top + 2, rect.Right - 2, rect.Top + 2);
            g.DrawLine(highlightPen, rect.Left + 2, rect.Top + 2, rect.Left + 2, rect.Bottom - 2);
            
            // Shadow edges (bottom and right)
            using var shadowPen = new Pen(shadow, 1.5f * scale);
            g.DrawLine(shadowPen, rect.Left + 2, rect.Bottom - 2, rect.Right - 2, rect.Bottom - 2);
            g.DrawLine(shadowPen, rect.Right - 2, rect.Top + 2, rect.Right - 2, rect.Bottom - 2);
            
            // Industrial rivets in corners
            int rivetSize = DpiScalingHelper.ScaleValue(3, scale);
            int rivetOffset = DpiScalingHelper.ScaleValue(3, scale);
            using var rivetBrush = new SolidBrush(Color.FromArgb(isHover ? 180 : 120, shadow));
            g.FillEllipse(rivetBrush, rect.Left + rivetOffset, rect.Top + rivetOffset, rivetSize, rivetSize);
            g.FillEllipse(rivetBrush, rect.Right - rivetOffset - rivetSize, rect.Top + rivetOffset, rivetSize, rivetSize);
            g.FillEllipse(rivetBrush, rect.Left + rivetOffset, rect.Bottom - rivetOffset - rivetSize, rivetSize, rivetSize);
            g.FillEllipse(rivetBrush, rect.Right - rivetOffset - rivetSize, rect.Bottom - rivetOffset - rivetSize, rivetSize, rivetSize);

            // Draw robust icons with industrial weight
            using var iconPen = new Pen(Color.FromArgb(40, 45, 50), 3f * scale)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round
            };
            int inset = DpiScalingHelper.ScaleValue(8, scale);
            switch (type)
            {
                case "minimize":
                    {
                        int y = rect.Y + (int)(rect.Height * 0.58f);
                        g.DrawLine(iconPen, rect.Left + inset, y, rect.Right - inset, y);
                        break;
                    }
                    
                case "maximize":
                    {
                        var iconRect = Rectangle.Inflate(rect, -inset, -inset);
                        g.DrawRectangle(iconPen, iconRect);
                        // Add inner frame for industrial look
                        var innerRect = Rectangle.Inflate(iconRect, -2, -2);
                        using var thinPen = new Pen(Color.FromArgb(60, 65, 75), 1.5f * scale);
                        g.DrawRectangle(thinPen, innerRect);
                        break;
                    }
                    
                case "restore":
                    {
                        var r1 = new Rectangle(rect.Left + inset, rect.Top + inset, rect.Width - inset * 2 - 3, rect.Height - inset * 2 - 3);
                        var r2 = new Rectangle(r1.Right - r1.Width + 3, r1.Bottom - r1.Height + 3, r1.Width, r1.Height);
                        g.DrawRectangle(iconPen, r1);
                        g.DrawRectangle(iconPen, r2);
                        break;
                    }
                    
                case "close":
                    {
                        // Thick industrial X
                        g.DrawLine(iconPen, rect.Left + inset, rect.Top + inset, rect.Right - inset, rect.Bottom - inset);
                        g.DrawLine(iconPen, rect.Right - inset, rect.Top + inset, rect.Left + inset, rect.Bottom - inset);
                        break;
                    }
            }
        }

        public bool OnMouseMove(Point location, out Rectangle invalidatedArea)
        {
            var prev = (_hoverClose, _hoverMax, _hoverMin);
            _hoverClose = _closeRect.Contains(location);
            _hoverMax   = _maxRect.Contains(location);
            _hoverMin   = _minRect.Contains(location);
            if (prev != (_hoverClose, _hoverMax, _hoverMin))
            {
                invalidatedArea = Rectangle.Union(Rectangle.Union(_closeRect, _maxRect), _minRect);
                return true;
            }
            invalidatedArea = Rectangle.Empty; return false;
        }

        public void OnMouseLeave(out Rectangle invalidatedArea)
        {
            if (_hoverClose || _hoverMax || _hoverMin)
            {
                _hoverClose = _hoverMax = _hoverMin = false;
                invalidatedArea = Rectangle.Union(Rectangle.Union(_closeRect, _maxRect), _minRect);
                return;
            }
            invalidatedArea = Rectangle.Empty;
        }

        public bool OnMouseDown(Point location, Form form, out Rectangle invalidatedArea)
        {
            if (_closeRect.Contains(location)) { form.Close(); invalidatedArea = _closeRect; return true; }
            if (_maxRect.Contains(location))  { form.WindowState = form.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized; invalidatedArea = _maxRect; return true; }
            if (_minRect.Contains(location))  { form.WindowState = FormWindowState.Minimized; invalidatedArea = _minRect; return true; }
            invalidatedArea = Rectangle.Empty; return false;
        }

        public bool HitTest(Point location) => _closeRect.Contains(location) || _maxRect.Contains(location) || _minRect.Contains(location);
    }
}
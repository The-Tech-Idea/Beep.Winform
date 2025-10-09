using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Caption.Renderers
{
    /// <summary>
    /// Gaming-style caption renderer with angular design and neon green accents.
    /// </summary>
    internal sealed class GamingCaptionRenderer : ICaptionRenderer
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
            int pad = DpiScalingHelper.ScaleValue(16, scale); 
            int btn = _showButtons ? Math.Max(30, (int)(_captionHeight() - DpiScalingHelper.ScaleValue(4, scale))) : 0; 
            int buttons = _showButtons ? 3 : 0; 
            rightInset = buttons > 0 ? (buttons * btn + (buttons + 1) * pad) : pad; 
            leftInset = pad; 
        }

        public void Paint(Graphics g, Rectangle captionBounds, float scale, IBeepTheme theme, FormWindowState windowState, out Rectangle invalidatedArea)
        {
            invalidatedArea = Rectangle.Empty; 
            if (!_showButtons) { _closeRect = _maxRect = _minRect = Rectangle.Empty; return; }
            int pad = DpiScalingHelper.ScaleValue(16, scale); 
            int btn = Math.Max(30, (int)(_captionHeight() - DpiScalingHelper.ScaleValue(4, scale))); 
            int top = captionBounds.Top + Math.Max(2, (captionBounds.Height - btn) / 2);
            int x = captionBounds.Right - pad - btn; 
            _closeRect = new Rectangle(x, top, btn, btn); 
            x -= (btn + pad); 
            _maxRect = new Rectangle(x, top, btn, btn); 
            x -= (btn + pad); 
            _minRect = new Rectangle(x, top, btn, btn);

            // Modern gaming RGB palette: neon green, electric blue, hot red
            Color gamingGreen = Color.FromArgb(0, 255, 65); // Razer Green
            Color gamingBlue = Color.FromArgb(0, 120, 255); // Electric Blue for maximize
            Color gamingRed = Color.FromArgb(255, 20, 60); // Hot Red
            
            DrawGamingButton(g, _minRect, gamingGreen, _hoverMin, scale, "minimize");
            DrawGamingButton(g, _maxRect, gamingBlue, _hoverMax, scale, windowState == FormWindowState.Maximized ? "restore" : "maximize");
            DrawGamingButton(g, _closeRect, gamingRed, _hoverClose, scale, "close");
        }

        private void DrawGamingButton(Graphics g, Rectangle rect, Color accentColor, bool isHover, float scale, string type)
        {
            if (rect.IsEmpty) return; 
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            
            // Enhanced hexagonal shape with sharper angles
            int cut = DpiScalingHelper.ScaleValue(4, scale);
            var points = new PointF[] 
            { 
                new(rect.Left + cut, rect.Top), 
                new(rect.Right - cut, rect.Top), 
                new(rect.Right, rect.Top + cut), 
                new(rect.Right, rect.Bottom - cut), 
                new(rect.Right - cut, rect.Bottom), 
                new(rect.Left + cut, rect.Bottom), 
                new(rect.Left, rect.Bottom - cut), 
                new(rect.Left, rect.Top + cut) 
            };
            
            // Glowing fill on hover with gradient
            if (isHover) 
            { 
                using var path = new GraphicsPath();
                path.AddPolygon(points);
                using var brush = new PathGradientBrush(path)
                {
                    CenterColor = Color.FromArgb(100, accentColor),
                    SurroundColors = new[] { Color.FromArgb(30, accentColor) }
                };
                g.FillPath(brush, path);
                
                // Outer glow effect on hover
                int glowInflate = DpiScalingHelper.ScaleValue(3, scale);
                using var glowPen = new Pen(Color.FromArgb(80, accentColor), 6f * scale)
                {
                    LineJoin = LineJoin.Round
                };
                var glowRect = Rectangle.Inflate(rect, glowInflate, glowInflate);
                var glowCut = cut + DpiScalingHelper.ScaleValue(2, scale);
                var glowPoints = new PointF[] 
                { 
                    new(glowRect.Left + glowCut, glowRect.Top), 
                    new(glowRect.Right - glowCut, glowRect.Top), 
                    new(glowRect.Right, glowRect.Top + glowCut), 
                    new(glowRect.Right, glowRect.Bottom - glowCut), 
                    new(glowRect.Right - glowCut, glowRect.Bottom), 
                    new(glowRect.Left + glowCut, glowRect.Bottom), 
                    new(glowRect.Left, glowRect.Bottom - glowCut), 
                    new(glowRect.Left, glowRect.Top + glowCut) 
                };
                g.DrawPolygon(glowPen, glowPoints);
            }
            
            // Main hexagonal outline with gaming aesthetic
            using var pen = new Pen(accentColor, 2.5f * scale)
            {
                LineJoin = LineJoin.Miter
            };
            g.DrawPolygon(pen, points);
            
            // Tech-inspired corner accents
            using var accentPen = new Pen(Color.FromArgb(isHover ? 255 : 180, accentColor), 1.5f * scale);
            int cornerLen = DpiScalingHelper.ScaleValue(5, scale);
            // Top-left corner
            g.DrawLine(accentPen, rect.Left + cut, rect.Top, rect.Left + cut + cornerLen, rect.Top);
            g.DrawLine(accentPen, rect.Left, rect.Top + cut, rect.Left, rect.Top + cut + cornerLen);
            // Top-right corner
            g.DrawLine(accentPen, rect.Right - cut - cornerLen, rect.Top, rect.Right - cut, rect.Top);
            g.DrawLine(accentPen, rect.Right, rect.Top + cut, rect.Right, rect.Top + cut + cornerLen);
            
            // Icons with rounded caps
            pen.Width = 2.5f * scale;
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            int inset = DpiScalingHelper.ScaleValue(7, scale);
            switch (type)
            {
                case "minimize": int y = rect.Y + (int)(rect.Height * 0.58f); g.DrawLine(pen, rect.Left + inset, y, rect.Right - inset, y); break;
                case "maximize": var iconRect = Rectangle.Inflate(rect, -inset, -inset); g.DrawRectangle(pen, iconRect); break;
                case "restore": var r1 = new Rectangle(rect.Left + inset, rect.Top + inset, rect.Width - inset * 2 - 3, rect.Height - inset * 2 - 3); g.DrawRectangle(pen, r1); break;
                case "close": g.DrawLine(pen, rect.Left + inset, rect.Top + inset, rect.Right - inset, rect.Bottom - inset); g.DrawLine(pen, rect.Right - inset, rect.Top + inset, rect.Left + inset, rect.Bottom - inset); break;
            }
        }

        public bool OnMouseMove(Point location, out Rectangle invalidatedArea) { var prev = (_hoverClose, _hoverMax, _hoverMin); _hoverClose = _closeRect.Contains(location); _hoverMax = _maxRect.Contains(location); _hoverMin = _minRect.Contains(location); if (prev != (_hoverClose, _hoverMax, _hoverMin)) { invalidatedArea = Rectangle.Union(Rectangle.Union(_closeRect, _maxRect), _minRect); return true; } invalidatedArea = Rectangle.Empty; return false; }
        public void OnMouseLeave(out Rectangle invalidatedArea) { if (_hoverClose || _hoverMax || _hoverMin) { _hoverClose = _hoverMax = _hoverMin = false; invalidatedArea = Rectangle.Union(Rectangle.Union(_closeRect, _maxRect), _minRect); return; } invalidatedArea = Rectangle.Empty; }
        public bool OnMouseDown(Point location, Form form, out Rectangle invalidatedArea) { if (_closeRect.Contains(location)) { form.Close(); invalidatedArea = _closeRect; return true; } if (_maxRect.Contains(location)) { form.WindowState = form.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized; invalidatedArea = _maxRect; return true; } if (_minRect.Contains(location)) { form.WindowState = FormWindowState.Minimized; invalidatedArea = _minRect; return true; } invalidatedArea = Rectangle.Empty; return false; }
        public bool HitTest(Point location) => _closeRect.Contains(location) || _maxRect.Contains(location) || _minRect.Contains(location);
    }
}
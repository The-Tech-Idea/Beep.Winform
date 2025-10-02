using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Caption.Renderers
{
    /// <summary>
    /// Neon-style caption renderer with glowing circular buttons and vibrant colors.
    /// </summary>
    internal sealed class NeonCaptionRenderer : ICaptionRenderer
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
        { int pad = (int)(16 * scale); int btn = _showButtons ? Math.Max(30, (int)(_captionHeight() - 4 * scale)) : 0; int buttons = _showButtons ? 3 : 0; rightInset = buttons > 0 ? (buttons * btn + (buttons + 1) * pad) : pad; leftInset = pad; }

        public void Paint(Graphics g, Rectangle captionBounds, float scale, IBeepTheme theme, FormWindowState windowState, out Rectangle invalidatedArea)
        {
            invalidatedArea = Rectangle.Empty; if (!_showButtons) { _closeRect = _maxRect = _minRect = Rectangle.Empty; return; }
            int pad = (int)(16 * scale); int btn = Math.Max(30, (int)(_captionHeight() - 4 * scale)); int top = captionBounds.Top + Math.Max(2, (captionBounds.Height - btn) / 2);
            int x = captionBounds.Right - pad - btn; _closeRect = new Rectangle(x, top, btn, btn); x -= (btn + pad); _maxRect = new Rectangle(x, top, btn, btn); x -= (btn + pad); _minRect = new Rectangle(x, top, btn, btn);

            // Modern cyberpunk palette: cyan-magenta dual-tone with purple accent
            Color neonCyan = Color.FromArgb(0, 255, 255); 
            Color neonMagenta = Color.FromArgb(255, 0, 255);
            Color neonPurple = Color.FromArgb(138, 43, 226); // BlueViolet for maximize
            
            DrawNeonButton(g, _minRect, neonCyan, _hoverMin, scale, "minimize");
            DrawNeonButton(g, _maxRect, neonPurple, _hoverMax, scale, windowState == FormWindowState.Maximized ? "restore" : "maximize");
            DrawNeonButton(g, _closeRect, neonMagenta, _hoverClose, scale, "close");
        }

        private void DrawNeonButton(Graphics g, Rectangle rect, Color neonColor, bool isHover, float scale, string type)
        {
            if (rect.IsEmpty) return; 
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            
            // Enhanced multi-layer glow effect
            int outerGlow = isHover ? 10 : 6;
            int innerGlow = isHover ? 6 : 3;
            
            // Outer intense glow layer
            using (var glowPen = new Pen(Color.FromArgb(isHover ? 140 : 90, neonColor), outerGlow * scale))
            {
                glowPen.LineJoin = LineJoin.Round;
                var outer = Rectangle.Inflate(rect, outerGlow, outerGlow);
                g.DrawEllipse(glowPen, outer);
            }
            
            // Inner glow layer for depth
            using (var glowPen = new Pen(Color.FromArgb(isHover ? 180 : 120, neonColor), innerGlow * scale))
            {
                var mid = Rectangle.Inflate(rect, innerGlow, innerGlow);
                g.DrawEllipse(glowPen, mid);
            }
            
            // Cyberpunk glass effect with radial gradient
            using (var path = new GraphicsPath())
            {
                path.AddEllipse(rect);
                using var brush = new PathGradientBrush(path)
                {
                    CenterColor = Color.FromArgb(isHover ? 180 : 120, neonColor),
                    SurroundColors = new[] { Color.FromArgb(8, 8, 12) },
                    FocusScales = new PointF(0.3f, 0.3f) // More concentrated center
                };
                g.FillEllipse(brush, rect);
            }
            
            // Bright neon outline with rounded caps
            using (var outline = new Pen(neonColor, 2.5f * scale) 
            { 
                LineJoin = LineJoin.Round,
                StartCap = LineCap.Round,
                EndCap = LineCap.Round
            }) 
            {
                g.DrawEllipse(outline, rect);
            }

            // Glyphs with rounded caps for smoother appearance
            using var pen = new Pen(Color.White, 2.3f * scale)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round
            };
            int inset = (int)(7 * scale);
            switch (type)
            {
                case "minimize": int y = rect.Y + rect.Height / 2; g.DrawLine(pen, rect.Left + inset, y, rect.Right - inset, y); break;
                case "maximize": var iconRect = Rectangle.Inflate(rect, -inset, -inset); g.DrawRectangle(pen, iconRect); break;
                case "restore": var r1 = new Rectangle(rect.Left + inset, rect.Top + inset, rect.Width - inset * 2 - 2, rect.Height - inset * 2 - 2); var r2 = new Rectangle(r1.Right - r1.Width + 3, r1.Bottom - r1.Height + 3, r1.Width, r1.Height); g.DrawRectangle(pen, r1); g.DrawRectangle(pen, r2); break;
                case "close": g.DrawLine(pen, rect.Left + inset, rect.Top + inset, rect.Right - inset, rect.Bottom - inset); g.DrawLine(pen, rect.Right - inset, rect.Top + inset, rect.Left + inset, rect.Bottom - inset); break;
            }
        }

        public bool OnMouseMove(Point location, out Rectangle invalidatedArea) { var prev = (_hoverClose, _hoverMax, _hoverMin); _hoverClose = _closeRect.Contains(location); _hoverMax = _maxRect.Contains(location); _hoverMin = _minRect.Contains(location); if (prev != (_hoverClose, _hoverMax, _hoverMin)) { invalidatedArea = Rectangle.Union(Rectangle.Union(_closeRect, _maxRect), _minRect); return true; } invalidatedArea = Rectangle.Empty; return false; }
        public void OnMouseLeave(out Rectangle invalidatedArea) { if (_hoverClose || _hoverMax || _hoverMin) { _hoverClose = _hoverMax = _hoverMin = false; invalidatedArea = Rectangle.Union(Rectangle.Union(_closeRect, _maxRect), _minRect); return; } invalidatedArea = Rectangle.Empty; }
        public bool OnMouseDown(Point location, Form form, out Rectangle invalidatedArea) { if (_closeRect.Contains(location)) { form.Close(); invalidatedArea = _closeRect; return true; } if (_maxRect.Contains(location)) { form.WindowState = form.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized; invalidatedArea = _maxRect; return true; } if (_minRect.Contains(location)) { form.WindowState = FormWindowState.Minimized; invalidatedArea = _minRect; return true; } invalidatedArea = Rectangle.Empty; return false; }
        public bool HitTest(Point location) => _closeRect.Contains(location) || _maxRect.Contains(location) || _minRect.Contains(location);
    }
}
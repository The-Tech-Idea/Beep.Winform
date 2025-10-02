using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Caption.Renderers
{
    /// <summary>
    /// Retro 80s-style caption renderer with circular neon buttons and gradients.
    /// </summary>
    internal sealed class RetroCaptionRenderer : ICaptionRenderer
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
        { int pad = (int)(18 * scale); int btn = _showButtons ? Math.Max(32, (int)(_captionHeight() - 2 * scale)) : 0; int buttons = _showButtons ? 3 : 0; rightInset = buttons > 0 ? (buttons * btn + (buttons + 1) * pad) : pad; leftInset = pad; }

        public void Paint(Graphics g, Rectangle captionBounds, float scale, IBeepTheme theme, FormWindowState windowState, out Rectangle invalidatedArea)
        {
            invalidatedArea = Rectangle.Empty; if (!_showButtons) { _closeRect = _maxRect = _minRect = Rectangle.Empty; return; }
            int pad = (int)(18 * scale); int btn = Math.Max(32, (int)(_captionHeight() - 2 * scale)); int top = captionBounds.Top + Math.Max(2, (captionBounds.Height - btn) / 2);
            int x = captionBounds.Right - pad - btn; _closeRect = new Rectangle(x, top, btn, btn); x -= (btn + pad); _maxRect = new Rectangle(x, top, btn, btn); x -= (btn + pad); _minRect = new Rectangle(x, top, btn, btn);

            // Authentic 80s vaporwave palette: Miami Vice inspired
            DrawRetroButton(g, _minRect, 
                Color.FromArgb(0, 255, 255),    // Cyan glow
                Color.FromArgb(255, 0, 128),    // Hot pink inner
                _hoverMin, scale, "minimize");
            DrawRetroButton(g, _maxRect, 
                Color.FromArgb(255, 71, 239),   // Purple glow  
                Color.FromArgb(0, 229, 255),    // Turquoise inner
                _hoverMax, scale, (windowState == FormWindowState.Maximized ? "restore" : "maximize"));
            DrawRetroButton(g, _closeRect, 
                Color.FromArgb(255, 200, 0),    // Gold glow
                Color.FromArgb(255, 20, 147),   // Deep pink inner
                _hoverClose, scale, "close");
        }

        private void DrawRetroButton(Graphics g, Rectangle rect, Color glowColor, Color innerColor, bool isHover, float scale, string type)
        {
            if (rect.IsEmpty) return; 
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // Enhanced multi-layer retro glow effect
            int outerGlow = isHover ? 10 : 6;
            int midGlow = isHover ? 6 : 4;
            
            // Outer glow ring (softest)
            using (var glowPen = new Pen(Color.FromArgb(isHover ? 100 : 60, glowColor), outerGlow * scale))
            {
                glowPen.LineJoin = LineJoin.Round;
                var outer = Rectangle.Inflate(rect, outerGlow, outerGlow);
                g.DrawEllipse(glowPen, outer);
            }
            
            // Mid glow ring (brighter)
            using (var glowPen = new Pen(Color.FromArgb(isHover ? 150 : 100, glowColor), midGlow * scale))
            {
                var mid = Rectangle.Inflate(rect, midGlow, midGlow);
                g.DrawEllipse(glowPen, mid);
            }

            // Enhanced radial gradient with stronger center
            using (var path = new GraphicsPath())
            {
                path.AddEllipse(rect);
                
                // Create color blend for smoother gradient
                var blend = new ColorBlend(3);
                blend.Colors = new[] 
                { 
                    Color.FromArgb(isHover ? 240 : 180, innerColor),  // Bright center
                    Color.FromArgb(isHover ? 160 : 100, innerColor),  // Mid fade
                    Color.FromArgb(15, 15, 25)                         // Dark edge
                };
                blend.Positions = new[] { 0f, 0.4f, 1f };
                
                using var brush = new PathGradientBrush(path)
                {
                    CenterColor = blend.Colors[0],
                    SurroundColors = new[] { blend.Colors[2] },
                    FocusScales = new PointF(0.2f, 0.2f) // Tight center focus
                };
                brush.InterpolationColors = blend;
                g.FillEllipse(brush, rect);
            }

            // Bright neon tube outline (dual-tone for authenticity)
            using (var innerOutline = new Pen(innerColor, 1.8f * scale))
            {
                g.DrawEllipse(innerOutline, Rectangle.Inflate(rect, -1, -1));
            }
            using (var outline = new Pen(glowColor, 2.8f * scale) 
            { 
                LineJoin = LineJoin.Round 
            }) 
            {
                g.DrawEllipse(outline, rect);
            }

            // Glyphs with retro thick line style
            using var glyph = new Pen(Color.FromArgb(255, 255, 255), 2.5f * scale)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round
            };
            int inset = (int)(7 * scale);
            switch (type)
            {
                case "minimize": int y = rect.Y + rect.Height / 2; g.DrawLine(glyph, rect.Left + inset, y, rect.Right - inset, y); break;
                case "maximize": var iconRect = Rectangle.Inflate(rect, -inset, -inset); g.DrawRectangle(glyph, iconRect); break;
                case "restore": var r1 = new Rectangle(rect.Left + inset, rect.Top + inset, rect.Width - inset * 2 - 3, rect.Height - inset * 2 - 3); var r2 = new Rectangle(r1.Right - r1.Width + 4, r1.Bottom - r1.Height + 4, r1.Width, r1.Height); g.DrawRectangle(glyph, r1); g.DrawRectangle(glyph, r2); break;
                case "close": g.DrawLine(glyph, rect.Left + inset, rect.Top + inset, rect.Right - inset, rect.Bottom - inset); g.DrawLine(glyph, rect.Right - inset, rect.Top + inset, rect.Left + inset, rect.Bottom - inset); break;
            }
        }

        public bool OnMouseMove(Point location, out Rectangle invalidatedArea) { var prev = (_hoverClose, _hoverMax, _hoverMin); _hoverClose = _closeRect.Contains(location); _hoverMax = _maxRect.Contains(location); _hoverMin = _minRect.Contains(location); if (prev != (_hoverClose, _hoverMax, _hoverMin)) { invalidatedArea = Rectangle.Union(Rectangle.Union(_closeRect, _maxRect), _minRect); return true; } invalidatedArea = Rectangle.Empty; return false; }
        public void OnMouseLeave(out Rectangle invalidatedArea) { if (_hoverClose || _hoverMax || _hoverMin) { _hoverClose = _hoverMax = _hoverMin = false; invalidatedArea = Rectangle.Union(Rectangle.Union(_closeRect, _maxRect), _minRect); return; } invalidatedArea = Rectangle.Empty; }
        public bool OnMouseDown(Point location, Form form, out Rectangle invalidatedArea) { if (_closeRect.Contains(location)) { form.Close(); invalidatedArea = _closeRect; return true; } if (_maxRect.Contains(location)) { form.WindowState = form.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized; invalidatedArea = _maxRect; return true; } if (_minRect.Contains(location)) { form.WindowState = FormWindowState.Minimized; invalidatedArea = _minRect; return true; } invalidatedArea = Rectangle.Empty; return false; }
        public bool HitTest(Point location) => _closeRect.Contains(location) || _maxRect.Contains(location) || _minRect.Contains(location);
    }
}
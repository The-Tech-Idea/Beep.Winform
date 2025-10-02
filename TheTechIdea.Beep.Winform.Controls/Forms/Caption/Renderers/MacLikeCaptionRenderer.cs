using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Caption.Renderers
{
    /// <summary>
    /// macOS-like caption renderer with red/yellow/green circular buttons on the left.
    /// </summary>
    internal sealed class MacLikeCaptionRenderer : ICaptionRenderer
    {
        private Form _host;
        private Func<IBeepTheme> _theme;
        private Func<int> _captionHeight;
        private bool _showButtons = true;

        // button geometry
        private Rectangle _closeRect, _minRect, _zoomRect; // mac: close (red), minimize (yellow), zoom (green)
        private bool _hoverClose, _hoverMin, _hoverZoom;

        public float CircleSizeFactor { get; set; } = 0.55f; // portion of caption height (slightly smaller for authenticity)
        public int LeftMargin { get; set; } = 12;
        public int Spacing { get; set; } = 8;
        public bool ShowGlyphsOnHover { get; set; } = true; // macOS shows action glyphs on hover

        public void UpdateHost(Form host, Func<IBeepTheme> themeProvider, Func<int> captionHeightProvider)
        { _host = host; _theme = themeProvider; _captionHeight = captionHeightProvider; }
        public void UpdateTheme(IBeepTheme theme) { /* nothing special */ }
        public void SetShowSystemButtons(bool show) => _showButtons = show;

        public void GetTitleInsets(Rectangle captionBounds, float scale, out int leftInset, out int rightInset)
        {
            int pad = (int)(8 * scale);
            if (!_showButtons) { leftInset = rightInset = pad; return; }
            int d = Math.Max(12, (int)(_captionHeight() * CircleSizeFactor));
            int w = (int)(LeftMargin * scale) + d * 3 + (int)(Spacing * scale) * 2 + pad;
            leftInset = w; rightInset = pad;
        }

        public void Paint(Graphics g, Rectangle captionBounds, float scale, IBeepTheme theme, FormWindowState windowState, out Rectangle invalidatedArea)
        {
            invalidatedArea = Rectangle.Empty;
            if (!_showButtons) { _closeRect = _minRect = _zoomRect = Rectangle.Empty; return; }
            int d = Math.Max(12, (int)(_captionHeight() * CircleSizeFactor));
            int left = captionBounds.Left + (int)(LeftMargin * scale);
            int y = captionBounds.Top + Math.Max(2, (captionBounds.Height - d) / 2);
            _closeRect = new Rectangle(left, y, d, d);
            _minRect = new Rectangle(_closeRect.Right + (int)(Spacing * scale), y, d, d);
            _zoomRect = new Rectangle(_minRect.Right + (int)(Spacing * scale), y, d, d);

            // Authentic macOS Big Sur/Monterey/Sonoma traffic light colors
            DrawMacButton(g, scale, _closeRect, 
                _hoverClose ? Color.FromArgb(237, 106, 94) : Color.FromArgb(255, 95, 86),   // Red
                _hoverClose, "close");
            DrawMacButton(g, scale, _minRect, 
                _hoverMin ? Color.FromArgb(245, 191, 79) : Color.FromArgb(255, 189, 46),    // Yellow/Amber
                _hoverMin, "minimize");
            DrawMacButton(g, scale, _zoomRect, 
                _hoverZoom ? Color.FromArgb(98, 197, 85) : Color.FromArgb(40, 205, 65),     // Green (corrected)
                _hoverZoom, "zoom");
        }

        private void DrawMacButton(Graphics g, float scale, Rectangle r, Color color, bool isHover, string action)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            
            // Subtle gradient for depth (like real macOS buttons)
            using (var path = new System.Drawing.Drawing2D.GraphicsPath())
            {
                path.AddEllipse(r);
                using var brush = new System.Drawing.Drawing2D.PathGradientBrush(path)
                {
                    CenterColor = color,
                    SurroundColors = new[] { ControlPaint.Dark(color, 0.08f) },
                    FocusScales = new PointF(0.7f, 0.7f)
                };
                g.FillEllipse(brush, r);
            }
            
            // Subtle border (more authentic)
            using var borderPen = new Pen(Color.FromArgb(80, 0, 0, 0), 0.8f * scale);
            g.DrawEllipse(borderPen, r);
            
            // Inner highlight for 3D effect
            var highlightRect = new Rectangle(r.X + 1, r.Y + 1, r.Width - 2, r.Height / 2);
            using (var highlightBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
                highlightRect, 
                Color.FromArgb(40, Color.White), 
                Color.FromArgb(0, Color.White), 
                System.Drawing.Drawing2D.LinearGradientMode.Vertical))
            {
                g.FillEllipse(highlightBrush, highlightRect);
            }
            
            // Show action glyph on hover (authentic macOS behavior)
            if (isHover && ShowGlyphsOnHover)
            {
                using var glyphPen = new Pen(Color.FromArgb(180, 70, 40, 30), 1.5f * scale)
                {
                    StartCap = System.Drawing.Drawing2D.LineCap.Round,
                    EndCap = System.Drawing.Drawing2D.LineCap.Round
                };
                
                int cx = r.X + r.Width / 2;
                int cy = r.Y + r.Height / 2;
                int iconSize = (int)(r.Width * 0.35f);
                
                switch (action)
                {
                    case "close": // X symbol
                        g.DrawLine(glyphPen, cx - iconSize, cy - iconSize, cx + iconSize, cy + iconSize);
                        g.DrawLine(glyphPen, cx + iconSize, cy - iconSize, cx - iconSize, cy + iconSize);
                        break;
                    case "minimize": // Minus symbol
                        g.DrawLine(glyphPen, cx - iconSize, cy, cx + iconSize, cy);
                        break;
                    case "zoom": // Double arrows or plus (using plus for simplicity)
                        g.DrawLine(glyphPen, cx, cy - iconSize, cx, cy + iconSize);
                        g.DrawLine(glyphPen, cx - iconSize, cy, cx + iconSize, cy);
                        break;
                }
            }
        }

        public bool OnMouseMove(Point location, out Rectangle invalidatedArea)
        {
            var prev = (_hoverClose, _hoverMin, _hoverZoom);
            _hoverClose = _closeRect.Contains(location);
            _hoverMin = _minRect.Contains(location);
            _hoverZoom = _zoomRect.Contains(location);
            if (prev != (_hoverClose, _hoverMin, _hoverZoom))
            {
                invalidatedArea = Rectangle.Union(Rectangle.Union(_closeRect, _minRect), _zoomRect);
                return true;
            }
            invalidatedArea = Rectangle.Empty; return false;
        }

        public void OnMouseLeave(out Rectangle invalidatedArea)
        {
            if (_hoverClose || _hoverMin || _hoverZoom)
            {
                _hoverClose = _hoverMin = _hoverZoom = false;
                invalidatedArea = Rectangle.Union(Rectangle.Union(_closeRect, _minRect), _zoomRect);
                return;
            }
            invalidatedArea = Rectangle.Empty;
        }

        public bool OnMouseDown(Point location, Form form, out Rectangle invalidatedArea)
        {
            if (_closeRect.Contains(location)) { form.Close(); invalidatedArea = _closeRect; return true; }
            if (_minRect.Contains(location)) { form.WindowState = FormWindowState.Minimized; invalidatedArea = _minRect; return true; }
            if (_zoomRect.Contains(location)) { form.WindowState = form.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized; invalidatedArea = _zoomRect; return true; }
            invalidatedArea = Rectangle.Empty; return false;
        }

        public bool HitTest(Point location) => _closeRect.Contains(location) || _minRect.Contains(location) || _zoomRect.Contains(location);
    }
}

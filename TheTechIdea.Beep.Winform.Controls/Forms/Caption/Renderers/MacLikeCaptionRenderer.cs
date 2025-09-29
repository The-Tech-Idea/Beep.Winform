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

        public int CircleDiameter { get; set; } = 12;
        public int LeftMargin { get; set; } = 10;
        public int TopMargin { get; set; } = 10;
        public int Spacing { get; set; } = 8;

        public void UpdateHost(Form host, Func<IBeepTheme> themeProvider, Func<int> captionHeightProvider)
        { _host = host; _theme = themeProvider; _captionHeight = captionHeightProvider; }
        public void UpdateTheme(IBeepTheme theme) { /* nothing special */ }
        public void SetShowSystemButtons(bool show) => _showButtons = show;

        public void GetTitleInsets(Rectangle captionBounds, float scale, out int leftInset, out int rightInset)
        {
            if (!_showButtons) { leftInset = rightInset = (int)(8 * scale); return; }
            int d = (int)(CircleDiameter * scale);
            int w = LeftMargin + d * 3 + Spacing * 2 + (int)(8 * scale);
            leftInset = w; rightInset = (int)(8 * scale);
        }

        public void Paint(Graphics g, Rectangle captionBounds, float scale, IBeepTheme theme, FormWindowState windowState, out Rectangle invalidatedArea)
        {
            invalidatedArea = Rectangle.Empty;
            if (!_showButtons) { _closeRect = _minRect = _zoomRect = Rectangle.Empty; return; }
            int d = (int)(CircleDiameter * scale);
            int y = TopMargin + (captionBounds.Height - d) / 2;
            _closeRect = new Rectangle(captionBounds.Left + LeftMargin, y, d, d);
            _minRect   = new Rectangle(_closeRect.Right + Spacing, y, d, d);
            _zoomRect  = new Rectangle(_minRect.Right + Spacing, y, d, d);

            // Draw filled circles
            DrawCircle(g, _closeRect, _hoverClose ? Color.FromArgb(230, 86, 73) : Color.FromArgb(255, 95, 86));
            DrawCircle(g, _minRect,   _hoverMin   ? Color.FromArgb(230, 177, 39) : Color.FromArgb(255, 189, 46));
            DrawCircle(g, _zoomRect,  _hoverZoom  ? Color.FromArgb(40, 201, 64)  : Color.FromArgb(39, 201, 63));
        }

        private static void DrawCircle(Graphics g, Rectangle r, Color color)
        {
            using var b = new SolidBrush(color);
            g.FillEllipse(b, r);
            using var p = new Pen(Color.FromArgb(30, Color.Black));
            g.DrawEllipse(p, r);
        }

        public bool OnMouseMove(Point location, out Rectangle invalidatedArea)
        {
            var prev = (_hoverClose, _hoverMin, _hoverZoom);
            _hoverClose = _closeRect.Contains(location);
            _hoverMin   = _minRect.Contains(location);
            _hoverZoom  = _zoomRect.Contains(location);
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
            if (_minRect.Contains(location))   { form.WindowState = FormWindowState.Minimized; invalidatedArea = _minRect; return true; }
            if (_zoomRect.Contains(location))  { form.WindowState = form.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized; invalidatedArea = _zoomRect; return true; }
            invalidatedArea = Rectangle.Empty; return false;
        }

        public bool HitTest(Point location) => _closeRect.Contains(location) || _minRect.Contains(location) || _zoomRect.Contains(location);
    }
}

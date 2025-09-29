using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Caption.Renderers
{
    /// <summary>
    /// KDE/Breeze-like caption renderer with subtle hover fills and right-aligned buttons.
    /// </summary>
    internal sealed class KdeCaptionRenderer : ICaptionRenderer
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
            int btn = _showButtons ? Math.Max(24, (int)(_captionHeight() - 8 * scale)) : 0;
            int pad = (int)(8 * scale);
            rightInset = _showButtons ? (btn * 3 + pad * 3) : pad; // right aligned
            leftInset = pad;
        }

        public void Paint(Graphics g, Rectangle captionBounds, float scale, IBeepTheme theme, FormWindowState windowState, out Rectangle invalidatedArea)
        {
            invalidatedArea = Rectangle.Empty;
            if (!_showButtons) { _closeRect = _maxRect = _minRect = Rectangle.Empty; return; }
            int btn = Math.Max(24, (int)(_captionHeight() - 8 * scale));
            int top = (int)(4 * scale);
            _closeRect = new Rectangle(captionBounds.Right - btn - 8, top, btn, btn);
            _maxRect   = new Rectangle(_closeRect.Left - btn - 6, top, btn, btn);
            _minRect   = new Rectangle(_maxRect.Left   - btn - 6, top, btn, btn);

            var fore = theme?.AppBarButtonForeColor ?? _host.ForeColor;
            using var p = new Pen(fore, 1.5f);

            // Minimize glyph
            int y = _minRect.Y + (int)(_minRect.Height * 0.6f);
            if (_hoverMin) { using var hb = new SolidBrush(Color.FromArgb(25, fore)); g.FillRectangle(hb, _minRect); }
            g.DrawLine(p, _minRect.Left + (int)(6*scale), y, _minRect.Right - (int)(6*scale), y);

            // Maximize/Restore glyph
            int w = (int)Math.Max(8*scale, _maxRect.Width / 3f);
            int cx = _maxRect.X + _maxRect.Width / 2; int cy = _maxRect.Y + _maxRect.Height / 2;
            var rect = new Rectangle(cx - w/2, cy - w/2, w, w);
            if (_hoverMax) { using var hb = new SolidBrush(Color.FromArgb(25, fore)); g.FillRectangle(hb, _maxRect); }
            if (windowState == FormWindowState.Maximized)
                g.DrawRectangle(p, Rectangle.Inflate(rect, -2, -2));
            else
                g.DrawRectangle(p, rect);

            // Close glyph
            int inset = (int)(6 * scale);
            if (_hoverClose) { using var hb = new SolidBrush(Color.FromArgb(32, Color.IndianRed)); g.FillRectangle(hb, _closeRect); }
            g.DrawLine(p, _closeRect.Left + inset, _closeRect.Top + inset, _closeRect.Right - inset, _closeRect.Bottom - inset);
            g.DrawLine(p, _closeRect.Right - inset, _closeRect.Top + inset, _closeRect.Left + inset, _closeRect.Bottom - inset);
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

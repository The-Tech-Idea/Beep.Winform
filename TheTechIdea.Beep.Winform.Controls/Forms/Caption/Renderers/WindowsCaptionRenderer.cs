using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls; // for CaptionGlyphProvider

namespace TheTechIdea.Beep.Winform.Controls.Forms.Caption.Renderers
{
    internal sealed class WindowsCaptionRenderer : ICaptionRenderer
    {
        private Form _host;
        private Func<IBeepTheme> _theme;
        private Func<int> _captionHeight;
        private bool _showButtons = true;

        private Rectangle _closeRect, _maxRect, _minRect;
        private bool _hoverClose, _hoverMax, _hoverMin;

        public void UpdateHost(Form host, Func<IBeepTheme> themeProvider, Func<int> captionHeightProvider)
        {
            _host = host; _theme = themeProvider; _captionHeight = captionHeightProvider;
        }
        public void UpdateTheme(IBeepTheme theme) { /* nothing special */ }
        public void SetShowSystemButtons(bool show) => _showButtons = show;

        public void GetTitleInsets(Rectangle captionBounds, float scale, out int leftInset, out int rightInset)
        {
            int btn = _showButtons ? Math.Max(24, (int)(_captionHeight() - 8 * scale)) : 0;
            int pad = (int)(8 * scale);
            rightInset = _showButtons ? (btn * 3 + pad * 3) : pad;
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

            using var pen = new Pen(theme?.AppBarButtonForeColor ?? _host.ForeColor, 1.5f) { Alignment = PenAlignment.Center };
            // Minimize
            if (_hoverMin) { using var hb = new SolidBrush(theme?.ButtonHoverBackColor ?? Color.LightGray); g.FillRectangle(hb, _minRect); }
            CaptionGlyphProvider.DrawMinimize(g, pen, _minRect, scale);
            // Maximize/Restore
            if (_hoverMax) { using var hb = new SolidBrush(theme?.ButtonHoverBackColor ?? Color.LightGray); g.FillRectangle(hb, _maxRect); }
            if (windowState == FormWindowState.Maximized)
                CaptionGlyphProvider.DrawRestore(g, pen, _maxRect, scale);
            else
                CaptionGlyphProvider.DrawMaximize(g, pen, _maxRect, scale);
            // Close
            if (_hoverClose) { using var hb = new SolidBrush(theme?.ButtonErrorBackColor ?? Color.IndianRed); g.FillRectangle(hb, _closeRect); }
            CaptionGlyphProvider.DrawClose(g, pen, _closeRect, scale);
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

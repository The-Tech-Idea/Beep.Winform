using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Caption.Renderers
{
    /// <summary>
    /// GNOME/Adwaita-like caption renderer with right-aligned flat glyphs, no glow.
    /// </summary>
    internal sealed class GnomeCaptionRenderer : ICaptionRenderer
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
            int pad = (int)(8 * scale);
            int btn = _showButtons ? Math.Max(24, (int)(_captionHeight() - 8 * scale)) : 0;
            int buttons = _showButtons ? 3 : 0;
            rightInset = buttons > 0 ? (buttons * btn + (buttons + 1) * pad) : pad; // buttons on right
            leftInset = pad;
        }

        public void Paint(Graphics g, Rectangle captionBounds, float scale, IBeepTheme theme, FormWindowState windowState, out Rectangle invalidatedArea)
        {
            invalidatedArea = Rectangle.Empty;
            if (!_showButtons) { _closeRect = _maxRect = _minRect = Rectangle.Empty; return; }
            int pad = (int)(8 * scale);
            int btn = Math.Max(24, (int)(_captionHeight() - 8 * scale));
            int top = captionBounds.Top + Math.Max(2, (captionBounds.Height - btn) / 2);

            int x = captionBounds.Right - pad - btn;
            _closeRect = new Rectangle(x, top, btn, btn);
            x -= (btn + pad);
            _maxRect = new Rectangle(x, top, btn, btn);
            x -= (btn + pad);
            _minRect = new Rectangle(x, top, btn, btn);

            var fore = theme?.AppBarButtonForeColor ?? _host.ForeColor;
            var gnomeBlue = Color.FromArgb(53, 132, 228); // GNOME Blue 3
            using var p = new Pen(fore, 1.6f * scale);

            // Modern GNOME/Adwaita: subtle rounded hover with clean icons
            int radius = (int)(6 * scale);
            
            if (_hoverMin) 
            { 
                using var hb = new SolidBrush(Color.FromArgb(25, gnomeBlue)); 
                g.FillRoundedRectangle(hb, _minRect, radius);
            }
            int y = _minRect.Y + (int)(_minRect.Height * 0.58f);
            g.DrawLine(p, _minRect.Left + (int)(6 * scale), y, _minRect.Right - (int)(6 * scale), y);

            if (_hoverMax) 
            { 
                using var hb = new SolidBrush(Color.FromArgb(25, gnomeBlue)); 
                g.FillRoundedRectangle(hb, _maxRect, radius);
            }
            int w = (int)Math.Max(10 * scale, _maxRect.Width / 2.8f);
            int cx = _maxRect.X + _maxRect.Width / 2; int cy = _maxRect.Y + _maxRect.Height / 2;
            var rect = new Rectangle(cx - w / 2, cy - w / 2, w, w);
            if (windowState == FormWindowState.Maximized)
                g.DrawRectangle(p, Rectangle.Inflate(rect, -2, -2));
            else
                g.DrawRectangle(p, rect);

            if (_hoverClose) 
            { 
                using var hb = new SolidBrush(Color.FromArgb(192, 28, 40)); // GNOME Red
                g.FillRoundedRectangle(hb, _closeRect, radius);
                p.Color = Color.White;
            }
            int inset = (int)(6 * scale);
            g.DrawLine(p, _closeRect.Left + inset, _closeRect.Top + inset, _closeRect.Right - inset, _closeRect.Bottom - inset);
            g.DrawLine(p, _closeRect.Right - inset, _closeRect.Top + inset, _closeRect.Left + inset, _closeRect.Bottom - inset);
        }

        public bool OnMouseMove(Point location, out Rectangle invalidatedArea)
        {
            var prev = (_hoverClose, _hoverMax, _hoverMin);
            _hoverClose = _closeRect.Contains(location);
            _hoverMax = _maxRect.Contains(location);
            _hoverMin = _minRect.Contains(location);
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
            if (_maxRect.Contains(location)) { form.WindowState = form.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized; invalidatedArea = _maxRect; return true; }
            if (_minRect.Contains(location)) { form.WindowState = FormWindowState.Minimized; invalidatedArea = _minRect; return true; }
            invalidatedArea = Rectangle.Empty; return false;
        }

        public bool HitTest(Point location) => _closeRect.Contains(location) || _maxRect.Contains(location) || _minRect.Contains(location);
    }
}

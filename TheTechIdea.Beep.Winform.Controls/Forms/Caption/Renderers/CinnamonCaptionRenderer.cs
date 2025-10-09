using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Caption.Renderers
{
    /// <summary>
    /// Cinnamon-like: right aligned controls, rounded hover plates, larger spacing.
    /// </summary>
    internal sealed class CinnamonCaptionRenderer : ICaptionRenderer
    {
        private Form _host;
        private Func<IBeepTheme> _theme;
        private Func<int> _captionHeight;
        private bool _showButtons = true;

        private RectangleF _closeRect, _maxRect, _minRect;
        private bool _hoverClose, _hoverMax, _hoverMin;

        public void UpdateHost(Form host, Func<IBeepTheme> themeProvider, Func<int> captionHeightProvider)
        { _host = host; _theme = themeProvider; _captionHeight = captionHeightProvider; }
        public void UpdateTheme(IBeepTheme theme) { }
        public void SetShowSystemButtons(bool show) => _showButtons = show;

        public void GetTitleInsets(GraphicsPath captionBounds, float scale, out int leftInset, out int rightInset)
        {
            int pad = DpiScalingHelper.ScaleValue(10, scale);
            int btn = _showButtons ? Math.Max(26, (int)(_captionHeight() - DpiScalingHelper.ScaleValue(6, scale))) : 0;
            int buttons = _showButtons ? 3 : 0;
            rightInset = buttons > 0 ? (buttons * btn + (buttons + 1) * pad) : pad;
            leftInset = pad;
        }

        public void Paint(Graphics g, GraphicsPath captionBounds, float scale, IBeepTheme theme, FormWindowState windowState, out Rectangle invalidatedArea)
        {
            invalidatedArea = Rectangle.Empty;
            var bounds = captionBounds.GetBounds();
            if (!_showButtons) { _closeRect = _maxRect = _minRect = RectangleF.Empty; return; }
            int pad = DpiScalingHelper.ScaleValue(10, scale);
            int btn = Math.Max(26, (int)(_captionHeight() - DpiScalingHelper.ScaleValue(6, scale)));
            float top = bounds.Top + Math.Max(2, (bounds.Height - btn) / 2);

            float x = bounds.Right - pad - btn;
            _closeRect = new RectangleF(x, top, btn, btn);
            x -= (btn + pad);
            _maxRect = new RectangleF(x, top, btn, btn);
            x -= (btn + pad);
            _minRect = new RectangleF(x, top, btn, btn);

            var fore = theme?.AppBarButtonForeColor ?? _host.ForeColor;
            var cinnamonOrange = Color.FromArgb(225, 140, 85); // Cinnamon warm accent
            using var p = new Pen(fore, 1.7f * scale);

            // Modern Cinnamon: warm colors with soft rounded edges
            int radius = DpiScalingHelper.ScaleValue(7, scale);
            
            if (_hoverMin) 
            { 
                using var hb = new SolidBrush(Color.FromArgb(45, cinnamonOrange)); 
                g.FillRoundedRectanglePath(hb, _minRect, radius);
            }
            float y = _minRect.Y + _minRect.Height * 0.58f;
            g.DrawLine(p, _minRect.Left + DpiScalingHelper.ScaleValue(6, scale), y, _minRect.Right - DpiScalingHelper.ScaleValue(6, scale), y);

            if (_hoverMax) 
            { 
                using var hb = new SolidBrush(Color.FromArgb(45, cinnamonOrange)); 
                g.FillRoundedRectanglePath(hb, _maxRect, radius);
            }
            float w = Math.Max(10 * scale, _maxRect.Width / 2.8f);
            float cx = _maxRect.X + _maxRect.Width / 2; 
            float cy = _maxRect.Y + _maxRect.Height / 2;
            var rect = new RectangleF(cx - w / 2, cy - w / 2, w, w);
            if (windowState == FormWindowState.Maximized) 
                g.DrawRectanglePath(p, new RectangleF(rect.X + 2, rect.Y + 2, rect.Width - 4, rect.Height - 4)); 
            else 
                g.DrawRectanglePath(p, rect);

            if (_hoverClose) 
            { 
                using var hb = new SolidBrush(Color.FromArgb(205, 70, 70)); // Warm red
                g.FillRoundedRectanglePath(hb, _closeRect, radius);
                p.Color = Color.White;
            }
            float inset = DpiScalingHelper.ScaleValue(6, scale);
            g.DrawLine(p, _closeRect.Left + inset, _closeRect.Top + inset, _closeRect.Right - inset, _closeRect.Bottom - inset);
            g.DrawLine(p, _closeRect.Right - inset, _closeRect.Top + inset, _closeRect.Left + inset, _closeRect.Bottom - inset);
        }

        public bool OnMouseMove(Point location, out GraphicsPath invalidatedArea)
        {
            var prev = (_hoverClose, _hoverMax, _hoverMin);
            _hoverClose = _closeRect.Contains(location);
            _hoverMax = _maxRect.Contains(location);
            _hoverMin = _minRect.Contains(location);
            if (prev != (_hoverClose, _hoverMax, _hoverMin))
            {
                invalidatedArea = GraphicsExtensions.CreateUnionPath(_closeRect, _maxRect, _minRect);
                return true;
            }
            invalidatedArea = new GraphicsPath(); 
            return false;
        }

        public void OnMouseLeave(out GraphicsPath invalidatedArea)
        {
            if (_hoverClose || _hoverMax || _hoverMin)
            {
                _hoverClose = _hoverMax = _hoverMin = false;
                invalidatedArea = GraphicsExtensions.CreateUnionPath(_closeRect, _maxRect, _minRect);
                return;
            }
            invalidatedArea = new GraphicsPath();
        }

        public bool OnMouseDown(Point location, Form form, out GraphicsPath invalidatedArea)
        {
            invalidatedArea = new GraphicsPath();
            if (_closeRect.Contains(location)) 
            { 
                form.Close(); 
                invalidatedArea = _closeRect.ToGraphicsPath();
                return true; 
            }
            if (_maxRect.Contains(location)) 
            { 
                form.WindowState = form.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized; 
                invalidatedArea = _maxRect.ToGraphicsPath();
                return true; 
            }
            if (_minRect.Contains(location)) 
            { 
                form.WindowState = FormWindowState.Minimized; 
                invalidatedArea = _minRect.ToGraphicsPath();
                return true; 
            }
            return false;
        }

        public bool HitTest(Point location) => _closeRect.Contains(location) || _maxRect.Contains(location) || _minRect.Contains(location);
    }
}

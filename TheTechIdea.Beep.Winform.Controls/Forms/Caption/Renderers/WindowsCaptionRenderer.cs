using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls; // for CaptionGlyphProvider
using TheTechIdea.Beep.Winform.Controls.Helpers;

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
            // Standardize computation across renderers
            int pad = DpiScalingHelper.ScaleValue(8, scale);
            int btn = _showButtons ? Math.Max(24, (int)(_captionHeight() - DpiScalingHelper.ScaleValue(8, scale))) : 0;
            int buttons = _showButtons ? 3 : 0;
            rightInset = buttons > 0 ? (buttons * btn + (buttons + 1) * pad) : pad;
            leftInset = pad;
        }

        public void Paint(Graphics g, Rectangle captionBounds, float scale, IBeepTheme theme, FormWindowState windowState, out Rectangle invalidatedArea)
        {
            invalidatedArea = Rectangle.Empty;
            int pad = DpiScalingHelper.ScaleValue(8, scale);
            int btn = Math.Max(24, (int)(_captionHeight() - DpiScalingHelper.ScaleValue(8, scale)));
            int top = captionBounds.Top + Math.Max(2, (captionBounds.Height - btn) / 2);
            
            if (!_showButtons) { _closeRect = _maxRect = _minRect = Rectangle.Empty; return; }

            int x = captionBounds.Right - pad - btn;
            _closeRect = new Rectangle(x, top, btn, btn);
            x -= (btn + pad);
            _maxRect   = new Rectangle(x, top, btn, btn);
            x -= (btn + pad);
            _minRect   = new Rectangle(x, top, btn, btn);

            // Use AppBarTitleForeColor for icons to ensure contrast with caption bar background
            // AppBarTitleForeColor is designed to contrast with AppBarBackColor (the caption bar),
            // while AppBarButtonForeColor might be intended for buttons on the main form body
            Color iconColor = theme?.AppBarTitleForeColor ?? Color.Empty;
            if (iconColor == Color.Empty)
            {
                var styleColor = theme?.AppBarTitleStyle?.TextColor;
                iconColor = styleColor.HasValue && styleColor.Value.A != 0 ? styleColor.Value : _host.ForeColor;
            }
            using var pen = new Pen(iconColor, 1.5f) { Alignment = PenAlignment.Center };
            // Minimize
            if (_hoverMin) { using var hb = new SolidBrush(theme?.ButtonHoverBackColor ?? Color.FromArgb(30, Color.Gray)); g.FillRectangle(hb, _minRect); }
            CaptionGlyphProvider.DrawMinimize(g, pen, _minRect, scale);
            // Maximize/Restore
            if (_hoverMax) { using var hb = new SolidBrush(theme?.ButtonHoverBackColor ?? Color.FromArgb(30, Color.Gray)); g.FillRectangle(hb, _maxRect); }
            if (windowState == FormWindowState.Maximized)
                CaptionGlyphProvider.DrawRestore(g, pen, _maxRect, scale);
            else
                CaptionGlyphProvider.DrawMaximize(g, pen, _maxRect, scale);
            // Close
            if (_hoverClose) 
            { 
                using var hb = new SolidBrush(theme?.ButtonErrorBackColor ?? Color.IndianRed); 
                g.FillRectangle(hb, _closeRect);
                // Use white for close icon when hovered for maximum contrast with red background
                pen.Color = Color.White;
            }
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

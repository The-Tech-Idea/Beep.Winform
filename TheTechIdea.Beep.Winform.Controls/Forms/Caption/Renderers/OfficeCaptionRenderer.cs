using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Caption.Renderers
{
    /// <summary>
    /// Office-like caption renderer (inspired by DevExpress/Office Blue):
    /// - Right-aligned buttons
    /// - Azure hover fills, red close hover
    /// - Slightly thicker glyphs
    /// </summary>
    internal sealed class OfficeCaptionRenderer : ICaptionRenderer
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
            int pad = DpiScalingHelper.ScaleValue(8, scale);
            int btn = _showButtons ? Math.Max(26, (int)(_captionHeight() - DpiScalingHelper.ScaleValue(6, scale))) : 0; // a little bigger
            int buttons = _showButtons ? 3 : 0;
            rightInset = buttons > 0 ? (buttons * btn + (buttons + 1) * pad) : pad;
            leftInset = pad;
        }

        public void Paint(Graphics g, Rectangle captionBounds, float scale, IBeepTheme theme, FormWindowState windowState, out Rectangle invalidatedArea)
        {
            invalidatedArea = Rectangle.Empty;
            if (!_showButtons) { _closeRect = _maxRect = _minRect = Rectangle.Empty; return; }
            int pad = DpiScalingHelper.ScaleValue(8, scale);
            int btn = Math.Max(28, (int)(_captionHeight() - DpiScalingHelper.ScaleValue(4, scale))); // Larger buttons
            int top = captionBounds.Top + Math.Max(2, (captionBounds.Height - btn) / 2);

            int x = captionBounds.Right - pad - btn;
            _closeRect = new Rectangle(x, top, btn, btn);
            x -= (btn + pad);
            _maxRect = new Rectangle(x, top, btn, btn);
            x -= (btn + pad);
            _minRect = new Rectangle(x, top, btn, btn);

            // Modern Office 365/Microsoft 365 colors
            Color officeBlue = Color.FromArgb(0, 120, 212); // Microsoft Blue
            Color hoverBlue = Color.FromArgb(243, 242, 241); // Neutral gray for light theme
            Color closeRed = Color.FromArgb(196, 43, 28); // Microsoft Red

            using var pen = new Pen(theme?.AppBarButtonForeColor == Color.Empty ? Color.FromArgb(50, 49, 48) : theme.AppBarButtonForeColor, 1.6f * scale);

            // Modern Office: subtle square hover with professional spacing
            if (_hoverMin) 
            { 
                using var hb = new SolidBrush(hoverBlue); 
                g.FillRectangle(hb, _minRect);
                // Subtle border on hover
                using var borderPen = new Pen(Color.FromArgb(200, 198, 196), 1f);
                g.DrawRectangle(borderPen, _minRect);
            }
            CaptionGlyphProvider.DrawMinimize(g, pen, _minRect, scale);

            if (_hoverMax) 
            { 
                using var hb = new SolidBrush(hoverBlue); 
                g.FillRectangle(hb, _maxRect);
                using var borderPen = new Pen(Color.FromArgb(200, 198, 196), 1f);
                g.DrawRectangle(borderPen, _maxRect);
            }
            if (windowState == FormWindowState.Maximized)
                CaptionGlyphProvider.DrawRestore(g, pen, _maxRect, scale);
            else
                CaptionGlyphProvider.DrawMaximize(g, pen, _maxRect, scale);

            if (_hoverClose) 
            { 
                using var hb = new SolidBrush(closeRed); 
                g.FillRectangle(hb, _closeRect);
                pen.Color = Color.White; // White icon on red background
            }
            CaptionGlyphProvider.DrawClose(g, pen, _closeRect, scale);
        }

        public bool OnMouseMove(Point location, out Rectangle invalidatedArea)
        {
            var prev = (_hoverClose, _hoverMax, _hoverMin);
            _hoverClose = _closeRect.Contains(location);
            _hoverMax = _maxRect.Contains(location);
            _hoverMin = _minRect.Contains(location);
            if (prev != (_hoverClose, _hoverMax, _hoverMin))
            { invalidatedArea = Rectangle.Union(Rectangle.Union(_closeRect, _maxRect), _minRect); return true; }
            invalidatedArea = Rectangle.Empty; return false;
        }

        public void OnMouseLeave(out Rectangle invalidatedArea)
        {
            if (_hoverClose || _hoverMax || _hoverMin)
            { _hoverClose = _hoverMax = _hoverMin = false; invalidatedArea = Rectangle.Union(Rectangle.Union(_closeRect, _maxRect), _minRect); return; }
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

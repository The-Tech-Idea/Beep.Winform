using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Caption.Renderers
{
    internal sealed class CorporateCaptionRenderer : ICaptionRenderer
    {
        private Form _host; private Func<IBeepTheme> _theme; private Func<int> _captionHeight; private bool _showButtons = true;
        private Rectangle _closeRect, _maxRect, _minRect; private bool _hoverClose, _hoverMax, _hoverMin;

        public void UpdateHost(Form host, Func<IBeepTheme> themeProvider, Func<int> captionHeightProvider) { _host = host; _theme = themeProvider; _captionHeight = captionHeightProvider; }
        public void UpdateTheme(IBeepTheme theme) { }
        public void SetShowSystemButtons(bool show) => _showButtons = show;

        public void GetTitleInsets(Rectangle captionBounds, float scale, out int leftInset, out int rightInset)
        {
            int pad = DpiScalingHelper.ScaleValue(8, scale); 
            int btn = _showButtons ? Math.Max(24, (int)(_captionHeight() - DpiScalingHelper.ScaleValue(8, scale))) : 0; 
            int buttons = _showButtons ? 3 : 0; 
            rightInset = buttons > 0 ? (buttons * btn + (buttons + 1) * pad) : pad; 
            leftInset = pad;
        }
        public void Paint(Graphics g, Rectangle captionBounds, float scale, IBeepTheme theme, FormWindowState windowState, out Rectangle invalidatedArea)
        {
            invalidatedArea = Rectangle.Empty; 
            if (!_showButtons) { _closeRect = _maxRect = _minRect = Rectangle.Empty; return; }
            int pad = DpiScalingHelper.ScaleValue(10, scale); 
            int btn = Math.Max(26, (int)(_captionHeight() - DpiScalingHelper.ScaleValue(6, scale))); 
            int top = captionBounds.Top + Math.Max(2, (captionBounds.Height - btn) / 2);
            int x = captionBounds.Right - pad - btn; 
            _closeRect = new Rectangle(x, top, btn, btn); 
            x -= (btn + pad); 
            _maxRect = new Rectangle(x, top, btn, btn); 
            x -= (btn + pad); 
            _minRect = new Rectangle(x, top, btn, btn);
            
            // Modern Corporate: professional tones with subtle depth using theme colors
            Color foreColor = theme?.AppBarButtonForeColor ?? _host.ForeColor;
            Color hoverColor = theme?.ButtonHoverBackColor ?? Color.FromArgb(224, 224, 224);
            using var pen = new Pen(foreColor, 1.8f * scale);
            int radius = DpiScalingHelper.ScaleValue(4, scale);

            if (_hoverMin) 
            { 
                using var hb = new SolidBrush(hoverColor); 
                g.FillRoundedRectangle(hb, _minRect, radius);
                // Subtle shadow for depth
                int shadowOffset = DpiScalingHelper.ScaleValue(1, scale);
                using var shadowBrush = new SolidBrush(Color.FromArgb(15, Color.Black));
                var shadowRect = new Rectangle(_minRect.X + shadowOffset, _minRect.Y + shadowOffset, _minRect.Width, _minRect.Height);
                g.FillRoundedRectangle(shadowBrush, shadowRect, radius);
            }
            CaptionGlyphProvider.DrawMinimize(g, pen, _minRect, scale);
            
            if (_hoverMax) 
            { 
                using var hb = new SolidBrush(hoverColor); 
                g.FillRoundedRectangle(hb, _maxRect, radius);
                int shadowOffset = DpiScalingHelper.ScaleValue(1, scale);
                using var shadowBrush = new SolidBrush(Color.FromArgb(15, Color.Black));
                var shadowRect = new Rectangle(_maxRect.X + shadowOffset, _maxRect.Y + shadowOffset, _maxRect.Width, _maxRect.Height);
                g.FillRoundedRectangle(shadowBrush, shadowRect, radius);
            }
            if (windowState == FormWindowState.Maximized) CaptionGlyphProvider.DrawRestore(g, pen, _maxRect, scale); else CaptionGlyphProvider.DrawMaximize(g, pen, _maxRect, scale);
            
            if (_hoverClose) 
            { 
                using var hb = new SolidBrush(Color.FromArgb(196, 43, 28)); // Professional red
                g.FillRoundedRectangle(hb, _closeRect, radius);
                pen.Color = Color.White;
            }
            CaptionGlyphProvider.DrawClose(g, pen, _closeRect, scale);
        }
        public bool OnMouseMove(Point location, out Rectangle invalidatedArea) { var prev = (_hoverClose, _hoverMax, _hoverMin); _hoverClose = _closeRect.Contains(location); _hoverMax = _maxRect.Contains(location); _hoverMin = _minRect.Contains(location); if (prev != (_hoverClose, _hoverMax, _hoverMin)) { invalidatedArea = Rectangle.Union(Rectangle.Union(_closeRect, _maxRect), _minRect); return true; } invalidatedArea = Rectangle.Empty; return false; }
        public void OnMouseLeave(out Rectangle invalidatedArea) { if (_hoverClose || _hoverMax || _hoverMin) { _hoverClose = _hoverMax = _hoverMin = false; invalidatedArea = Rectangle.Union(Rectangle.Union(_closeRect, _maxRect), _minRect); return; } invalidatedArea = Rectangle.Empty; }
        public bool OnMouseDown(Point location, Form form, out Rectangle invalidatedArea) { if (_closeRect.Contains(location)) { form.Close(); invalidatedArea = _closeRect; return true; } if (_maxRect.Contains(location)) { form.WindowState = form.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized; invalidatedArea = _maxRect; return true; } if (_minRect.Contains(location)) { form.WindowState = FormWindowState.Minimized; invalidatedArea = _minRect; return true; } invalidatedArea = Rectangle.Empty; return false; }
        public bool HitTest(Point location) => _closeRect.Contains(location) || _maxRect.Contains(location) || _minRect.Contains(location);
    }
}
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Caption.Renderers
{
    internal sealed class HighContrastCaptionRenderer : ICaptionRenderer
    {
        private Form _host; private Func<IBeepTheme> _theme; private Func<int> _captionHeight; private bool _showButtons = true;
        private RectangleF _closeRect, _maxRect, _minRect; private bool _hoverClose, _hoverMax, _hoverMin;

        public void UpdateHost(Form host, Func<IBeepTheme> themeProvider, Func<int> captionHeightProvider)
        { _host = host; _theme = themeProvider; _captionHeight = captionHeightProvider; }
        public void UpdateTheme(IBeepTheme theme) { }
        public void SetShowSystemButtons(bool show) => _showButtons = show;

        public void GetTitleInsets(GraphicsPath captionBounds, float scale, out int leftInset, out int rightInset)
        {
            int btn = _showButtons ? Math.Max(24, (int)(_captionHeight() - DpiScalingHelper.ScaleValue(8, scale))) : 0;
            int pad = DpiScalingHelper.ScaleValue(8, scale);
            rightInset = _showButtons ? (btn * 3 + pad * 3) : pad; 
            leftInset = pad;
        }

        public void Paint(Graphics g, GraphicsPath captionBounds, float scale, IBeepTheme theme, FormWindowState windowState, out Rectangle invalidatedArea)
        {
            invalidatedArea = Rectangle.Empty;
            var bounds = captionBounds.GetBounds();
            if (!_showButtons) { _closeRect = _maxRect = _minRect = RectangleF.Empty; return; }
            
            int btn = Math.Max(28, (int)(_captionHeight() - DpiScalingHelper.ScaleValue(6, scale))); // Larger for accessibility
            float top = bounds.Top + Math.Max(2, (bounds.Height - btn) / 2);
            int pad = DpiScalingHelper.ScaleValue(10, scale); // More spacing for accessibility
            _closeRect = new RectangleF(bounds.Right - btn - pad, top, btn, btn);
            _maxRect   = new RectangleF(_closeRect.Left - btn - pad, top, btn, btn);
            _minRect   = new RectangleF(_maxRect.Left   - btn - pad, top, btn, btn);

            // Modern High Contrast: WCAG AAA compliant with 4:5:1 contrast ratio
            // Inverted colors on hover for maximum clarity
            DrawHCButton(g, _minRect, scale, isHover: _hoverMin, type: "minimize", windowState);
            DrawHCButton(g, _maxRect, scale, isHover: _hoverMax, type: (windowState == FormWindowState.Maximized ? "restore" : "maximize"), windowState);
            DrawHCButton(g, _closeRect, scale, isHover: _hoverClose, type: "close", windowState);
        }

        private void DrawHCButton(Graphics g, RectangleF rect, float scale, bool isHover, string type, FormWindowState ws)
        {
            // High contrast mode: black background + white glyphs normally, inverted on hover
            Color back = isHover ? Color.White : Color.Black;
            Color fore = isHover ? Color.Black : Color.White;
            Color border = isHover ? Color.Black : Color.White;
            
            using (var b = new SolidBrush(back)) g.FillRectanglePath(b, rect);
            
            // Thick border for high visibility (WCAG guideline)
            using var borderPen = new Pen(border, 3.5f * scale);
            g.DrawRectanglePath(borderPen, rect);
            
            // Extra thick glyphs for better visibility
            using var p = new Pen(fore, 3.5f * scale);
            var rectInt = Rectangle.Round(rect);
            switch (type)
            {
                case "minimize": CaptionGlyphProvider.DrawMinimize(g, p, rectInt, scale); break;
                case "maximize": CaptionGlyphProvider.DrawMaximize(g, p, rectInt, scale); break;
                case "restore": CaptionGlyphProvider.DrawRestore(g, p, rectInt, scale); break;
                case "close": CaptionGlyphProvider.DrawClose(g, p, rectInt, scale); break;
            }
        }

        public bool OnMouseMove(Point location, out GraphicsPath invalidatedArea)
        {
            var prev = (_hoverClose, _hoverMax, _hoverMin);
            _hoverClose = _closeRect.Contains(location); _hoverMax = _maxRect.Contains(location); _hoverMin = _minRect.Contains(location);
            if (prev != (_hoverClose, _hoverMax, _hoverMin)) { invalidatedArea = GraphicsExtensions.CreateUnionPath(_closeRect, _maxRect, _minRect); return true; }
            invalidatedArea = new GraphicsPath(); return false;
        }

        public void OnMouseLeave(out GraphicsPath invalidatedArea)
        {
            if (_hoverClose || _hoverMax || _hoverMin) { _hoverClose = _hoverMax = _hoverMin = false; invalidatedArea = GraphicsExtensions.CreateUnionPath(_closeRect, _maxRect, _minRect); return; }
            invalidatedArea = new GraphicsPath();
        }

        public bool OnMouseDown(Point location, Form form, out GraphicsPath invalidatedArea)
        {
            if (_closeRect.Contains(location)) { form.Close(); invalidatedArea = _closeRect.ToGraphicsPath(); return true; }
            if (_maxRect.Contains(location))  { form.WindowState = form.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized; invalidatedArea = _maxRect.ToGraphicsPath(); return true; }
            if (_minRect.Contains(location))  { form.WindowState = FormWindowState.Minimized; invalidatedArea = _minRect.ToGraphicsPath(); return true; }
            invalidatedArea = new GraphicsPath(); return false;
        }

        public bool HitTest(Point location) => _closeRect.Contains(location) || _maxRect.Contains(location) || _minRect.Contains(location);
    }

    internal sealed class SoftCaptionRenderer : ICaptionRenderer
    {
        private Form _host; private Func<IBeepTheme> _theme; private Func<int> _captionHeight; private bool _showButtons = true;
        private RectangleF _closeRect, _maxRect, _minRect; private bool _hoverClose, _hoverMax, _hoverMin;

        public void UpdateHost(Form host, Func<IBeepTheme> themeProvider, Func<int> captionHeightProvider)
        { _host = host; _theme = themeProvider; _captionHeight = captionHeightProvider; }
        public void UpdateTheme(IBeepTheme theme) { }
        public void SetShowSystemButtons(bool show) => _showButtons = show;

        public void GetTitleInsets(GraphicsPath captionBounds, float scale, out int leftInset, out int rightInset)
        {
            int btn = _showButtons ? Math.Max(24, (int)(_captionHeight() - DpiScalingHelper.ScaleValue(8, scale))) : 0;
            int pad = DpiScalingHelper.ScaleValue(8, scale);
            rightInset = _showButtons ? (btn * 3 + pad * 3) : pad; 
            leftInset = pad;
        }

        public void Paint(Graphics g, GraphicsPath captionBounds, float scale, IBeepTheme theme, FormWindowState windowState, out Rectangle invalidatedArea)
        {
            invalidatedArea = Rectangle.Empty;
            var bounds = captionBounds.GetBounds();
            if (!_showButtons) { _closeRect = _maxRect = _minRect = RectangleF.Empty; return; }
            
            int btn = Math.Max(28, (int)(_captionHeight() - DpiScalingHelper.ScaleValue(6, scale)));
            float top = bounds.Top + Math.Max(2, (bounds.Height - btn) / 2);
            int pad = DpiScalingHelper.ScaleValue(10, scale);
            _closeRect = new RectangleF(bounds.Right - btn - pad, top, btn, btn);
            _maxRect   = new RectangleF(_closeRect.Left - btn - pad, top, btn, btn);
            _minRect   = new RectangleF(_maxRect.Left   - btn - pad, top, btn, btn);

            // Modern Soft UI (Neumorphism): gentle colors with smooth gradients and depth
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            
            // Use theme colors for consistency
            Color buttonColor = theme?.AppBarButtonForeColor ?? Color.FromArgb(120, 150, 220);
            Color closeColor = theme?.AppBarCloseButtonColor ?? Color.FromArgb(220, 120, 150);
            
            DrawSoft(g, _minRect, buttonColor, _hoverMin, scale, "minimize", windowState);
            DrawSoft(g, _maxRect, buttonColor, _hoverMax, scale, (windowState == FormWindowState.Maximized ? "restore" : "maximize"), windowState);
            DrawSoft(g, _closeRect, closeColor, _hoverClose, scale, "close", windowState);
        }

        private void DrawSoft(Graphics g, RectangleF rect, Color baseColor, bool hover, float scale, string type, FormWindowState ws)
        {
            float radius = 14 * scale; // Very rounded for soft look
            
            // Neumorphic gradient effect
            var lightColor = ControlPaint.Light(baseColor, 0.3f);
            var darkColor = ControlPaint.Dark(baseColor, 0.1f);
            var fillColor = hover ? Color.FromArgb(220, baseColor) : Color.FromArgb(160, baseColor);
            
            // Gradient background for depth
            using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                rect, lightColor, darkColor, 135f))
            {
                brush.SetSigmaBellShape(0.5f);
                g.FillRoundedRectanglePath(brush, rect, radius);
            }
            
            // Subtle outer glow on hover
            if (hover)
            {
                using var glowPen = new Pen(Color.FromArgb(60, baseColor), 4f * scale);
                var glowRect = new RectangleF(rect.X - 2, rect.Y - 2, rect.Width + 4, rect.Height + 4);
                g.DrawRoundedRectanglePath(glowPen, glowRect, radius + 2);
            }
            
            // Soft border
            using var borderPen = new Pen(Color.FromArgb(100, ControlPaint.Dark(baseColor, 0.2f)), 1.5f * scale);
            g.DrawRoundedRectanglePath(borderPen, rect, radius);
            
            // White icons for good contrast
            using var icon = new Pen(Color.White, 2.0f * scale) 
            { 
                StartCap = System.Drawing.Drawing2D.LineCap.Round, 
                EndCap = System.Drawing.Drawing2D.LineCap.Round 
            };
            var rectInt = Rectangle.Round(rect);
            switch (type)
            {
                case "minimize": CaptionGlyphProvider.DrawMinimize(g, icon, rectInt, scale); break;
                case "maximize": CaptionGlyphProvider.DrawMaximize(g, icon, rectInt, scale); break;
                case "restore": CaptionGlyphProvider.DrawRestore(g, icon, rectInt, scale); break;
                case "close": CaptionGlyphProvider.DrawClose(g, icon, rectInt, scale); break;
            }
        }

        public bool OnMouseMove(Point location, out GraphicsPath invalidatedArea)
        {
            var prev = (_hoverClose, _hoverMax, _hoverMin);
            _hoverClose = _closeRect.Contains(location); _hoverMax = _maxRect.Contains(location); _hoverMin = _minRect.Contains(location);
            if (prev != (_hoverClose, _hoverMax, _hoverMin)) { invalidatedArea = GraphicsExtensions.CreateUnionPath(_closeRect, _maxRect, _minRect); return true; }
            invalidatedArea = new GraphicsPath(); return false;
        }

        public void OnMouseLeave(out GraphicsPath invalidatedArea)
        {
            if (_hoverClose || _hoverMax || _hoverMin) { _hoverClose = _hoverMax = _hoverMin = false; invalidatedArea = GraphicsExtensions.CreateUnionPath(_closeRect, _maxRect, _minRect); return; }
            invalidatedArea = new GraphicsPath();
        }

        public bool OnMouseDown(Point location, Form form, out GraphicsPath invalidatedArea)
        {
            if (_closeRect.Contains(location)) { form.Close(); invalidatedArea = _closeRect.ToGraphicsPath(); return true; }
            if (_maxRect.Contains(location))  { form.WindowState = form.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized; invalidatedArea = _maxRect.ToGraphicsPath(); return true; }
            if (_minRect.Contains(location))  { form.WindowState = FormWindowState.Minimized; invalidatedArea = _minRect.ToGraphicsPath(); return true; }
            invalidatedArea = new GraphicsPath(); return false;
        }

        public bool HitTest(Point location) => _closeRect.Contains(location) || _maxRect.Contains(location) || _minRect.Contains(location);
    }
}

// Extension methods for rounded rectangles - DEPRECATED: Use GraphicsExtensions.cs instead
internal static class GraphicsExtensions
{
    public static void FillRoundedRectangle(this Graphics g, Brush brush, Rectangle rect, int radius)
    {
        using var path = GetRoundedRectPath(rect, radius);
        g.FillPath(brush, path);
    }

    public static void DrawRoundedRectangle(this Graphics g, Pen pen, Rectangle rect, int radius)
    {
        using var path = GetRoundedRectPath(rect, radius);
        g.DrawPath(pen, path);
    }

    private static System.Drawing.Drawing2D.GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
    {
        var path = new System.Drawing.Drawing2D.GraphicsPath();
        if (radius <= 0) { path.AddRectangle(rect); return path; }
        
        int diameter = Math.Min(radius * 2, Math.Min(rect.Width, rect.Height));
        var arc = new Rectangle(rect.Location, new Size(diameter, diameter));
        
        path.AddArc(arc, 180, 90);
        arc.X = rect.Right - diameter;
        path.AddArc(arc, 270, 90);
        arc.Y = rect.Bottom - diameter;
        path.AddArc(arc, 0, 90);
        arc.X = rect.Left;
        path.AddArc(arc, 90, 90);
        path.CloseFigure();
        return path;
    }
}
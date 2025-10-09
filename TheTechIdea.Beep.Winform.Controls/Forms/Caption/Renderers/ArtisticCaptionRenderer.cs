using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Caption.Renderers
{
    /// <summary>
    /// Artistic-style caption renderer with creative visual elements.
    /// </summary>
    internal sealed class ArtisticCaptionRenderer : ICaptionRenderer
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
            int pad = DpiScalingHelper.ScaleValue(8, scale); 
            int btn = _showButtons ? Math.Max(24, (int)(_captionHeight() - DpiScalingHelper.ScaleValue(8, scale))) : 0; 
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
            int btn = Math.Max(28, (int)(_captionHeight() - DpiScalingHelper.ScaleValue(6, scale))); 
            float top = bounds.Top + Math.Max(2, (bounds.Height - btn) / 2);
            float x = bounds.Right - pad - btn; 
            _closeRect = new RectangleF(x, top, btn, btn); 
            x -= (btn + pad); 
            _maxRect = new RectangleF(x, top, btn, btn); 
            x -= (btn + pad); 
            _minRect = new RectangleF(x, top, btn, btn);
            
            // Modern Artistic: vibrant Material Design 3 colors with smooth circular buttons
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            
            Color[] colors = 
            { 
                Color.FromArgb(103, 80, 164),   // Deep Purple - minimize
                Color.FromArgb(0, 150, 136),     // Teal - maximize
                Color.FromArgb(244, 67, 54)      // Red - close
            };
            RectangleF[] rects = { _minRect, _maxRect, _closeRect };
            bool[] hovers = { _hoverMin, _hoverMax, _hoverClose };
            
            for (int i = 0; i < 3; i++) 
            { 
                // Gradient background for depth
                if (hovers[i]) 
                { 
                    using (var path = GraphicsExtensions.CreateCirclePath(rects[i]))
                    {
                        using var brush = new System.Drawing.Drawing2D.PathGradientBrush(path)
                        {
                            CenterColor = Color.FromArgb(220, colors[i]),
                            SurroundColors = new[] { Color.FromArgb(140, colors[i]) }
                        };
                        g.FillPath(brush, path);
                    }
                }
                else
                {
                    using var hb = new SolidBrush(Color.FromArgb(70, colors[i])); 
                    g.FillCircle(hb, rects[i]);
                }
                
                // Vibrant outline
                using var pen = new Pen(colors[i], 2.2f * scale);
                g.DrawCircle(pen, rects[i]);
            }
            
            using var iconPen = new Pen(Color.White, 2.2f * scale);
            CaptionGlyphProvider.DrawMinimize(g, iconPen, Rectangle.Round(_minRect), scale);
            if (windowState == FormWindowState.Maximized) CaptionGlyphProvider.DrawRestore(g, iconPen, Rectangle.Round(_maxRect), scale); else CaptionGlyphProvider.DrawMaximize(g, iconPen, Rectangle.Round(_maxRect), scale);
            CaptionGlyphProvider.DrawClose(g, iconPen, Rectangle.Round(_closeRect), scale);
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
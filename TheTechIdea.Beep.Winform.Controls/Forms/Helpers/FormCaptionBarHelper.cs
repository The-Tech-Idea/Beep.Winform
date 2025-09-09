using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.BaseImage;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Helpers
{
    internal sealed class FormCaptionBarHelper
    {
        public delegate void PaddingAdjuster(ref Padding padding); // must match BeepiForm signature
        private readonly IBeepModernFormHost _host;
        private readonly FormOverlayPainterRegistry _overlayRegistry;
        private readonly Action<PaddingAdjuster> _registerPaddingProvider;
        
        // Logo/Icon functionality
        private string _logoImagePath = string.Empty;
        private ImagePainter _logoPainter;
        private bool _showLogo = false;
        private Size _logoSize = new Size(20, 20);
        private Padding _logoMargin = new Padding(8, 8, 8, 8);

        public bool ShowCaptionBar { get; set; } = true;
        public int CaptionHeight { get; set; } = 36;
        public bool ShowSystemButtons { get; set; } = true;
        public bool EnableCaptionGradient { get; set; } = true;
        
        // Logo/Icon properties
        public string LogoImagePath
        {
            get => _logoImagePath;
            set
            {
                if (_logoImagePath != value)
                {
                    _logoImagePath = value;
                    InitializeLogoPainter();
                    _showLogo = !string.IsNullOrEmpty(value);
                    Form.Invalidate();
                }
            }
        }

        public bool ShowLogo
        {
            get => _showLogo;
            set
            {
                if (_showLogo != value)
                {
                    _showLogo = value;
                    Form.Invalidate();
                }
            }
        }

        public Size LogoSize
        {
            get => _logoSize;
            set
            {
                if (_logoSize != value)
                {
                    _logoSize = value;
                    if (_showLogo) Form.Invalidate();
                }
            }
        }

        public Padding LogoMargin
        {
            get => _logoMargin;
            set
            {
                if (_logoMargin != value)
                {
                    _logoMargin = value;
                    if (_showLogo) Form.Invalidate();
                }
            }
        }

        public Rectangle CloseRect { get; private set; }
        public Rectangle MaxRect { get; private set; }
        public Rectangle MinRect { get; private set; }
        private bool _hoverClose, _hoverMax, _hoverMin;
        private Form Form => _host.AsForm;
        private IBeepTheme Theme => _host.CurrentTheme;
        
        public FormCaptionBarHelper(IBeepModernFormHost host, FormOverlayPainterRegistry overlayRegistry, Action<PaddingAdjuster> registerPaddingProvider)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));
            _overlayRegistry = overlayRegistry ?? throw new ArgumentNullException(nameof(overlayRegistry));
            _registerPaddingProvider = registerPaddingProvider ?? throw new ArgumentNullException(nameof(registerPaddingProvider));
            _overlayRegistry.Add(PaintOverlay);
            _registerPaddingProvider((ref Padding p) => { if (ShowCaptionBar) p.Top += CaptionHeight; });
        }

        private void InitializeLogoPainter()
        {
            _logoPainter?.Dispose();
            _logoPainter = null;

            if (!string.IsNullOrEmpty(_logoImagePath))
            {
                try
                {
                    _logoPainter = new ImagePainter(_logoImagePath, Theme);
                    _logoPainter.ScaleMode = ImageScaleMode.KeepAspectRatio;
                    _logoPainter.Alignment = ContentAlignment.MiddleCenter;
                    _logoPainter.ApplyThemeOnImage = true;
                    _logoPainter.ImageEmbededin = ImageEmbededin.Form;
                }
                catch (Exception)
                {
                    // Handle image loading errors silently
                    _logoPainter = null;
                }
            }
        }

        public void UpdateTheme()
        {
            // Update logo painter with current theme
            if (_logoPainter != null && Theme != null)
            {
                _logoPainter.CurrentTheme = Theme;
                if (_logoPainter.ApplyThemeOnImage)
                {
                    // Force theme reapplication
                    Form.Invalidate();
                }
            }
        }

        public bool IsPointInSystemButtons(Point p) => ShowSystemButtons && (CloseRect.Contains(p) || MaxRect.Contains(p) || MinRect.Contains(p));
        public bool IsCursorOverSystemButton => IsPointInSystemButtons(Form.PointToClient(Cursor.Position));
        
        public void OnMouseMove(MouseEventArgs e)
        {
            if (!ShowCaptionBar || !ShowSystemButtons) return;
            var prev = (_hoverClose, _hoverMax, _hoverMin);
            _hoverClose = CloseRect.Contains(e.Location);
            _hoverMax = MaxRect.Contains(e.Location);
            _hoverMin = MinRect.Contains(e.Location);
            if (prev != (_hoverClose, _hoverMax, _hoverMin))
            {
                var invalid = Rectangle.Union(CloseRect, MaxRect);
                invalid = Rectangle.Union(invalid, MinRect);
                Form.Invalidate(invalid);
            }
        }
        
        public void OnMouseLeave()
        {
            if (_hoverClose || _hoverMax || _hoverMin)
            {
                _hoverClose = _hoverMax = _hoverMin = false;
                var invalid = Rectangle.Union(CloseRect, MaxRect);
                invalid = Rectangle.Union(invalid, MinRect);
                Form.Invalidate(invalid);
            }
        }
        
        public void OnMouseDown(MouseEventArgs e)
        {
            if (!ShowCaptionBar || !ShowSystemButtons) return;
            if (CloseRect.Contains(e.Location)) Form.Close();
            else if (MaxRect.Contains(e.Location)) ToggleMaximize();
            else if (MinRect.Contains(e.Location)) Form.WindowState = FormWindowState.Minimized;
        }
        
        private void ToggleMaximize() => Form.WindowState = Form.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
        
        private void LayoutButtons()
        {
            if (!ShowCaptionBar || !ShowSystemButtons)
            { CloseRect = MaxRect = MinRect = Rectangle.Empty; return; }
            float scale = Form.DeviceDpi / 96f;
            int btnSize = Math.Max(24, (int)(CaptionHeight - 8 * scale));
            int top = (int)(4 * scale);
            CloseRect = new Rectangle(Form.ClientSize.Width - btnSize - 8, top, btnSize, btnSize);
            MaxRect = new Rectangle(CloseRect.Left - btnSize - 6, top, btnSize, btnSize);
            MinRect = new Rectangle(MaxRect.Left - btnSize - 6, top, btnSize, btnSize);
        }
        
        private void PaintOverlay(Graphics g)
        {
            if (!ShowCaptionBar) return;
            LayoutButtons();
            var rect = new Rectangle(0, 0, Form.ClientSize.Width, CaptionHeight);
            if (rect.Width <= 0 || rect.Height <= 0) return;
            
            // Draw caption background
            if (EnableCaptionGradient && Theme != null && Theme.AppBarBackColor != Color.Empty)
            {
                using var brush = new LinearGradientBrush(rect, ControlPaint.Light(Theme.AppBarBackColor, .05f), ControlPaint.Dark(Theme.AppBarBackColor, .05f), LinearGradientMode.Vertical);
                g.FillRectangle(brush, rect);
            }
            else
            {
                using var b = new SolidBrush(Theme?.AppBarBackColor ?? SystemColors.ControlDark);
                g.FillRectangle(b, rect);
            }

            // Draw logo/icon using ImagePainter (if enabled)
            int titleStartX = 10; // Default title start position
            if (_showLogo && _logoPainter != null && _logoPainter.HasImage)
            {
                var logoRect = new Rectangle(
                    _logoMargin.Left,
                    _logoMargin.Top + (CaptionHeight - _logoSize.Height) / 2,
                    _logoSize.Width,
                    _logoSize.Height
                );
                
                try
                {
                    _logoPainter.DrawImage(g, logoRect);
                    // Adjust title start position to account for logo
                    titleStartX = logoRect.Right + _logoMargin.Right;
                }
                catch (Exception)
                {
                    // Handle painting errors silently
                }
            }

            // Draw form title
            using (var sf = new StringFormat { LineAlignment = StringAlignment.Center })
            using (var titleBrush = new SolidBrush(Theme?.AppBarTitleForeColor ?? Form.ForeColor))
            {
                var titleRect = new Rectangle(titleStartX, 0, rect.Width - titleStartX - 160, rect.Height);
                g.DrawString(Form.Text, Form.Font, titleBrush, titleRect, sf);
            }
            
            // Draw system buttons
            if (ShowSystemButtons)
            {
                using var pen = new Pen(Theme?.AppBarButtonForeColor ?? Form.ForeColor, 1.5f) { Alignment = PenAlignment.Center };
                float scale = Form.DeviceDpi / 96f;
                
                // Minimize button
                if (_hoverMin) { using var hb = new SolidBrush(Theme?.ButtonHoverBackColor ?? Color.LightGray); g.FillRectangle(hb, MinRect); }
                CaptionGlyphProvider.DrawMinimize(g, pen, MinRect, scale);
                
                // Maximize/Restore button
                if (_hoverMax) { using var hb = new SolidBrush(Theme?.ButtonHoverBackColor ?? Color.LightGray); g.FillRectangle(hb, MaxRect); }
                if (Form.WindowState == FormWindowState.Maximized) 
                    CaptionGlyphProvider.DrawRestore(g, pen, MaxRect, scale); 
                else 
                    CaptionGlyphProvider.DrawMaximize(g, pen, MaxRect, scale);
                
                // Close button
                if (_hoverClose) { using var hb = new SolidBrush(Theme?.ButtonErrorBackColor ?? Color.IndianRed); g.FillRectangle(hb, CloseRect); }
                CaptionGlyphProvider.DrawClose(g, pen, CloseRect, scale);
            }
        }

        public void Dispose()
        {
            _logoPainter?.Dispose();
        }
    }
}

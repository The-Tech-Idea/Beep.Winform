using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Helpers
{
    /// <summary>
    /// Helper for painting form backgrounds with gradient and texture support.
    /// Handles background fills and optional acrylic fallback rendering.
    /// </summary>
    internal class FormBackgroundPainter : IDisposable
    {
        private readonly IBeepModernFormHost _host;
        private bool _enableGradient = false;
        private Color _gradientStartColor = Color.White;
        private Color _gradientEndColor = Color.LightGray;
        private LinearGradientMode _gradientMode = LinearGradientMode.Vertical;
        private string _backgroundImagePath = "";
        private Image _backgroundImage;
        private bool _disposed = false;

        /// <summary>Gets or sets whether gradient background is enabled</summary>
        public bool EnableGradient
        {
            get => _enableGradient;
            set { _enableGradient = value; _host.Invalidate(); }
        }

        /// <summary>Gets or sets the gradient start color</summary>
        public Color GradientStartColor
        {
            get => _gradientStartColor;
            set { _gradientStartColor = value; _host.Invalidate(); }
        }

        /// <summary>Gets or sets the gradient end color</summary>
        public Color GradientEndColor
        {
            get => _gradientEndColor;
            set { _gradientEndColor = value; _host.Invalidate(); }
        }

        /// <summary>Gets or sets the gradient direction</summary>
        public LinearGradientMode GradientMode
        {
            get => _gradientMode;
            set { _gradientMode = value; _host.Invalidate(); }
        }

        /// <summary>Gets or sets the background image path</summary>
        public string BackgroundImagePath
        {
            get => _backgroundImagePath;
            set
            {
                if (_backgroundImagePath != value)
                {
                    _backgroundImagePath = value;
                    LoadBackgroundImage();
                }
            }
        }

        public FormBackgroundPainter(IBeepModernFormHost host)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));
        }

        /// <summary>
        /// Paints the background within the specified bounds.
        /// </summary>
        /// <param name="g">Graphics context</param>
        /// <param name="bounds">Paint bounds</param>
        public void Paint(Graphics g, Rectangle bounds)
        {
            if (_disposed || bounds.Width <= 0 || bounds.Height <= 0)
                return;

            try
            {
                // Paint background image if available
                if (_backgroundImage != null)
                {
                    g.DrawImage(_backgroundImage, bounds);
                    return;
                }

                // Paint gradient background
                if (_enableGradient)
                {
                    using var brush = new LinearGradientBrush(bounds, _gradientStartColor, _gradientEndColor, _gradientMode);
                    g.FillRectangle(brush, bounds);
                }
                else
                {
                    // Use theme background color or form background color
                    var bgColor = _host.CurrentTheme?.BackColor ?? _host.AsForm.BackColor;
                    if (bgColor != Color.Empty && bgColor != Color.Transparent)
                    {
                        using var brush = new SolidBrush(bgColor);
                        g.FillRectangle(brush, bounds);
                    }
                }
            }
            catch (Exception ex)
            {
                // Fallback to solid color
                var fallbackColor = _host.CurrentTheme?.BackColor ?? SystemColors.Control;
                using var brush = new SolidBrush(fallbackColor);
                g.FillRectangle(brush, bounds);
            }
        }

        /// <summary>
        /// Applies theme colors to background painting.
        /// </summary>
        public void ApplyTheme()
        {
            if (_host.CurrentTheme != null)
            {
                // Update gradient colors from theme if not explicitly set
                if (_gradientStartColor == Color.White && _gradientEndColor == Color.LightGray)
                {
                    _gradientStartColor = ControlPaint.Light(_host.CurrentTheme.BackColor, 0.1f);
                    _gradientEndColor = ControlPaint.Dark(_host.CurrentTheme.BackColor, 0.1f);
                }
                
                // Load background image from theme if available - use ThemeName as fallback
                if (string.IsNullOrEmpty(_backgroundImagePath) && !string.IsNullOrEmpty(_host.CurrentTheme.ThemeName))
                {
                    // BackgroundImagePath = _host.CurrentTheme.ThemeName; // Could map theme name to image path
                }
            }
        }

        private void LoadBackgroundImage()
        {
            try
            {
                _backgroundImage?.Dispose();
                _backgroundImage = null;

                if (!string.IsNullOrEmpty(_backgroundImagePath) && System.IO.File.Exists(_backgroundImagePath))
                {
                    _backgroundImage = Image.FromFile(_backgroundImagePath);
                }
            }
            catch
            {
                // Ignore image loading errors
                _backgroundImage = null;
            }

            _host.Invalidate();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _backgroundImage?.Dispose();
                _disposed = true;
            }
        }
    }
}
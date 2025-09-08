using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepiForm
    {
        // Style properties
        private BeepFormStyle _formStyle = BeepFormStyle.Modern;
        private Color _shadowColor = Color.FromArgb(50, 0, 0, 0);
        private bool _enableGlow = true;
        private Color _glowColor = Color.FromArgb(100, 72, 170, 255);
        private float _glowSpread = 8f;
        private int _shadowDepth = 6;

        // Overlay painters registration (multicast)
        private readonly List<Action<Graphics>> _overlayPainters = new();
        protected void RegisterOverlayPainter(Action<Graphics> painter) { if (painter != null) _overlayPainters.Add(painter); }

        [Category("Beep Style")]
        [DefaultValue(BeepFormStyle.Modern)]
        public BeepFormStyle FormStyle
        {
            get => _formStyle;
            set { if (_formStyle != value) { _formStyle = value; ApplyFormStyle(); Invalidate(); } }
        }

        [Category("Beep Style")]
        public Color ShadowColor
        {
            get => _shadowColor;
            set { if (_shadowColor != value) { _shadowColor = value; Invalidate(); } }
        }

        [Category("Beep Style")]
        [DefaultValue(6)]
        public int ShadowDepth
        {
            get => _shadowDepth;
            set { if (_shadowDepth != value) { _shadowDepth = value; Invalidate(); } }
        }

        [Category("Beep Style")]
        [DefaultValue(true)]
        public bool EnableGlow
        {
            get => _enableGlow;
            set { if (_enableGlow != value) { _enableGlow = value; Invalidate(); } }
        }

        [Category("Beep Style")]
        public Color GlowColor
        {
            get => _glowColor;
            set { if (_glowColor != value) { _glowColor = value; Invalidate(); } }
        }

        [Category("Beep Style")]
        [DefaultValue(8f)]
        public float GlowSpread
        {
            get => _glowSpread;
            set { if (Math.Abs(_glowSpread - value) > float.Epsilon) { _glowSpread = Math.Max(0f, value); Invalidate(); } }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BeepFormStylePresets StylePresets { get; } = new();

        public void ApplyPreset(string key)
        {
            if (StylePresets.TryGet(key, out var m))
            {
                _borderRadius = m.BorderRadius;
                _borderThickness = m.BorderThickness;
                _shadowDepth = m.ShadowDepth;
                _enableGlow = m.EnableGlow;
                _glowSpread = m.GlowSpread;
                try { _captionHeight = m.CaptionHeight; } catch { }
                try { _ribbonHeight = m.RibbonHeight; } catch { }
                Invalidate();
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            // Just invalidate - we'll always regenerate the path when painting
            base.OnSizeChanged(e);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        { 
            // Simple drawing during active resize/move
          

            base.OnPaint(e);
            
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            if (WindowState != FormWindowState.Maximized)
            {
                if (_shadowDepth > 0)
                {
                    using var path = GetFormPath();
                    using var shadowBrush = new SolidBrush(_shadowColor);
                    g.TranslateTransform(_shadowDepth, _shadowDepth);
                    g.FillPath(shadowBrush, path);
                    g.TranslateTransform(-_shadowDepth, -_shadowDepth);
                }

                if (_enableGlow && _glowSpread > 0f)
                {
                    using var path = GetFormPath();
                    using var glowPen = new Pen(_glowColor, _glowSpread) { LineJoin = LineJoin.Round };
                    g.DrawPath(glowPen, path);
                }
            }

            using (var path = GetFormPath())
            {
                using var backBrush = new SolidBrush(BackColor);
                g.FillPath(backBrush, path);

                if (_borderThickness > 0)
                {
                    using var borderPen = new Pen(BorderColor, _borderThickness);
                    g.DrawPath(borderPen, path);
                }
            }

            // Overlay painters
            foreach (var painter in _overlayPainters) painter(g);
        }

        private GraphicsPath GetFormPath()
        {
            // Always create a fresh path - no caching
            // This ensures we always draw with current dimensions
            var path = new GraphicsPath();
            
            // Use current client dimensions directly
            int currentWidth = ClientSize.Width;
            int currentHeight = ClientSize.Height;
            
            // Safety check
            if (currentWidth <= 0 || currentHeight <= 0)
            {
                return path; // Empty path
            }

            var rect = new Rectangle(0, 0, currentWidth, currentHeight);

            if (_borderRadius > 0 && WindowState != FormWindowState.Maximized)
            {
                int diameter = Math.Min(_borderRadius * 2, Math.Min(rect.Width, rect.Height));
                if (diameter > 0)
                {
                    var arcRect = new Rectangle(rect.Location, new Size(diameter, diameter));
                    path.AddArc(arcRect, 180, 90);
                    arcRect.X = rect.Right - diameter; path.AddArc(arcRect, 270, 90);
                    arcRect.Y = rect.Bottom - diameter; path.AddArc(arcRect, 0, 90);
                    arcRect.X = rect.Left; path.AddArc(arcRect, 90, 90);
                    path.CloseFigure();
                }
                else
                {
                    path.AddRectangle(rect);
                }
            }
            else
            {
                path.AddRectangle(rect);
            }

            return path;
        }

        private void ApplyMetrics(BeepFormStyle style)
        {
            if (!BeepFormStyleMetricsDefaults.Map.TryGetValue(style, out var m)) return;
            _borderRadius = m.BorderRadius;
            _borderThickness = m.BorderThickness;
            _shadowDepth = m.ShadowDepth;
            _enableGlow = m.EnableGlow;
            _glowSpread = m.GlowSpread;
            // Sync caption/ribbon sizes if caption feature exists
            try { _captionHeight = m.CaptionHeight; } catch { }
            try { _ribbonHeight = m.RibbonHeight; } catch { }
        }

        private void ApplyThemeMapping()
        {
            if (_currentTheme == null) return;
            BackColor = _currentTheme.ButtonBackColor;
            BorderColor = _currentTheme.BorderColor;
        }

        partial void OnApplyFormStyle()
        {
            ApplyMetrics(_formStyle);
            switch (_formStyle)
            {
                case BeepFormStyle.Modern:
                    ApplyThemeMapping();
                    _glowColor = Color.FromArgb(100, 72, 170, 255);
                    break;
                case BeepFormStyle.Metro:
                    ApplyThemeMapping();
                    break;
                case BeepFormStyle.Glass:
                    ApplyThemeMapping();
                    _glowColor = Color.FromArgb(120, 255, 255, 255);
                    break;
                case BeepFormStyle.Office:
                    ApplyThemeMapping();
                    break;
                case BeepFormStyle.ModernDark:
                    _borderRadius = 8; _borderThickness = 1; _shadowDepth = 8; _enableGlow = true; _glowColor = Color.FromArgb(80, 0, 0, 0); _glowSpread = 10f; BackColor = Color.FromArgb(32, 32, 32); BorderColor = Color.FromArgb(64, 64, 64);
                    break;
                case BeepFormStyle.Material:
                    ApplyThemeMapping();
                    _borderThickness = 0;
                    _shadowDepth = 5;
                    _enableGlow = true;
                    _glowColor = Color.FromArgb(60, 0, 0, 0);
                    _glowSpread = 15f;
                    BorderColor = Color.Transparent;
                    break;
                case BeepFormStyle.Minimal:
                    ApplyThemeMapping();
                    _shadowDepth = 0; _enableGlow = false;
                    break;
                case BeepFormStyle.Classic:
                    BackColor = SystemColors.Control; BorderColor = SystemColors.ActiveBorder;
                    _shadowDepth = 0; _enableGlow = false;
                    break;
                case BeepFormStyle.Custom:
                    ApplyThemeMapping();
                    break;
            }
            Invalidate();
            ApplyAcrylicEffectIfNeeded();
            OnApplyFormStyleAnimated();
        }

        partial void ApplyAcrylicEffectIfNeeded();
        partial void OnApplyFormStyleAnimated();
    }
}

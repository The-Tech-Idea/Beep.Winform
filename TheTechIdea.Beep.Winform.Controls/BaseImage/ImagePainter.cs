using Svg;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using TheTechIdea.Beep.Desktop.Common.Util;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Vis.Modules.Managers;
using TheTechIdea.Beep.Winform.Controls.Converters;

namespace TheTechIdea.Beep.Winform.Controls.BaseImage
{
    /// <summary>
    /// Standalone image painter that provides BeepImage rendering capabilities
    /// without requiring a control instance. Uses partials for clean separation
    /// of core, drawing, loading, theme, and geometry logic.
    /// </summary>
    public partial class ImagePainter : IDisposable
    {
        // Core fields
        protected Image _regularImage;
        protected SvgDocument _svgDocument;
        protected bool _isSvg = false;
        protected string _imagePath;

        // Transformation
        protected float _manualRotationAngle = 0;
        protected bool _flipX = false;
        protected bool _flipY = false;
        protected float _scaleFactor = 1.0f;
        protected ImageScaleMode _scaleMode = ImageScaleMode.KeepAspectRatio;
        protected System.Windows.Forms.Padding _contentPadding = System.Windows.Forms.Padding.Empty;
        protected System.Drawing.ContentAlignment _alignment = System.Drawing.ContentAlignment.MiddleCenter;

        // Effects
        protected bool _grayscale = false;
        protected float _opacity = 1.0f;
        protected ImageClipShape _clipShape = ImageClipShape.None;
        protected float _cornerRadius = 10f;
        protected System.Drawing.Drawing2D.GraphicsPath _customClipPath = null;
        protected bool _drawBackground = false;
        protected Color _backgroundColor = Color.Transparent;
        protected bool _drawBorder = false;
        protected Color _borderColor = Color.Transparent;
        protected float _borderThickness = 1f;

        // Theme
        protected IBeepTheme _currentTheme;
        protected ImageEmbededin _imageEmbededin = ImageEmbededin.Button;
        protected bool _applyThemeOnImage = false;
        protected Color _fillColor = Color.Black;
        protected Color _strokeColor = Color.Black;

        // Quality
        public SmoothingMode Smoothing { get; set; } = SmoothingMode.AntiAlias;
        public InterpolationMode Interpolation { get; set; } = InterpolationMode.HighQualityBicubic;
        public PixelOffsetMode PixelOffset { get; set; } = PixelOffsetMode.HighQuality;
        public TextRenderingHint TextRendering { get; set; } = TextRenderingHint.ClearTypeGridFit;

        // Caching
        protected Bitmap _cachedRenderedImage;
        protected Rectangle _cachedImageRect;
        protected bool _stateChanged = true;
        protected string _lastImagePath;
        protected Rectangle _lastImageRect;
        protected float _lastRotation;
        protected float _lastScale;
        protected float _lastAlpha;
        protected int _lastShakeOffset;
        protected ImageClipShape _lastClipShape;
        protected bool _lastFlipX;
        protected bool _lastFlipY;
        protected int _lastColorSignature;
        protected readonly object _cacheLock = new object();
        public bool UseCaching { get; set; } = true;

        // Animation snapshot values
        protected float _pulseScale = 1.0f;
        protected float _fadeAlpha = 1.0f;
        protected int _shakeOffset = 0;

        public ImagePainter() { }
        public ImagePainter(string imagePath) : this() => ImagePath = imagePath;
        public ImagePainter(string imagePath, IBeepTheme theme) : this(imagePath) => CurrentTheme = theme;

        // Properties (core)
        public string ImagePath
        {
            get => _imagePath;
            set
            {
                if (_imagePath == value) return;
                _imagePath = value;
                _stateChanged = true;
                InvalidateCache();
                if (!string.IsNullOrEmpty(_imagePath)) LoadImage(_imagePath); else ClearImage();
            }
        }

        public Image Image
        {
            get => _regularImage;
            set
            {
                if (ReferenceEquals(_regularImage, value)) return;
                DisposeImages();
                _regularImage = value;
                _isSvg = false;
                _stateChanged = true;
                InvalidateCache();
            }
        }

        public SvgDocument SvgDocument
        {
            get => _svgDocument;
            set
            {
                if (ReferenceEquals(_svgDocument, value)) return;
                DisposeImages();
                _svgDocument = value;
                _isSvg = value != null;
                _stateChanged = true;
                InvalidateCache();
            }
        }
        private string _themeName;
       [Browsable(true)]
        [Category("Appearance")]
        [TypeConverter(typeof(ThemeEnumConverter))]
        public string Theme
        {
            get => _themeName;
            set
            {
                _themeName = value;
                _currentTheme = BeepThemesManager.GetTheme(value);
                //ApplyTheme();
            }
        }
        public float ManualRotationAngle { get => _manualRotationAngle; set { if (Math.Abs(_manualRotationAngle - value) > 0.1f) { _manualRotationAngle = value; _stateChanged = true; InvalidateCache(); } } }
        public bool FlipX { get => _flipX; set { if (_flipX != value) { _flipX = value; _stateChanged = true; InvalidateCache(); } } }
        public bool FlipY { get => _flipY; set { if (_flipY != value) { _flipY = value; _stateChanged = true; InvalidateCache(); } } }
        public float ScaleFactor { get => _scaleFactor; set { _scaleFactor = value; _stateChanged = true; InvalidateCache(); } }
        public ImageScaleMode ScaleMode { get => _scaleMode; set { _scaleMode = value; _stateChanged = true; InvalidateCache(); } }
        public System.Drawing.ContentAlignment Alignment { get => _alignment; set { _alignment = value; _stateChanged = true; InvalidateCache(); } }
        public System.Windows.Forms.Padding ContentPadding { get => _contentPadding; set { _contentPadding = value; _stateChanged = true; InvalidateCache(); } }
        public bool Grayscale { get => _grayscale; set { _grayscale = value; _stateChanged = true; InvalidateCache(); } }
        public float Opacity { get => _opacity; set { _opacity = Math.Max(0, Math.Min(1, value)); _stateChanged = true; InvalidateCache(); } }
        public ImageClipShape ClipShape { get => _clipShape; set { if (_clipShape != value) { _clipShape = value; _stateChanged = true; InvalidateCache(); } } }
        public float CornerRadius { get => _cornerRadius; set { _cornerRadius = Math.Max(0, value); _stateChanged = true; InvalidateCache(); } }
        public System.Drawing.Drawing2D.GraphicsPath CustomClipPath { get => _customClipPath; set { _customClipPath = value; if (value != null) ClipShape = ImageClipShape.Custom; _stateChanged = true; InvalidateCache(); } }
        public bool DrawBackground { get => _drawBackground; set { _drawBackground = value; _stateChanged = true; InvalidateCache(); } }
        public Color BackgroundColor { get => _backgroundColor; set { _backgroundColor = value; _stateChanged = true; InvalidateCache(); } }
        public bool DrawBorder { get => _drawBorder; set { _drawBorder = value; _stateChanged = true; InvalidateCache(); } }
        public Color BorderColor { get => _borderColor; set { _borderColor = value; _stateChanged = true; InvalidateCache(); } }
        public float BorderThickness { get => _borderThickness; set { _borderThickness = Math.Max(0.1f, value); _stateChanged = true; InvalidateCache(); } }

        public IBeepTheme CurrentTheme { get => _currentTheme; set { _currentTheme = value; if (_applyThemeOnImage) ApplyThemeToSvg(); _stateChanged = true; InvalidateCache(); } }
        public ImageEmbededin ImageEmbededin { get => _imageEmbededin; set { if (_imageEmbededin != value) { _imageEmbededin = value; _stateChanged = true; InvalidateCache(); if (_applyThemeOnImage) ApplyThemeToSvg(); } } }
        public bool ApplyThemeOnImage { get => _applyThemeOnImage; set { _applyThemeOnImage = value; _stateChanged = true; InvalidateCache(); if (value) ApplyThemeToSvg(); } }
        public Color FillColor { get => _fillColor; set { _fillColor = value; _stateChanged = true; InvalidateCache(); } }
        public Color StrokeColor { get => _strokeColor; set { _strokeColor = value; _stateChanged = true; InvalidateCache(); } }

        public bool HasImage => (_isSvg && _svgDocument != null) || _regularImage != null;
        public Size ImageSize => _isSvg && _svgDocument != null
            ? new Size((int)_svgDocument.GetDimensions().Width, (int)_svgDocument.GetDimensions().Height)
            : (_regularImage?.Size ?? Size.Empty);

        // Animation properties (external drivers can set)
        public float PulseScale { get => _pulseScale; set { _pulseScale = value; _stateChanged = true; InvalidateCache(); } }
        public float FadeAlpha { get => _fadeAlpha; set { _fadeAlpha = value; _stateChanged = true; InvalidateCache(); } }
        public int ShakeOffset { get => _shakeOffset; set { _shakeOffset = value; _stateChanged = true; InvalidateCache(); } }

        protected void InvalidateCache()
        {
            lock (_cacheLock)
            {
                _cachedRenderedImage?.Dispose();
                _cachedRenderedImage = null;
            }
        }

        // IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                InvalidateCache();
                DisposeImages();
                _customClipPath?.Dispose();
            }
        }

        public void ClearImage()
        {
            DisposeImages();
            _stateChanged = true;
            InvalidateCache();
        }
    }
}

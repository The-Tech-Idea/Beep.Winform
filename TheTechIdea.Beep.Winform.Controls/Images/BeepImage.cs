using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Text.RegularExpressions;
using Svg;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Converters;
using TheTechIdea.Beep.Winform.Controls.Images.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Images
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(BeepImage))]
    [Category("Beep Controls")]
    [DisplayName("Beep Image")]
    [DesignTimeVisible(true)]
    [Description("A control that displays an image (SVG, PNG, JPG, BMP) with enhanced clipping support.")]
    public partial class BeepImage : BaseControl
    {
        #region Fields
        private Image regularImage;
        private bool isSvg = false;
        private string _advancedImagePath;
        private bool _flipX = false;
        private bool _flipY = false;

        // Property for the image path (SVG, PNG, JPG, BMP)
        protected string _imagepath;
        private ImageClipShape _clipShape = ImageClipShape.None;
        private GraphicsPath _customClipPath = null;
        private float _cornerRadius = 10f; // For rounded rectangle
        private bool _useRegionClipping = false; // Use Region-based clipping for better performance

        #region State Tracking Fields
        // Add these fields to track image state
        private string _lastImagePath;
        private Rectangle _lastImageRect;
        private float _lastRotation;
        private float _lastScale;
        private float _lastAlpha;
        private int _lastShakeOffset;
        private ImageClipShape _lastClipShape;
        private bool _lastFlipX;
        private bool _lastFlipY;
        private bool _stateChanged = true;
        private Bitmap _cachedRenderedImage;
        private Rectangle _cachedImageRect;
        #endregion
        #endregion

        #region Properties
        private bool _preserveSvgBackgrounds = false;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("When true, background elements in SVG images retain their original colors during theme application.")]
        public bool PreserveSvgBackgrounds
        {
            get => _preserveSvgBackgrounds;
            set
            {
                _preserveSvgBackgrounds = value;
                if (_applyThemeOnImage && svgDocument != null)
                {
                    ApplyThemeToSvg();
                    Invalidate();
                }
            }
        }

        public SvgDocument svgDocument { get; private set; }
        // Cache to avoid re-applying the same theme repeatedly
        private string _lastSvgThemeSignature;
        // Placeholder for future rasterized bitmap cache keyed by signature+size
        private readonly Dictionary<string, Bitmap> _rasterizedSvgCache = new Dictionary<string, Bitmap>();

        [Category("Appearance")]
        [Description("Determines the shape used to clip the image")]
        public ImageClipShape ClipShape
        {
            get => _clipShape;
            set
            {
                if (_clipShape != value)
                {
                    _clipShape = value;
                    _stateChanged = true;
                    _cachedRenderedImage?.Dispose();
                    _cachedRenderedImage = null;
                    Invalidate();
                }
            }
        }

        [Category("Appearance")]
        [Description("Corner radius for RoundedRect shape")]
        public float CornerRadius
        {
            get => _cornerRadius;
            set
            {
                _cornerRadius = Math.Max(0, value);
                _stateChanged = true;
                _cachedRenderedImage?.Dispose();
                _cachedRenderedImage = null;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("Use region-based clipping for better performance with complex shapes")]
        [DefaultValue(false)]
        public bool UseRegionClipping
        {
            get => _useRegionClipping;
            set
            {
                if (_useRegionClipping != value)
                {
                    _useRegionClipping = value;
                    _stateChanged = true;
                    _cachedRenderedImage?.Dispose();
                    _cachedRenderedImage = null;
                    Invalidate();
                }
            }
        }

        [Browsable(false)]
        public GraphicsPath CustomClipPath
        {
            get => _customClipPath;
            set
            {
                _customClipPath = value;
                if (value != null)
                    ClipShape = ImageClipShape.Custom;
                _stateChanged = true;
                _cachedRenderedImage?.Dispose();
                _cachedRenderedImage = null;
                Invalidate();
            }
        }

        private int _baseSize = 50; // Default size
        private bool _grayscale = false;
        [Category("Effects")]
        public bool Grayscale
        {
            get => _grayscale;
            set { _grayscale = value; _stateChanged = true; _cachedRenderedImage?.Dispose(); _cachedRenderedImage = null; Invalidate(); }
        }

        private float _opacity = 1.0f;
        [Category("Effects")]
        [Description("Opacity from 0.0 (transparent) to 1.0 (opaque).")]
        public float Opacity
        {
            get => _opacity;
            set { _opacity = Math.Max(0, Math.Min(1, value)); _stateChanged = true; _cachedRenderedImage?.Dispose(); _cachedRenderedImage = null; Invalidate(); }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The default size of the image before scaling.")]
        public int BaseSize
        {
            get => _baseSize;
            set
            {
                _baseSize = value;
                if (Size.Width <= _baseSize) // If not scaled, apply base size
                {
                    Size = new Size(_baseSize, _baseSize);
                }
                _stateChanged = true;
                _cachedRenderedImage?.Dispose();
                _cachedRenderedImage = null;
                Invalidate();
            }
        }

        Color fillColor;
        Color strokeColor;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("fill color")]
        public Color FillColor
        {
            get => fillColor;
            set
            {
                fillColor = value;
                _stateChanged = true;
                _cachedRenderedImage?.Dispose();
                _cachedRenderedImage = null;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("stroke color")]
        public Color StrokeColor
        {
            get => strokeColor;
            set
            {
                strokeColor = value;
                _stateChanged = true;
                _cachedRenderedImage?.Dispose();
                _cachedRenderedImage = null;
                Invalidate();
            }
        }

        private ImageEmbededin _imageEmbededin = ImageEmbededin.Button;
        [Category("Appearance")]
        [Description("Indicates where the image is embedded.")]
        public ImageEmbededin ImageEmbededin
        {
            get => _imageEmbededin;
            set
            {
                if (_imageEmbededin != value)
                {
                    _imageEmbededin = value;
                    _stateChanged = true; // MenuStyle color rules changed -> recache
                    _cachedRenderedImage?.Dispose();
                    _cachedRenderedImage = null;
                    ApplyThemeToSvg();
                    Invalidate();
                }
            }
        }

        private float _velocity = 0.0f;
        [Category("Behavior")]
        [Description("Sets the velocity of the spin in degrees per frame.")]
        public float Velocity
        {
            get => _velocity;
            set
            {
                _velocity = value;
                Invalidate(); // Repaint when the velocity changes
            }
        }

        private float _manualRotationAngle = 0; // Manual rotation angle
        private bool _allowManualRotation = true; // Allows toggling between manual and spinning

        public float ManualRotationAngle
        {
            get => _manualRotationAngle;
            set
            {
                if (Math.Abs(_manualRotationAngle - value) > 0.1f)
                {
                    if (IsSpinning)
                    {
                        _manualRotationAngle = value;
                    }
                    else
                    {
                        _manualRotationAngle = value;
                    }
                    _stateChanged = true;
                    _cachedRenderedImage?.Dispose();
                    _cachedRenderedImage = null;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Allows manual rotation when spinning is disabled.")]
        public bool AllowManualRotation
        {
            get => _allowManualRotation;
            set
            {
                if (_allowManualRotation != value)
                {
                    _allowManualRotation = value;
                    if (!_allowManualRotation)
                        _manualRotationAngle = 0; // Reset manual rotation if disabled
                    _stateChanged = true;
                    _cachedRenderedImage?.Dispose();
                    _cachedRenderedImage = null;
                    Invalidate();
                }
            }
        }

        private Timer _spinTimer;
        private float _rotationAngle;
        private bool _isSpinning = false;

        [Category("Behavior")]
        [Description("Indicates whether the image should spin.")]
        public bool IsSpinning
        {
            get => _isSpinning;
            set
            {
                if (DesignMode)
                    return; // Skip if in design mode
                if (_isSpinning == value)
                    return; // Skip if the state isn't changing

                _isSpinning = value;

                if (_isSpinning)
                {
                    StartSpin(); // Start or ensure spinning continues
                }
                else
                {
                    StopSpin(); // Stop spinning if requested
                }
            }
        }

        [Category("Behavior")]
        [Description("Sets the speed of the spin in degrees per frame.")]
        public float SpinSpeed { get; set; } = 5f;

        [Browsable(true)]
        [Category("Appearance")]
        [Editor("TheTechIdea.Beep.Winform.Controls.Design.Server.Editors.BeepImagePathEditor, TheTechIdea.Beep.Winform.Controls.Design.Server", typeof(System.Drawing.Design.UITypeEditor))]
        [Description("Select the image file (SVG, PNG, JPG, etc.) to load.")]
        public string ImagePath
        {
            get => _imagepath;
            set
            {
                // If the new value is the same as the current, but we don't actually have an image loaded,
                // force a reload attempt. This fixes cases where the first load failed and the same path is set again.
                if (string.Equals(_imagepath, value, StringComparison.OrdinalIgnoreCase))
                {
                    if (!HasImage && !string.IsNullOrWhiteSpace(value))
                    {
                        _stateChanged = true;
                        _cachedRenderedImage?.Dispose();
                        _cachedRenderedImage = null;

                        bool imageLoaded = LoadImage(value);
                        if (imageLoaded)
                        {
                            if (_applyThemeOnImage && _currentTheme != null)
                            {
                                ApplyTheme();
                            }
                            Invalidate();
                            if (Parent != null)
                            {
                                Parent.Invalidate(this.Bounds);
                            }
                            Update();
                        }
                        else
                        {
                            // If reload failed, clear to safe state
                            ClearImage();
                            Invalidate();
                            if (Parent != null)
                            {
                                Parent.Invalidate(this.Bounds);
                            }
                            Update();
                        }
                    }
                    return; // No change needed if image is already correctly loaded
                }

                // Normal path: value actually changed
                _imagepath = value;
                _stateChanged = true;

                // Clear any cached rendered image immediately
                _cachedRenderedImage?.Dispose();
                _cachedRenderedImage = null;

                if (!string.IsNullOrEmpty(_imagepath))
                {
                    bool imageLoaded = LoadImage(_imagepath);
                    if (imageLoaded)
                    {
                        // Apply theme if configured
                        if (_applyThemeOnImage && _currentTheme != null)
                        {
                            ApplyTheme();
                        }

                        // Force immediate invalidation and refresh
                        Invalidate();
                        if (Parent != null)
                        {
                            Parent.Invalidate(this.Bounds);
                        }
                        Update(); // Force immediate repaint
                    }
                }
                else
                {
                    ClearImage();
                    Invalidate();
                    if (Parent != null)
                    {
                        Parent.Invalidate(this.Bounds);
                    }
                    Update(); // Force immediate repaint
                }
            }
        }

        private float _scaleFactor = 1.0f;
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public float ScaleFactor
        {
            get => _scaleFactor;
            set
            {
                _scaleFactor = value;
                _stateChanged = true;
                _cachedRenderedImage?.Dispose();
                _cachedRenderedImage = null;
                Invalidate(); // Repaint when the scale factor changes
            }
        }

        private ImageScaleMode _scaleMode = ImageScaleMode.KeepAspectRatio;
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public ImageScaleMode ScaleMode
        {
            get => _scaleMode;
            set
            {
                _scaleMode = value;
                _stateChanged = true;
                _cachedRenderedImage?.Dispose();
                _cachedRenderedImage = null;
                Invalidate(); // Repaint when the scale mode changes
            }
        }

        bool _applyThemeOnImage = false;
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("Appearance")]
        public bool ApplyThemeOnImage
        {
            get => _applyThemeOnImage;
            set
            {
                _applyThemeOnImage = value;
                _stateChanged = true; // theming affects rendering
                _cachedRenderedImage?.Dispose();
                _cachedRenderedImage = null;
                ApplyTheme();
                Invalidate();
            }
        }

        private bool _isstillimage = false;
        private SvgPaintServer tmpfillcolor;
        private SvgPaintServer tmpstrokecolor;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("Appearance")]
        public bool IsStillImage { get { return _isstillimage; } set { _isstillimage = value; } }

        public bool HasImage
        {
            get
            {
                return (isSvg && svgDocument != null) || regularImage != null;
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("Appearance")]
        public Size GetImageSize()
        {
            if (isSvg && svgDocument != null)
            {
                var dimensions = svgDocument.GetDimensions();
                return new Size((int)dimensions.Width, (int)dimensions.Height);
            }
            else if (regularImage != null)
            {
                return regularImage.Size;
            }
            return Size.Empty;
        }

        /// <summary>
        /// Gets or sets the image displayed in the control.
        /// Supports both regular images and SVG files.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The image displayed on the control.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new Image Image
        {
            get => regularImage;
            set
            {
                if (value == null)
                {
                    ClearImage();  // Clear both SVG and regular images
                    return;
                }

                // If the image has a Tag that indicates it's an SVG path, handle it as an SVG
                if (value.Tag is string path && IsSvgPath(path))
                {
                    LoadSvg(path);
                }
                else
                {
                    regularImage = value;
                    svgDocument = null;  // Clear any loaded SVG
                    _stateChanged = true;
                    _cachedRenderedImage?.Dispose();
                    _cachedRenderedImage = null;
                    Invalidate();  // Trigger repaint
                }
            }
        }

        #region Animation
        [Category("Animation")]
        public bool IsPulsing
        {
            get => _isPulsing;
            set
            {
                _isPulsing = value;
                if (DesignMode)
                    return;
                StartSpin(); // reuse same timer
            }
        }

        [Category("Animation")]
        public bool IsBouncing
        {
            get => _isBouncing;
            set
            {
                _isBouncing = value;
                if (DesignMode)
                    return;
                StartSpin();
            }
        }

        [Category("Animation")]
        public bool IsFading
        {
            get => _isFading;
            set
            {
                _isFading = value;
                _fadeAlpha = 1.0f;
                _fadeDirection = -1;
                if (DesignMode)
                    return;
                StartSpin();
            }
        }

        [Category("Animation")]
        public bool IsShaking
        {
            get => _isShaking;
            set
            {
                _isShaking = value;
                _shakeOffset = 0;
                _shakeDirection = 1;
                if (DesignMode)
                    return;
                StartSpin();
            }
        }

        private bool _isPulsing = false;
        private bool _isBouncing = false;
        private bool _isShaking = false;
        private bool _isFading = false;

        private float _pulseScale = 1.0f;
        private int _pulseDirection = 1;

        private float _fadeAlpha = 1.0f;
        private int _fadeDirection = -1;

        private int _shakeOffset = 0;
        private int _shakeDirection = 1;
        #endregion
        #endregion

        public BeepImage()
        {
            // Enable double buffering and optimized painting
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                       ControlStyles.AllPaintingInWmPaint |
                       ControlStyles.UserPaint |
                       ControlStyles.Selectable |
                       ControlStyles.StandardClick |
                       ControlStyles.ResizeRedraw |
                       ControlStyles.SupportsTransparentBackColor, true);
            UpdateStyles();

            // Set default size for designer
            this.Size = new Size(100, 100);
            this.MinimumSize = new Size(16, 16);

            BoundProperty = "ImagePath";
            fillColor = Color.Black;
            strokeColor = Color.Black;
            this.Visible = true;
            // Enable tab stop for proper focus behavior
            TabStop = true;
        }

        protected override Size DefaultSize
        {
            get { return new Size(100, 100); }
        }
    }
}

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Winform.Controls.Converters;
 
using TheTechIdea.Beep.Winform.Controls.Base.Helpers.Painters;



namespace TheTechIdea.Beep.Winform.Controls.Base
{
    public partial class BaseControl
    {
        #region Painter strategy
        public enum BaseControlPainterKind
        {
            Auto,
            Classic,
            Material,
            Card,
            NeoBrutalist,
            ReadingCard,
            SimpleButton,
            KeyboardShortcut,
            Minimalist,
            Glassmorphism,
            Neumorphism,
            FluentAcrylic // NEW
        }

        private BaseControlPainterKind _painterKind = BaseControlPainterKind.Auto;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Select the painter (renderer) used to draw the control. Auto picks Material when Material ProgressBarStyle is enabled, otherwise Classic.")]
        public BaseControlPainterKind PainterKind
        {
            get => _painterKind;
            set
            {
                if (_painterKind == value) return;
                _painterKind = value;
                UpdatePainterFromKind();
                Invalidate();
            }
        }
        public bool EnableMaterialStyle { get;set; } = false;
        private void UpdatePainterFromKind()
        {
            switch (_painterKind)
            {
                case BaseControlPainterKind.Classic:
                    _painter = new ClassicBaseControlPainter();
                    break;
                case BaseControlPainterKind.Material:
                    _painter = new MaterialBaseControlPainter();
                    break;
                case BaseControlPainterKind.Card:
                    _painter = new CardBaseControlPainter();
                    break;
                case BaseControlPainterKind.NeoBrutalist:
                    _painter = new NeoBrutalistBaseControlPainter();
                    break;
                case BaseControlPainterKind.ReadingCard:
                    _painter = new ReadingCardBaseControlPainter();
                    break;
                case BaseControlPainterKind.Minimalist:
                    _painter = new MinimalistBaseControlPainter();
                    break;
                case BaseControlPainterKind.Glassmorphism:
                    _painter = new GlassmorphismBaseControlPainter();
                    break;
                case BaseControlPainterKind.Neumorphism:
                    _painter = new NeumorphismBaseControlPainter();
                    break;
                case BaseControlPainterKind.FluentAcrylic:
                    _painter = new FluentAcrylicBaseControlPainter();
                    break;
                case BaseControlPainterKind.SimpleButton:
                    _painter = new ButtonBaseControlPainter();
                    break;
                case BaseControlPainterKind.KeyboardShortcut:
                    _painter = new ShortcutCardBaseControlPainter();
                    break;
                case BaseControlPainterKind.Auto:
                default:
                    // Auto defaults to Classic painter
                    _painter = new MinimalistBaseControlPainter();
                    break;
            }
        }
        #endregion

        #region Text Property Override
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The text displayed by the control.")]
        public override string Text
        {
            get => _text;
            set
            {
                if (_isInitializing) return;
                
                if (value != null && value == "BaseControl")
                {
                    value = "";
                }

                _text = value;
                OnTextChanged(EventArgs.Empty);
                Invalidate();
            }
        }
        #endregion

        #region Info Property
        [Category("Appearance")]
        [Localizable(true)]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SimpleItem Info
        {
            get => _info;
            set => _info = value;
        }
        #endregion

        #region Tooltips (Rich)
        private string _toolTipText = string.Empty;

        [Browsable(false)]
        public bool UseRichToolTip { get; set; } = true;
        #endregion
        public DrawingLayer ExternalDrawingLayer { get; set; } =  DrawingLayer.AfterAll;  
        #region Static/Location Control
        [Browsable(true)]
        [Category("Behavior")]
        public bool StaticNotMoving 
        { 
            get => _staticNotMoving; 
            set 
            { 
                _staticNotMoving = value; 
                _originalLocation = Location; 
            } 
        }
        #endregion

        #region DPI Scaling Support
        [Category("DPI/Scaling")]
        [Browsable(false)]
        [Description("Disable all DPI/scaling logic for this control (AutoScale, DPI-based size/font scaling).")]
    private bool _disableDpiAndScaling = false; // default opt-out of custom scaling
        [Category("DPI/Scaling")]
        [Browsable(false)]
        public bool DisableDpiAndScaling
        {
            get => _disableDpiAndScaling;
            set
            {
                _disableDpiAndScaling = value;
                // Do not force AutoScale changes; let the framework/parent form handle scaling
                Invalidate();
            }
        }

        [Category("DPI/Scaling")]
        [Browsable(false)]
        protected float DpiScaleFactor => DisableDpiAndScaling || _dpi == null ? 1.0f : _dpi.DpiScaleFactor;

        protected virtual void UpdateDpiScaling(Graphics g)
        {
            if (DisableDpiAndScaling || _dpi == null) return;
            _dpi.UpdateDpiScaling(g);
        }
        public int ScaleValue(int value) => (DisableDpiAndScaling || _dpi == null) ? value : _dpi.ScaleValue(value);
        public Size ScaleSize(Size size) => (DisableDpiAndScaling || _dpi == null) ? size : _dpi.ScaleSize(size);
        public Font ScaleFont(Font font) => (DisableDpiAndScaling || _dpi == null) ? font : _dpi.ScaleFont(font);
        public Padding ScalePadding(Padding padding) => (DisableDpiAndScaling || _dpi == null) ? padding : _dpi.ScalePadding(padding);
        public Rectangle ScaleRectangle(Rectangle rect) => (DisableDpiAndScaling || _dpi == null) ? rect : _dpi.ScaleRectangle(rect);
        public Point ScalePoint(Point point) => (DisableDpiAndScaling || _dpi == null) ? point : new Point(_dpi.ScaleValue(point.X), _dpi.ScaleValue(point.Y));
        public SizeF ScaleSizeF(SizeF size) => (DisableDpiAndScaling || _dpi == null) ? size : _dpi.ScaleSize(size);
    public PointF ScalePointF(PointF point) => (DisableDpiAndScaling || _dpi == null) ? point : new PointF(_dpi.ScaleValue(point.X), _dpi.ScaleValue(point.Y));

        /// <summary>
        /// Returns a scaled image size based on the current DPI scale.
        /// </summary>
        public Size GetScaledImageSize(Size baseSize) => (DisableDpiAndScaling || _dpi == null) ? baseSize : _dpi.ScaleSize(baseSize);

        /// <summary>
        /// Returns a scaled image size based on the current DPI scale.
        /// </summary>
        public Size GetScaledImageSize(int width, int height) => GetScaledImageSize(new Size(width, height));

        /// <summary>
        /// Sets an ImageList's ImageSize to a DPI-scaled size derived from a base design-time size.
        /// Note: This only updates ImageSize; callers are responsible for providing images at appropriate resolutions if needed.
        /// </summary>
        public void SetImageListScaledSize(System.Windows.Forms.ImageList imageList, Size baseImageSize)
        {
            if (imageList == null) return;
            var scaled = GetScaledImageSize(baseImageSize);
            if (imageList.ImageSize != scaled)
            {
                imageList.ImageSize = scaled;
            }
        }

        /// <summary>
        /// Sets an ImageList's ImageSize to a DPI-scaled size using width/height.
        /// </summary>
        public void SetImageListScaledSize(System.Windows.Forms.ImageList imageList, int width, int height)
        {
            SetImageListScaledSize(imageList, new Size(width, height));
        }

        /// <summary>
        /// Rescales the images currently in the ImageList to the specified target size using high-quality resampling,
        /// preserving image keys when possible. This helps avoid blurring when DPI increases.
        /// </summary>
        public void RescaleImageListImages(System.Windows.Forms.ImageList imageList, Size targetSize)
        {
            if (imageList == null) return;
            if (targetSize.Width <= 0 || targetSize.Height <= 0) return;

            // Capture existing images and keys
            var count = imageList.Images?.Count ?? 0;
            if (count == 0)
            {
                imageList.ImageSize = targetSize;
                return;
            }

            var keys = imageList.Images.Keys;
            var originals = new System.Drawing.Image[count];
            for (int i = 0; i < count; i++)
            {
                try { originals[i] = (System.Drawing.Image)imageList.Images[i]?.Clone(); } catch { originals[i] = imageList.Images[i]; }
            }

            // Adjust list settings first so newly added images follow the target size
            var originalColorDepth = imageList.ColorDepth;
            var originalTransparentColor = imageList.TransparentColor;
            imageList.ImageSize = targetSize;
            imageList.ColorDepth = originalColorDepth; // preserve
            imageList.TransparentColor = originalTransparentColor;

            // Rebuild images at the new size
            imageList.Images.Clear();
            for (int i = 0; i < count; i++)
            {
                var src = originals[i];
                if (src == null)
                {
                    imageList.Images.Add(new System.Drawing.Bitmap(targetSize.Width, targetSize.Height));
                    continue;
                }

                using (var bmp = new System.Drawing.Bitmap(targetSize.Width, targetSize.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                using (var g = System.Drawing.Graphics.FromImage(bmp))
                {
                    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    g.Clear(System.Drawing.Color.Transparent);
                    g.DrawImage(src, new System.Drawing.Rectangle(0, 0, targetSize.Width, targetSize.Height));

                    var key = (keys != null && i < keys.Count) ? keys[i] : null;
                    if (!string.IsNullOrEmpty(key))
                    {
                        imageList.Images.Add(key, (System.Drawing.Image)bmp.Clone());
                    }
                    else
                    {
                        imageList.Images.Add((System.Drawing.Image)bmp.Clone());
                    }
                }
            }
        }

        /// <summary>
        /// Rescales ImageList images based on a base design-time size and current DPI.
        /// </summary>
        public void RescaleImageListImagesForCurrentDpi(System.Windows.Forms.ImageList imageList, Size baseImageSize)
        {
            var target = GetScaledImageSize(baseImageSize);
            RescaleImageListImages(imageList, target);
        }

    [Category("DPI/Scaling")]
    [Browsable(false)]
    public int CurrentDpi => (DisableDpiAndScaling || _dpi == null) ? 96 : _dpi.CurrentDpi;

    /// <summary>
    /// Raised when the DPI scaling factor changes (per-monitor DPI changes, window moved between monitors, etc.).
    /// Override OnDpiChanged in derived controls to react directly.
    /// Hidden from the designer's Events list; intended for runtime use.
    /// </summary>
    [Category("DPI/Scaling")]
    [Browsable(false)]
    public event EventHandler DpiChanged;

        protected virtual void OnDpiChanged()
        {
            DpiChanged?.Invoke(this, EventArgs.Empty);
        }

        // Internal hook used by ControlDpiHelper to raise the public event and virtual method.
        internal void OnDpiChangedInternal()
        {
            OnDpiChanged();
        }
        #endregion

        #region Theme Management
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
                ApplyTheme();
            }
        }
     
        public bool ApplyThemeToChilds { get; set; } = true;
        #endregion

        #region Font Management
        private bool _useThemeFont = true;
        private TypeStyleFontSize _overrideFontSize = TypeStyleFontSize.None;

        [Browsable(true)]
        [Category("Appearance")]
        public bool UseThemeFont
        {
            get => _useThemeFont;
            set
            {
                _useThemeFont = value;
                if (value) SetFont();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        public TypeStyleFontSize OverrideFontSize
        {
            get => _overrideFontSize;
            set
            {
                _overrideFontSize = value;
                SetFont();
                Invalidate();
            }
        }
        #endregion

        #region IBeepUIComponent Basic Properties
        [Browsable(true)]
        [Category("Data")]
        public string ComponentName
        {
            get => Name;
            set
            {
                Name = value;
                PropertyChanged?.Invoke(this, new BeepComponentEventArgs(this, nameof(ComponentName), LinkedProperty, value));
            }
        }

        public IBeepUIComponent Form { get; set; }
        public string GuidID { get => _guidId; set => _guidId = value; }
        public string BlockID { get => _blockId; set => _blockId = value; }
        public string FieldID { get => _fieldId; set => _fieldId = value; }
        public int Id { get => _id; set => _id = value; }
        public List<object> Items { get => _items; set => _items = value ?? new List<object>(); }
        
        public object SelectedValue 
        { 
            get => _selectedValue; 
            set 
            { 
                _selectedValue = value; 
                OnSelected?.Invoke(this, new BeepComponentEventArgs(this, BoundProperty, LinkedProperty, value)); 
            } 
        }

        // Data binding context
        public object DataContext { get; set; }

        public virtual string BoundProperty 
        { 
            get => _boundProperty; 
            set 
            { 
                _boundProperty = value;
                if (DataContext != null)
                {
                    _dataBinding.SetBinding(_boundProperty, DataSourceProperty);
                }
            } 
        }
        
        public string DataSourceProperty 
        { 
            get => _dataSourceProperty; 
            set 
            { 
                _dataSourceProperty = value;
                if (DataContext != null)
                {
                    _dataBinding.SetBinding(BoundProperty, _dataSourceProperty);
                }
            } 
        }
        
        public string LinkedProperty { get => _linkedProperty; set => _linkedProperty = value; }

        public object Oldvalue => _oldValue;
        
        public Color BorderColor { get => _borderColor; set { _borderColor = value; Invalidate(); } }
        public bool IsRequired { get => _isRequired; set => _isRequired = value; }
        public bool IsSelected 
        { 
            get => _isSelected; 
            set 
            { 
                _isSelected = value; 
                Invalidate(); 
                if (value) OnSelected?.Invoke(this, new BeepComponentEventArgs(this, BoundProperty, LinkedProperty, GetValue())); 
            } 
        }
        
        public bool IsDeleted { get => _isDeleted; set => _isDeleted = value; }
        public bool IsNew { get => _isNew; set => _isNew = value; }
        public bool IsDirty { get => _isDirty; set => _isDirty = value; }
        public bool IsReadOnly { get => _isReadOnly; set => _isReadOnly = value; }
        public bool IsEditable { get => _isEditable; set => _isEditable = value; }
        public bool IsVisible { get => _isVisible; set { _isVisible = value; Visible = value; } }
        public bool IsFrameless { get => _isFrameless; set { _isFrameless = value; Invalidate(); } }
        public DbFieldCategory Category { get => _category; set => _category = value; }

        // Validation parity
        [Browsable(false)]
        public bool IsValid { get; set; } = true;
        #endregion

        #region BeepControl Parity: Parent/Child Theming
        protected bool _isChild = true;
        protected Color _parentBackColor = Color.Empty;
        protected Color _tempBackColor = Color.Empty;
        private Control _parentControl;

        [Browsable(true)]
        [Category("Appearance")]
        public Control ParentControl
        {
            get => _parentControl;
            set
            {
                _parentControl = value;
                if (value != null)
                {
                    _parentBackColor = value.BackColor;
                }
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        public Color ParentBackColor
        {
            get => _parentBackColor;
            set
            {
                _parentBackColor = value;
                // Invalidate();  // Trigger repaint
            }
        }
      
        [Browsable(true)]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsChild
        {
            get => _isChild;
            set
            {
                _isChild = value;
                if (this.Parent != null)
                {
                    if (value)
                    {
                        _parentBackColor = this.Parent.BackColor;
                        _tempBackColor = BackColor;
                        BackColor = _parentBackColor;
                        
                    }
                    else
                    {
                        BackColor = _tempBackColor;
                    }
                }

                Invalidate();  // Trigger repaint
            }
        }

        #endregion

        #region Hit Testing Properties
        public List<ControlHitTest> HitList { get { return  _hitTest.HitList; } set {  _hitTest.HitList= value; } }
        public ControlHitTest HitTestControl { get { return _hitTest.HitTestControl; } set { _hitTest.HitTestControl = value; } }
      
        public bool HitAreaEventOn { get { return _hitTest.HitAreaEventOn; } set { _hitTest.HitAreaEventOn = value; } }
       
        #endregion

        #region Direct Appearance Properties (Previously Delegated)
        /// <summary>
        /// Property to enable automatic size compensation for Material Design
        /// </summary>
        [Browsable(false)] // Hidden - use StylePreset instead
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Category("Material Design")]
        [Description("Automatically adjusts control size to accommodate Material Design spacing requirements")]
        [DefaultValue(true)]
        public bool MaterialAutoSizeCompensation
        {
            get => _materialAutoSizeCompensation;
            set
            {
                if (_materialAutoSizeCompensation != value)
                {
                    _materialAutoSizeCompensation = value;

                    // Trigger immediate compensation when enabled and using Material painter
                    if (value && PainterKind == BaseControlPainterKind.Material && !_isInitializing)
                    {
                        ApplyMaterialSizeCompensation();
                    }

                    // Trigger property changed handler
                    OnMaterialPropertyChanged();

                    // Update layout if material helper exists
                    UpdateMaterialLayout();

                    Invalidate();
                }
            }
        }
        // Need to have private value for each property to and rewrite each property to have invalidate() call
        /// <summary>
        /// Alternative sizing mode for Material Design - preserves content area instead of following Material specs
        /// </summary>
        [Browsable(true)] // Hidden - use StylePreset instead
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Category("Material Design")]
        [Description("When enabled, preserves the original content area size instead of following Material Design size specifications")]
        [DefaultValue(false)]
        public bool MaterialPreserveContentArea
        {
            get => _materialPreserveContentArea;
            set
            {
                if (_materialPreserveContentArea == value) return;
                _materialPreserveContentArea = value;
                OnMaterialPropertyChanged();
                UpdateMaterialLayout();
                Invalidate();
            }
        }
        private bool _materialPreserveContentArea = false;

        // Basic appearance
        [Browsable(true)]
        public bool ShowAllBorders
        {
            get => _showAllBorders;
            set { if (_showAllBorders == value) return; _showAllBorders = value; Invalidate(); }
        }
        private bool _showAllBorders = false;

        [Browsable(true)]
        public bool ShowTopBorder
        {
            get => _showTopBorder;
            set { if (_showTopBorder == value) return; _showTopBorder = value; Invalidate(); }
        }
        private bool _showTopBorder = false;

        [Browsable(true)]
        public bool ShowBottomBorder
        {
            get => _showBottomBorder;
            set { if (_showBottomBorder == value) return; _showBottomBorder = value; Invalidate(); }
        }
        private bool _showBottomBorder = false;

        [Browsable(true)]
        public bool ShowLeftBorder
        {
            get => _showLeftBorder;
            set { if (_showLeftBorder == value) return; _showLeftBorder = value; Invalidate(); }
        }
        private bool _showLeftBorder = false;

        [Browsable(true)]
        public bool ShowRightBorder
        {
            get => _showRightBorder;
            set { if (_showRightBorder == value) return; _showRightBorder = value; Invalidate(); }
        }
        private bool _showRightBorder = false;

        [Browsable(true)]
        public int BorderThickness
        {
            get => _borderThickness;
            set
            {
                if (_borderThickness == value) return;
                _borderThickness = Math.Max(0, value);
                UpdateControlRegion();
                Invalidate();
            }
        }
        private int _borderThickness = 1;

        [Browsable(true)]
        public int BorderRadius
        {
            get => _borderRadius;
            set
            {
                if (_borderRadius == value) return;
                _borderRadius = Math.Max(0, value);
                UpdateControlRegion();
                Invalidate();
            }
        }
        private int _borderRadius = 8;

        [Browsable(true)]
        public bool IsRounded
        {
            get => _isRounded;
            set
            {
                if (_isRounded == value) return;
                _isRounded = value;
                UpdateControlRegion();
                Invalidate();
            }
        }
        private bool _isRounded = true;

        [Browsable(true)]
        public DashStyle BorderDashStyle
        {
            get => _borderDashStyle;
            set { if (_borderDashStyle == value) return; _borderDashStyle = value; Invalidate(); }
        }
        private DashStyle _borderDashStyle = DashStyle.Solid;

        [Browsable(true)]
        public Color InactiveBorderColor
        {
            get => _inactiveBorderColor;
            set { if (_inactiveBorderColor == value) return; _inactiveBorderColor = value; Invalidate(); }
        }
        private Color _inactiveBorderColor = Color.Gray;

        // Shadow
        [Browsable(true)]
        public bool ShowShadow
        {
            get => _showShadow;
            set { if (_showShadow == value) return; _showShadow = value; Invalidate(); }
        }
        private bool _showShadow = false;

        [Browsable(true)]
        public Color ShadowColor
        {
            get => _shadowColor;
            set { if (_shadowColor == value) return; _shadowColor = value; Invalidate(); }
        }
        private Color _shadowColor = Color.Black;

        [Browsable(true)]
        public float ShadowOpacity
        {
            get => _shadowOpacity;
            set { if (Math.Abs(_shadowOpacity - value) < float.Epsilon) return; _shadowOpacity = Math.Max(0f, Math.Min(1f, value)); Invalidate(); }
        }
        private float _shadowOpacity = 0.25f;

        [Browsable(true)]
        public int ShadowOffset
        {
            get => _shadowOffset;
            set { if (_shadowOffset == value) return; _shadowOffset = Math.Max(0, value); Invalidate(); }
        }
        private int _shadowOffset = 3;

        // Gradients
        [Browsable(true)]
        public bool UseGradientBackground
        {
            get => _useGradientBackground;
            set { if (_useGradientBackground == value) return; _useGradientBackground = value; Invalidate(); }
        }
        private bool _useGradientBackground = false;

        [Browsable(true)]
        public LinearGradientMode GradientDirection
        {
            get => _gradientDirection;
            set { if (_gradientDirection == value) return; _gradientDirection = value; Invalidate(); }
        }
        private LinearGradientMode _gradientDirection = LinearGradientMode.Horizontal;

        [Browsable(true)]
        public Color GradientStartColor
        {
            get => _gradientStartColor;
            set { if (_gradientStartColor == value) return; _gradientStartColor = value; Invalidate(); }
        }
        private Color _gradientStartColor = Color.LightGray;

        [Browsable(true)]
        public Color GradientEndColor
        {
            get => _gradientEndColor;
            set { if (_gradientEndColor == value) return; _gradientEndColor = value; Invalidate(); }
        }
        private Color _gradientEndColor = Color.Gray;

        // Modern gradients
        [Browsable(true)]
        public ModernGradientType ModernGradientType
        {
            get => _modernGradientType;
            set { if (_modernGradientType == value) return; _modernGradientType = value; Invalidate(); }
        }
        private ModernGradientType _modernGradientType = ModernGradientType.None;

        [Browsable(true)]
        public List<GradientStop> GradientStops
        {
            get => _gradientStops;
            set { _gradientStops = value ?? new List<GradientStop>(); Invalidate(); }
        }
        private List<GradientStop> _gradientStops = new List<GradientStop>();

        [Browsable(true)]
        public PointF RadialCenter
        {
            get => _radialCenter;
            set { if (_radialCenter == value) return; _radialCenter = value; Invalidate(); }
        }
        private PointF _radialCenter = new PointF(0.5f, 0.5f);

        [Browsable(true)]
        public float GradientAngle
        {
            get => _gradientAngle;
            set { if (Math.Abs(_gradientAngle - value) < float.Epsilon) return; _gradientAngle = value; Invalidate(); }
        }
        private float _gradientAngle = 0f;

        [Browsable(true)]
        public bool UseGlassmorphism
        {
            get => _useGlassmorphism;
            set { if (_useGlassmorphism == value) return; _useGlassmorphism = value; Invalidate(); }
        }
        private bool _useGlassmorphism = false;

        [Browsable(true)]
        public float GlassmorphismBlur
        {
            get => _glassmorphismBlur;
            set { if (Math.Abs(_glassmorphismBlur - value) < float.Epsilon) return; _glassmorphismBlur = Math.Max(0f, value); Invalidate(); }
        }
        private float _glassmorphismBlur = 10f;

        [Browsable(true)]
        public float GlassmorphismOpacity
        {
            get => _glassmorphismOpacity;
            set { if (Math.Abs(_glassmorphismOpacity - value) < float.Epsilon) return; _glassmorphismOpacity = Math.Max(0f, Math.Min(1f, value)); Invalidate(); }
        }
        private float _glassmorphismOpacity = 0.1f;

        // Material UI - Hide specific Material Design properties to avoid conflicts with StylePreset
        [Browsable(false)] // Hidden - use StylePreset instead 
        [EditorBrowsable(EditorBrowsableState.Never)]
        public MaterialTextFieldVariant MaterialBorderVariant
        {
            get => _materialBorderVariant;
            set
            {
                if (_materialBorderVariant == value) return;
                _materialBorderVariant = value;
                OnMaterialPropertyChanged();
                UpdateMaterialLayout();
                Invalidate();
            }
        }
        private MaterialTextFieldVariant _materialBorderVariant = MaterialTextFieldVariant.Standard;

        [Browsable(true)]
        public bool FloatingLabel
        {
            get => _floatingLabel;
            set { if (_floatingLabel == value) return; _floatingLabel = value; OnMaterialPropertyChanged(); UpdateMaterialLayout(); Invalidate(); }
        }
        private bool _floatingLabel = true;

        [Browsable(true)]
        public string LabelText
        {
            get => _labelText;
            set { if (string.Equals(_labelText, value, StringComparison.Ordinal)) return; _labelText = value ?? string.Empty; OnMaterialPropertyChanged(); UpdateMaterialLayout(); Invalidate(); }
        }
        private string _labelText = string.Empty;

        [Browsable(true)]
        public string HelperText
        {
            get => _helperText;
            set { if (string.Equals(_helperText, value, StringComparison.Ordinal)) return; _helperText = value ?? string.Empty; OnMaterialPropertyChanged(); UpdateMaterialLayout(); Invalidate(); }
        }
        private string _helperText = string.Empty;

        [Browsable(true)]
        public Color FocusBorderColor
        {
            get => _focusBorderColor;
            set { if (_focusBorderColor == value) return; _focusBorderColor = value; Invalidate(); }
        }
        private Color _focusBorderColor = Color.RoyalBlue;

        [Browsable(true)]
        public Color FilledBackgroundColor
        {
            get => _filledBackgroundColor;
            set { if (_filledBackgroundColor == value) return; _filledBackgroundColor = value; Invalidate(); }
        }
        private Color _filledBackgroundColor = Color.FromArgb(20, 0, 0, 0);
        // Badge
        private string _badgeText = "";
        [Browsable(true)] public virtual string BadgeText
        {
            get => _badgeText;
            set
            {
                if(value.Equals(_badgeText)) return;
                _badgeText = value;
                UpdateRegionForBadge();
                Invalidate();
            }
        }
        [Browsable(true)] public Color BadgeBackColor { get; set; } = Color.Red;
        [Browsable(true)] public Color BadgeForeColor { get; set; } = Color.White;
        [Browsable(true)] public Font BadgeFont { get; set; } = new Font("Arial", 8, FontStyle.Bold);
        [Browsable(true)] public BadgeShape BadgeShape { get; set; } = BadgeShape.Circle;

        // State colors
        [Browsable(true)] public Color HoverBackColor { get; set; } = Color.LightBlue;
        [Browsable(true)] public Color HoverBorderColor { get; set; } = Color.Blue;
        [Browsable(true)] public Color HoverForeColor { get; set; } = Color.Black;
        [Browsable(true)] public Color PressedBackColor { get; set; } = Color.Gray;
        [Browsable(true)] public Color PressedBorderColor { get; set; } = Color.DarkGray;
        [Browsable(true)] public Color PressedForeColor { get; set; } = Color.White;
        [Browsable(true)] public Color FocusBackColor { get; set; } = Color.LightYellow;
        [Browsable(true)] public Color FocusForeColor { get; set; } = Color.Black;
        [Browsable(true)] public Color DisabledBackColor { get; set; } = Color.LightGray;
        [Browsable(true)] public Color DisabledBorderColor { get; set; } = Color.Gray;
        [Browsable(true)] public Color DisabledForeColor { get; set; } = Color.DarkGray;
        [Browsable(true)] public Color SelectedBackColor { get; set; } = Color.LightGreen;
        [Browsable(true)] public Color SelectedBorderColor { get; set; } = Color.Green;
        [Browsable(true)] public Color SelectedForeColor { get; set; } = Color.Black;
        [Browsable(true)] public Color TempBackColor { get; set; }= Color.LightGray;
        // Effects
        [Browsable(true)] public bool ShowFocusIndicator { get => _effects.ShowFocusIndicator; set { _effects.ShowFocusIndicator = value; Invalidate(); } }
        [Browsable(true)] public Color FocusIndicatorColor { get => _effects.FocusIndicatorColor; set { _effects.FocusIndicatorColor = value; Invalidate(); } }
        [Browsable(true)] public bool EnableRippleEffect { get => _effects.EnableRippleEffect; set => _effects.EnableRippleEffect = value; }
        [Browsable(true)] public DisplayAnimationType AnimationType { get => _effects.AnimationType; set { _effects.AnimationType = value; Invalidate(); } }
        [Browsable(true)] public int AnimationDuration { get => _effects.AnimationDuration; set { _effects.AnimationDuration = value; Invalidate(); } }
        [Browsable(true)] public EasingType Easing { get => _effects.Easing; set { _effects.Easing = value; Invalidate(); } }
        [Browsable(true)] public SlideDirection SlideFrom { get => _effects.SlideFrom; set { _effects.SlideFrom = value; Invalidate(); } }

        #region Material Design Properties

        public enum MaterialOutsideBackgroundMode
        {
            ControlBackColor,
            ParentBackColor,
            Transparent
        }

        [Browsable(false)] // Hidden - use StylePreset instead
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Category("Material Design")]
        [Description("Determines how the area outside the material field is painted when Material ProgressBarStyle is enabled.")]
        [DefaultValue(MaterialOutsideBackgroundMode.ParentBackColor)]
        public MaterialOutsideBackgroundMode MaterialOutsideBackground { get; set; } = MaterialOutsideBackgroundMode.ParentBackColor;
        // Additional BeepControl parity properties
        private bool _canBeSelected = true;
        [Browsable(true)]
        public bool CanBeSelected
        {
            get => _canBeSelected   ;
            set
            {
                if (_canBeSelected == value) return;
                _canBeSelected = value;
                if (!value && IsSelected) IsSelected = false;
                Invalidate();
            }
        }
        private bool _canBeHovered = true;
        [Browsable(true)]
        public bool CanBeHovered
        {
            get => _canBeHovered;
            set
            {
                if (_canBeHovered == value) return;
                _canBeHovered = value;
                if (!value && IsHovered) IsHovered = false;
                Invalidate();
            }
        }

        private bool _canBePressed = true;
        [Browsable(true)]
        public bool CanBePressed
        {
            get => _canBePressed;
            set
            {
                if (_canBePressed == value) return;
                _canBePressed = value;
                if (!value && IsPressed) IsPressed = false;
                Invalidate();
            }
        }

        // Note: BaseControl.cs has a private field `_canBeFocused` and CanFocus() uses it.
        // Keep them in sync here.
        [Browsable(true)]
        public bool CanBeFocused
        {
            get => _canBeFocused;
            set
            {
                if (_canBeFocused == value) return;
                _canBeFocused = value;
                try
                {
                    // Allow keyboard focus/tab navigation when true
                    this.SetStyle(ControlStyles.Selectable, value);
                    this.TabStop = value;
                }
                catch { /* design-time safe */ }
                Invalidate();
            }
        }

        private bool _isBorderAffectedByTheme = true;
        [Browsable(true)]
        public bool IsBorderAffectedByTheme
        {
            get => _isBorderAffectedByTheme;
            set
            {
                if (_isBorderAffectedByTheme == value) return;
                _isBorderAffectedByTheme = value;
                Invalidate();
            }
        }

        private bool _isShadowAffectedByTheme = true;
        [Browsable(true)]
        public bool IsShadowAffectedByTheme
        {
            get => _isShadowAffectedByTheme;
            set
            {
                if (_isShadowAffectedByTheme == value) return;
                _isShadowAffectedByTheme = value;
                Invalidate();
            }
        }

        private bool _isRoundedAffectedByTheme = true;
        [Browsable(true)]
        public bool IsRoundedAffectedByTheme
        {
            get => _isRoundedAffectedByTheme;
            set
            {
                if (_isRoundedAffectedByTheme == value) return;
                _isRoundedAffectedByTheme = value;
                UpdateControlRegion();
                Invalidate();
            }
        }

        private BorderStyle _borderStyle = BorderStyle.FixedSingle;
        [Browsable(true)]
        public new BorderStyle BorderStyle
        {
            get => _borderStyle;
            set
            {
                if (_borderStyle == value) return;
                _borderStyle = value;
                Invalidate();
            }
        }

        // Drawing rect and offsets
        [Browsable(false)] 
        public Rectangle DrawingRect
        {
            get { return _drawingRect; }
            set 
            { 
                if (_drawingRect == value) return;
                _drawingRect = value;
                // Do not Invalidate here; drawing pipeline sets this every frame
            }
        }
        private Rectangle _drawingRect;

        private GraphicsPath _innerShape;
        // Drawing rect and offsets
        [Browsable(false)]
        public GraphicsPath InnerShape
        {
          get { return _innerShape; }
            set
            {
                _innerShape = value;
                //Invalidate();
            }
        }

        [Browsable(false)] public int LeftoffsetForDrawingRect { get; set; } = 0;
        [Browsable(false)] public int TopoffsetForDrawingRect { get; set; } = 0;
        [Browsable(false)] public int RightoffsetForDrawingRect { get; set; } = 0;
        [Browsable(false)] public int BottomoffsetForDrawingRect { get; set; } = 0;

        // Image scaling support
        [Browsable(true)] public ImageScaleMode ScaleMode { get; set; } = ImageScaleMode.KeepAspectRatio;

        // Utility properties for compatibility
        [Browsable(true)] public bool GridMode { get; set; } = false;
        [Browsable(true)] public bool IsSelectedOptionOn { get; set; } = false;
        
        // Additional state parity flags
        [Browsable(true)] public bool IsDefault { get; set; } = false;
        [Browsable(true)] public bool IsAcceptButton { get; set; } = false;
        [Browsable(true)] public bool IsCancelButton { get; set; } = false;

        // Custom border parity
        private bool _isCustomBorder = false;
        [Browsable(true)] public bool IsCustomeBorder { get => _isCustomBorder; set { _isCustomBorder = value; Invalidate(); } }

        /// <summary>
        /// Updates painter layout (replaces material helper usage)
        /// </summary>
        private void UpdateMaterialLayout()
        {
            EnsurePainter();
            _painter?.UpdateLayout(this);
        }
        #endregion

        #region Extra Parity Properties
        // Saved identifiers parity
        [Browsable(true)] public string SavedID { get; set; }
        [Browsable(true)] public string SavedGuidID { get; set; }

        // Alias to match BeepControl naming
        [Browsable(true)] public Color HoveredBackcolor { get => HoverBackColor; set { HoverBackColor = value; Invalidate(); } }
        #endregion

        #region Events
        public event EventHandler<BeepComponentEventArgs> PropertyChanged;
        public event EventHandler<BeepComponentEventArgs> PropertyValidate;
        public event EventHandler<BeepComponentEventArgs> OnSelected;
        public event EventHandler<BeepComponentEventArgs> OnValidate;
        public event EventHandler<BeepComponentEventArgs> OnValueChanged;
        public event EventHandler<BeepComponentEventArgs> OnLinkedValueChanged;
        public event EventHandler<BeepComponentEventArgs> SubmitChanges;

        public event EventHandler<ControlHitTestArgs> OnControlHitTest
        {
            add => _hitTest.OnControlHitTest += value;
            remove => _hitTest.OnControlHitTest -= value;
        }

        public event EventHandler<ControlHitTestArgs> HitDetected
        {
            add => _hitTest.HitDetected += value;
            remove => _hitTest.HitDetected -= value;
        }

        // Key events
        public event EventHandler TabKeyPressed
        {
            add => _input.TabKeyPressed += value;
            remove => _input.TabKeyPressed -= value;
        }

        public event EventHandler ShiftTabKeyPressed
        {
            add => _input.ShiftTabKeyPressed += value;
            remove => _input.ShiftTabKeyPressed -= value;
        }

        public event EventHandler EnterKeyPressed
        {
            add => _input.EnterKeyPressed += value;
            remove => _input.EnterKeyPressed -= value;
        }

        public event EventHandler EscapeKeyPressed
        {
            add => _input.EscapeKeyPressed += value;
            remove => _input.EscapeKeyPressed -= value;
        }

        public event EventHandler LeftArrowKeyPressed
        {
            add => _input.LeftArrowKeyPressed += value;
            remove => _input.LeftArrowKeyPressed -= value;
        }

        public event EventHandler RightArrowKeyPressed
        {
            add => _input.RightArrowKeyPressed += value;
            remove => _input.RightArrowKeyPressed -= value;
        }

        public event EventHandler UpArrowKeyPressed
        {
            add => _input.UpArrowKeyPressed += value;
            remove => _input.UpArrowKeyPressed -= value;
        }

        public event EventHandler DownArrowKeyPressed
        {
            add => _input.DownArrowKeyPressed += value;
            remove => _input.DownArrowKeyPressed -= value;
        }

        public event EventHandler PageUpKeyPressed
        {
            add => _input.PageUpKeyPressed += value;
            remove => _input.PageUpKeyPressed -= value;
        }

        public event EventHandler PageDownKeyPressed
        {
            add => _input.PageDownKeyPressed += value;
            remove => _input.PageDownKeyPressed -= value;
        }

        public event EventHandler HomeKeyPressed
        {
            add => _input.HomeKeyPressed += value;
            remove => _input.HomeKeyPressed -= value;
        }

        public event EventHandler EndKeyPressed
        {
            add => _input.EndKeyPressed += value;
            remove => _input.EndKeyPressed -= value;
        }

        public event EventHandler<KeyEventArgs> DialogKeyDetected
        {
            add => _input.DialogKeyDetected += value;
            remove => _input.DialogKeyDetected -= value;
        }
        #endregion
        #endregion

        #region BeepContextMenu
        private ContextMenus.BeepContextMenu _beepContextMenu;

        [Browsable(true)]
        [Category("Beep")]
        [Description("Context menu for this control. All inherited controls can access this property.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ContextMenus.BeepContextMenu BeepContextMenu
        {
            get => _beepContextMenu;
            set
            {
                _beepContextMenu = value;
                Invalidate();
            }
        }
        #endregion

        #region Painting and Layout
        private BeepControlStyle _controlstyle = BeepControlStyle.Material3;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The visual style/painter to use for rendering the sidebar.")]
        [DefaultValue(BeepControlStyle.Material3)]
        public BeepControlStyle ControlStyle
        {
            get => _controlstyle;
            set
            {
                if (_controlstyle != value)
                {
                    _controlstyle = value;

                    Invalidate();
                }
            }
        }
        private bool _useThemeColors = true;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Use theme colors instead of custom accent color.")]
        [DefaultValue(true)]
        public bool UseThemeColors
        {
            get => _useThemeColors;
            set
            {
                _useThemeColors = value;
                Invalidate();
            }
        }
        #endregion
    }
}
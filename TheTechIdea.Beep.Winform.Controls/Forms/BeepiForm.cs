using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Desktop.Common.Util;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Vis.Modules.Managers;
using TheTechIdea.Beep.Winform.Controls.Converters;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Forms.Helpers;
using TheTechIdea.Beep.Winform.Controls.Managers;
using TheTechIdea.Beep.Winform.Controls.BaseImage;


namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepiForm : Form, IBeepModernFormHost
    {
        // === Helper registration infrastructure (restored) ===
        private delegate void PaddingAdjuster(ref Padding padding);
        private readonly List<PaddingAdjuster> _paddingProviders = new();
        private void RegisterPaddingProvider(PaddingAdjuster provider) { if (provider != null) _paddingProviders.Add(provider); }
        private void ComputeExtraNonClientPadding(ref Padding padding) { foreach (var p in _paddingProviders) p(ref padding); }
        private void RegisterOverlayPainter(Action<Graphics> painter) { _overlayRegistry?.Add(painter); }
        // === End helper infrastructure ===

        // UI manager (restored from designer removal)
        public BeepFormUIManager beepuiManager1; // made public for derived designer forms
        // Legacy caption state (for when helper not yet created)
        private bool _legacyShowCaptionBar = true;
        private bool _legacyShowSystemButtons = true;
        private bool _legacyEnableCaptionGradient = true;
        private int _legacyCaptionHeight = 36;

        // Helpers
        private FormStateStore _state;
        private FormRegionHelper _regionHelper;
        private FormLayoutHelper _layoutHelper;
        private FormShadowGlowPainter _shadowGlow;
        private FormOverlayPainterRegistry _overlayRegistry;
        private FormThemeHelper _themeHelper;
        private FormStyleHelper _styleHelper;
        private FormHitTestHelper _hitTestHelper;
        private FormCaptionBarHelper _captionHelper;

        // IBeepModernFormHost implementation
        Form IBeepModernFormHost.AsForm => this;
        IBeepTheme IBeepModernFormHost.CurrentTheme => _currentTheme;
        bool IBeepModernFormHost.IsInDesignMode => DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime;
        void IBeepModernFormHost.UpdateRegion() => _regionHelper?.EnsureRegion(true);

        #region Fields
        protected int _resizeMargin = 8;
        protected int _borderRadius = 0;
        protected int _borderThickness = 1;
        private Color _borderColor = Color.Red;
        private bool _inpopupmode = false;
        private string _title = "BeepiForm";
        private bool _inMoveOrResize = false;
        protected IBeepTheme _currentTheme = BeepThemesManager.GetDefaultTheme();
        private bool _applythemetochilds = true;
        public event EventHandler OnFormClose;
        public event EventHandler OnFormLoad;
        public event EventHandler OnFormShown;
        public event EventHandler<FormClosingEventArgs> PreClose;
        private int _savedBorderRadius = 0;
        private int _savedBorderThickness = 1;
        #endregion

        #region STYLE (merged from Style partial)
        private BeepFormStyle _formStyle = BeepFormStyle.Modern;
        private Color _shadowColor = Color.FromArgb(50, 0, 0, 0);
        private bool _enableGlow = true;
        private Color _glowColor = Color.FromArgb(100, 72, 170, 255);
        private float _glowSpread = 8f;
        private int _shadowDepth = 6;

        [Category("Beep Style")][DefaultValue(BeepFormStyle.Modern)] public BeepFormStyle FormStyle { get => _formStyle; set { if (_formStyle != value) { _formStyle = value; ApplyFormStyle(); Invalidate(); } } }
        [Category("Beep Style")] public Color ShadowColor { get => _shadowColor; set { if (_shadowColor != value) { _shadowColor = value; SyncStyleToHelpers(); Invalidate(); } } }
        [Category("Beep Style"), DefaultValue(6)] public int ShadowDepth { get => _shadowDepth; set { if (_shadowDepth != value) { _shadowDepth = value; SyncStyleToHelpers(); Invalidate(); } } }
        [Category("Beep Style"), DefaultValue(true)] public bool EnableGlow { get => _enableGlow; set { if (_enableGlow != value) { _enableGlow = value; SyncStyleToHelpers(); Invalidate(); } } }
        [Category("Beep Style")] public Color GlowColor { get => _glowColor; set { if (_glowColor != value) { _glowColor = value; SyncStyleToHelpers(); Invalidate(); } } }
        [Category("Beep Style"), DefaultValue(8f)] public float GlowSpread { get => _glowSpread; set { if (Math.Abs(_glowSpread - value) > float.Epsilon) { _glowSpread = Math.Max(0f, value); SyncStyleToHelpers(); Invalidate(); } } }
        
        // Logo properties now delegated to FormCaptionBarHelper
        [Category("Beep Logo")]
        [Description("Path to the logo image file (SVG, PNG, JPG, etc.)")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))
        ]
        public string LogoImagePath
        {
            get => _captionHelper?.LogoImagePath ?? string.Empty;
            set
            {
                if (_captionHelper != null)
                    _captionHelper.LogoImagePath = value;
            }
        }

        [Category("Beep Logo"), DefaultValue(false)]
        [Description("Whether to show the logo on the form caption")]
        public bool ShowLogo
        {
            get => _captionHelper?.ShowLogo ?? false;
            set
            {
                if (_captionHelper != null)
                    _captionHelper.ShowLogo = value;
            }
        }

        // Alias for BeepFormUIManager compatibility
        [Browsable(false)]
        [Description("Alias for ShowLogo - used by BeepFormUIManager")]
        public bool ShowIconInCaption
        {
            get => ShowLogo;
            set => ShowLogo = value;
        }

        [Category("Beep Logo")]
        [Description("Size of the logo icon")]
        public Size LogoSize
        {
            get => _captionHelper?.LogoSize ?? new Size(20, 20);
            set
            {
                if (_captionHelper != null)
                    _captionHelper.LogoSize = value;
            }
        }

        [Category("Beep Logo")]
        [Description("Margin around the logo icon")]
        public Padding LogoMargin
        {
            get => _captionHelper?.LogoMargin ?? new Padding(8, 8, 8, 8);
            set
            {
                if (_captionHelper != null)
                    _captionHelper.LogoMargin = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)] public BeepFormStylePresets StylePresets { get; } = new();
        public void ApplyPreset(string key) { if (StylePresets.TryGet(key, out var m)) { _borderRadius = m.BorderRadius; _borderThickness = m.BorderThickness; _shadowDepth = m.ShadowDepth; _enableGlow = m.EnableGlow; _glowSpread = m.GlowSpread; SyncStyleToHelpers(); Invalidate(); } }
        private void SyncStyleToHelpers() { if (!UseHelperInfrastructure || _shadowGlow == null) return; _shadowGlow.ShadowColor = _shadowColor; _shadowGlow.ShadowDepth = _shadowDepth; _shadowGlow.EnableGlow = _enableGlow; _shadowGlow.GlowColor = _glowColor; _shadowGlow.GlowSpread = _glowSpread; }
        private void ApplyMetrics(BeepFormStyle style) { if (!BeepFormStyleMetricsDefaults.Map.TryGetValue(style, out var m)) return; _borderRadius = m.BorderRadius; _borderThickness = m.BorderThickness; _shadowDepth = m.ShadowDepth; _enableGlow = m.EnableGlow; _glowSpread = m.GlowSpread; SyncStyleToHelpers(); }
        private void ApplyThemeMapping() { if (_currentTheme == null) return; BackColor = _currentTheme.ButtonBackColor; BorderColor = _currentTheme.BorderColor; }
        #endregion

        #region Animations (merged)
        private bool _animateMaximizeRestore = true;
        private bool _animateStyleChange = true;
        public bool AnimateMaximizeRestore { get => _animateMaximizeRestore; set => _animateMaximizeRestore = value; }
        public bool AnimateStyleChange { get => _animateStyleChange; set => _animateStyleChange = value; }
        private async Task AnimateOpacityAsync(double from, double to, int durationMs) { try { double start = from, end = to; int steps = 10; double delta = (end - start) / steps; int delay = Math.Max(8, durationMs / steps); Opacity = Math.Clamp(start, 0, 1); for (int i = 0; i < steps; i++) { await Task.Delay(delay); Opacity = Math.Clamp(Opacity + delta, 0, 1); } Opacity = Math.Clamp(end, 0, 1); } catch { } }
        #endregion

        #region Acrylic / Backdrop / Mica (merged)
        private bool _enableAcrylicForGlass = true; public bool EnableAcrylicForGlass { get => _enableAcrylicForGlass; set { _enableAcrylicForGlass = value; if (IsHandleCreated) ApplyAcrylicEffectIfNeeded(); } }
        private bool _enableMicaBackdrop = false; public bool EnableMicaBackdrop { get => _enableMicaBackdrop; set { _enableMicaBackdrop = value; if (IsHandleCreated) ApplyMicaBackdropIfNeeded(); } }
        private BackdropType _backdrop = BackdropType.None; public BackdropType Backdrop { get => _backdrop; set { _backdrop = value; if (IsHandleCreated) ApplyBackdrop(); } }
        private void ApplyAcrylicEffectIfNeeded() { if (!IsHandleCreated) return; if (_formStyle == BeepFormStyle.Glass && _enableAcrylicForGlass) TryEnableAcrylic(); else TryDisableAcrylic(); }
        private void ApplyMicaBackdropIfNeeded() { if (!IsHandleCreated) return; if (_enableMicaBackdrop) TryEnableMica(); else TryDisableMica(); }
        protected override void OnHandleCreated(EventArgs e) { base.OnHandleCreated(e); ApplyAcrylicEffectIfNeeded(); ApplyMicaBackdropIfNeeded(); ApplyImmersiveDarkMode(); }
        private void ApplyBackdrop() { try { TryDisableMica(); } catch { } try { TryDisableAcrylic(); } catch { } switch (_backdrop) { case BackdropType.Mica: TryEnableMica(); break; case BackdropType.Acrylic: TryEnableAcrylic(); break; case BackdropType.Tabbed: TryEnableSystemBackdrop(3); break; case BackdropType.Transient: TryEnableSystemBackdrop(2); break; case BackdropType.Blur: TryEnableBlurBehind(); break; case BackdropType.None: default: break; } }
        private void TryEnableSystemBackdrop(int type) { try { var attr = (DWMWINDOWATTRIBUTE)38; DwmSetWindowAttribute(this.Handle, attr, ref type, Marshal.SizeOf<int>()); } catch { } }
        private void TryEnableBlurBehind() { try { var accent = new ACCENT_POLICY { AccentState = ACCENT_STATE.ACCENT_ENABLE_BLURBEHIND }; int size = Marshal.SizeOf(accent); IntPtr p = Marshal.AllocHGlobal(size); Marshal.StructureToPtr(accent, p, false); var data = new WINDOWCOMPOSITIONATTRIBDATA { Attribute = WINDOWCOMPOSITIONATTRIB.WCA_ACCENT_POLICY, Data = p, SizeOfData = size }; SetWindowCompositionAttribute(this.Handle, ref data); Marshal.FreeHGlobal(p); } catch { } }
        private void TryEnableAcrylic() { try { var accent = new ACCENT_POLICY { AccentState = ACCENT_STATE.ACCENT_ENABLE_ACRYLICBLURBEHIND, AccentFlags = 2, GradientColor = (uint)((0xCC << 24) | (BackColor.B << 16) | (BackColor.G << 8) | BackColor.R) }; int size = Marshal.SizeOf(accent); IntPtr p = Marshal.AllocHGlobal(size); Marshal.StructureToPtr(accent, p, false); var data = new WINDOWCOMPOSITIONATTRIBDATA { Attribute = WINDOWCOMPOSITIONATTRIB.WCA_ACCENT_POLICY, Data = p, SizeOfData = size }; SetWindowCompositionAttribute(this.Handle, ref data); Marshal.FreeHGlobal(p); } catch { } }
        private void TryDisableAcrylic() { try { var accent = new ACCENT_POLICY { AccentState = ACCENT_STATE.ACCENT_DISABLED }; int size = Marshal.SizeOf(accent); IntPtr p = Marshal.AllocHGlobal(size); Marshal.StructureToPtr(accent, p, false); var data = new WINDOWCOMPOSITIONATTRIBDATA { Attribute = WINDOWCOMPOSITIONATTRIB.WCA_ACCENT_POLICY, Data = p, SizeOfData = size }; SetWindowCompositionAttribute(this.Handle, ref data); Marshal.FreeHGlobal(p); } catch { } }
        private void TryEnableMica() { try { int trueVal = 1; DwmSetWindowAttribute(this.Handle, DWMWINDOWATTRIBUTE.DWMWA_MICA_EFFECT, ref trueVal, Marshal.SizeOf<int>()); } catch { } }
        private void TryDisableMica() { try { int falseVal = 0; DwmSetWindowAttribute(this.Handle, DWMWINDOWATTRIBUTE.DWMWA_MICA_EFFECT, ref falseVal, Marshal.SizeOf<int>()); } catch { } }
        [DllImport("user32.dll")] private static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WINDOWCOMPOSITIONATTRIBDATA data);
        private enum WINDOWCOMPOSITIONATTRIB { WCA_ACCENT_POLICY = 19 }
        private enum ACCENT_STATE { ACCENT_DISABLED = 0, ACCENT_ENABLE_GRADIENT = 1, ACCENT_ENABLE_TRANSPARENTGRADIENT = 2, ACCENT_ENABLE_BLURBEHIND = 3, ACCENT_ENABLE_ACRYLICBLURBEHIND = 4, ACCENT_ENABLE_HOSTBACKDROP = 5 }
        [StructLayout(LayoutKind.Sequential)] private struct ACCENT_POLICY { public ACCENT_STATE AccentState; public int AccentFlags; public uint GradientColor; public int AnimationId; }
        [StructLayout(LayoutKind.Sequential)] private struct WINDOWCOMPOSITIONATTRIBDATA { public WINDOWCOMPOSITIONATTRIB Attribute; public IntPtr Data; public int SizeOfData; }
        private enum DWMWINDOWATTRIBUTE { DWMWA_USE_IMMERSIVE_DARK_MODE = 20, DWMWA_MICA_EFFECT = 1029 }
        [DllImport("dwmapi.dll")] private static extern int DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE attr, ref int attrValue, int attrSize);
        #endregion

        #region Ribbon (merged)
        private bool _showRibbonPlaceholder = false; private int _ribbonHeight = 80; private bool _showQuickAccess = true; private BeepRibbonControl? _ribbon;
        [Category("Beep Ribbon"), DefaultValue(false)] public bool ShowRibbonPlaceholder { get => _showRibbonPlaceholder; set { _showRibbonPlaceholder = value; if (value) EnsureRibbon(); Invalidate(); } }
        [Browsable(false)] public BeepRibbonControl? Ribbon => _ribbon;
        [Category("Beep Ribbon"), DefaultValue(80)] public int RibbonHeight { get => _ribbonHeight; set { _ribbonHeight = Math.Max(40, value); if (_ribbon != null) _ribbon.Height = _ribbonHeight; Invalidate(); } }
        [Category("Beep Ribbon"), DefaultValue(true)] public bool ShowQuickAccess { get => _showQuickAccess; set { _showQuickAccess = value; if (_ribbon != null) _ribbon.QuickAccess.Visible = value; Invalidate(); } }
        private void EnsureRibbon() { if (_ribbon != null) return; _ribbon = new BeepRibbonControl { Left = 0, Top = (_captionHelper?.CaptionHeight ?? 36), Width = Width, Height = _ribbonHeight, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right }; _ribbon.QuickAccess.Visible = _showQuickAccess; _ribbon.ApplyThemeFromBeep(_currentTheme); Controls.Add(_ribbon); _ribbon.BringToFront(); }
        #endregion

        #region Snap Hints (merged)
        private bool _showSnapHints = true; public bool ShowSnapHints { get => _showSnapHints; set { _showSnapHints = value; Invalidate(); } }
        private Rectangle _snapLeft, _snapRight, _snapTop; private bool _showSnapOverlay;
        private void SnapHints_OnMouseMove(MouseEventArgs e) { if (!_showSnapHints) return; if (WindowState == FormWindowState.Normal && (MouseButtons & MouseButtons.Left) == MouseButtons.Left) { var screen = Screen.FromPoint(Cursor.Position).WorkingArea; int thickness = 8; _snapLeft = new Rectangle(screen.Left, screen.Top, thickness, screen.Height); _snapRight = new Rectangle(screen.Right - thickness, screen.Top, thickness, screen.Height); _snapTop = new Rectangle(screen.Left, screen.Top, screen.Width, thickness); _showSnapOverlay = true; Invalidate(); } else if (_showSnapOverlay) { _showSnapOverlay = false; Invalidate(); } }
        private void SnapHints_OnMouseLeave() { if (_showSnapOverlay) { _showSnapOverlay = false; Invalidate(); } }
        private void SnapHints_OnPaintOverlay(Graphics g) { if (!_showSnapOverlay) return; using var br = new SolidBrush(Color.FromArgb(40, 0, 120, 215)); g.FillRectangle(br, _snapLeft); g.FillRectangle(br, _snapRight); g.FillRectangle(br, _snapTop); }
        #endregion

        #region DPI handling mode
        public enum DpiHandlingMode { Framework, Manual }
        [Browsable(true)][Category("DPI/Scaling")][Description("How DPI awareness and scaling are handled. Framework = let WinForms manage DPI. Manual = use BeepiForm's custom handling.")] public DpiHandlingMode DpiMode { get; set; } = DpiHandlingMode.Framework;
        #endregion

        #region Caption Properties (delegate to helper while maintaining legacy fallback)
        // Caption properties (restored API expected by derived popup/dialog forms)
        [Category("Beep Caption")]
        [DefaultValue(true)]
        public bool ShowCaptionBar
        {
            get => _captionHelper?.ShowCaptionBar ?? _legacyShowCaptionBar;
            set { if (_captionHelper != null) _captionHelper.ShowCaptionBar = value; _legacyShowCaptionBar = value; Invalidate(); }
        }

        [Category("Beep Caption")]
        [DefaultValue(36)]
        public int CaptionHeight
        {
            get => _captionHelper?.CaptionHeight ?? _legacyCaptionHeight;
            set { if (value < 24) value = 24; if (_captionHelper != null) _captionHelper.CaptionHeight = value; _legacyCaptionHeight = value; Invalidate(); }
        }

        [Category("Beep Caption")]
        [DefaultValue(true)]
        public bool ShowSystemButtons
        {
            get => _captionHelper?.ShowSystemButtons ?? _legacyShowSystemButtons;
            set { if (_captionHelper != null) _captionHelper.ShowSystemButtons = value; _legacyShowSystemButtons = value; Invalidate(); }
        }

        [Category("Beep Caption")]
        [DefaultValue(true)]
        public bool EnableCaptionGradient
        {
            get => _captionHelper?.EnableCaptionGradient ?? _legacyEnableCaptionGradient;
            set { if (_captionHelper != null) _captionHelper.EnableCaptionGradient = value; _legacyEnableCaptionGradient = value; Invalidate(); }
        }

        [Category("Beep Caption")]
        [Description("Padding around the caption content")]
        public Padding CaptionPadding
        {
            get => _captionHelper?.LogoMargin ?? new Padding(8, 8, 8, 8);
            set 
            { 
                if (_captionHelper != null) 
                    _captionHelper.LogoMargin = value; 
                Invalidate(); 
            }
        }

        // Windows 11 Dark Mode support
        [Category("Beep Caption")]
        [DefaultValue(false)]
        [Description("Enable Windows 11 immersive dark mode for the window")]
        public bool UseImmersiveDarkMode
        {
            get => _useImmersiveDarkMode;
            set 
            { 
                if (_useImmersiveDarkMode != value)
                {
                    _useImmersiveDarkMode = value;
                    if (IsHandleCreated)
                        ApplyImmersiveDarkMode();
                }
            }
        }
        private bool _useImmersiveDarkMode = false;

        private void ApplyImmersiveDarkMode()
        {
            if (!IsHandleCreated) return;
            try
            {
                int value = _useImmersiveDarkMode ? 1 : 0;
                DwmSetWindowAttribute(Handle, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, ref value, Marshal.SizeOf<int>());
            }
            catch
            {
                // Handle silently - not all Windows versions support this
            }
        }

        // Initialize helper with legacy state
        private void InitializeCaptionHelperWithLegacyState()
        {
            if (_captionHelper != null)
            {
                _captionHelper.ShowCaptionBar = _legacyShowCaptionBar;
                _captionHelper.ShowSystemButtons = _legacyShowSystemButtons;
                _captionHelper.EnableCaptionGradient = _legacyEnableCaptionGradient;
                _captionHelper.CaptionHeight = _legacyCaptionHeight;
            }
        }
        #endregion

        #region Properties
        [Browsable(true)][Category("Appearance")] public string Title { get => _title; set => _title = value; }
        [Browsable(true)][Category("Appearance")] public bool ApplyThemeToChilds { get => _applythemetochilds; set => _applythemetochilds = value; }
        [Browsable(true)][Category("Appearance"), Description("The Thickness of the form's border."), DefaultValue(3), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] public int BorderThickness { get => _borderThickness; set { _borderThickness = value; if (UseHelperInfrastructure) _state.RegionDirty = true; Invalidate(); } }
        [Browsable(true)][Category("Appearance"), Description("The radius of the form's border."), DefaultValue(5), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] public int BorderRadius { get => _borderRadius; set { _borderRadius = Math.Max(0, value); if (UseHelperInfrastructure) { _state.RegionDirty = true; _regionHelper?.InvalidateRegion(); } if (IsHandleCreated && ClientSize.Width > 0 && ClientSize.Height > 0) UpdateFormRegion(); Invalidate(); } }
        [Browsable(true)][Category("Appearance")] public bool InPopMode { get => _inpopupmode; set { _inpopupmode = value; Invalidate(); } }
        private string _theme = string.Empty;
        [Browsable(true)][TypeConverter(typeof(ThemeConverter))][DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] public string Theme { get => _theme; set { if (value != _theme) { _theme = value; _currentTheme = BeepThemesManager.GetTheme(value); ApplyTheme(); } } }
        [Browsable(true)][Category("Appearance"), Description("Sets the color of the form's border.")] public Color BorderColor { get => _borderColor; set { _borderColor = value; Invalidate(); } }
        public bool UseHelperInfrastructure { get; set; } = true;
        #endregion

        #region Ctor
        public BeepiForm()
        {
            // Manual InitializeComponent replacement (designer removed)
            var container = new System.ComponentModel.Container();
            beepuiManager1 = new BeepFormUIManager(container)
            {
                Theme = "DefaultTheme",
                Title = "Beep Form",
                BeepiForm = this,
                IsRounded = true,
                ShowBorder = true,
                ShowShadow = false
            };
            AutoScaleMode = AutoScaleMode.Dpi;
            SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.SupportsTransparentBackColor, true);
            UpdateStyles();
            DoubleBuffered = true;
            BackColor = SystemColors.Control;
            FormBorderStyle = FormBorderStyle.None;
            if (UseHelperInfrastructure)
            {
                _state = new FormStateStore();
                _shadowGlow = new FormShadowGlowPainter();
                _overlayRegistry = new FormOverlayPainterRegistry();
                _regionHelper = new FormRegionHelper(this);
                _layoutHelper = new FormLayoutHelper(this);
                _themeHelper = new FormThemeHelper(this);
                _styleHelper = new FormStyleHelper(this, _shadowGlow);
                _captionHelper = new FormCaptionBarHelper(this, _overlayRegistry, padAdj => RegisterPaddingProvider((ref Padding p) => padAdj(ref p)));
                InitializeCaptionHelperWithLegacyState(); // Apply legacy defaults to helper
                _hitTestHelper = new FormHitTestHelper(this,
                    captionEnabled: () => _captionHelper?.ShowCaptionBar == true,
                    captionHeight: () => _captionHelper?.CaptionHeight ?? 0,
                    isOverSystemButton: () => _captionHelper?.IsCursorOverSystemButton == true,
                    resizeMargin: _resizeMargin);
                RegisterMouseMoveHandler(e => _captionHelper.OnMouseMove(e));
                RegisterMouseLeaveHandler(() => _captionHelper.OnMouseLeave());
                RegisterMouseDownHandler(e => _captionHelper.OnMouseDown(e));
                // Ribbon & Snap overlay registrations
                RegisterPaddingProvider((ref Padding p) => { if (_showRibbonPlaceholder) p.Top += _ribbonHeight; });
                RegisterOverlayPainter(RibbonOverlay);
                RegisterMouseMoveHandler(SnapHints_OnMouseMove);
                RegisterMouseLeaveHandler(SnapHints_OnMouseLeave);
                RegisterOverlayPainter(SnapHints_OnPaintOverlay);
            }
        }
        // Ribbon overlay painter method
        private void RibbonOverlay(Graphics g) { if (!_showRibbonPlaceholder) return; EnsureRibbon(); }
        #endregion

        #region Lifecycle
        private void BeepiForm_Load(object? sender, EventArgs e) { if (BackColor == Color.Transparent || BackColor == Color.Empty) { BackColor = SystemColors.Control; } ApplyTheme(); Invalidate(); Update(); OnFormLoad?.Invoke(this, EventArgs.Empty); }
        private void BeepiForm_VisibleChanged(object? sender, EventArgs e) { if (Visible) { if (BackColor == Color.Transparent || BackColor == Color.Empty) { BackColor = _currentTheme?.BackColor ?? SystemColors.Control; } Invalidate(); Update(); } }
        protected override void OnShown(EventArgs e) { base.OnShown(e); try { beepuiManager1.Initialize(this); } catch { } OnFormShown?.Invoke(this, EventArgs.Empty); }
        protected override void OnLoad(EventArgs e) { base.OnLoad(e); }
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) { base.SetBoundsCore(x, y, width, height, specified); if ((specified & BoundsSpecified.Size) != 0) { PerformLayout(); } }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (UseHelperInfrastructure && _regionHelper != null)
            {
                _regionHelper.EnsureRegion();
            }
            else if (!UseHelperInfrastructure)
            {
                UpdateFormRegion();
            }
            Invalidate();
        }
        #endregion

        #region Apply Theme / Style
        public virtual void ApplyTheme() 
        { 
            SuspendLayout(); 
            try 
            { 
                // Update BeepFormUIManager theme
                try 
                { 
                    if (beepuiManager1.Theme != Theme) 
                        beepuiManager1.Theme = Theme; 
                } 
                catch { }
                
                // Apply background color from theme
                Color newBackColor = _currentTheme?.BackColor ?? SystemColors.Control; 
                if (newBackColor == Color.Transparent || newBackColor == Color.Empty) 
                { 
                    newBackColor = SystemColors.Control; 
                } 
                BackColor = newBackColor; 
                
                // Apply border color from theme
                BorderColor = _currentTheme?.BorderColor ?? SystemColors.ControlDark; 
                
                // Update caption helper theme
                if (_captionHelper != null)
                {
                    _captionHelper.UpdateTheme();
                }
                
                // Apply form style (this will sync to all helpers)
                ApplyFormStyle(); 
                
                // Update ribbon theme
                try 
                { 
                    _ribbon?.ApplyThemeFromBeep(_currentTheme); 
                } 
                catch { } 
            } 
            finally 
            { 
                ResumeLayout(true); 
                Invalidate(); 
                Update(); 
            } 
        }
        protected void ApplyFormStyle()
        {
            // Apply metrics first (border radius, thickness, shadow depth, etc.)
            ApplyMetrics(_formStyle);
            
            // Apply style-specific customizations
            switch (_formStyle)
            {
                case BeepFormStyle.Modern:
                    ApplyThemeMapping();
                    _glowColor = Color.FromArgb(100, 72, 170, 255);
                    break;
                    
                case BeepFormStyle.Metro:
                    ApplyThemeMapping();
                    _glowColor = Color.FromArgb(80, 0, 100, 200);
                    break;
                    
                case BeepFormStyle.Glass:
                    ApplyThemeMapping();
                    _glowColor = Color.FromArgb(120, 255, 255, 255);
                    break;
                    
                case BeepFormStyle.Office:
                    ApplyThemeMapping();
                    _glowColor = Color.FromArgb(90, 50, 100, 200);
                    break;
                    
                case BeepFormStyle.ModernDark:
                    // Override theme mapping for dark style
                    _borderRadius = 8;
                    _borderThickness = 1;
                    _shadowDepth = 8;
                    _enableGlow = true;
                    _glowColor = Color.FromArgb(80, 0, 0, 0);
                    _glowSpread = 10f;
                    BackColor = Color.FromArgb(32, 32, 32);
                    BorderColor = Color.FromArgb(64, 64, 64);
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
                    _shadowDepth = 0;
                    _enableGlow = false;
                    _glowSpread = 0f;
                    break;
                    
                case BeepFormStyle.Classic:
                    // Use system colors for classic style
                    BackColor = SystemColors.Control;
                    BorderColor = SystemColors.ActiveBorder;
                    _shadowDepth = 0;
                    _enableGlow = false;
                    _glowSpread = 0f;
                    break;
                    
                case BeepFormStyle.Custom:
                    ApplyThemeMapping();
                    // Custom style uses metrics from BeepFormStyleMetricsDefaults
                    break;
            }
            
            // Sync all changes to helpers
            SyncStyleToHelpers();
            
            // Update logo painter theme if caption helper exists
            UpdateLogoPainterTheme();
            
            // Apply visual effects
            ApplyAcrylicEffectIfNeeded();
            
            // Trigger layout updates
            if (UseHelperInfrastructure && _regionHelper != null)
            {
                _regionHelper.InvalidateRegion();
            }
            else
            {
                UpdateFormRegion();
            }
            
            // Animate style change if enabled
            if (IsHandleCreated && _animateStyleChange)
            {
                _ = AnimateOpacityAsync(0.8, 1.0, 200);
            }
            
            // Force repaint
            Invalidate();
        }

        private void UpdateLogoPainterTheme()
        {
            // Update logo painter theme when style changes
            if (_captionHelper != null && !string.IsNullOrEmpty(_captionHelper.LogoImagePath))
            {
                try
                {
                    // Re-initialize logo painter with new theme
                    _captionHelper.LogoImagePath = _captionHelper.LogoImagePath; // Triggers re-initialization
                }
                catch (Exception)
                {
                    // Handle silently
                }
            }
        }
        #endregion

        #region Maximize helpers
        public void ToggleMaximize() { WindowState = WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized; }
        private void ApplyMaximizedWindowFix() { if (WindowState == FormWindowState.Maximized) { _savedBorderRadius = _borderRadius; _savedBorderThickness = _borderThickness; _borderRadius = 0; _borderThickness = 0; Padding = new Padding(0); Region = null; } else { _borderRadius = _savedBorderRadius; _borderThickness = _savedBorderThickness; Padding = new Padding(Math.Max(0, _borderThickness)); } }
        #endregion

        #region Shapes/Regions
        private GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius) { GraphicsPath path = new GraphicsPath(); if (radius <= 0 || rect.Width <= 0 || rect.Height <= 0) { if (rect.Width > 0 && rect.Height > 0) path.AddRectangle(rect); return path; } int diameter = Math.Min(rect.Width, rect.Height); diameter = Math.Min(diameter, radius * 2); if (diameter <= 0) { path.AddRectangle(rect); return path; } try { Rectangle arcRect = new Rectangle(rect.X, rect.Y, diameter, diameter); path.AddArc(arcRect, 180, 90); arcRect.X = rect.Right - diameter; path.AddArc(arcRect, 270, 90); arcRect.Y = rect.Bottom - diameter; path.AddArc(arcRect, 0, 90); arcRect.X = rect.Left; path.AddArc(arcRect, 90, 90); path.CloseFigure(); } catch (ArgumentException) { path.Reset(); if (rect.Width > 0 && rect.Height > 0) path.AddRectangle(rect); } return path; }
        private void UpdateFormRegion() { if (!IsHandleCreated) return; if (WindowState == FormWindowState.Maximized || _borderRadius <= 0) { Region = null; return; } if (ClientSize.Width <= 0 || ClientSize.Height <= 0) { Region = null; return; } using var path = GetRoundedRectanglePath(new Rectangle(0, 0, ClientSize.Width, ClientSize.Height), _borderRadius); Region = new Region(path); }
        #endregion

        #region Layout helpers
        public virtual void AdjustControls() { Rectangle adjustedClientArea = GetAdjustedClientRectangle(); foreach (Control control in Controls) { if (control.Dock == DockStyle.Fill) { control.Bounds = adjustedClientArea; } else if (control.Dock == DockStyle.Top) { control.Bounds = new Rectangle(adjustedClientArea.Left, adjustedClientArea.Top, adjustedClientArea.Width, control.Height); adjustedClientArea.Y += control.Height; adjustedClientArea.Height -= control.Height; } else if (control.Dock == DockStyle.Bottom) { control.Bounds = new Rectangle(adjustedClientArea.Left, adjustedClientArea.Bottom - control.Height, adjustedClientArea.Width, control.Height); adjustedClientArea.Height -= control.Height; } else if (control.Dock == DockStyle.Left) { control.Bounds = new Rectangle(adjustedClientArea.Left, adjustedClientArea.Top, control.Width, adjustedClientArea.Height); adjustedClientArea.X += control.Width; adjustedClientArea.Width -= control.Width; } else if (control.Dock == DockStyle.Right) { control.Bounds = new Rectangle(adjustedClientArea.Right - control.Width, adjustedClientArea.Top, control.Width, adjustedClientArea.Height); adjustedClientArea.Width -= control.Width; } else { control.Left = Math.Max(control.Left, adjustedClientArea.Left); control.Top = Math.Max(control.Top, adjustedClientArea.Top); int maxWidth = adjustedClientArea.Right - control.Left; int maxHeight = adjustedClientArea.Bottom - control.Top; control.Width = Math.Min(control.Width, maxWidth); control.Height = Math.Min(control.Height, maxHeight); } } }
        public Rectangle GetAdjustedClientRectangle() { var extra = new Padding(0); ComputeExtraNonClientPadding(ref extra); int effectiveBorder = (Padding.Left >= _borderThickness && Padding.Right >= _borderThickness && Padding.Top >= _borderThickness && Padding.Bottom >= _borderThickness) ? 0 : _borderThickness; int adjustedWidth = Math.Max(0, ClientSize.Width - (2 * effectiveBorder) - extra.Left - extra.Right); int adjustedHeight = Math.Max(0, ClientSize.Height - (2 * effectiveBorder) - extra.Top - extra.Bottom); return new Rectangle(extra.Left + effectiveBorder, extra.Top + effectiveBorder, adjustedWidth, adjustedHeight); }
        protected new Rectangle DisplayRectangle { get { var extra = new Padding(0); ComputeExtraNonClientPadding(ref extra); int effectiveBorder = (Padding.Left >= _borderThickness && Padding.Right >= _borderThickness && Padding.Top >= _borderThickness && Padding.Bottom >= _borderThickness) ? 0 : _borderThickness; int adjustedWidth = Math.Max(0, ClientSize.Width - (effectiveBorder * 2) - extra.Left - extra.Right); int adjustedHeight = Math.Max(0, ClientSize.Height - (effectiveBorder * 2) - extra.Top - extra.Bottom); return new Rectangle(effectiveBorder + extra.Left, effectiveBorder + extra.Top, adjustedWidth, adjustedHeight); } }
        #endregion

        #region Paint Override
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            // Simple fallback painting during active resize/move
            if (_inMoveOrResize)
            {
                g.Clear(BackColor);
                return;
            }

            if (UseHelperInfrastructure && _shadowGlow != null && _regionHelper != null && _overlayRegistry != null)
            {
                // Use helper-based rendering
                var formPath = GetFormPath();
                using (formPath)
                {
                    // Paint shadow and glow effects behind the form
                    if (WindowState != FormWindowState.Maximized)
                    {
                        _shadowGlow.PaintShadow(g, formPath);
                        _shadowGlow.PaintGlow(g, formPath);
                    }

                    // Fill the form background
                    using var backBrush = new SolidBrush(BackColor);
                    g.FillPath(backBrush, formPath);

                    // Draw border if specified
                    if (_borderThickness > 0)
                    {
                        using var borderPen = new Pen(BorderColor, _borderThickness);
                        g.DrawPath(borderPen, formPath);
                    }
                }

                // Paint overlays (caption with logo, ribbons, etc.)
                _overlayRegistry.PaintOverlays(g);
            }
            else
            {
                // Fallback to direct painting when helpers are disabled
                PaintDirectly(g);
            }

            base.OnPaint(e);
        }

        private void PaintDirectly(Graphics g)
        {
            // Direct painting fallback
            var formPath = GetFormPath();
            using (formPath)
            {
                if (WindowState != FormWindowState.Maximized)
                {
                    // Shadow
                    if (_shadowDepth > 0)
                    {
                        using var shadowPath = (GraphicsPath)formPath.Clone();
                        using var shadowBrush = new SolidBrush(_shadowColor);
                        g.TranslateTransform(_shadowDepth, _shadowDepth);
                        g.FillPath(shadowBrush, shadowPath);
                        g.TranslateTransform(-_shadowDepth, -_shadowDepth);
                    }

                    // Glow
                    if (_enableGlow && _glowSpread > 0f)
                    {
                        using var glowPen = new Pen(_glowColor, _glowSpread) { LineJoin = LineJoin.Round };
                        g.DrawPath(glowPen, formPath);
                    }
                }

                // Background
                using var backBrush = new SolidBrush(BackColor);
                g.FillPath(backBrush, formPath);

                // Border
                if (_borderThickness > 0)
                {
                    using var borderPen = new Pen(BorderColor, _borderThickness);
                    g.DrawPath(borderPen, formPath);
                }
            }
        }

        private GraphicsPath GetFormPath()
        {
            var path = new GraphicsPath();
            int currentWidth = ClientSize.Width;
            int currentHeight = ClientSize.Height;
            
            if (currentWidth <= 0 || currentHeight <= 0)
            {
                return path;
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

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (UseHelperInfrastructure && _regionHelper != null)
            {
                _regionHelper.InvalidateRegion();
            }
            Invalidate();
        }
        #endregion

        #region Redraw helpers / Mouse Aggregators
        public void BeginUpdate() => User32.SendMessage(Handle, User32.WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero); public void EndUpdate() { User32.SendMessage(this.Handle, User32.WM_SETREDRAW, new IntPtr(1), IntPtr.Zero); this.Refresh(); }
        private readonly List<Action<MouseEventArgs>> _mouseMoveHandlers = new(); private readonly List<Action> _mouseLeaveHandlers = new(); private readonly List<Action<MouseEventArgs>> _mouseDownHandlers = new();
        protected void RegisterMouseMoveHandler(Action<MouseEventArgs> handler) { if (handler != null) _mouseMoveHandlers.Add(handler); }
        protected void RegisterMouseLeaveHandler(Action handler) { if (handler != null) _mouseLeaveHandlers.Add(handler); }
        protected void RegisterMouseDownHandler(Action<MouseEventArgs> handler) { if (handler != null) _mouseDownHandlers.Add(handler); }
        protected override void OnMouseMove(MouseEventArgs e) { if (_inpopupmode) return; base.OnMouseMove(e); foreach (var h in _mouseMoveHandlers) h(e); }
        protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); Cursor = Cursors.Default; foreach (var h in _mouseLeaveHandlers) h(); }
        protected override void OnMouseDown(MouseEventArgs e) { base.OnMouseDown(e); foreach (var h in _mouseDownHandlers) h(e); }
        #endregion

        #region HitTest / WndProc
        private const int WM_NCHITTEST = 0x84; private const int WM_ENTERSIZEMOVE = 0x0231; private const int WM_EXITSIZEMOVE = 0x0232; private const int WM_GETMINMAXINFO = 0x0024; private const int HTCLIENT = 1; private const int HTCAPTION = 2; private const int HTLEFT = 10; private const int HTRIGHT = 11; private const int HTTOP = 12; private const int HTTOPLEFT = 13; private const int HTTOPRIGHT = 14; private const int HTBOTTOM = 15; private const int HTBOTTOMLEFT = 16; private const int HTBOTTOMRIGHT = 17; private const int WM_DPICHANGED = 0x02E0;
        private bool IsOverChildControl(Point clientPos) { var child = GetChildAtPoint(clientPos, GetChildAtPointSkip.Invisible | GetChildAtPointSkip.Transparent); return child != null; }
        private bool IsInDraggableArea(Point clientPos) { if (UseHelperInfrastructure && _captionHelper != null && _captionHelper.ShowCaptionBar) { if (clientPos.Y <= _captionHelper.CaptionHeight && !_captionHelper.IsPointInSystemButtons(clientPos) && !IsOverChildControl(clientPos)) return true; return false; } return clientPos.Y <= 36 && !IsOverChildControl(clientPos); }
        [DllImport("user32.dll")] private static extern uint GetDpiForWindow(IntPtr hWnd);
        protected override void WndProc(ref Message m) { switch (m.Msg) { case WM_DPICHANGED: if (DpiMode == DpiHandlingMode.Manual) { var suggested = Marshal.PtrToStructure<RECT>(m.LParam); var suggestedBounds = Rectangle.FromLTRB(suggested.left, suggested.top, suggested.right, suggested.bottom); this.Bounds = suggestedBounds; uint dpi = GetDpiForWindow(this.Handle); } break; case WM_ENTERSIZEMOVE: _inMoveOrResize = true; break; case WM_EXITSIZEMOVE: _inMoveOrResize = false; UpdateFormRegion(); Invalidate(); break; case WM_GETMINMAXINFO: AdjustMaximizedBounds(m.LParam); break; case WM_NCHITTEST when !_inpopupmode: if (UseHelperInfrastructure && _hitTestHelper != null) { if (_hitTestHelper.HandleNcHitTest(ref m)) return; } else { Point pos = PointToClient(new Point(m.LParam.ToInt32())); int margin = _resizeMargin; if (pos.X <= margin && pos.Y <= margin) { m.Result = (IntPtr)HTTOPLEFT; return; } if (pos.X >= ClientSize.Width - margin && pos.Y <= margin) { m.Result = (IntPtr)HTTOPRIGHT; return; } if (pos.X <= margin && pos.Y >= ClientSize.Height - margin) { m.Result = (IntPtr)HTBOTTOMLEFT; return; } if (pos.X >= ClientSize.Width - margin && pos.Y >= ClientSize.Height - margin) { m.Result = (IntPtr)HTBOTTOMRIGHT; return; } if (pos.X <= margin) { m.Result = (IntPtr)HTLEFT; return; } if (pos.X >= ClientSize.Width - margin) { m.Result = (IntPtr)HTRIGHT; return; } if (pos.Y <= margin) { m.Result = (IntPtr)HTTOP; return; } if (pos.Y >= ClientSize.Height - margin) { m.Result = (IntPtr)HTBOTTOM; return; } if (IsOverChildControl(pos)) { m.Result = (IntPtr)HTCLIENT; return; } m.Result = IsInDraggableArea(pos) ? (IntPtr)HTCAPTION : (IntPtr)HTCLIENT; return; } break; } base.WndProc(ref m); }
        [StructLayout(LayoutKind.Sequential)] private struct POINT { public int x; public int y; }
        [StructLayout(LayoutKind.Sequential)] private struct MINMAXINFO { public POINT ptReserved; public POINT ptMaxSize; public POINT ptMaxPosition; public POINT ptMinTrackSize; public POINT ptMaxTrackSize; }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)] private struct MONITORINFO { public int cbSize; public RECT rcMonitor; public RECT rcWork; public int dwFlags; }
        [StructLayout(LayoutKind.Sequential)] private struct RECT { public int left; public int top; public int right; public int bottom; }
        [DllImport("user32.dll")] private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi); [DllImport("user32.dll")] private static extern IntPtr MonitorFromWindow(IntPtr handle, int flags); private const int MONITOR_DEFAULTTONEAREST = 2;
        private void AdjustMaximizedBounds(IntPtr lParam) { MINMAXINFO mmi = Marshal.PtrToStructure<MINMAXINFO>(lParam); IntPtr monitor = MonitorFromWindow(this.Handle, MONITOR_DEFAULTTONEAREST); if (monitor != IntPtr.Zero) { MONITORINFO monitorInfo = new MONITORINFO(); monitorInfo.cbSize = Marshal.SizeOf(typeof(MONITORINFO)); GetMonitorInfo(monitor, ref monitorInfo); Rectangle rcWorkArea = Rectangle.FromLTRB(monitorInfo.rcWork.left, monitorInfo.rcWork.top, monitorInfo.rcWork.right, monitorInfo.rcWork.bottom); Rectangle rcMonitorArea = Rectangle.FromLTRB(monitorInfo.rcMonitor.left, monitorInfo.rcMonitor.top, monitorInfo.rcMonitor.right, monitorInfo.rcMonitor.bottom); mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.Left - rcMonitorArea.Left); mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.Top - rcMonitorArea.Top); mmi.ptMaxSize.x = rcWorkArea.Width; mmi.ptMaxSize.y = rcWorkArea.Height; Marshal.StructureToPtr(mmi, lParam, true); } }
        #endregion

        private void InitializeComponent()
        {

        }
    }
}
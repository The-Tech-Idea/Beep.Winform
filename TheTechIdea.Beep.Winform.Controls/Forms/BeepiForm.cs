using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.ConfigUtil;
 
using TheTechIdea.Beep.Vis.Modules;
 
using TheTechIdea.Beep.Winform.Controls.BaseImage;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Converters;
using TheTechIdea.Beep.Winform.Controls.Forms.Caption;
using TheTechIdea.Beep.Winform.Controls.Forms.Helpers;
using TheTechIdea.Beep.Winform.Controls.Managers;

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

        // Central design-mode detection (more reliable than DesignMode alone)
        private static bool IsDesignProcess()
        {
            try
            {
                string proc = Process.GetCurrentProcess().ProcessName;
                return proc.Equals("devenv", StringComparison.OrdinalIgnoreCase)
                       || proc.Equals("Blend", StringComparison.OrdinalIgnoreCase)
                       || proc.Equals("XDesProc", StringComparison.OrdinalIgnoreCase);
            }
            catch { return false; }
        }
        // Force runtime behavior even in designer (user request to remove any check for design host)
        private bool InDesignHost => false;

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
       // private FormStyleHelper _styleHelper;
        private FormHitTestHelper _hitTestHelper;
        private FormCaptionBarHelper _captionHelper;
        private FormBorderPainter _borderPainter;  // *** ADDED: Border painting helper
                                                   // Helpers (add this near the other helpers)

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
        private BeepControlStyle _controlstyle = BeepControlStyle.Material3;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The visual style/painter to use for rendering the sidebar.")]
        [DefaultValue(BeepControlStyle.Material3)]
        public BeepControlStyle Style
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

        // IBeepModernFormHost implementation
        Form IBeepModernFormHost.AsForm => this;
        IBeepTheme IBeepModernFormHost.CurrentTheme => _currentTheme ?? BeepThemesManager.GetDefaultTheme();
        bool IBeepModernFormHost.IsInDesignMode => InDesignHost;
        void IBeepModernFormHost.UpdateRegion() => _regionHelper?.EnsureRegion(true);

        #region Fields
        protected int _resizeMargin = 8;
        protected int _borderRadius = 8;
        protected int _borderThickness = 3;
        private Color _borderColor = Color.Red;
        private bool _inpopupmode = false;
        private string _title = "BeepiForm";
        private bool _inMoveOrResize = false;
        // Theme reference is nullable during design-time to avoid loading heavy theme machinery
        protected IBeepTheme? _currentTheme; // nullable; resolved on demand

        // Removed incomplete FallbackTheme class (was causing compile errors)
        // private class FallbackTheme : IBeepTheme { /* removed */ }
        private bool _applythemetochilds = true;
        public event EventHandler OnFormClose;
        public event EventHandler OnFormLoad;
        public event EventHandler OnFormShown;
        public event EventHandler<FormClosingEventArgs> PreClose;
        private int _savedBorderRadius = 0;
        private int _savedBorderThickness = 3; // Match default _borderThickness
        #endregion

        #region STYLE (merged from Style partial)
        private BeepFormStyle _formStyle = BeepFormStyle.Modern;
        private Color _shadowColor = Color.FromArgb(50, 0, 0, 0);
        private bool _enableGlow = true;
        private Color _glowColor = Color.FromArgb(100, 72, 170, 255);
        private float _glowSpread = 8f;
        private int _shadowDepth = 6;

        [Category("Beep ProgressBarStyle")][DefaultValue(BeepFormStyle.Modern)] public BeepFormStyle FormStyle { get => _formStyle; set { if (_formStyle == value) return; _formStyle = value; try { ApplyFormStyle(); _captionHelper?.SetStyle(value); } catch (Exception ex) { Debug.WriteLine($"FormStyle apply error: {ex.Message}"); } Invalidate(); } }
        [Category("Beep ProgressBarStyle")] public Color ShadowColor { get => _shadowColor; set { if (_shadowColor != value) { _shadowColor = value; if (!InDesignHost) SyncStyleToHelpers(); Invalidate(); } } }
        [Category("Beep ProgressBarStyle"), DefaultValue(6)] public int ShadowDepth { get => _shadowDepth; set { if (_shadowDepth != value) { _shadowDepth = value; if (!InDesignHost) SyncStyleToHelpers(); Invalidate(); } } }
        [Category("Beep ProgressBarStyle"), DefaultValue(true)] public bool EnableGlow { get => _enableGlow; set { if (_enableGlow != value) { _enableGlow = value; if (!InDesignHost) SyncStyleToHelpers(); Invalidate(); } } }
        [Category("Beep ProgressBarStyle")] public Color GlowColor { get => _glowColor; set { if (_glowColor != value) { _glowColor = value; if (!InDesignHost) SyncStyleToHelpers(); Invalidate(); } } }
        [Category("Beep ProgressBarStyle"), DefaultValue(8f)] public float GlowSpread { get => _glowSpread; set { if (Math.Abs(_glowSpread - value) > float.Epsilon) { _glowSpread = Math.Max(0f, value); if (!InDesignHost) SyncStyleToHelpers(); Invalidate(); } } }

        // New: auto mapping/preset toggles
        [Category("Beep ProgressBarStyle"), DefaultValue(true), Description("If true, when the current theme is dark, renderer preset keys with .dark will be applied (if available).")]
        public bool AutoPickDarkPresets { get; set; } = true;

        [Category("Beep ProgressBarStyle"), DefaultValue(true), Description("If true, switching CaptionRenderer maps FormStyle and tries to apply a matching preset.")]
        public bool AutoApplyRendererPreset { get; set; } = true;
        // Logo properties now delegated to FormCaptionBarHelper
        [Category("Beep Logo")]
        [Description("Path to the logo image file (SVG, PNG, JPG, etc.)")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))
        ]
        public string LogoImagePath
        {
            get => _captionHelper?.LogoImagePath ?? string.Empty;
            set { if (_captionHelper != null) { _captionHelper.LogoImagePath = value; Invalidate(); } }
        }

        [Category("Beep Logo"), DefaultValue(false)]
        [Description("Whether to show the logo on the form caption")]
        public bool ShowLogo
        {
            get => _captionHelper?.ShowLogo ?? false;
            set { if (_captionHelper != null) _captionHelper.ShowLogo = value; }
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
            set { if (_captionHelper != null) _captionHelper.LogoSize = value; }
        }

        [Category("Beep Logo")]
        [Description("Margin around the logo icon")]
        public Padding LogoMargin
        {
            get => _captionHelper?.LogoMargin ?? new Padding(8, 8, 8, 8);
            set { if (_captionHelper != null) _captionHelper.LogoMargin = value; }
        }
        // MDI properties-----------------------------------------


    // End MDI properties-----------------------------------------
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)] public BeepFormStylePresets StylePresets { get; } = new();
        public void ApplyPreset(string key) { if (StylePresets.TryGet(key, out var m)) { _borderRadius = m.BorderRadius; _borderThickness = m.BorderThickness; _shadowDepth = m.ShadowDepth; _enableGlow = m.EnableGlow; _glowSpread = m.GlowSpread; if (!InDesignHost) SyncStyleToHelpers(); Invalidate(); } }
        private void SyncStyleToHelpers() { if (InDesignHost || !UseHelperInfrastructure || _shadowGlow == null) return; _shadowGlow.ShadowColor = _shadowColor; _shadowGlow.ShadowDepth = _shadowDepth; _shadowGlow.EnableGlow = _enableGlow; _shadowGlow.GlowColor = _glowColor; _shadowGlow.GlowSpread = _glowSpread; }
        private void ApplyMetrics(BeepFormStyle style) { if (InDesignHost) return; if (!BeepFormStyleMetricsDefaults.Map.TryGetValue(style, out var m)) return; _borderRadius = m.BorderRadius; /* Don't override user's BorderThickness: _borderThickness = m.BorderThickness; */ _shadowDepth = m.ShadowDepth; _enableGlow = m.EnableGlow; _glowSpread = m.GlowSpread; SyncStyleToHelpers(); }
        private void ApplyThemeMapping() { if (InDesignHost || _currentTheme == null) return; BackColor = _currentTheme.ButtonBackColor; BorderColor = _currentTheme.BorderColor; }
        #endregion

        #region Animations (merged)
        private bool _animateMaximizeRestore = true;
        private bool _animateStyleChange = true;
        public bool AnimateMaximizeRestore { get => _animateMaximizeRestore; set => _animateMaximizeRestore = value; }
        public bool AnimateStyleChange { get => _animateStyleChange; set => _animateStyleChange = value; }
        private async Task AnimateOpacityAsync(double from, double to, int durationMs) { if (InDesignHost) return; try { double start = from, end = to; int steps = 10; double delta = (end - start) / steps; int delay = Math.Max(8, durationMs / steps); Opacity = Math.Clamp(start, 0, 1); for (int i = 0; i < steps; i++) { await Task.Delay(delay); Opacity = Math.Clamp(Opacity + delta, 0, 1); } Opacity = Math.Clamp(end, 0, 1); } catch { } }
        #endregion

        #region Acrylic / Backdrop / Mica (merged)
        private bool _enableAcrylicForGlass = true; public bool EnableAcrylicForGlass { get => _enableAcrylicForGlass; set { _enableAcrylicForGlass = value; if (!InDesignHost && IsHandleCreated) ApplyAcrylicEffectIfNeeded(); } }
        private bool _enableMicaBackdrop = false; public bool EnableMicaBackdrop { get => _enableMicaBackdrop; set { _enableMicaBackdrop = value; if (!InDesignHost && IsHandleCreated) ApplyMicaBackdropIfNeeded(); } }
        private BackdropType _backdrop = BackdropType.None; public BackdropType Backdrop { get => _backdrop; set { _backdrop = value; if (!InDesignHost && IsHandleCreated) ApplyBackdrop(); } }
        private void ApplyAcrylicEffectIfNeeded() { if (InDesignHost || !IsHandleCreated) return; if (_formStyle == BeepFormStyle.Glass && _enableAcrylicForGlass) TryEnableAcrylic(); else TryDisableAcrylic(); }
        private void ApplyMicaBackdropIfNeeded() { if (InDesignHost || !IsHandleCreated) return; if (_enableMicaBackdrop) TryEnableMica(); else TryDisableMica(); }
        protected override void OnHandleCreated(EventArgs e) { base.OnHandleCreated(e); if (InDesignHost) return; ApplyAcrylicEffectIfNeeded(); ApplyMicaBackdropIfNeeded(); ApplyImmersiveDarkMode(); }
        private void ApplyBackdrop() { if (InDesignHost) return; try { TryDisableMica(); } catch { } try { TryDisableAcrylic(); } catch { } switch (_backdrop) { case BackdropType.Mica: TryEnableMica(); break; case BackdropType.Acrylic: TryEnableAcrylic(); break; case BackdropType.Tabbed: TryEnableSystemBackdrop(3); break; case BackdropType.Transient: TryEnableSystemBackdrop(2); break; case BackdropType.Blur: TryEnableBlurBehind(); break; case BackdropType.None: default: break; } }
        private void TryEnableSystemBackdrop(int type) { if (InDesignHost) return; try { var attr = (DWMWINDOWATTRIBUTE)38; DwmSetWindowAttribute(this.Handle, attr, ref type, Marshal.SizeOf<int>()); } catch { } }
        private void TryEnableBlurBehind() { if (InDesignHost) return; try { var accent = new ACCENT_POLICY { AccentState = ACCENT_STATE.ACCENT_ENABLE_BLURBEHIND }; int size = Marshal.SizeOf(accent); IntPtr p = Marshal.AllocHGlobal(size); Marshal.StructureToPtr(accent, p, false); var data = new WINDOWCOMPOSITIONATTRIBDATA { Attribute = WINDOWCOMPOSITIONATTRIB.WCA_ACCENT_POLICY, Data = p, SizeOfData = size }; SetWindowCompositionAttribute(this.Handle, ref data); Marshal.FreeHGlobal(p); } catch { } }
        private void TryEnableAcrylic() { if (InDesignHost) return; try { var accent = new ACCENT_POLICY { AccentState = ACCENT_STATE.ACCENT_ENABLE_ACRYLICBLURBEHIND, AccentFlags = 2, GradientColor = (uint)((0xCC << 24) | (BackColor.B << 16) | (BackColor.G << 8) | BackColor.R) }; int size = Marshal.SizeOf(accent); IntPtr p = Marshal.AllocHGlobal(size); Marshal.StructureToPtr(accent, p, false); var data = new WINDOWCOMPOSITIONATTRIBDATA { Attribute = WINDOWCOMPOSITIONATTRIB.WCA_ACCENT_POLICY, Data = p, SizeOfData = size }; SetWindowCompositionAttribute(this.Handle, ref data); Marshal.FreeHGlobal(p); } catch { } }
        private void TryDisableAcrylic() { if (InDesignHost) return; try { var accent = new ACCENT_POLICY { AccentState = ACCENT_STATE.ACCENT_DISABLED }; int size = Marshal.SizeOf(accent); IntPtr p = Marshal.AllocHGlobal(size); Marshal.StructureToPtr(accent, p, false); var data = new WINDOWCOMPOSITIONATTRIBDATA { Attribute = WINDOWCOMPOSITIONATTRIB.WCA_ACCENT_POLICY, Data = p, SizeOfData = size }; SetWindowCompositionAttribute(this.Handle, ref data); Marshal.FreeHGlobal(p); } catch { } }
        private void TryEnableMica() { if (InDesignHost) return; try { int trueVal = 1; DwmSetWindowAttribute(this.Handle, DWMWINDOWATTRIBUTE.DWMWA_MICA_EFFECT, ref trueVal, Marshal.SizeOf<int>()); } catch { } }
        private void TryDisableMica() { if (InDesignHost) return; try { int falseVal = 0; DwmSetWindowAttribute(this.Handle, DWMWINDOWATTRIBUTE.DWMWA_MICA_EFFECT, ref falseVal, Marshal.SizeOf<int>()); } catch { } }
        [DllImport("user32.dll")] private static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WINDOWCOMPOSITIONATTRIBDATA data);
        private enum WINDOWCOMPOSITIONATTRIB { WCA_ACCENT_POLICY = 19 }
        private enum ACCENT_STATE { ACCENT_DISABLED = 0, ACCENT_ENABLE_GRADIENT = 1, ACCENT_ENABLE_TRANSPARENTGRADIENT = 2, ACCENT_ENABLE_BLURBEHIND = 3, ACCENT_ENABLE_ACRYLICBLURBEHIND = 4, ACCENT_ENABLE_HOSTBACKDROP = 5 }
        [StructLayout(LayoutKind.Sequential)] private struct ACCENT_POLICY { public ACCENT_STATE AccentState; public int AccentFlags; public uint GradientColor; public int AnimationId; }
        [StructLayout(LayoutKind.Sequential)] private struct WINDOWCOMPOSITIONATTRIBDATA { public WINDOWCOMPOSITIONATTRIB Attribute; public IntPtr Data; public int SizeOfData; }
        private enum DWMWINDOWATTRIBUTE { DWMWA_USE_IMMERSIVE_DARK_MODE = 20, DWMWA_MICA_EFFECT = 1029 }
        [DllImport("dwmapi.dll")] private static extern int DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE attr, ref int attrValue, int attrSize);
        #endregion

        #region Snap Hints (merged)
        private bool _showSnapHints = true; public bool ShowSnapHints { get => _showSnapHints; set { _showSnapHints = value; Invalidate(); } }
        private Rectangle _snapLeft, _snapRight, _snapTop; private bool _showSnapOverlay;
        private void SnapHints_OnMouseMove(MouseEventArgs e) { if (InDesignHost || !_showSnapHints) return; if (WindowState == FormWindowState.Normal && (MouseButtons & MouseButtons.Left) == MouseButtons.Left) { var screen = Screen.FromPoint(Cursor.Position).WorkingArea; int thickness = 8; _snapLeft = new Rectangle(screen.Left, screen.Top, thickness, screen.Height); _snapRight = new Rectangle(screen.Right - thickness, screen.Top, thickness, screen.Height); _snapTop = new Rectangle(screen.Left, screen.Top, screen.Width, thickness); _showSnapOverlay = true; Invalidate(); } else if (_showSnapOverlay) { _showSnapOverlay = false; Invalidate(); } }
        private void SnapHints_OnMouseLeave() { if (_showSnapOverlay) { _showSnapOverlay = false; Invalidate(); } }
        private void SnapHints_OnPaintOverlay(Graphics g) { if (InDesignHost || !_showSnapHints) return; using var br = new SolidBrush(Color.FromArgb(40, 0, 120, 215)); g.FillRectangle(br, _snapLeft); g.FillRectangle(br, _snapRight); g.FillRectangle(br, _snapTop); }
        #endregion

        #region DPI handling mode
        public enum DpiHandlingMode { Framework, Manual }
        [Browsable(true)][Category("DPI/Scaling")][Description("How DPI awareness and scaling are handled. Framework = let WinForms manage DPI. Manual = use BeepiForm's custom handling.")] public DpiHandlingMode DpiMode { get; set; } = DpiHandlingMode.Framework;
        #endregion

        #region Caption Properties
        [Category("Beep Caption")]
        [DefaultValue(true)]
        public bool ShowCaptionBar
        {
            get => _captionHelper?.ShowCaptionBar ?? _legacyShowCaptionBar; set
            {
                if (_captionHelper != null) _captionHelper.ShowCaptionBar = value;
                _legacyShowCaptionBar = value;
                Invalidate();
                if (IsHandleCreated) PerformLayout();   // add this line
            }
        }
            [Category("Beep Caption")][DefaultValue(36)] public int CaptionHeight { get => _captionHelper?.CaptionHeight ?? _legacyCaptionHeight; set
            {
                if (value < 24) value = 24;
                if (_captionHelper != null) _captionHelper.CaptionHeight = value;
                _legacyCaptionHeight = value;
                Invalidate();
                if (IsHandleCreated) PerformLayout();   // add this line
            }
        }
        [Category("Beep Caption")][DefaultValue(true)] public bool ShowSystemButtons { get => _captionHelper?.ShowSystemButtons ?? _legacyShowSystemButtons; set { if (_captionHelper != null) _captionHelper.ShowSystemButtons = value; _legacyShowSystemButtons = value; Invalidate(); } }
        [Category("Beep Caption")][DefaultValue(true)] public bool EnableCaptionGradient { get => _captionHelper?.EnableCaptionGradient ?? _legacyEnableCaptionGradient; set { if (_captionHelper != null) _captionHelper.EnableCaptionGradient = value; _legacyEnableCaptionGradient = value; Invalidate(); } }
        [Category("Beep Caption")][Description("Padding around the caption content")] public Padding CaptionPadding { get => _captionHelper?.LogoMargin ?? new Padding(8, 8, 8, 8); set { if (_captionHelper != null) _captionHelper.LogoMargin = value; Invalidate(); } }
        [Category("Beep Caption")][DefaultValue(false)][Description("Enable Windows 11 immersive dark mode for the window")] public bool UseImmersiveDarkMode { get => _useImmersiveDarkMode; set { if (_useImmersiveDarkMode != value) { _useImmersiveDarkMode = value; if (!InDesignHost && IsHandleCreated) ApplyImmersiveDarkMode(); } } }
        private bool _useImmersiveDarkMode = false;
        private void ApplyImmersiveDarkMode() { if (InDesignHost || !IsHandleCreated) return; try { int value = _useImmersiveDarkMode ? 1 : 0; DwmSetWindowAttribute(Handle, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, ref value, Marshal.SizeOf<int>()); } catch { } }
        private void InitializeCaptionHelperWithLegacyState() { if (_captionHelper != null) { _captionHelper.ShowCaptionBar = _legacyShowCaptionBar; _captionHelper.ShowSystemButtons = _legacyShowSystemButtons; _captionHelper.EnableCaptionGradient = _legacyEnableCaptionGradient; _captionHelper.CaptionHeight = _legacyCaptionHeight; } }

        // New: expose CaptionRendererKind (deprecated - use FormStyle instead)
        [Category("Beep Caption")]
        [DefaultValue(Forms.Caption.CaptionRendererKind.Windows)]
        [TypeConverter(typeof(TheTechIdea.Beep.Winform.Controls.Forms.Caption.Design.CaptionRendererKindConverter))]
        [Editor(typeof(TheTechIdea.Beep.Winform.Controls.Forms.Caption.Design.CaptionRendererKindEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Obsolete("Use FormStyle property instead. CaptionRenderer is deprecated and will be removed in a future version.")]
        public Forms.Caption.CaptionRendererKind CaptionRenderer
        {
            get => _captionHelper?.RendererKind ?? Forms.Caption.CaptionRendererKind.Windows;
            set { _captionHelper?.SwitchRenderer(value); Invalidate(); }
        }

        // New: separate buttons in caption bar
        [Category("Beep Caption"), DefaultValue(false)]
        public bool ShowThemeButton
        {
            get => _captionHelper?.ShowThemeButton ?? false;
            set { if (_captionHelper != null) { _captionHelper.ShowThemeButton = value; Invalidate(); } }
        }

        [Category("Beep Caption"), DefaultValue(false)]
        public bool ShowStyleButton
        {
            get => _captionHelper?.ShowStyleButton ?? false;
            set { if (_captionHelper != null) { _captionHelper.ShowStyleButton = value; Invalidate(); } }
        }

        [Category("Beep Caption"), Description("Icon path for the Theme button (optional)"), Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))
        ]
        public string ThemeButtonIconPath
        {
            get => _captionHelper?.ThemeButtonIconPath ?? string.Empty;
            set { if (_captionHelper != null) { _captionHelper.ThemeButtonIconPath = value; Invalidate(); } }
        }

        [Category("Beep Caption"), Description("Icon path for the ProgressBarStyle button (optional)"), Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))
        ]
        public string StyleButtonIconPath
        {
            get => _captionHelper?.StyleButtonIconPath ?? string.Empty;
            set { if (_captionHelper != null) { _captionHelper.StyleButtonIconPath = value; Invalidate(); } }
        }

        // Kept for backward compatibility but hidden
        [Browsable(false)]
        [DefaultValue(Forms.Caption.CaptionBarExtraButtonKind.None)]
        public Forms.Caption.CaptionBarExtraButtonKind CaptionExtraButton
        {
            get
            {
                if ((_captionHelper?.ShowThemeButton ?? false) && !(_captionHelper?.ShowStyleButton ?? false)) return Forms.Caption.CaptionBarExtraButtonKind.Themes;
                if (!(_captionHelper?.ShowThemeButton ?? false) && (_captionHelper?.ShowStyleButton ?? false)) return Forms.Caption.CaptionBarExtraButtonKind.StyleToggle;
                return Forms.Caption.CaptionBarExtraButtonKind.None;
            }
            set
            {
                if (_captionHelper == null) return;
                _captionHelper.ShowThemeButton = value == Forms.Caption.CaptionBarExtraButtonKind.Themes;
                _captionHelper.ShowStyleButton = value == Forms.Caption.CaptionBarExtraButtonKind.StyleToggle;
                Invalidate();
            }
        }
        #endregion

        #region Properties
        [Browsable(true)][Category("Appearance")] public string Title 
        { get => _title; 
            set
            {
                _title = value; 
                if (beepuiManager1 != null)
                {
                    beepuiManager1.Title = value;
                }
                Text = value;
                if (!InDesignHost) Text = value; 
                Invalidate();
            }
        }
        [Browsable(true)][Category("Appearance")] public bool ApplyThemeToChilds { get => _applythemetochilds; set => _applythemetochilds = value; }
        [Browsable(true)][Category("Appearance"), Description("The Thickness of the form's border."), DefaultValue(3), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int BorderThickness
        {
            get => _borderThickness;
            set
            {
                _borderThickness = value;
                if (!InDesignHost && UseHelperInfrastructure)
                    _state.RegionDirty = true;
                if (!InDesignHost && WindowState != FormWindowState.Maximized)
                    Padding = new Padding(Math.Max(0, _borderThickness));
                
                // Force WM_NCCALCSIZE recalculation and repaint when border thickness changes
                if (IsHandleCreated && WindowState != FormWindowState.Maximized)
                {
                    // Force Windows to recalculate non-client area
                    SetWindowPos(Handle, IntPtr.Zero, 0, 0, 0, 0, 
                        SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
                    // Repaint the non-client area
                    RedrawWindow(Handle, IntPtr.Zero, IntPtr.Zero, RDW_FRAME | RDW_INVALIDATE | RDW_UPDATENOW);
                }
                
                Invalidate();
            }
        }
        [Browsable(true)][Category("Appearance"), Description("The radius of the form's border."), DefaultValue(5), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] public int BorderRadius { get => _borderRadius; set { _borderRadius = Math.Max(0, value); if (!InDesignHost && UseHelperInfrastructure) { _state.RegionDirty = true; _regionHelper?.InvalidateRegion(); } if (!InDesignHost && IsHandleCreated && ClientSize.Width > 0 && ClientSize.Height > 0) UpdateFormRegion(); Invalidate(); } }
        [Browsable(true)][Category("Appearance")] public bool InPopMode { get => _inpopupmode; set { _inpopupmode = value; Invalidate(); } }
        private string _theme = string.Empty;
        [Browsable(true)][TypeConverter(typeof(ThemeConverter))][DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] public string Theme { get => _theme; set { if (value != _theme) { _theme = value; if (!InDesignHost) { _currentTheme = BeepThemesManager.GetTheme(value); ApplyTheme(); } } } }
        [Browsable(true)][Category("Appearance"), Description("Sets the color of the form's border.")]
        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
                Invalidate();
                // Ensure non-client (custom) border repaints with new color
                if (IsHandleCreated && WindowState != FormWindowState.Maximized)
                {
                    RedrawWindow(Handle, IntPtr.Zero, IntPtr.Zero, RDW_FRAME | RDW_INVALIDATE | RDW_UPDATENOW);
                }
            }
        }
        public bool UseHelperInfrastructure { get; set; } = true;
        #endregion

        #region Ctor
        public BeepiForm()
        {
            try
            {
                var container = new System.ComponentModel.Container();
                beepuiManager1 = new BeepFormUIManager(container)
                {
                    Theme = "DefaultType",
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

                if (!InDesignHost)
                {
                    _currentTheme = BeepThemesManager.GetDefaultTheme();
                }
                // else leave null and guard usages

                if (InDesignHost)
                {
                    return; // skip heavy init
                }

                if (UseHelperInfrastructure)
                {
                    _state = new FormStateStore();
                    _shadowGlow = new FormShadowGlowPainter();
                    _overlayRegistry = new FormOverlayPainterRegistry();
                    _regionHelper = new FormRegionHelper(this);
                    _layoutHelper = new FormLayoutHelper(this);
                    _themeHelper = new FormThemeHelper(this);
                //    _styleHelper = new FormStyleHelper(this, _shadowGlow);
                    _borderPainter = new FormBorderPainter(this);  // *** ADDED: Initialize border painter
                    _captionHelper = new FormCaptionBarHelper(this, _overlayRegistry, padAdj => RegisterPaddingProvider((ref Padding p) => padAdj(ref p)));
                   
                    //-------------
                    InitializeCaptionHelperWithLegacyState();
                    _hitTestHelper = new FormHitTestHelper(this,
                        captionEnabled: () => _captionHelper?.ShowCaptionBar == true,
                        captionHeight: () => _captionHelper?.CaptionHeight ?? 0,
                        isOverSystemButton: () => _captionHelper?.IsCursorOverSystemButton == true,
                        resizeMargin: _resizeMargin);
                    RegisterMouseMoveHandler(e => _captionHelper.OnMouseMove(e));
                    RegisterMouseLeaveHandler(() => _captionHelper.OnMouseLeave());
                    RegisterMouseDownHandler(e => _captionHelper.OnMouseDown(e));
                    RegisterMouseMoveHandler(SnapHints_OnMouseMove);
                    RegisterMouseLeaveHandler(SnapHints_OnMouseLeave);
                    RegisterOverlayPainter(SnapHints_OnPaintOverlay);
                    // Caption bar painted in client area as overlay for proper mouse interaction
                    RegisterOverlayPainter(g => _captionHelper.PaintOverlay(g));
                }
            }
            catch (Exception ex)
            {
                if (InDesignHost)
                {
                    Debug.WriteLine("BeepiForm design-time init error: " + ex.Message);
                }
                else
                {
                    throw;
                }
            }
        }
        #endregion

        #region Lifecycle
        private void BeepiForm_Load(object? sender, EventArgs e) { if (InDesignHost) return; if (BackColor == Color.Transparent || BackColor == Color.Empty) BackColor = SystemColors.Control; ApplyTheme(); Invalidate(); Update(); OnFormLoad?.Invoke(this, EventArgs.Empty); }
        private void BeepiForm_VisibleChanged(object? sender, EventArgs e) { if (InDesignHost) return; if (Visible) { if (BackColor == Color.Transparent || BackColor == Color.Empty) BackColor = _currentTheme?.BackColor ?? SystemColors.Control; Invalidate(); Update(); } }
        protected override void OnShown(EventArgs e) { base.OnShown(e); if (InDesignHost) return; try { beepuiManager1.Initialize(this); } catch { } OnFormShown?.Invoke(this, EventArgs.Empty); }
        protected override void OnLoad(EventArgs e) { base.OnLoad(e); if (InDesignHost) return; }
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) { base.SetBoundsCore(x, y, width, height, specified); if (!InDesignHost && (specified & BoundsSpecified.Size) != 0) { PerformLayout(); } }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (InDesignHost) return;
            
            ApplyMaximizedWindowFix();
            
            if (UseHelperInfrastructure && _regionHelper != null)
                _regionHelper.EnsureRegion();
            else if (!UseHelperInfrastructure)
                UpdateFormRegion();
            
            // Invalidate non-client area to repaint border
            if (IsHandleCreated && WindowState != FormWindowState.Maximized)
            {
                RedrawWindow(Handle, IntPtr.Zero, IntPtr.Zero, RDW_FRAME | RDW_INVALIDATE | RDW_UPDATENOW);
            }
            
            Invalidate();
        }
        #endregion

        #region Apply Theme / Style
        public virtual void ApplyTheme()
        {
            if (InDesignHost) { Invalidate(); return; }
            SuspendLayout();
            try
            {
                Color newBackColor = _currentTheme?.BackColor ?? SystemColors.Control;
                if (newBackColor == Color.Transparent || newBackColor == Color.Empty) newBackColor = SystemColors.Control;
                BackColor = newBackColor;
                BorderColor = _currentTheme?.BorderColor ?? SystemColors.ControlDark;
                _captionHelper?.UpdateTheme();
                ApplyFormStyle();
            }
            finally
            {
                ResumeLayout(true);
                Invalidate();
                // Repaint non-client area (border) when theme changes
                if (IsHandleCreated && WindowState != FormWindowState.Maximized)
                {
                    RedrawWindow(Handle, IntPtr.Zero, IntPtr.Zero, RDW_FRAME | RDW_INVALIDATE | RDW_UPDATENOW);
                }
                Update();
            }
        }
        protected void ApplyFormStyle()
        {
            if (InDesignHost) { Invalidate(); return; }

            // 1) Pull all structural metrics (including BorderRadius/BorderThickness) from defaults
            ApplyMetrics(_formStyle);

            // 2) Apply style-specific visual tweaks (no radius/thickness overrides here)
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
                    // Metrics (radius/thickness/etc.) already come from ApplyMetrics
                    _glowColor = Color.FromArgb(80, 0, 0, 0);
                    BackColor = Color.FromArgb(32, 32, 32);
                    BorderColor = Color.FromArgb(64, 64, 64);
                    break;

                case BeepFormStyle.Material:
                    // Metrics already applied (thickness=0, radius=4); keep transparent border and glow color
                    ApplyThemeMapping();
                    _glowColor = Color.FromArgb(60, 0, 0, 0);
                    BorderColor = Color.Transparent;
                    break;

                case BeepFormStyle.Minimal:
                    // Rely on metrics; nothing structural to override
                    ApplyThemeMapping();
                    break;

                case BeepFormStyle.Classic:
                    // Rely on metrics; apply classic system colors
                    BackColor = SystemColors.Control;
                    BorderColor = SystemColors.ActiveBorder;
                    break;

                case BeepFormStyle.Gnome:
                    ApplyThemeMapping();
                    _glowColor = Color.FromArgb(60, 100, 100, 100);
                    break;

                case BeepFormStyle.Kde:
                    ApplyThemeMapping();
                    _glowColor = Color.FromArgb(80, 0, 120, 200);
                    break;

                case BeepFormStyle.Cinnamon:
                    ApplyThemeMapping();
                    _glowColor = Color.FromArgb(90, 200, 100, 50);
                    break;

                case BeepFormStyle.Elementary:
                    ApplyThemeMapping();
                    _glowColor = Color.FromArgb(70, 150, 150, 150);
                    break;

                case BeepFormStyle.Fluent:
                    ApplyThemeMapping();
                    _glowColor = Color.FromArgb(120, 0, 120, 255);
                    break;

                case BeepFormStyle.NeoBrutalist:
                    BackColor = Color.White;
                    BorderColor = Color.Black;
                    _glowColor = Color.Transparent;
                    break;

                // New styles
                case BeepFormStyle.Neon:
                    BackColor = Color.FromArgb(20, 20, 20);
                    BorderColor = Color.FromArgb(0, 255, 255);
                    _glowColor = Color.FromArgb(150, 0, 255, 255);
                    break;

                case BeepFormStyle.Retro:
                    BackColor = Color.FromArgb(45, 25, 45);
                    BorderColor = Color.FromArgb(255, 100, 200);
                    _glowColor = Color.FromArgb(120, 255, 50, 150);
                    break;

                case BeepFormStyle.Gaming:
                    BackColor = Color.FromArgb(15, 15, 25);
                    BorderColor = Color.FromArgb(0, 255, 0);
                    _glowColor = Color.FromArgb(100, 0, 255, 0);
                    break;

                case BeepFormStyle.Corporate:
                    BackColor = Color.FromArgb(248, 248, 248);
                    BorderColor = Color.FromArgb(180, 180, 180);
                    _glowColor = Color.FromArgb(40, 100, 100, 100);
                    break;

                case BeepFormStyle.Artistic:
                    ApplyThemeMapping();
                    _glowColor = Color.FromArgb(140, 200, 50, 150);
                    break;

                case BeepFormStyle.HighContrast:
                    BackColor = Color.Black;
                    BorderColor = Color.White;
                    _glowColor = Color.Transparent;
                    break;

                case BeepFormStyle.Soft:
                    ApplyThemeMapping();
                    _glowColor = Color.FromArgb(80, 200, 200, 255);
                    break;

                case BeepFormStyle.Industrial:
                    BackColor = Color.FromArgb(60, 60, 70);
                    BorderColor = Color.FromArgb(120, 120, 130);
                    _glowColor = Color.FromArgb(100, 200, 200, 200);
                    break;

                case BeepFormStyle.Custom:
                    ApplyThemeMapping();
                    break;
            }

            // 3) Push to helpers and region
            SyncStyleToHelpers();
            UpdateLogoPainterTheme();
            ApplyAcrylicEffectIfNeeded();

            // Ensure padding reflects border thickness when not maximized
            if (WindowState != FormWindowState.Maximized)
                Padding = new Padding(Math.Max(0, _borderThickness));

            if (UseHelperInfrastructure && _regionHelper != null)
                _regionHelper.InvalidateRegion();
            else
                UpdateFormRegion();

            if (IsHandleCreated && _animateStyleChange)
                _ = AnimateOpacityAsync(0.8, 1.0, 200);

            Invalidate();
        }
        private void UpdateLogoPainterTheme() { if (_captionHelper != null && !string.IsNullOrEmpty(_captionHelper.LogoImagePath)) { try { _captionHelper.LogoImagePath = _captionHelper.LogoImagePath; } catch { } } }
        #endregion

        #region Maximize helpers
        public void ToggleMaximize() { if (InDesignHost) return; WindowState = WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized; }
        private void ApplyMaximizedWindowFix() { if (InDesignHost) return; if (WindowState == FormWindowState.Maximized) { _savedBorderRadius = _borderRadius; _savedBorderThickness = _borderThickness; _borderRadius = 0; _borderThickness = 0; Padding = new Padding(0); Region = null; } else { _borderRadius = _savedBorderRadius; _borderThickness = _savedBorderThickness; Padding = new Padding(Math.Max(0, _borderThickness)); } }
        #endregion

        #region Shapes/Regions
        private GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius) { GraphicsPath path = new GraphicsPath(); if (radius <= 0 || rect.Width <= 0 || rect.Height <= 0) { if (rect.Width > 0 && rect.Height > 0) path.AddRectangle(rect); return path; } int diameter = Math.Min(rect.Width, rect.Height); diameter = Math.Min(diameter, radius * 2); if (diameter <= 0) { path.AddRectangle(rect); return path; } try { Rectangle arcRect = new Rectangle(rect.X, rect.Y, diameter, diameter); path.AddArc(arcRect, 180, 90); arcRect.X = rect.Right - diameter; path.AddArc(arcRect, 270, 90); arcRect.Y = rect.Bottom - diameter; path.AddArc(arcRect, 0, 90); arcRect.X = rect.Left; path.AddArc(arcRect, 90, 90); path.CloseFigure(); } catch (ArgumentException) { path.Reset(); if (rect.Width > 0 && rect.Height > 0) path.AddRectangle(rect); } return path; }
        private void UpdateFormRegion() 
        { 
            if (InDesignHost || !IsHandleCreated) return; 
            
            if (WindowState == FormWindowState.Maximized || _borderRadius <= 0) 
            { 
                Region = null; 
                return; 
            } 
            
            if (ClientSize.Width <= 0 || ClientSize.Height <= 0) 
            { 
                Region = null; 
                return; 
            } 
            
            // Use the full client rectangle for the Region
            // The border will be painted INSIDE this region
            using var path = GetRoundedRectanglePath(
                new Rectangle(0, 0, ClientSize.Width, ClientSize.Height), 
                _borderRadius); 
            Region = new Region(path); 
        }
        #endregion

        #region Layout helpers
        public virtual void AdjustControls() { if (InDesignHost) return; Rectangle adjustedClientArea = GetAdjustedClientRectangle(); foreach (Control control in Controls) { if (control.Dock == DockStyle.Fill) { control.Bounds = adjustedClientArea; } else if (control.Dock == DockStyle.Top) { control.Bounds = new Rectangle(adjustedClientArea.Left, adjustedClientArea.Top, adjustedClientArea.Width, control.Height); adjustedClientArea.Y += control.Height; adjustedClientArea.Height -= control.Height; } else if (control.Dock == DockStyle.Bottom) { control.Bounds = new Rectangle(adjustedClientArea.Left, adjustedClientArea.Bottom - control.Height, adjustedClientArea.Width, control.Height); adjustedClientArea.Height -= control.Height; } else if (control.Dock == DockStyle.Left) { control.Bounds = new Rectangle(adjustedClientArea.Left, adjustedClientArea.Top, control.Width, adjustedClientArea.Height); adjustedClientArea.X += control.Width; adjustedClientArea.Width -= control.Width; } else if (control.Dock == DockStyle.Right) { control.Bounds = new Rectangle(adjustedClientArea.Right - control.Width, adjustedClientArea.Top, control.Width, adjustedClientArea.Height); adjustedClientArea.Width -= control.Width; } else { control.Left = Math.Max(control.Left, adjustedClientArea.Left); control.Top = Math.Max(control.Top, adjustedClientArea.Top); int maxWidth = adjustedClientArea.Right - control.Left; int maxHeight = adjustedClientArea.Bottom - control.Top; control.Width = Math.Min(control.Width, maxWidth); control.Height = Math.Min(control.Height, maxHeight); } } }
        public Rectangle GetAdjustedClientRectangle() { var extra = new Padding(0); ComputeExtraNonClientPadding(ref extra); int effectiveBorder = (Padding.Left >= _borderThickness && Padding.Right >= _borderThickness && Padding.Top >= _borderThickness && Padding.Bottom >= _borderThickness) ? 0 : _borderThickness; int adjustedWidth = Math.Max(0, ClientSize.Width - (2 * effectiveBorder) - extra.Left - extra.Right); int adjustedHeight = Math.Max(0, ClientSize.Height - (2 * effectiveBorder) - extra.Top - extra.Bottom); return new Rectangle(extra.Left + effectiveBorder, extra.Top + effectiveBorder, adjustedWidth, adjustedHeight); }
        // BeepiForm.cs
        public override Rectangle DisplayRectangle
        {
            get
            {
                var extra = new Padding(0);
                ComputeExtraNonClientPadding(ref extra); // adds CaptionHeight when ShowCaptionBar = true
                int effectiveBorder =
                    (Padding.Left >= _borderThickness && Padding.Right >= _borderThickness &&
                     Padding.Top >= _borderThickness && Padding.Bottom >= _borderThickness) ? 0 : _borderThickness;

                int adjustedWidth = Math.Max(0, ClientSize.Width - (effectiveBorder * 2) - extra.Left - extra.Right);
                int adjustedHeight = Math.Max(0, ClientSize.Height - (effectiveBorder * 2) - extra.Top - extra.Bottom);

                return new Rectangle(effectiveBorder + extra.Left,
                                     effectiveBorder + extra.Top,
                                     adjustedWidth,
                                     adjustedHeight);
            }
        }

        #endregion

        #region Paint Override
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (InDesignHost)
            {
                e.Graphics.Clear(BackColor);
                using var pen = new Pen(Color.Gray, 1) { Alignment = PenAlignment.Inset };
                e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, Width - 1, Height - 1));
                TextRenderer.DrawText(e.Graphics, Text, Font, new Point(8, 8), ForeColor);
                return;
            }
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            if (_inMoveOrResize) { g.Clear(BackColor); return; }
            if (UseHelperInfrastructure && _shadowGlow != null && _regionHelper != null && _overlayRegistry != null)
            {
                System.Diagnostics.Debug.WriteLine("[BeepiForm.OnPaint] Using helper infrastructure");
                var formPath = GetFormPath();
                using (formPath)
                {
                    if (WindowState != FormWindowState.Maximized)
                    {
                        _shadowGlow.PaintShadow(g, formPath);
                        _shadowGlow.PaintGlow(g, formPath);
                    }
                    using var backBrush = new SolidBrush(BackColor);
                    g.FillPath(backBrush, formPath);
                    
                    // *** Border painting moved to WM_NCPAINT (non-client area)
                    // Border is now painted in PaintNonClientBorder() via WndProc
                    // System.Diagnostics.Debug.WriteLine($"[BeepiForm.OnPaint] About to call _borderPainter.PaintBorder, _borderPainter is null? {_borderPainter == null}");
                    // _borderPainter?.PaintBorder(g, formPath);
                }
                _overlayRegistry.PaintOverlays(g);
            }
            else
            {
                PaintDirectly(g);
            }
          
        }
        private void PaintDirectly(Graphics g)
        {
            if (InDesignHost) return;
            var formPath = GetFormPath();
            using (formPath)
            {
                if (WindowState != FormWindowState.Maximized)
                {
                    if (_shadowDepth > 0)
                    {
                        using var shadowPath = (GraphicsPath)formPath.Clone();
                        using var shadowBrush = new SolidBrush(_shadowColor);
                        g.TranslateTransform(_shadowDepth, _shadowDepth);
                        g.FillPath(shadowBrush, shadowPath);
                        g.TranslateTransform(-_shadowDepth, -_shadowDepth);
                    }
                    if (_enableGlow && _glowSpread > 0f)
                    {
                        using var glowPen = new Pen(_glowColor, _glowSpread) { LineJoin = LineJoin.Round };
                        g.DrawPath(glowPen, formPath);
                    }
                }
                using var backBrush = new SolidBrush(BackColor);
                g.FillPath(backBrush, formPath);
                
                // *** Border painting moved to WM_NCPAINT (non-client area)
                // Border is now painted in PaintNonClientBorder() via WndProc
                // _borderPainter?.PaintBorder(g, formPath);
            }
        }
        private GraphicsPath GetFormPath()
        {
            var path = new GraphicsPath();
            var rect = new Rectangle(0, 0, ClientSize.Width, ClientSize.Height);
            if (rect.Width <= 0 || rect.Height <= 0) return path;
            
            if (_borderRadius > 0 && WindowState != FormWindowState.Maximized)
            {
                int diameter = Math.Min(_borderRadius * 2, Math.Min(rect.Width, rect.Height));
                if (diameter > 0)
                {
                    var arcRect = new Rectangle(rect.X, rect.Y, diameter, diameter);
                    path.AddArc(arcRect, 180, 90);
                    arcRect.X = rect.Right - diameter;
                    path.AddArc(arcRect, 270, 90);
                    arcRect.Y = rect.Bottom - diameter;
                    path.AddArc(arcRect, 0, 90);
                    arcRect.X = rect.Left;
                    path.AddArc(arcRect, 90, 90);
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
        protected override void OnSizeChanged(EventArgs e) { base.OnSizeChanged(e); if (InDesignHost) return; if (UseHelperInfrastructure && _regionHelper != null) _regionHelper.InvalidateRegion(); Invalidate(); }
        #endregion

        #region Redraw helpers / Mouse Aggregators
        public void BeginUpdate() { if (!InDesignHost) User32.SendMessage(Handle, User32.WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero); }
        public void EndUpdate() { if (!InDesignHost) { User32.SendMessage(this.Handle, User32.WM_SETREDRAW, new IntPtr(1), IntPtr.Zero); this.Refresh(); } }
        private readonly List<Action<MouseEventArgs>> _mouseMoveHandlers = new(); private readonly List<Action> _mouseLeaveHandlers = new(); private readonly List<Action<MouseEventArgs>> _mouseDownHandlers = new();
        protected void RegisterMouseMoveHandler(Action<MouseEventArgs> handler) { if (handler != null && !InDesignHost) _mouseMoveHandlers.Add(handler); }
        protected void RegisterMouseLeaveHandler(Action handler) { if (handler != null && !InDesignHost) _mouseLeaveHandlers.Add(handler); }
        protected void RegisterMouseDownHandler(Action<MouseEventArgs> handler) { if (handler != null && !InDesignHost) _mouseDownHandlers.Add(handler); }
        protected override void OnMouseMove(MouseEventArgs e) { if (_inpopupmode || InDesignHost) return; base.OnMouseMove(e); foreach (var h in _mouseMoveHandlers) h(e); }
        protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); if (InDesignHost) return; Cursor = Cursors.Default; foreach (var h in _mouseLeaveHandlers) h(); }
        protected override void OnMouseDown(MouseEventArgs e) { base.OnMouseDown(e); if (InDesignHost) return; foreach (var h in _mouseDownHandlers) h(e); }
        #endregion

        #region HitTest / WndProc
        // Windows Messages
        private const int WM_NCHITTEST = 0x84;
        private const int WM_NCCALCSIZE = 0x83;
        private const int WM_NCPAINT = 0x85;
        private const int WM_NCACTIVATE = 0x86;
        private const int WM_ENTERSIZEMOVE = 0x0231;
        private const int WM_EXITSIZEMOVE = 0x0232;
        private const int WM_GETMINMAXINFO = 0x0024;
        private const int WM_DPICHANGED = 0x02E0;
        
        // Toggle to completely disable any custom window border drawing/spacing
        private bool _drawCustomWindowBorder = false;
        public bool DrawCustomWindowBorder
        {
            get => _drawCustomWindowBorder;
            set
            {
                if (_drawCustomWindowBorder == value) return;
                _drawCustomWindowBorder = value;
                if (!InDesignHost && IsHandleCreated)
                {
                    // Force frame recalculation and repaint when toggled
                    SetWindowPos(this.Handle, IntPtr.Zero, 0, 0, 0, 0, (uint)(SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED));
                    RedrawWindow(this.Handle, IntPtr.Zero, IntPtr.Zero, RDW_FRAME | RDW_INVALIDATE | RDW_UPDATENOW);
                }
            }
        }
        
        // Hit Test Constants
        private const int HTCLIENT = 1;
        private const int HTCAPTION = 2;
        private const int HTLEFT = 10;
        private const int HTRIGHT = 11;
        private const int HTTOP = 12;
        private const int HTTOPLEFT = 13;
        private const int HTTOPRIGHT = 14;
        private const int HTBOTTOM = 15;
        private const int HTBOTTOMLEFT = 16;
        private const int HTBOTTOMRIGHT = 17;
        
        // Window Styles
        private const int WS_SIZEBOX = 0x00040000;
        private bool IsOverChildControl(Point clientPos) { var child = GetChildAtPoint(clientPos, GetChildAtPointSkip.Invisible | GetChildAtPointSkip.Transparent); return child != null; }
        private bool IsInDraggableArea(Point clientPos) { if (UseHelperInfrastructure && _captionHelper != null && _captionHelper.ShowCaptionBar) { if (clientPos.Y <= _captionHelper.CaptionHeight && !_captionHelper.IsPointInSystemButtons(clientPos) && !IsOverChildControl(clientPos)) return true; return false; } return clientPos.Y <= 36 && !IsOverChildControl(clientPos); }
        [DllImport("user32.dll")] private static extern uint GetDpiForWindow(IntPtr hWnd);
        
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                if (!InDesignHost)
                {
                    // Always add WS_SIZEBOX so Windows accepts HT* sizing codes on borderless forms
                    cp.Style |= WS_SIZEBOX;
                }
                return cp;
            }
        }
        
        protected override void WndProc(ref Message m)
        {
            if (InDesignHost) { base.WndProc(ref m); return; }
            switch (m.Msg)
            {
                case WM_NCCALCSIZE:
                    // Reserve space for custom border in non-client area (caption is in client area)
                    if (_drawCustomWindowBorder && m.WParam != IntPtr.Zero && WindowState != FormWindowState.Maximized)
                    {
                        NCCALCSIZE_PARAMS nccsp = (NCCALCSIZE_PARAMS)Marshal.PtrToStructure(m.LParam, typeof(NCCALCSIZE_PARAMS));
                        
                        int borderThickness = _borderThickness;
                        
                        // Shrink client rect to reserve space for border only (not caption)
                        nccsp.rgrc[0].top += borderThickness;
                        nccsp.rgrc[0].left += borderThickness;
                        nccsp.rgrc[0].right -= borderThickness;
                        nccsp.rgrc[0].bottom -= borderThickness;
                        
                        Marshal.StructureToPtr(nccsp, m.LParam, false);
                        m.Result = IntPtr.Zero;
                        return;
                    }
                    break;
                    
                case WM_NCACTIVATE:
                    // Prevent default title bar repaint by setting lParam to -1
                    if (_drawCustomWindowBorder && WindowState != FormWindowState.Maximized)
                    {
                        m.LParam = new IntPtr(-1);
                    }
                    break;
                    
                case WM_NCPAINT:
                    // Paint custom border in non-client area (caption bar painted in client area)
                    if (_drawCustomWindowBorder && WindowState != FormWindowState.Maximized)
                    {
                        PaintNonClientBorder();
                        m.Result = IntPtr.Zero;
                        return;
                    }
                    break;
                    
                case WM_DPICHANGED:
                    if (DpiMode == DpiHandlingMode.Manual)
                    {
                        var suggested = Marshal.PtrToStructure<RECT>(m.LParam);
                        var suggestedBounds = Rectangle.FromLTRB(suggested.left, suggested.top, suggested.right, suggested.bottom);
                        this.Bounds = suggestedBounds;
                        uint dpi = GetDpiForWindow(this.Handle);
                    }
                    break;
                case WM_ENTERSIZEMOVE: _inMoveOrResize = true; break;
                case WM_EXITSIZEMOVE: _inMoveOrResize = false; UpdateFormRegion(); Invalidate(); break;
                case WM_GETMINMAXINFO: AdjustMaximizedBounds(m.LParam); break;
                case WM_NCHITTEST when !_inpopupmode:
                    if (UseHelperInfrastructure && _hitTestHelper != null) { if (_hitTestHelper.HandleNcHitTest(ref m)) return; }
                    else
                    {
                        Point pos = PointToClient(new Point(m.LParam.ToInt32())); int margin = _resizeMargin;
                        if (pos.X <= margin && pos.Y <= margin) { m.Result = (IntPtr)HTTOPLEFT; return; }
                        if (pos.X >= ClientSize.Width - margin && pos.Y <= margin) { m.Result = (IntPtr)HTTOPRIGHT; return; }
                        if (pos.X <= margin && pos.Y >= ClientSize.Height - margin) { m.Result = (IntPtr)HTBOTTOMLEFT; return; }
                        if (pos.X >= ClientSize.Width - margin && pos.Y >= ClientSize.Height - margin) { m.Result = (IntPtr)HTBOTTOMRIGHT; return; }
                        if (pos.X <= margin) { m.Result = (IntPtr)HTLEFT; return; }
                        if (pos.X >= ClientSize.Width - margin) { m.Result = (IntPtr)HTRIGHT; return; }
                        if (pos.Y <= margin) { m.Result = (IntPtr)HTTOP; return; }
                        if (pos.Y >= ClientSize.Height - margin) { m.Result = (IntPtr)HTBOTTOM; return; }
                        if (IsOverChildControl(pos)) { m.Result = (IntPtr)HTCLIENT; return; }
                        m.Result = IsInDraggableArea(pos) ? (IntPtr)HTCAPTION : (IntPtr)HTCLIENT; return;
                    }
                    break;
            }
            base.WndProc(ref m);
        }
        [StructLayout(LayoutKind.Sequential)] private struct POINT { public int x; public int y; }
        [StructLayout(LayoutKind.Sequential)] private struct MINMAXINFO { public POINT ptReserved; public POINT ptMaxSize; public POINT ptMaxPosition; public POINT ptMinTrackSize; public POINT ptMaxTrackSize; }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)] private struct MONITORINFO { public int cbSize; public RECT rcMonitor; public RECT rcWork; public int dwFlags; }
        [StructLayout(LayoutKind.Sequential)] private struct RECT { public int left; public int top; public int right; public int bottom; }
        
        [StructLayout(LayoutKind.Sequential)]
        private struct NCCALCSIZE_PARAMS
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public RECT[] rgrc;
            public IntPtr lppos;
        }
        [DllImport("user32.dll")] private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);
        [DllImport("user32.dll")] private static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);
        [DllImport("user32.dll")] private static extern IntPtr GetWindowDC(IntPtr hWnd);
        [DllImport("user32.dll")] private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("user32.dll")] private static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, uint flags);
        [DllImport("user32.dll")] private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        private const int MONITOR_DEFAULTTONEAREST = 2;
        private const uint RDW_FRAME = 0x0400;
        private const uint RDW_INVALIDATE = 0x0001;
        private const uint RDW_UPDATENOW = 0x0100;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOZORDER = 0x0004;
        private const uint SWP_FRAMECHANGED = 0x0020;
        private void AdjustMaximizedBounds(IntPtr lParam) { MINMAXINFO mmi = Marshal.PtrToStructure<MINMAXINFO>(lParam); IntPtr monitor = MonitorFromWindow(this.Handle, MONITOR_DEFAULTTONEAREST); if (monitor != IntPtr.Zero) { MONITORINFO monitorInfo = new MONITORINFO(); monitorInfo.cbSize = Marshal.SizeOf(typeof(MONITORINFO)); GetMonitorInfo(monitor, ref monitorInfo); Rectangle rcWorkArea = Rectangle.FromLTRB(monitorInfo.rcWork.left, monitorInfo.rcWork.top, monitorInfo.rcWork.right, monitorInfo.rcWork.bottom); Rectangle rcMonitorArea = Rectangle.FromLTRB(monitorInfo.rcMonitor.left, monitorInfo.rcMonitor.top, monitorInfo.rcMonitor.right, monitorInfo.rcMonitor.bottom); mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.Left - rcMonitorArea.Left); mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.Top - rcMonitorArea.Top); mmi.ptMaxSize.x = rcWorkArea.Width; mmi.ptMaxSize.y = rcWorkArea.Height; Marshal.StructureToPtr(mmi, lParam, true); } }
        
        private void PaintNonClientBorder()
        {
            if (InDesignHost || !IsHandleCreated || _borderPainter == null || !_drawCustomWindowBorder)
                return;
                
            IntPtr hdc = GetWindowDC(this.Handle);
            if (hdc == IntPtr.Zero)
                return;
                
            try
            {
                using (Graphics g = Graphics.FromHdc(hdc))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    // Delegate border painting to helper (single source of truth)
                    Rectangle windowRect = new Rectangle(0, 0, Width, Height);
                    _borderPainter.PaintWindowBorder(g, windowRect, _borderRadius, _borderThickness);
                }
            }
            finally
            {
                ReleaseDC(this.Handle, hdc);
            }
        }
        #endregion

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // BeepiForm
            // 
            ClientSize = new Size(617, 261);
            Name = "BeepiForm";
            ResumeLayout(false);

        }
    }
}
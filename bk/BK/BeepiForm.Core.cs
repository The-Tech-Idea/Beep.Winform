using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Forms.Helpers;
using TheTechIdea.Beep.Winform.Controls.Managers;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepiForm.Core.cs - Fields, Constructor, and Helper Infrastructure
    /// </summary>
    public partial class BeepiForm
    {
        #region Helper Infrastructure
        private delegate void PaddingAdjuster(ref Padding padding);
        private readonly List<PaddingAdjuster> _paddingProviders = new();
        private void RegisterPaddingProvider(PaddingAdjuster provider) { if (provider != null) _paddingProviders.Add(provider); }
        internal void ComputeExtraNonClientPadding(ref Padding padding) { foreach (var p in _paddingProviders) p(ref padding); }
        private void RegisterOverlayPainter(Action<Graphics> painter) { _overlayRegistry?.Add(painter); }
        #endregion

        #region Design Mode Detection
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
        
        internal bool InDesignHost => false;
        #endregion

        #region Fields
        // UI Manager
        public BeepFormUIManager beepuiManager1;
        
        // Helpers
        private FormStateStore _state;
        private FormRegionHelper _regionHelper;
        private FormLayoutHelper _layoutHelper;
        private FormShadowGlowPainter _shadowGlow;
        private FormOverlayPainterRegistry _overlayRegistry;
        private FormThemeHelper _themeHelper;
        private FormHitTestHelper _hitTestHelper;
        private FormCaptionBarHelper _captionHelper;
        private FormBorderPainter _borderPainter;
        
        // Geometry cache
        private GraphicsPath? _cachedClientPath;
        private GraphicsPath? _cachedWindowPath;
        internal bool _pathsDirty = true;
        
        // Core state fields
        protected int _resizeMargin = 8;
        protected int _borderRadius = 8;
        protected int _borderThickness = 3;
        private Color _borderColor = Color.Red;
        private bool _inpopupmode = false;
        private string _title = "BeepiForm";
        internal bool _inMoveOrResize = false;
        protected IBeepTheme? _currentTheme;
        private bool _applythemetochilds = true;
        private int _savedBorderRadius = 0;
        private int _savedBorderThickness = 3;
        
        // Style fields
        internal BeepFormStyle _formStyle = BeepFormStyle.Modern;
        internal Color _shadowColor = Color.FromArgb(50, 0, 0, 0);
    internal bool _enableGlow = false;
    internal Color _glowColor = Color.Transparent;
        internal float _glowSpread = 8f;
        internal int _shadowDepth = 6;
        
        // Caption legacy state
        private bool _legacyShowCaptionBar = true;
        private bool _legacyShowSystemButtons = true;
        private bool _legacyEnableCaptionGradient = true;
        private int _legacyCaptionHeight = 36;
        
        // Control style
        private bool _useThemeColors = true;
        private BeepControlStyle _controlstyle = BeepControlStyle.Material3;
        
        // Backdrop/effects
        private bool _enableAcrylicForGlass = true;
        private bool _enableMicaBackdrop = false;
        private BackdropType _backdrop = BackdropType.None;
        private bool _useImmersiveDarkMode = false;
        
        // Animations
        private bool _animateMaximizeRestore = true;
        private bool _animateStyleChange = true;
        
        // Snap hints
        private bool _showSnapHints = true;
        private Rectangle _snapLeft, _snapRight, _snapTop;
        private bool _showSnapOverlay;
        
        // Border control
        internal bool _drawCustomWindowBorder = true;
        
        // Theme
        private string _theme = string.Empty;
        
        // Mouse handlers
        private readonly List<Action<MouseEventArgs>> _mouseMoveHandlers = new();
        private readonly List<Action> _mouseLeaveHandlers = new();
        private readonly List<Action<MouseEventArgs>> _mouseDownHandlers = new();
        #endregion

        #region Constructor
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
                SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | 
                         ControlStyles.AllPaintingInWmPaint | ControlStyles.SupportsTransparentBackColor, true);
                UpdateStyles();
                DoubleBuffered = true;
                BackColor = SystemColors.Control;
                FormBorderStyle = FormBorderStyle.None;

                if (!InDesignHost)
                {
                    _currentTheme = BeepThemesManager.GetDefaultTheme();
                }

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
                    _borderPainter = new FormBorderPainter(this);
                    _captionHelper = new FormCaptionBarHelper(this, _overlayRegistry, 
                        padAdj => RegisterPaddingProvider((ref Padding p) => padAdj(ref p)));

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

        #region Helper Methods
        protected void RegisterMouseMoveHandler(Action<MouseEventArgs> handler) 
        { 
            if (handler != null && !InDesignHost) _mouseMoveHandlers.Add(handler); 
        }
        
        protected void RegisterMouseLeaveHandler(Action handler) 
        { 
            if (handler != null && !InDesignHost) _mouseLeaveHandlers.Add(handler); 
        }
        
        protected void RegisterMouseDownHandler(Action<MouseEventArgs> handler) 
        { 
            if (handler != null && !InDesignHost) _mouseDownHandlers.Add(handler); 
        }
        
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

        #region Overlay/Snap Hints Handlers
        // Minimal stub implementations to satisfy registrations; can be enhanced later
        private void SnapHints_OnMouseMove(MouseEventArgs e)
        {
            if (!_showSnapHints || InDesignHost) return;
            // Placeholder logic: no-op for now
        }

        private void SnapHints_OnMouseLeave()
        {
            if (!_showSnapHints || InDesignHost) return;
            // Placeholder logic: no-op for now
        }

        private void SnapHints_OnPaintOverlay(Graphics g)
        {
            if (!_showSnapHints || InDesignHost) return;
            // Placeholder overlay painter (no visual until fully implemented)
        }
        #endregion

        #region IBeepModernFormHost Implementation
        Form IBeepModernFormHost.AsForm => this;
        IBeepTheme IBeepModernFormHost.CurrentTheme => _currentTheme ?? BeepThemesManager.GetDefaultTheme();
        bool IBeepModernFormHost.IsInDesignMode => InDesignHost;
        void IBeepModernFormHost.UpdateRegion() => _regionHelper?.EnsureRegion(true);
        #endregion

        // Referenced by properties; keeps Win11 tweaks centralized
        private void EnableModernWindowFeatures()
        {
            if (InDesignHost || !IsHandleCreated) return;
            try
            {
                // At minimum, trigger a non-client refresh so caption/system metrics are reapplied
                TheTechIdea.Beep.Winform.Controls.Helpers.User32.RedrawWindow(
                    Handle, IntPtr.Zero, IntPtr.Zero,
                    TheTechIdea.Beep.Winform.Controls.Helpers.User32.RDW_FRAME |
                    TheTechIdea.Beep.Winform.Controls.Helpers.User32.RDW_INVALIDATE |
                    TheTechIdea.Beep.Winform.Controls.Helpers.User32.RDW_UPDATENOW);
            }
            catch { }
        }

        #region Helper Access Properties
        public bool UseHelperInfrastructure { get; set; } = true;
        #endregion
    }
}

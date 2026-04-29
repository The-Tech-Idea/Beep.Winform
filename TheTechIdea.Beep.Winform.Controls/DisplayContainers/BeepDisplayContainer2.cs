using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.DisplayContainers.Helpers;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;


namespace TheTechIdea.Beep.Winform.Controls.DisplayContainers
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Display Container 2")]
    [Description("A modern, self-contained display container for addins with native rendering and advanced features.")]
    public partial class BeepDisplayContainer2 : BaseControl, IDisplayContainer
    {
        #region Fields

        private readonly List<AddinTab> _tabs = new();
        private readonly Dictionary<string, IDM_Addin> _addins = new();
        private AddinTab? _activeTab;
        private AddinTab? _hoveredTab;
        private ContainerTypeEnum _containerType = ContainerTypeEnum.TabbedPanel;
        private ContainerDisplayMode _displayMode = ContainerDisplayMode.Tabbed;
        private TabPosition _tabPosition = TabPosition.Top;
        private bool _showCloseButtons = true;
        private bool _allowTabReordering = true;
        private bool _enableAnimations = true;
        private AnimationSpeed _animationSpeed = AnimationSpeed.Normal;
        private System.Windows.Forms.Timer? _animationTimer;
        private Rectangle _tabArea;
        private Rectangle _contentArea;
        private int _tabHeight = 36;
        private int _tabMinWidth = 80;
        private int _tabMaxWidth = 200;
        private int _scrollOffset = 0;
        private bool _needsScrolling = false;
        private bool _keyboardFocusActive = false;
        private bool _emptyStateButtonHovered = false;
        private Rectangle _scrollLeftButton;
        private Rectangle _scrollRightButton;
        private Rectangle _overflowButton;
        private Rectangle _newTabButton;

        // Helper classes
        private TabPaintHelper? _paintHelper;
        private TabLayoutHelper? _layoutHelper;
        private TabAnimationHelper? _animationHelper;

        // Theme colors
        private Color _tabBackColor;
        private Color _tabForeColor;
        private Color _activeTabBackColor;
        private Color _activeTabForeColor;
        private Color _hoverTabBackColor;
        private Color _borderColor;
        private Color _contentBackColor;

        // Tab transition animation
        private AddinTab? _previousTab;
        
        // Batch update mode to prevent flickering when adding multiple tabs
        private bool _batchMode = false;
        private int _batchUpdateDepth = 0;

        // When true, tab strip height (or width for vertical) is derived from the
        // actual font + padding metrics rather than the fixed _tabHeight value.
        private bool _autoTabHeight = true;

        // Scroll / utility button hover & press tracking.
        // Values: 0 = none, 1 = scroll-left/up, 2 = scroll-right/down, 3 = new-tab button.
        private int _hoveredScrollButton = 0;
        private int _pressedScrollButton = 0;

        // Sliding indicator transition: lerps the active-tab accent bar from the
        // previously-active tab to the newly-active one over ~INDICATOR_ANIM_STEPS ticks.
        private Rectangle _indicatorFrom  = Rectangle.Empty;
        private Rectangle _indicatorTo    = Rectangle.Empty;
        private float     _indicatorProgress = 1f; // 1 = settled; 0..1 = animating

        // Empty-state placeholder shown when no tabs are open.
        private string _emptyStateText = "No tabs open.\nClick + to add a new tab.";
        private string _emptyStateIcon = "tab_placeholder"; // symbolic name — rendered as a simple drawn icon
        private bool   _showEmptyState = true;

        // ---- Enhancement 4: Keyboard navigation ----
        private bool _enableKeyboardNav = true;

        // ---- Enhancement 5: Drag-to-reorder tabs ----
        // Dragging starts after the cursor moves more than _dragThreshold pixels.
        private AddinTab? _dragTab         = null;
        private Point     _dragStartPoint  = Point.Empty;
        private Point     _dragGhostLoc    = Point.Empty;
        private bool      _isDragging      = false;
        private int       _dropInsertIndex = -1;
        private const int _dragThreshold   = 5;

        // ---- Enhancement 6: Gradient tab strip background ----
        private bool  _useTabStripGradient     = false;
        // Color.Empty = auto-derive a slightly darker shade from _tabBackColor.
        private Color _tabStripGradientEndColor = Color.Empty;

        // ---- Tooltip hover card ----
        private System.Windows.Forms.Timer? _tooltipTimer;
        private AddinTab? _tooltipTab;   // tab whose tooltip is currently visible
        private bool _showTooltip = false;

        // Drop indicator pulse animation (0..1 sine wave for visual emphasis)
        private float _dropIndicatorPulse = 0f;

        // ---- Enhancement: Ctrl+P tab quick switcher overlay ----
        private bool _showQuickSwitcher = false;
        private string _quickSwitcherFilter = string.Empty;
        private int _quickSwitcherSelectedIndex = 0;
        private List<AddinTab> _quickSwitcherFilteredTabs = new();
        private Rectangle _quickSwitcherBounds = Rectangle.Empty;

        private BeepControlStyle _lastControlStyle = BeepControlStyle.None;

        // Tab group colour palette — auto-assigned colours for VS Code-style groups.
        private readonly Dictionary<string, Color> _tabGroupColors = new(StringComparer.OrdinalIgnoreCase);
        private static readonly Color[] _defaultGroupPalette = new[]
        {
            Color.FromArgb(0, 120, 212),  // Blue
            Color.FromArgb(218, 92, 42),   // Orange
            Color.FromArgb(80, 180, 80),   // Green
            Color.FromArgb(180, 80, 180),  // Purple
            Color.FromArgb(220, 160, 30),  // Gold
            Color.FromArgb(200, 60, 60),   // Red
            Color.FromArgb(60, 180, 180),  // Teal
            Color.FromArgb(160, 120, 200), // Lavender
        };
        private int _nextGroupColorIndex = 0;

        /// <summary>
        /// Gets or sets the visual style of the control.
        /// Shadows the base property to trigger tab style updates.
        /// </summary>
        public new BeepControlStyle ControlStyle
        {
            get => base.ControlStyle;
            set
            {
                base.ControlStyle = value;
                if (_lastControlStyle != value)
                {
                    UpdateTabStyleFromControlStyle();
                    _lastControlStyle = value;
                    // Update helpers immediately — do NOT rely on PropertyChanged which fires late.
                    UpdateTabPainterStyle();
                    ApplyThemeColorsToTabs();
                    
                    // Crucial: border/shadow bounds changed, recompute areas
                    RecalculateLayout();
                    
                    // Propagation happens inside ApplyTheme() (called by the base class after the
                    // setter) with a fully-refreshed _currentTheme.  Calling it here would push the
                    // OLD theme to addins because _currentTheme has not been updated yet for the
                    // new style — that is exactly what caused the double-click-to-update bug.
                    // PropagateThemeToAddins() is therefore intentionally NOT called here.
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Occurs when an addin is added to the container.
        /// </summary>
        public event EventHandler<ContainerEvents>? AddinAdded;
        /// <summary>
        /// Occurs when an addin is removed from the container.
        /// </summary>
        public event EventHandler<ContainerEvents>? AddinRemoved;
        /// <summary>
        /// Occurs when an addin is moved within the container.
        /// </summary>
        public event EventHandler<ContainerEvents>? AddinMoved;
        /// <summary>
        /// Occurs when the active addin is changing.
        /// </summary>
        public event EventHandler<ContainerEvents>? AddinChanging;
        /// <summary>
        /// Occurs when the active addin has changed.
        /// </summary>
        public event EventHandler<ContainerEvents>? AddinChanged;
        /// <summary>
        /// Occurs before a module is called.
        /// </summary>
        public event EventHandler<IPassedArgs>? PreCallModule;
        /// <summary>
        /// Occurs before an item is shown.
        /// </summary>
        public event EventHandler<IPassedArgs>? PreShowItem;
        /// <summary>
        /// Occurs when a key combination is pressed.
        /// </summary>
        public event EventHandler<KeyCombination>? KeyPressed;
        /// <summary>
        /// Occurs when the tab-strip requests creating a new tab.
        /// </summary>
        public event EventHandler<ContainerEvents>? NewTabRequested;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BeepDisplayContainer2"/> class.
        /// </summary>
        public BeepDisplayContainer2():base()
        {
            InitializeComponent();
            InitializeContainer();
        }

        private void InitializeComponent()
        {
            // Prevent hosted UserControls with AutoScaleMode.Font from triggering
            // PerformAutoScale() cascades that reset their Bounds and cover the tab header.
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;

            // Follow BaseControl patterns for painting setup
            SetStyle(ControlStyles.SupportsTransparentBackColor | 
                     ControlStyles.ResizeRedraw | 
                     ControlStyles.UserPaint | 
                     ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.Selectable, true);  // P4: allow keyboard focus
            
            DoubleBuffered = true;
            UseExternalBufferedGraphics = true;
            TabStop = true; // P4: allow Tab-key focus cycling
            
            // Enable high-quality rendering like BaseControl
            EnableHighQualityRendering = true;
            
            // Use form style paint for modern appearance (like BeepMenuBar)
            UseFormStylePaint = false; // Container paints its own background, tabs use BeepStyling
            
            // Set modern defaults for better appearance
            IsRounded = true;
            BorderRadius = 8;
            BorderThickness = 1;
            
            // Set default ControlStyle for tabs (can be overridden by user)
            if (ControlStyle == BeepControlStyle.None)
            {
                ControlStyle = BeepControlStyle.Modern;
            }
            
            // Disable BaseControl splash/ripple effects for this container
            EnableSplashEffect = false;
            // Transparent background support (like BeepMenuBar)
            IsTransparentBackground = false; // Default to opaque for containers
            
            // Set initial BackColor from theme
            //if (IsTransparentBackground)
            //{
            //    base.BackColor = Color.Transparent;
            //}
            //else
            //{
            //    base.BackColor = Color.White;
            //}

            // Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            // Ensure control is visible and will paint
            Visible = true;
        }

        private void InitializeContainer()
        {
            // Get control style from FormStyle for modern appearance
            var controlStyle = ControlStyle;
            
            // Initialize helpers with modern styling
            _paintHelper = new TabPaintHelper(_currentTheme, controlStyle, IsTransparentBackground) { OwnerControl = this };
            _layoutHelper = new TabLayoutHelper { OwnerControl = this };
            _animationHelper = new TabAnimationHelper(() => Invalidate());
            
            _animationTimer = new System.Windows.Forms.Timer { Interval = 16 }; // 60 FPS
            _animationTimer.Tick += AnimationTimer_Tick;

            // Do NOT start the timer here — it is started on-demand by StartAnimation()
            // and self-stops when no animations are active.  This eliminates constant
            // drawing/CPU usage when the container is idle.

            // Initialize theme colors (follows BaseControl pattern)
            ApplyThemeColorsToTabs();

            // Calculate initial layout
            RecalculateLayout();
            
            // Hook into handle creation to force initial paint
            HandleCreated += (s, e) => Invalidate(true);
            
            // Hook into visible changed to force paint when control becomes visible
            VisibleChanged += (s, e) =>
            {
                if (Visible)
                {
                    Invalidate(true);
                }
            };
            
            // Subscribe to ControlStyle changes to update tab appearance
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ControlStyle))
                {
                    UpdateTabPainterStyle();
                    Invalidate();
                }
            };
        }
        
        /// <summary>
        /// Updates the tab painter and layout when ControlStyle changes
        /// </summary>
        private void UpdateTabPainterStyle()
        {
            if (_paintHelper != null)
            {
                _paintHelper.ControlStyle = ControlStyle;
                _paintHelper.IsTransparent = IsTransparentBackground;
            }
            
            // Update layout helper with new style and font for proper tab sizing
            if (_layoutHelper != null)
            {
                _layoutHelper.UpdateStyle(ControlStyle, TextFont);
            }
            
            // Recalculate layout with new metrics
            RecalculateLayout();
        }

        private void UpdateTabStyleFromControlStyle()
        {
            switch (ControlStyle)
            {
                case BeepControlStyle.FigmaCard:
                case BeepControlStyle.TailwindCard:
                case BeepControlStyle.DiscordStyle:
                case BeepControlStyle.StripeDashboard:
                case BeepControlStyle.NeoBrutalist:
                    TabStyle = TabStyle.Card;
                    break;
                case BeepControlStyle.Minimal:
                case BeepControlStyle.NotionMinimal:
                case BeepControlStyle.VercelClean:
                    TabStyle = TabStyle.Minimal;
                    break;
                case BeepControlStyle.GlassAcrylic:
                case BeepControlStyle.Windows11Mica:
                case BeepControlStyle.Fluent2:
                case BeepControlStyle.Material3:
                    TabStyle = TabStyle.Underline;
                    break;
                case BeepControlStyle.PillRail:
                case BeepControlStyle.ChakraUI:
                    TabStyle = TabStyle.Capsule;
                    break;
                case BeepControlStyle.iOS15:
                case BeepControlStyle.MacOSBigSur:
                    TabStyle = TabStyle.Segmented;
                    break;
                case BeepControlStyle.Bootstrap:
                case BeepControlStyle.AntDesign:
                case BeepControlStyle.Neumorphism:
                    TabStyle = TabStyle.Button;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Applies the current theme to the container, tabs, and all hosted addin controls.
        /// Does NOT call base.ApplyTheme() — the base implementation propagates a stale
        /// snapshot to child controls before helpers are updated, forcing the double-set
        /// workaround.  We replicate the needed base assignments here then do our own
        /// full propagation to hosted addins.
        /// </summary>
        public override void ApplyTheme()
        {
            // --- 1. Ensure _currentTheme is current ---
            // _currentTheme is set by the Theme property setter before ApplyTheme() is called,
            // but guard defensively for direct calls (e.g. from InitializeContainer).
            if (_currentTheme == null)
            {
                try
                {
                    _currentTheme = BeepThemesManager.GetTheme(BeepThemesManager.CurrentThemeName)
                                    ?? BeepThemesManager.GetDefaultTheme();
                }
                catch
                {
                    _currentTheme = BeepThemesManager.GetDefaultTheme();
                }
            }

            if (_currentTheme == null)
            {
                ApplyFallbackColors();
                Invalidate();
                return;
            }

            // --- 2. Replicate base BaseControl.ApplyTheme() property assignments ---
            // (avoids calling base which propagates old snapshot to child controls)
            try
            {
                ForeColor               = _currentTheme.ForeColor;
                BorderColor             = _currentTheme.BorderColor;
                ShadowColor             = _currentTheme.ShadowColor;
                GradientStartColor      = _currentTheme.GradientStartColor;
                GradientEndColor        = _currentTheme.GradientEndColor;
                BadgeForeColor          = _currentTheme.BadgeForeColor;
                BadgeBackColor          = _currentTheme.BadgeBackColor;
                DisabledForeColor       = _currentTheme.DisabledForeColor;
                DisabledBackColor       = _currentTheme.DisabledBackColor;
                DisabledBorderColor     = _currentTheme.DisabledBorderColor;
                UpdateTooltipTheme();
            }
            catch { /* non-fatal — continue with tab-specific updates */ }

            // --- 3. Update tab helpers with latest theme + style ---
            var controlStyle = ControlStyle;
            if (_paintHelper == null)
            {
                _paintHelper = new TabPaintHelper(_currentTheme, controlStyle, IsTransparentBackground) { OwnerControl = this };
            }
            else
            {
                _paintHelper.Theme         = _currentTheme;
                _paintHelper.ControlStyle  = controlStyle;
                _paintHelper.IsTransparent = IsTransparentBackground;
            }
            _paintHelper.TabStyle = TabStyle;

            // --- 4. Apply theme colors to tab strip ---
            ApplyThemeColorsToTabs();

            // --- 5. Set TextFont from theme.TabFont (authoritative tab typography) ---
            // Uses the same BeepThemesManager.ToFont converter as DrawTab() so layout
            // measurements and rendering are always consistent.
            var tabTypography = _currentTheme.TabFont ?? _currentTheme.LabelFont;
            if (tabTypography != null)
            {
                try
                {
                    var tabFont = FontListHelper.GetFont(tabTypography.FontFamily, tabTypography.FontSize, tabTypography.FontStyle);
                    if (tabFont != null)
                        TextFont = tabFont;
                }
                catch { /* keep existing font on error */ }
            }

            // --- 6. Sync layout helper + recalculate ---
            if (_layoutHelper != null)
                _layoutHelper.UpdateStyle(controlStyle, TextFont);
            RecalculateLayout();

            // --- 7. Set container background ---
            if (IsTransparentBackground)
            {
                base.BackColor = Color.Transparent;
            }
            else if (IsChild && Parent != null)
            {
                base.BackColor = Parent.BackColor;
            }
            else
            {
                base.BackColor = _contentBackColor;
            }

            // --- 8. Propagate theme to every hosted addin ---
            // This eliminates the need to set FormStyle/ControlStyle twice.
            PropagateThemeToAddins();

            // --- 9. Repaint ---
            Invalidate();
        }

        /// <summary>
        /// Pushes the current theme and control style to all hosted addin controls.
        /// Called from ApplyTheme() and from ControlStyle setter so that a single
        /// assignment is always sufficient.
        /// </summary>
        private void PropagateThemeToAddins()
        {
            if (_currentTheme == null) return;
            var targetStyle = ControlStyle;
            foreach (var addin in _addins.Values)
            {
                try
                {
                    if (addin is Control ctrl)
                    {
                        PropagateThemeToControlTree(ctrl, _currentTheme, targetStyle);
                    }
                }
                catch { /* don't let a single bad addin break the whole update */ }
            }
        }

        private void PropagateThemeToControlTree(Control control, IBeepTheme theme, BeepControlStyle targetStyle)
        {
            if (control.IsDisposed) return;

            if (control is IBeepUIComponent uiComponent)
            {
                if (control is BaseControl bc && bc.ControlStyle != targetStyle)
                    bc.ControlStyle = targetStyle;

                uiComponent.ApplyTheme(theme);
            }

            foreach (Control child in control.Controls)
            {
                PropagateThemeToControlTree(child, theme, targetStyle);
            }
        }

        #endregion

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
        }

        #region Helper Methods

        private AddinTab? GetTabAt(Point point)
        {
            return _layoutHelper?.GetTabAt(_tabs, point);
        }

        private Rectangle GetCloseButtonRect(Rectangle tabBounds)
        {
            return _layoutHelper?.GetCloseButtonRect(tabBounds) ?? Rectangle.Empty;
        }

        private void StartAnimation(AddinTab tab, float targetProgress)
        {
            if (!_enableAnimations || _animationHelper == null || tab == null) return;

            tab.TargetAnimationProgress = Math.Max(0, Math.Min(1, targetProgress));
            _animationHelper.StartAnimation(tab, targetProgress);

            // Restart the timer if it was self-stopped — ensures hover animations run.
            EnsureAnimationTimerRunning();
        }

        private void EnsureAnimationTimerRunning()
        {
            if (_animationTimer != null && !_animationTimer.Enabled && _enableAnimations)
            {
                _animationTimer.Start();
            }
        }

        private void AnimationTimer_Tick(object? sender, EventArgs e)
        {
            bool needsInvalidate = false;
            bool anyAnimationActive = false;

            if (_animationHelper != null)
            {
                // Update hover animations for all tabs.
                _animationHelper.UpdateAnimations(_tabs, _animationSpeed);

                // Check if any tab still has a pending hover animation.
                foreach (var tab in _tabs)
                {
                    if (Math.Abs(tab.AnimationProgress - tab.TargetAnimationProgress) > 0.01f)
                    {
                        anyAnimationActive = true;
                        break;
                    }
                }
            }

            // Advance the sliding-indicator animation.
            if (_indicatorProgress < 1f)
            {
                const float baseStep = 0.09f;
                float remaining = 1f - _indicatorProgress;
                float step = Math.Max(0.015f, baseStep * remaining / 0.5f + 0.01f);
                _indicatorProgress = Math.Min(1f, _indicatorProgress + step);
                needsInvalidate = true;
                anyAnimationActive = true;
            }

            // Advance drop indicator pulse while dragging
            if (_isDragging && _dropInsertIndex >= 0)
            {
                _dropIndicatorPulse += 0.08f;
                if (_dropIndicatorPulse > MathF.Tau) _dropIndicatorPulse -= MathF.Tau;
                needsInvalidate = true;
                anyAnimationActive = true;
            }
            else if (_dropIndicatorPulse != 0f)
            {
                _dropIndicatorPulse = 0f;
            }

            if (needsInvalidate)
            {
                if (!_tabArea.IsEmpty)
                    Invalidate(_tabArea);
                else
                    Invalidate();
            }

            // Self-stop the timer when no animations are active — eliminates constant drawing.
            if (!anyAnimationActive && _animationTimer != null && _animationTimer.Enabled)
            {
                _animationTimer.Stop();
            }
        }
        
        /// <summary>
        /// Begin batch mode to add multiple tabs without flickering.
        /// Call EndUpdate() when done to trigger a single repaint.
        /// </summary>
        public void BeginUpdate()
        {
            _batchUpdateDepth++;
            if (_batchUpdateDepth == 1)
            {
                _batchMode = true;
                // Suspend layout to prevent multiple recalculations
                SuspendLayout();
            }
        }
        
        /// <summary>
        /// End batch mode and trigger a single repaint for all changes.
        /// </summary>
        public void EndUpdate()
        {
            _batchUpdateDepth = Math.Max(0, _batchUpdateDepth - 1);
            if (_batchUpdateDepth == 0)
            {
                _batchMode = false;
                // Resume layout and perform a single recalculation
                ResumeLayout(true);
                // Single invalidation for all changes
                if (IsHandleCreated)
                {
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Apply the style preset to this container
        /// </summary>
        public void SetTabStylePreset(TheTechIdea.Beep.Winform.Controls.TabStyle style)
        {
            TheTechIdea.Beep.Winform.Controls.Styling.TabStylePresets.ApplyPreset(this, style);
        }

        /// <summary>
        /// Gets or auto-assigns a colour for the named tab group.
        /// </summary>
        public Color GetGroupColor(string? groupName)
        {
            if (string.IsNullOrEmpty(groupName))
                return Color.Empty;

            if (_tabGroupColors.TryGetValue(groupName, out var color))
                return color;

            color = _defaultGroupPalette[_nextGroupColorIndex % _defaultGroupPalette.Length];
            _nextGroupColorIndex++;
            _tabGroupColors[groupName] = color;
            return color;
        }

        /// <summary>
        /// Assigns a tab to a named group and returns the group accent colour.
        /// </summary>
        public Color AssignTabToGroup(AddinTab tab, string? groupName)
        {
            tab.TabGroup = groupName;
            return GetGroupColor(groupName);
        }

        /// <summary>
        /// Removes a tab from its group.
        /// </summary>
        public void RemoveTabFromGroup(AddinTab tab)
        {
            tab.TabGroup = null;
        }

        /// <summary>
        /// Clears all group colour assignments.
        /// </summary>
        public void ClearGroupColors()
        {
            _tabGroupColors.Clear();
            _nextGroupColorIndex = 0;
            foreach (var tab in _tabs)
                tab.TabGroup = null;
            Invalidate();
        }

        #endregion
    }
}

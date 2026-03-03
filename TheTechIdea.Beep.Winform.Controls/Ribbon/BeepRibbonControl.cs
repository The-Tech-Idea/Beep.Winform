using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Text.Json;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Accessibility;
using TheTechIdea.Beep.Winform.Controls.Backstage;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Customization;
using TheTechIdea.Beep.Winform.Controls.Gallery;
using TheTechIdea.Beep.Winform.Controls.Rendering;
using TheTechIdea.Beep.Winform.Controls.Search;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Winform.Controls.Tokens;
using TheTechIdea.Beep.Winform.Controls.Tooltips;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepRibbonControl : Control
    {
        private readonly TabControl _tabs = new() { Dock = DockStyle.Fill };
        private readonly ToolStrip _quickAccess = new() { Dock = DockStyle.Top, GripStyle = ToolStripGripStyle.Hidden, RenderMode = ToolStripRenderMode.System };
        private readonly Panel _contextHeader = new() { Dock = DockStyle.Top, Height = 18 };
        private readonly BindingList<SimpleItem> _commandItems = new();
        private readonly Dictionary<ToolStripItem, SimpleItem> _commandMap = new();
        private readonly Dictionary<BeepRibbonGroup, List<SimpleItem>> _groupCommandNodes = new();
        private readonly Dictionary<string, SimpleItem> _commandLookup = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, string> _galleryLastSelection = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, List<string>> _galleryPinnedKeys = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, List<string>> _galleryRecentKeys = new(StringComparer.OrdinalIgnoreCase);
        private readonly List<string> _quickAccessCommandKeys = [];
        private readonly List<Image> _generatedImages = new();
        private readonly ToolStripTextBox _searchBox = new() { AutoSize = false, Width = 180, Visible = false };
        private readonly ToolStripDropDownButton _searchResultsButton = new() { Text = "Find", Visible = false, AutoToolTip = true };
        private readonly List<SimpleItem> _searchResults = [];
        private readonly RibbonSuperTooltip _superTooltip = new();
        private SimpleItem? _hoveredTooltipCommand;
        private RibbonSuperTooltipModel? _hoveredTooltipModel;
        private readonly List<string> _searchHistory = [];
        private readonly Dictionary<string, int> _searchCommandUsage = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, int> _searchCategoryBoosts = new(StringComparer.OrdinalIgnoreCase);
        private int _searchRequestVersion;
        private int _searchResultSelectionIndex = -1;
        private bool _searchIncludeBackstage;
        private bool _showSearchHistorySuggestions = true;
        private bool _useSuperToolTips = true;
        private int _superTooltipDurationMs = 9000;
        private int _searchHistoryLimit = 20;
        private int _searchMaxResults = 12;
        private Func<SimpleItem, int>? _searchScoreBoostProvider;
        private IRibbonSearchTelemetry? _searchTelemetry;
        private Func<SimpleItem, RibbonSuperTooltipModel>? _superTooltipModelProvider;
        private readonly ToolTip _keyTipToolTip = new() { ShowAlways = true, AutomaticDelay = 0, InitialDelay = 0, ReshowDelay = 0 };
        private readonly Dictionary<ToolStripItem, string> _keyTips = [];
        private readonly Dictionary<string, ToolStripItem> _keyTipLookup = new(StringComparer.OrdinalIgnoreCase);
        private readonly RibbonKeyboardMap _keyboardMap = new();
        private RibbonVariantMatrix _variantMatrix = RibbonVariantMatrix.CreateDefault();
        private readonly List<string> _lastTokenImportDiagnostics = [];
        private readonly List<SimpleItem> _mergeBaseSnapshot = [];
        private readonly ContextMenuStrip _minimizedTabPopup = new();
        private readonly List<Image> _minimizedGeneratedImages = [];
        private RibbonTheme _theme = new();
        private bool _darkMode;
        private RibbonLayoutMode _layoutMode = RibbonLayoutMode.Classic;
        private RibbonDensity _density = RibbonDensity.Comfortable;
        private RibbonSearchMode _searchMode = RibbonSearchMode.Off;
        private RibbonPersonalizationOptions _personalizationOptions = RibbonPersonalizationOptions.All;
        private bool _quickAccessAboveRibbon = true;
        private bool _enableKeyTips = true;
        private bool _keyTipsVisible;
        private string _keyTipInputBuffer = string.Empty;
        private DateTime _lastKeyTipInput = DateTime.MinValue;
        private bool _allowMinimize = true;
        private bool _isMinimized;
        private bool _showMinimizedPopupOnTabClick = true;
        private int _expandedRibbonHeight = 120;
        private bool _isMerged;
        private bool _suspendCommandRebuild;
        private RibbonCustomizationState? _pendingCustomizationState;
        private List<SimpleItem>? _defaultCustomizationSnapshot;
        private List<string>? _defaultQuickAccessSnapshot;
        private bool _isApplyingResponsiveLayout;
        private IRibbonSearchProvider? _searchProvider;
        private bool _followGlobalFormStyle = true;
        private FormStyle _ribbonFormStyle = FormStyle.Modern;
        private RibbonStylePreset _resolvedStylePreset = RibbonStylePreset.OfficeLight;
        private bool _subscribedToThemeManager;

        // Backstage
        private readonly ToolStripDropDownButton _backstageButton;
        private readonly ToolStripDropDown _backstageDropDown;
        private readonly ToolStripControlHost _backstageHost;
        private readonly Panel _backstagePanelContent = new() { BackColor = SystemColors.ControlLightLight, Size = new Size(600, 400) };
        private readonly BindingList<SimpleItem> _backstageItems = new();
        private readonly BindingList<SimpleItem> _backstageRecentItems = new();
        private readonly BindingList<SimpleItem> _backstagePinnedItems = new();
        private int _backstageRecentLimit = 12;
        private bool _backstageShowTimestamps = true;
        private bool _backstageUseRelativeTimestamps = true;
        private Func<SimpleItem, DateTime, string>? _backstageTimestampFormatter;
        private readonly SplitContainer _backstageSplit = new() { Dock = DockStyle.Fill, IsSplitterFixed = false, SplitterWidth = 5 };
        private readonly ListBox _backstageNavList = new() { Dock = DockStyle.Fill, BorderStyle = BorderStyle.None, IntegralHeight = false };
        private readonly Panel _backstageContentHost = new() { Dock = DockStyle.Fill };
        private readonly Label _backstageTitle = new() { Dock = DockStyle.Top, Height = 42, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(12, 0, 8, 0) };
        private readonly FlowLayoutPanel _backstageActions = new() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true, Padding = new Padding(8) };
        private readonly FlowLayoutPanel _backstageFooter = new() { Dock = DockStyle.Bottom, Height = 42, FlowDirection = FlowDirection.RightToLeft, WrapContents = false, Padding = new Padding(6, 4, 6, 4) };
        private readonly BindingList<SimpleItem> _backstageFooterItems = new();
        private readonly SimpleItem _backstageFooterSection = new() { Text = "Footer", IsVisible = true, IsEnabled = true };
        private readonly Dictionary<int, SimpleItem> _backstageSectionMap = [];
        private readonly List<Image> _backstageGeneratedImages = [];
        private int _activeBackstageIndex = -1;
        private readonly Timer _backstageTransitionTimer = new() { Interval = 16 };
        private bool _enableBackstageTransitions = true;
        private int _backstageTransitionDurationMs = 180;
        private int _backstageTransitionEffectiveDurationMs = 180;
        private DateTime _backstageTransitionStartUtc = DateTime.UtcNow;
        private Size _backstageTransitionStartSize = new(600, 400);
        private Size _backstageTransitionTargetSize = new(600, 400);
        private readonly Dictionary<string, int> _contextualRuleMap = new(StringComparer.OrdinalIgnoreCase);
        private string _activeContextKey = string.Empty;
        private readonly Timer _contextTransitionTimer = new() { Interval = 16 };
        private bool _enableContextTransitions = true;
        private int _contextTransitionDurationMs = 180;
        private int _contextTransitionEffectiveDurationMs = 180;
        private float _contextTransitionProgress = 1f;
        private bool _ribbonRightToLeft;
        private bool _adaptiveTransitionTiming = true;
        private bool _respectSystemReducedMotion = true;
        private bool _reducedMotion;

        // Contextual groups
        private sealed class ContextualGroup
        {
            public string Name { get; init; } = string.Empty;
            public Color Color { get; init; } = Color.LightBlue;
            public bool Visible { get; set; }
            public List<TabPage> Pages { get; } = [];
        }

        private readonly List<ContextualGroup> _contextGroups = [];
        private readonly Dictionary<TabPage, ContextualGroup> _pageToGroup = [];

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TabControl Tabs => _tabs;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolStrip QuickAccess => _quickAccess;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> CommandItems => _commandItems;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public RibbonTheme Theme
        {
            get => _theme;
            set
            {
                _theme = value ?? new RibbonTheme();
                Invalidate(true);
                ApplyTheme();
            }
        }

        [DefaultValue(false)]
        public bool DarkMode
        {
            get => _darkMode;
            set
            {
                _darkMode = value;
                _resolvedStylePreset = RibbonThemeMapper.GetPreset(_ribbonFormStyle, _darkMode);
                if (_followGlobalFormStyle)
                {
                    ApplyThemeFromBeep(BeepThemesManager.CurrentTheme, _ribbonFormStyle);
                }
                else
                {
                    ApplyTheme();
                }
            }
        }

        [DefaultValue(true)]
        public bool FollowGlobalFormStyle
        {
            get => _followGlobalFormStyle;
            set
            {
                if (_followGlobalFormStyle == value) return;
                _followGlobalFormStyle = value;
                if (_followGlobalFormStyle)
                {
                    _ribbonFormStyle = BeepThemesManager.CurrentStyle;
                    ApplyThemeFromBeep(BeepThemesManager.CurrentTheme, _ribbonFormStyle);
                }
            }
        }

        [DefaultValue(FormStyle.Modern)]
        public FormStyle RibbonFormStyle
        {
            get => _ribbonFormStyle;
            set
            {
                if (_ribbonFormStyle == value) return;
                _ribbonFormStyle = value;
                if (!_followGlobalFormStyle)
                {
                    ApplyThemeFromBeep(BeepThemesManager.CurrentTheme, _ribbonFormStyle);
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RibbonStylePreset ResolvedStylePreset => _resolvedStylePreset;

        [DefaultValue(RibbonLayoutMode.Classic)]
        public RibbonLayoutMode LayoutMode
        {
            get => _layoutMode;
            set
            {
                if (_layoutMode == value) return;
                _layoutMode = value;
                RefreshCommandView();
            }
        }

        [DefaultValue(RibbonDensity.Comfortable)]
        public RibbonDensity Density
        {
            get => _density;
            set
            {
                if (_density == value) return;
                _density = value;
                RefreshCommandView();
            }
        }

        [DefaultValue(RibbonSearchMode.Off)]
        public RibbonSearchMode SearchMode
        {
            get => _searchMode;
            set
            {
                if (_searchMode == value) return;
                _searchMode = value;
                EnsureSearchControls();
            }
        }

        [DefaultValue(false)]
        public bool SearchIncludeBackstage
        {
            get => _searchIncludeBackstage;
            set => _searchIncludeBackstage = value;
        }

        [DefaultValue(true)]
        public bool ShowSearchHistorySuggestions
        {
            get => _showSearchHistorySuggestions;
            set => _showSearchHistorySuggestions = value;
        }

        [DefaultValue(true)]
        public bool UseSuperToolTips
        {
            get => _useSuperToolTips;
            set
            {
                if (_useSuperToolTips == value) return;
                _useSuperToolTips = value;
                RefreshCommandView();
            }
        }

        [DefaultValue(9000)]
        public int SuperTooltipDurationMs
        {
            get => _superTooltipDurationMs;
            set => _superTooltipDurationMs = Math.Max(1200, value);
        }

        [DefaultValue(20)]
        public int SearchHistoryLimit
        {
            get => _searchHistoryLimit;
            set => _searchHistoryLimit = Math.Max(1, value);
        }

        [DefaultValue(12)]
        public int SearchMaxResults
        {
            get => _searchMaxResults;
            set => _searchMaxResults = Math.Clamp(value, 4, 64);
        }

        [DefaultValue(RibbonPersonalizationOptions.All)]
        public RibbonPersonalizationOptions PersonalizationOptions
        {
            get => _personalizationOptions;
            set => _personalizationOptions = value;
        }

        [DefaultValue(true)]
        public bool QuickAccessAboveRibbon
        {
            get => _quickAccessAboveRibbon;
            set
            {
                if (_quickAccessAboveRibbon == value) return;
                _quickAccessAboveRibbon = value;
                ApplyQuickAccessPlacement();
            }
        }

        [DefaultValue(true)]
        public bool EnableKeyTips
        {
            get => _enableKeyTips;
            set
            {
                if (_enableKeyTips == value) return;
                _enableKeyTips = value;
                if (!_enableKeyTips)
                {
                    HideKeyTips();
                }
                else
                {
                    RefreshKeyTips();
                }
            }
        }

        [DefaultValue(true)]
        public bool AllowMinimize
        {
            get => _allowMinimize;
            set
            {
                if (_allowMinimize == value) return;
                _allowMinimize = value;
                if (!_allowMinimize && _isMinimized)
                {
                    IsMinimized = false;
                }
            }
        }

        [DefaultValue(false)]
        public bool IsMinimized
        {
            get => _isMinimized;
            set
            {
                if (_isMinimized == value) return;
                _isMinimized = value;
                ApplyMinimizedState();
                RibbonMinimizedChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        [DefaultValue(true)]
        public bool ShowMinimizedPopupOnTabClick
        {
            get => _showMinimizedPopupOnTabClick;
            set => _showMinimizedPopupOnTabClick = value;
        }

        [DefaultValue(12)]
        public int BackstageRecentLimit
        {
            get => _backstageRecentLimit;
            set => _backstageRecentLimit = Math.Max(1, value);
        }

        [DefaultValue(true)]
        public bool BackstageShowTimestamps
        {
            get => _backstageShowTimestamps;
            set
            {
                if (_backstageShowTimestamps == value) return;
                _backstageShowTimestamps = value;
                if (_activeBackstageIndex >= 0)
                {
                    ShowBackstageSection(_activeBackstageIndex);
                }
            }
        }

        [DefaultValue(true)]
        public bool BackstageUseRelativeTimestamps
        {
            get => _backstageUseRelativeTimestamps;
            set
            {
                if (_backstageUseRelativeTimestamps == value) return;
                _backstageUseRelativeTimestamps = value;
                if (_activeBackstageIndex >= 0)
                {
                    ShowBackstageSection(_activeBackstageIndex);
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<SimpleItem, DateTime, string>? BackstageTimestampFormatter
        {
            get => _backstageTimestampFormatter;
            set
            {
                _backstageTimestampFormatter = value;
                if (_activeBackstageIndex >= 0)
                {
                    ShowBackstageSection(_activeBackstageIndex);
                }
            }
        }

        [DefaultValue(true)]
        public bool EnableBackstageTransitions
        {
            get => _enableBackstageTransitions;
            set => _enableBackstageTransitions = value;
        }

        [DefaultValue(180)]
        public int BackstageTransitionDurationMs
        {
            get => _backstageTransitionDurationMs;
            set => _backstageTransitionDurationMs = Math.Max(50, value);
        }

        [DefaultValue(true)]
        public bool EnableContextTransitions
        {
            get => _enableContextTransitions;
            set
            {
                if (_enableContextTransitions == value) return;
                _enableContextTransitions = value;
                if (!_enableContextTransitions)
                {
                    _contextTransitionTimer.Stop();
                    _contextTransitionProgress = 1f;
                    _contextHeader.Invalidate();
                }
            }
        }

        [DefaultValue(180)]
        public int ContextTransitionDurationMs
        {
            get => _contextTransitionDurationMs;
            set => _contextTransitionDurationMs = Math.Max(50, value);
        }

        [DefaultValue(true)]
        public bool AdaptiveTransitionTiming
        {
            get => _adaptiveTransitionTiming;
            set => _adaptiveTransitionTiming = value;
        }

        [DefaultValue(true)]
        public bool RespectSystemReducedMotion
        {
            get => _respectSystemReducedMotion;
            set => _respectSystemReducedMotion = value;
        }

        [DefaultValue(false)]
        public bool ReducedMotion
        {
            get => _reducedMotion;
            set
            {
                if (_reducedMotion == value) return;
                _reducedMotion = value;
                if (_reducedMotion)
                {
                    _contextTransitionTimer.Stop();
                    _backstageTransitionTimer.Stop();
                    _contextTransitionProgress = 1f;
                    _contextHeader.Invalidate();
                }
            }
        }

        [DefaultValue(false)]
        public bool RibbonRightToLeft
        {
            get => _ribbonRightToLeft;
            set
            {
                if (_ribbonRightToLeft == value) return;
                _ribbonRightToLeft = value;
                RightToLeft = _ribbonRightToLeft ? RightToLeft.Yes : RightToLeft.No;
                ApplyRightToLeftLayout();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IRibbonSearchProvider? SearchProvider
        {
            get => _searchProvider;
            set => _searchProvider = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IRibbonSearchTelemetry? SearchTelemetry
        {
            get => _searchTelemetry;
            set => _searchTelemetry = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<SimpleItem, int>? SearchScoreBoostProvider
        {
            get => _searchScoreBoostProvider;
            set => _searchScoreBoostProvider = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<SimpleItem, RibbonSuperTooltipModel>? SuperTooltipModelProvider
        {
            get => _superTooltipModelProvider;
            set => _superTooltipModelProvider = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IReadOnlyList<string> SearchHistory => _searchHistory;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RibbonKeyboardMap KeyboardMap => _keyboardMap;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RibbonVariantMatrix VariantMatrix
        {
            get => _variantMatrix;
            set => _variantMatrix = value ?? RibbonVariantMatrix.CreateDefault();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IReadOnlyList<string> LastTokenImportDiagnostics => _lastTokenImportDiagnostics;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IReadOnlyList<string> QuickAccessCommandKeys => _quickAccessCommandKeys;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ActiveContextKey => _activeContextKey;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsMerged => _isMerged;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasCustomizationDefaults => _defaultCustomizationSnapshot is { Count: > 0 };

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> BackstageItems => _backstageItems;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> BackstageRecentItems => _backstageRecentItems;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> BackstagePinnedItems => _backstagePinnedItems;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> BackstageFooterItems => _backstageFooterItems;

        // Backstage right content surface
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Panel BackstageContent => _backstageContentHost;

        // Full backstage host panel (navigation + content)
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Panel BackstageSurface => _backstagePanelContent;

        public event EventHandler<RibbonCommandInvokedEventArgs>? CommandInvoked;
        public event EventHandler<BackstageCommandInvokedEventArgs>? BackstageCommandInvoked;
        public event EventHandler<BackstageSectionChangedEventArgs>? BackstageSectionChanged;
        public event EventHandler<RibbonMergedEventArgs>? RibbonMerged;
        public event EventHandler? RibbonMinimizedChanged;
        public event EventHandler? RibbonCustomizationRequested;
        public event EventHandler<RibbonCustomizationAppliedEventArgs>? RibbonCustomizationApplied;
        public event EventHandler? RibbonCustomizationReset;
        public event EventHandler? RibbonCustomizationCanceled;
        public event EventHandler<RibbonSearchExecutedEventArgs>? SearchExecuted;
        public event EventHandler<RibbonTooltipActionRequestedEventArgs>? TooltipActionRequested;

        public BeepRibbonControl()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            Controls.Add(_tabs);
            Controls.Add(_contextHeader);
            Controls.Add(_quickAccess);
            Height = 120;

            _commandItems.ListChanged += CommandItems_ListChanged;
            _backstageItems.ListChanged += BackstageItems_ListChanged;
            _backstageRecentItems.ListChanged += BackstageItems_ListChanged;
            _backstagePinnedItems.ListChanged += BackstageItems_ListChanged;
            _backstageFooterItems.ListChanged += BackstageFooterItems_ListChanged;

            // Backstage setup
            _backstageButton = new ToolStripDropDownButton("File") { ShowDropDownArrow = true, AutoToolTip = false };
            _backstageDropDown = new ToolStripDropDown { Padding = Padding.Empty, AutoClose = true, AutoSize = false };
            InitializeBackstageLayout();
            _backstageHost = new ToolStripControlHost(_backstagePanelContent) { AutoSize = false, Size = _backstagePanelContent.Size, Margin = Padding.Empty, Padding = Padding.Empty };
            _backstageDropDown.Items.Add(_backstageHost);
            _backstageButton.DropDownOpening += BackstageButton_DropDownOpening;
            _backstageDropDown.Closed += BackstageDropDown_Closed;
            _backstageButton.DropDown = _backstageDropDown;
            _quickAccess.Items.Insert(0, _backstageButton);
            InitializeSearchControls();

            _quickAccess.Renderer = new BeepRibbonToolStripRenderer(this);
            _contextHeader.Paint += ContextHeader_Paint;
            _tabs.ControlAdded += (_, __) => _contextHeader.Invalidate();
            _tabs.ControlRemoved += (_, __) => _contextHeader.Invalidate();
            _tabs.SelectedIndexChanged += (_, __) =>
            {
                _contextHeader.Invalidate();
                RefreshKeyTipsVisibility();
            };
            _tabs.SelectedIndexChanged += Tabs_SelectedIndexChanged;
            _tabs.MouseUp += Tabs_MouseUp;
            _tabs.MouseDoubleClick += Tabs_MouseDoubleClick;

            _keyboardMap.Register(Keys.F6, () => FocusRibbonPane(1));
            _keyboardMap.Register(Keys.Shift | Keys.F6, () => FocusRibbonPane(-1));
            _contextTransitionTimer.Tick += ContextTransitionTimer_Tick;
            _backstageTransitionTimer.Tick += BackstageTransitionTimer_Tick;

            RibbonAccessibilityHelper.ApplyControlAccessibility(_quickAccess, "Quick Access Toolbar", "Primary ribbon quick access commands.", AccessibleRole.ToolBar);
            RibbonAccessibilityHelper.ApplyControlAccessibility(_tabs, "Ribbon Tabs", "Ribbon tabs and command groups.", AccessibleRole.PageTabList);
            RibbonAccessibilityHelper.ApplyControlAccessibility(_backstageNavList, "Backstage Navigation", "Backstage section list.", AccessibleRole.Outline);
            RibbonAccessibilityHelper.ApplyControlAccessibility(_backstageActions, "Backstage Actions", "Actions available for the selected backstage section.", AccessibleRole.Pane);
            RibbonAccessibilityHelper.ApplyControlAccessibility(_backstageFooter, "Backstage Footer Actions", "Footer commands such as options and account actions.", AccessibleRole.ToolBar);

            ApplyRightToLeftLayout();
            ApplyQuickAccessPlacement();
            ApplySearchAccessibility();
            ApplyPaneTabOrder();
            TrySubscribeThemeManager();
            if (_followGlobalFormStyle)
            {
                SyncWithGlobalThemeAndStyle();
            }
            else
            {
                ApplyTheme();
            }
        }

        private void TrySubscribeThemeManager()
        {
            if (_subscribedToThemeManager) return;
            try
            {
                BeepThemesManager.ThemeChanged -= OnGlobalThemeChanged;
                BeepThemesManager.ThemeChanged += OnGlobalThemeChanged;
                BeepThemesManager.FormStyleChanged -= OnGlobalFormStyleChanged;
                BeepThemesManager.FormStyleChanged += OnGlobalFormStyleChanged;
                _subscribedToThemeManager = true;
            }
            catch
            {
                // best effort only
            }
        }

        private void OnGlobalThemeChanged(object? sender, ThemeChangeEventArgs e)
        {
            if (IsDisposed || !_followGlobalFormStyle) return;
            try
            {
                var nextTheme = e?.NewTheme ?? BeepThemesManager.CurrentTheme;
                ApplyThemeFromBeep(nextTheme, _ribbonFormStyle);
            }
            catch
            {
                // keep ribbon stable if theme manager fails
            }
        }

        private void OnGlobalFormStyleChanged(object? sender, StyleChangeEventArgs e)
        {
            if (IsDisposed || !_followGlobalFormStyle) return;
            _ribbonFormStyle = e.NewStyle;
            _resolvedStylePreset = RibbonThemeMapper.GetPreset(_ribbonFormStyle, _darkMode);
            // FormStyleChanged fires before BeepThemesManager.CurrentThemeName is updated.
            // ThemeChanged will fire immediately after with the correct new theme.
        }

        private void UnsubscribeThemeManager()
        {
            if (!_subscribedToThemeManager) return;
            try
            {
                BeepThemesManager.ThemeChanged -= OnGlobalThemeChanged;
                BeepThemesManager.FormStyleChanged -= OnGlobalFormStyleChanged;
            }
            catch
            {
                // no-op
            }
            _subscribedToThemeManager = false;
        }

        private void CommandItems_ListChanged(object? sender, ListChangedEventArgs e)
        {
            if (_suspendCommandRebuild)
            {
                return;
            }
            BuildFromSimpleItems();
        }

        private void BackstageItems_ListChanged(object? sender, ListChangedEventArgs e)
        {
            BuildBackstageFromSimpleItems();
        }

        private void BackstageFooterItems_ListChanged(object? sender, ListChangedEventArgs e)
        {
            BuildBackstageFooterActions();
        }

        private void Tabs_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            if (!_allowMinimize) return;

            for (int i = 0; i < _tabs.TabCount; i++)
            {
                var rect = _tabs.GetTabRect(i);
                if (rect.Contains(e.Location))
                {
                    IsMinimized = !IsMinimized;
                    break;
                }
            }
        }

        private void Tabs_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (!_isMinimized || !_showMinimizedPopupOnTabClick)
            {
                return;
            }

            // Mouse click path is handled in Tabs_MouseUp to avoid duplicate popups.
            if (Control.MouseButtons == MouseButtons.Left)
            {
                return;
            }

            if (_tabs.SelectedIndex >= 0)
            {
                ShowMinimizedPopupForTabIndex(_tabs.SelectedIndex);
            }
        }

        private void Tabs_MouseUp(object? sender, MouseEventArgs e)
        {
            int tabIndex = GetTabIndexAt(e.Location);
            if (e.Button == MouseButtons.Right)
            {
                ShowTabHeaderContextMenu(e.Location);
                return;
            }

            if (e.Button != MouseButtons.Left || !_isMinimized || !_showMinimizedPopupOnTabClick || tabIndex < 0)
            {
                return;
            }

            ShowMinimizedPopupForTabIndex(tabIndex);
        }

        private int GetTabIndexAt(Point location)
        {
            for (int i = 0; i < _tabs.TabCount; i++)
            {
                if (_tabs.GetTabRect(i).Contains(location))
                {
                    return i;
                }
            }

            return -1;
        }

        private void ShowTabHeaderContextMenu(Point location)
        {
            var menu = new ContextMenuStrip();
            menu.Font = BeepThemesManager.ToFont(_theme.CommandTypography);

            string minimizeText = _isMinimized ? "Show the Ribbon" : "Minimize the Ribbon";
            menu.Items.Add(minimizeText, null, (_, __) =>
            {
                if (_allowMinimize)
                {
                    IsMinimized = !_isMinimized;
                }
            });

            string qatPlacementText = _quickAccessAboveRibbon
                ? "Show Quick Access Toolbar Below the Ribbon"
                : "Show Quick Access Toolbar Above the Ribbon";
            menu.Items.Add(qatPlacementText, null, (_, __) => QuickAccessAboveRibbon = !_quickAccessAboveRibbon);

            var layoutMenu = new ToolStripMenuItem("Ribbon Layout");
            AddCheckedMenuItem(layoutMenu, "Classic", _layoutMode == RibbonLayoutMode.Classic, () => LayoutMode = RibbonLayoutMode.Classic);
            AddCheckedMenuItem(layoutMenu, "Simplified", _layoutMode == RibbonLayoutMode.Simplified, () => LayoutMode = RibbonLayoutMode.Simplified);
            menu.Items.Add(layoutMenu);

            var densityMenu = new ToolStripMenuItem("Ribbon Density");
            AddCheckedMenuItem(densityMenu, "Comfortable", _density == RibbonDensity.Comfortable, () => Density = RibbonDensity.Comfortable);
            AddCheckedMenuItem(densityMenu, "Compact", _density == RibbonDensity.Compact, () => Density = RibbonDensity.Compact);
            AddCheckedMenuItem(densityMenu, "Touch", _density == RibbonDensity.Touch, () => Density = RibbonDensity.Touch);
            menu.Items.Add(densityMenu);

            menu.Items.Add(new ToolStripSeparator());
            AddCheckedMenuItem(menu.Items, "Dark Mode", _darkMode, () => DarkMode = !_darkMode);
            AddCheckedMenuItem(menu.Items, "Search Includes Backstage", _searchIncludeBackstage, () => SearchIncludeBackstage = !_searchIncludeBackstage);
            AddCheckedMenuItem(menu.Items, "Reduced Motion", _reducedMotion, () => ReducedMotion = !_reducedMotion);

            if ((_personalizationOptions & (RibbonPersonalizationOptions.RibbonTabs | RibbonPersonalizationOptions.RibbonGroups | RibbonPersonalizationOptions.CommandOrder)) != 0)
            {
                menu.Items.Add(new ToolStripSeparator());
                menu.Items.Add(CreateCustomizeRibbonMenuItem());
            }

            menu.Closed += (_, __) => menu.Dispose();
            menu.Show(_tabs, location);
        }

        private void RequestRibbonCustomization()
        {
            if (RibbonCustomizationRequested != null)
            {
                RibbonCustomizationRequested.Invoke(this, EventArgs.Empty);
                return;
            }

            ShowCustomizeRibbonDialog(FindForm());
        }

        public ToolStripMenuItem CreateCustomizeRibbonMenuItem(string text = "Customize Ribbon...")
        {
            var item = new ToolStripMenuItem(string.IsNullOrWhiteSpace(text) ? "Customize Ribbon..." : text)
            {
                AccessibleName = "Customize ribbon",
                AccessibleDescription = "Open ribbon customization for tabs, groups, and quick access commands."
            };
            item.Click += (_, __) => RequestRibbonCustomization();
            return item;
        }

        private static void AddCheckedMenuItem(ToolStripItemCollection items, string text, bool isChecked, Action onClick)
        {
            var item = new ToolStripMenuItem(text)
            {
                Checked = isChecked,
                CheckOnClick = false
            };
            item.Click += (_, __) => onClick();
            items.Add(item);
        }

        private static void AddCheckedMenuItem(ToolStripMenuItem parent, string text, bool isChecked, Action onClick)
        {
            AddCheckedMenuItem(parent.DropDownItems, text, isChecked, onClick);
        }

        private void ShowMinimizedPopupForTabIndex(int tabIndex)
        {
            if (!_isMinimized || tabIndex < 0 || tabIndex >= _tabs.TabCount)
            {
                return;
            }

            HideMinimizedPopup();

            if (_tabs.SelectedIndex != tabIndex)
            {
                _tabs.SelectedIndex = tabIndex;
            }

            if (_tabs.TabPages[tabIndex].Tag is not SimpleItem tabNode)
            {
                return;
            }

            foreach (var groupNode in tabNode.Children.Where(IsVisibleNode))
            {
                var groupCommands = NormalizeSeparators(groupNode.Children.Where(IsVisibleNode));
                if (groupCommands.Count == 0)
                {
                    continue;
                }

                var groupItem = new ToolStripMenuItem(GetDisplayText(groupNode))
                {
                    Font = BeepThemesManager.ToFont(_theme.GroupTypography),
                    ForeColor = _theme.Text,
                    BackColor = _theme.GroupBack
                };

                BuildMinimizedPopupMenu(groupItem.DropDownItems, groupCommands);
                _minimizedTabPopup.Items.Add(groupItem);
            }

            if (_minimizedTabPopup.Items.Count == 0)
            {
                return;
            }

            _minimizedTabPopup.Renderer = new BeepRibbonToolStripRenderer(this);
            _minimizedTabPopup.Closed -= MinimizedTabPopup_Closed;
            _minimizedTabPopup.Closed += MinimizedTabPopup_Closed;

            var rect = _tabs.GetTabRect(tabIndex);
            _minimizedTabPopup.Show(_tabs, new Point(rect.Left, rect.Bottom));
        }

        private void BuildMinimizedPopupMenu(ToolStripItemCollection parent, IEnumerable<SimpleItem> nodes)
        {
            foreach (var node in nodes.Where(IsVisibleNode))
            {
                if (node.IsSeparator)
                {
                    parent.Add(new ToolStripSeparator());
                    continue;
                }

                var item = new ToolStripMenuItem(GetDisplayText(node), CreateTransientCommandImage(node.ImagePath, true))
                {
                    Enabled = node.IsEnabled,
                    Checked = node.IsChecked,
                    CheckOnClick = node.IsCheckable,
                    Font = BeepThemesManager.ToFont(_theme.CommandTypography),
                };

                ConfigureCommandItem(item, node);
                item.Click += (_, __) =>
                {
                    RaiseCommandInvoked(node, item);
                    HideMinimizedPopup();
                };
                parent.Add(item);
                _commandMap[item] = node;

                if (node.Children.Count > 0)
                {
                    BuildMinimizedPopupMenu(item.DropDownItems, node.Children);
                }
            }
        }

        private Image? CreateTransientCommandImage(string? imagePath, bool small)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return null;
            }

            int size = GetIconSize(small);
            var bmp = new Bitmap(size, size);
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.Clear(Color.Transparent);
            using var clipPath = new GraphicsPath();
            clipPath.AddRectangle(new Rectangle(0, 0, size, size));
            StyledImagePainter.PaintWithTint(g, clipPath, imagePath, _theme.IconColor, 1f);
            _minimizedGeneratedImages.Add(bmp);
            return bmp;
        }

        private void MinimizedTabPopup_Closed(object? sender, ToolStripDropDownClosedEventArgs e)
        {
            CleanupMinimizedPopup();
        }

        private void HideMinimizedPopup()
        {
            if (_minimizedTabPopup.Visible)
            {
                _minimizedTabPopup.Close();
                return;
            }

            CleanupMinimizedPopup();
        }

        private void CleanupMinimizedPopup()
        {
            RemovePopupMappings(_minimizedTabPopup.Items);
            _minimizedTabPopup.Items.Clear();
            DisposeMinimizedImages();
        }

        private void RemovePopupMappings(ToolStripItemCollection items)
        {
            foreach (ToolStripItem item in items)
            {
                _commandMap.Remove(item);
                if (item is ToolStripDropDownItem dropDownItem && dropDownItem.DropDownItems.Count > 0)
                {
                    RemovePopupMappings(dropDownItem.DropDownItems);
                }
            }
        }

        private void DisposeMinimizedImages()
        {
            foreach (var image in _minimizedGeneratedImages)
            {
                image.Dispose();
            }
            _minimizedGeneratedImages.Clear();
        }

        private int CalculateMinimizedHeight()
        {
            int quickAccessHeight = Math.Max(_quickAccess.Height, _quickAccess.PreferredSize.Height);
            int tabsHeaderHeight = Math.Max(26, _tabs.ItemSize.Height > 0 ? _tabs.ItemSize.Height : Font.Height + 12);
            return quickAccessHeight + _contextHeader.Height + tabsHeaderHeight + 4;
        }

        private void ApplyMinimizedState()
        {
            if (_isMinimized)
            {
                _expandedRibbonHeight = Math.Max(_expandedRibbonHeight, Height);
                Height = CalculateMinimizedHeight();
            }
            else
            {
                HideMinimizedPopup();
                int minimumExpandedHeight = CalculateMinimizedHeight() + 18;
                if (_expandedRibbonHeight < minimumExpandedHeight)
                {
                    _expandedRibbonHeight = minimumExpandedHeight;
                }
                Height = _expandedRibbonHeight;
            }

            _tabs.Invalidate();
            _contextHeader.Invalidate();
        }

        public void ToggleMinimized()
        {
            if (!_allowMinimize) return;
            IsMinimized = !IsMinimized;
        }

        private void InitializeBackstageLayout()
        {
            _backstageSplit.SplitterDistance = 180;
            _backstageSplit.Panel1MinSize = 140;
            _backstageSplit.Panel2MinSize = 260;

            _backstageSplit.Panel1.Controls.Clear();
            _backstageSplit.Panel1.Controls.Add(_backstageNavList);

            _backstageContentHost.Controls.Clear();
            _backstageContentHost.Controls.Add(_backstageActions);
            _backstageContentHost.Controls.Add(_backstageFooter);
            _backstageContentHost.Controls.Add(_backstageTitle);

            _backstageSplit.Panel2.Controls.Clear();
            _backstageSplit.Panel2.Controls.Add(_backstageContentHost);

            _backstagePanelContent.Controls.Clear();
            _backstagePanelContent.Controls.Add(_backstageSplit);

            _backstageNavList.SelectedIndexChanged -= BackstageNavList_SelectedIndexChanged;
            _backstageNavList.SelectedIndexChanged += BackstageNavList_SelectedIndexChanged;
            _backstageActions.SizeChanged -= BackstageActions_SizeChanged;
            _backstageActions.SizeChanged += BackstageActions_SizeChanged;
            BuildBackstageFooterActions();
        }

        public void BuildBackstageFromSimpleItems()
        {
            BuildBackstageFromSimpleItems(_backstageItems);
        }

        public void BuildBackstageFromSimpleItems(IEnumerable<SimpleItem>? sectionNodes)
        {
            _backstageSectionMap.Clear();
            _backstageNavList.Items.Clear();
            ClearBackstageActions();
            _activeBackstageIndex = -1;

            if (sectionNodes == null)
            {
                _backstageTitle.Text = string.Empty;
                return;
            }

            int index = 0;
            foreach (var section in sectionNodes.Where(IsVisibleNode))
            {
                _backstageSectionMap[index] = section;
                _backstageNavList.Items.Add(GetDisplayText(section));
                index++;
            }

            if (_backstageNavList.Items.Count > 0)
            {
                _backstageNavList.SelectedIndex = 0;
            }
            else
            {
                _backstageTitle.Text = string.Empty;
            }
        }

        public void LoadStandardBackstageTemplate(string applicationTitle = "Application", bool replaceExistingSections = true)
        {
            var sections = RibbonBackstageTemplateBuilder.CreateStandardTemplate(applicationTitle);
            if (replaceExistingSections)
            {
                _backstageItems.Clear();
            }

            foreach (var section in sections)
            {
                _backstageItems.Add(section);
            }
        }

        public void BindBackstageRecentItems(IEnumerable<RibbonRecentItemModel>? items, bool clearCurrent = true)
        {
            if (clearCurrent)
            {
                _backstageRecentItems.Clear();
                _backstagePinnedItems.Clear();
            }

            if (items == null)
            {
                return;
            }

            foreach (var model in items)
            {
                if (model == null)
                {
                    continue;
                }

                var node = model.ToSimpleItem();
                if (model.IsPinned)
                {
                    PinBackstageItem(node);
                }
                else
                {
                    AddBackstageRecentItem(node);
                }
            }
        }

        public void AddBackstageFooterAction(SimpleItem item)
        {
            if (item == null)
            {
                return;
            }

            _backstageFooterItems.Add(item);
        }

        public void ClearBackstageFooterActionItems()
        {
            _backstageFooterItems.Clear();
        }

        public void AddBackstageRecentItem(SimpleItem item, bool pin = false)
        {
            if (item == null)
            {
                return;
            }

            string key = GetMergeKey(item);
            RemoveBackstageItemByKey(_backstageRecentItems, key);

            if (pin)
            {
                PinBackstageItem(item);
                return;
            }

            if (string.IsNullOrWhiteSpace(item.SubText3))
            {
                item.SubText3 = DateTime.UtcNow.ToString("O");
            }

            _backstageRecentItems.Insert(0, item);
            while (_backstageRecentItems.Count > _backstageRecentLimit)
            {
                _backstageRecentItems.RemoveAt(_backstageRecentItems.Count - 1);
            }
        }

        public bool PinBackstageItem(SimpleItem item)
        {
            if (item == null)
            {
                return false;
            }

            string key = GetMergeKey(item);
            if (string.IsNullOrWhiteSpace(key))
            {
                return false;
            }

            RemoveBackstageItemByKey(_backstagePinnedItems, key);
            RemoveBackstageItemByKey(_backstageRecentItems, key);
            _backstagePinnedItems.Insert(0, item);
            return true;
        }

        public bool UnpinBackstageItem(string itemKey, bool moveToRecent = true)
        {
            int index = FindBackstageItemIndexByKey(_backstagePinnedItems, itemKey);
            if (index < 0)
            {
                return false;
            }

            var item = _backstagePinnedItems[index];
            _backstagePinnedItems.RemoveAt(index);
            if (moveToRecent)
            {
                AddBackstageRecentItem(item, false);
            }
            return true;
        }

        public void ClearBackstageRecentItems()
        {
            _backstageRecentItems.Clear();
        }

        public void ClearBackstagePinnedItems()
        {
            _backstagePinnedItems.Clear();
        }

        public void ShowBackstageSection(int index)
        {
            if (!_backstageSectionMap.TryGetValue(index, out var section))
            {
                return;
            }

            _activeBackstageIndex = index;
            _backstageTitle.Text = GetDisplayText(section);
            ClearBackstageActions();

            foreach (var action in GetBackstageActions(section))
            {
                if (action.IsSeparator)
                {
                    var sep = new Label
                    {
                        Height = 1,
                        Width = Math.Max(80, _backstageActions.ClientSize.Width - 24),
                        Margin = new Padding(4, 8, 4, 8),
                        BackColor = _theme.GroupBorder
                    };
                    _backstageActions.Controls.Add(sep);
                    continue;
                }

                var row = CreateBackstageActionRow(action);
                _backstageActions.Controls.Add(row);
            }

            BackstageSectionChanged?.Invoke(this, new BackstageSectionChangedEventArgs(section, index));
        }

        private string GetBackstageActionButtonText(SimpleItem action)
        {
            string text = GetDisplayText(action);
            if (IsBackstageSectionHeader(action))
            {
                return text;
            }

            return text;
        }

        private Panel CreateBackstageActionRow(SimpleItem action)
        {
            var row = new Panel
            {
                Width = Math.Max(120, _backstageActions.ClientSize.Width - 30),
                Height = 36,
                Margin = new Padding(4),
                Padding = new Padding(0),
                BackColor = Color.Transparent,
                Tag = action
            };

            var button = new Button
            {
                Dock = DockStyle.Fill,
                Text = GetBackstageActionButtonText(action),
                TextAlign = ContentAlignment.MiddleLeft,
                FlatStyle = FlatStyle.Flat,
                BackColor = _theme.TabActiveBack,
                ForeColor = _theme.Text,
                Font = BeepThemesManager.ToFont(_theme.CommandTypography),
                AccessibleName = GetDisplayText(action),
                AccessibleDescription = BuildToolTip(action),
                Tag = action
            };
            button.FlatAppearance.BorderColor = _theme.GroupBorder;
            button.FlatAppearance.MouseDownBackColor = _theme.GroupBack;
            button.FlatAppearance.MouseOverBackColor = ControlPaint.Light(_theme.GroupBack, .1f);
            button.Click += BackstageActionButton_Click;
            button.MouseUp += BackstageActionButton_MouseUp;
            RibbonAccessibilityHelper.ApplyControlAccessibility(
                button,
                GetDisplayText(action),
                BuildToolTip(action),
                AccessibleRole.PushButton);

            if (_backstageShowTimestamps && TryGetBackstageItemTimestamp(action, out var openedUtc))
            {
                var stamp = new Label
                {
                    Dock = DockStyle.Right,
                    Width = 88,
                    TextAlign = ContentAlignment.MiddleRight,
                    ForeColor = ControlPaint.Dark(_theme.Text),
                    BackColor = Color.Transparent,
                    Font = BeepThemesManager.ToFont(_theme.GroupTypography),
                    Text = FormatBackstageTimestamp(action, openedUtc),
                    Tag = action,
                    AccessibleName = $"Last opened for {GetDisplayText(action)}",
                    AccessibleDescription = openedUtc.ToLocalTime().ToString("G")
                };
                RibbonAccessibilityHelper.ApplyControlAccessibility(
                    stamp,
                    stamp.AccessibleName,
                    stamp.AccessibleDescription,
                    AccessibleRole.StaticText);
                row.Controls.Add(stamp);
            }

            if (!string.IsNullOrWhiteSpace(action.ImagePath))
            {
                var image = CreateBackstageImage(action.ImagePath);
                if (image != null)
                {
                    button.Image = image;
                    button.ImageAlign = ContentAlignment.MiddleLeft;
                    button.TextImageRelation = TextImageRelation.ImageBeforeText;
                }
            }

            if (!IsBackstageSectionHeader(action))
            {
                var pinButton = new Button
                {
                    Dock = DockStyle.Right,
                    Width = 30,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = _theme.TabActiveBack,
                    ForeColor = IsPinnedBackstageItem(action) ? _theme.FocusBorder : _theme.Text,
                    Font = BeepThemesManager.ToFont(_theme.CommandTypography),
                    Text = IsPinnedBackstageItem(action) ? "*" : "o",
                    Tag = action,
                    TextAlign = ContentAlignment.MiddleCenter,
                    AccessibleName = $"Pin {GetDisplayText(action)}",
                    AccessibleDescription = "Toggle pinned state for this backstage item"
                };
                pinButton.FlatAppearance.BorderColor = _theme.GroupBorder;
                pinButton.FlatAppearance.MouseDownBackColor = _theme.GroupBack;
                pinButton.FlatAppearance.MouseOverBackColor = _theme.HoverBack;
                pinButton.Click += BackstageInlinePinButton_Click;
                RibbonAccessibilityHelper.ApplyControlAccessibility(
                    pinButton,
                    pinButton.AccessibleName,
                    pinButton.AccessibleDescription,
                    AccessibleRole.CheckButton);
                row.Controls.Add(pinButton);
            }

            row.Controls.Add(button);

            return row;
        }

        private void BackstageInlinePinButton_Click(object? sender, EventArgs e)
        {
            if (sender is not Button pinButton || pinButton.Tag is not SimpleItem action)
            {
                return;
            }

            string key = GetMergeKey(action);
            if (string.IsNullOrWhiteSpace(key))
            {
                return;
            }

            if (IsPinnedBackstageItem(action))
            {
                UnpinBackstageItem(key, moveToRecent: true);
            }
            else
            {
                PinBackstageItem(action);
            }

            if (_activeBackstageIndex >= 0)
            {
                ShowBackstageSection(_activeBackstageIndex);
            }
        }

        private List<SimpleItem> GetBackstageActions(SimpleItem section)
        {
            var actions = section.Children.Where(IsVisibleNode).ToList();
            string sectionName = GetDisplayText(section);

            bool includeRecent = sectionName.Equals("Open", StringComparison.OrdinalIgnoreCase) ||
                                 sectionName.Equals("Recent", StringComparison.OrdinalIgnoreCase) ||
                                 sectionName.Contains("Recent", StringComparison.OrdinalIgnoreCase);

            if (!includeRecent)
            {
                return actions;
            }

            if (_backstagePinnedItems.Count > 0)
            {
                if (actions.Count > 0) actions.Add(new SimpleItem { IsSeparator = true });
                actions.Add(new SimpleItem { Text = "Pinned", IsEnabled = false, IsVisible = true });
                actions.AddRange(_backstagePinnedItems.Where(IsVisibleNode));
            }

            if (_backstageRecentItems.Count > 0)
            {
                if (actions.Count > 0) actions.Add(new SimpleItem { IsSeparator = true });
                actions.Add(new SimpleItem { Text = "Recent", IsEnabled = false, IsVisible = true });
                actions.AddRange(_backstageRecentItems.Where(IsVisibleNode));
            }

            return actions;
        }

        private void BackstageActions_SizeChanged(object? sender, EventArgs e)
        {
            int width = Math.Max(120, _backstageActions.ClientSize.Width - 30);
            foreach (var panel in _backstageActions.Controls.OfType<Panel>())
            {
                panel.Width = width;
            }

            foreach (var separator in _backstageActions.Controls.OfType<Label>())
            {
                separator.Width = Math.Max(80, _backstageActions.ClientSize.Width - 24);
            }

            foreach (var button in _backstageActions.Controls.OfType<Button>())
            {
                button.Width = width;
            }
        }

        private void BuildBackstageFooterActions()
        {
            ClearBackstageFooterActions();

            foreach (var item in _backstageFooterItems.Where(IsVisibleNode))
            {
                int width = Math.Clamp(TextRenderer.MeasureText(GetDisplayText(item), BeepThemesManager.ToFont(_theme.CommandTypography)).Width + 34, 96, 180);
                var button = new Button
                {
                    AutoSize = false,
                    Height = 30,
                    Width = width,
                    Margin = new Padding(4, 3, 4, 3),
                    Text = GetDisplayText(item),
                    TextAlign = ContentAlignment.MiddleCenter,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = _theme.TabActiveBack,
                    ForeColor = _theme.Text,
                    Font = BeepThemesManager.ToFont(_theme.CommandTypography),
                    Tag = item
                };
                button.FlatAppearance.BorderColor = _theme.GroupBorder;
                button.FlatAppearance.MouseDownBackColor = _theme.GroupBack;
                button.FlatAppearance.MouseOverBackColor = _theme.HoverBack;
                RibbonAccessibilityHelper.ApplyControlAccessibility(
                    button,
                    GetDisplayText(item),
                    BuildToolTip(item),
                    AccessibleRole.PushButton);
                if (!string.IsNullOrWhiteSpace(item.ImagePath))
                {
                    var image = CreateBackstageImage(item.ImagePath);
                    if (image != null)
                    {
                        button.Image = image;
                        button.ImageAlign = ContentAlignment.MiddleLeft;
                        button.TextImageRelation = TextImageRelation.ImageBeforeText;
                        button.TextAlign = ContentAlignment.MiddleRight;
                        button.Padding = new Padding(6, 0, 6, 0);
                    }
                }
                button.Click += BackstageFooterButton_Click;
                _backstageFooter.Controls.Add(button);
            }
        }

        private void ClearBackstageFooterActions()
        {
            var controls = _backstageFooter.Controls.Cast<Control>().ToList();
            foreach (var control in controls)
            {
                if (control is Button button)
                {
                    button.Click -= BackstageFooterButton_Click;
                }
                control.Dispose();
            }
            _backstageFooter.Controls.Clear();
        }

        private void BackstageFooterButton_Click(object? sender, EventArgs e)
        {
            if (sender is not Button button || button.Tag is not SimpleItem action)
            {
                return;
            }

            BackstageCommandInvoked?.Invoke(this, new BackstageCommandInvokedEventArgs(_backstageFooterSection, action));
        }

        private void BackstageNavList_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_backstageNavList.SelectedIndex < 0)
            {
                return;
            }

            ShowBackstageSection(_backstageNavList.SelectedIndex);
        }

        private void BackstageActionButton_Click(object? sender, EventArgs e)
        {
            if (sender is not Button button || button.Tag is not SimpleItem action) return;
            if (_activeBackstageIndex < 0 || !_backstageSectionMap.TryGetValue(_activeBackstageIndex, out var section)) return;
            if (IsBackstageSectionHeader(action)) return;

            if (action.Children.Count > 0)
            {
                var menu = new ContextMenuStrip();
                BuildBackstageChildMenu(menu.Items, section, action.Children);
                menu.Closed += (_, __) => menu.Dispose();
                var screenPoint = button.PointToScreen(new Point(button.Width - 2, button.Height));
                menu.Show(screenPoint);
                return;
            }

            BackstageCommandInvoked?.Invoke(this, new BackstageCommandInvokedEventArgs(section, action));
        }

        private void BackstageActionButton_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (sender is not Button button || button.Tag is not SimpleItem action) return;
            if (IsBackstageSectionHeader(action)) return;

            string key = GetMergeKey(action);
            bool isPinned = IsPinnedBackstageItem(action);
            bool isRecent = IsRecentBackstageItem(action);
            var menu = new ContextMenuStrip();
            menu.Font = BeepThemesManager.ToFont(_theme.CommandTypography);

            if (isPinned)
            {
                menu.Items.Add("Unpin from list", null, (_, __) => UnpinBackstageItem(key, moveToRecent: true));
            }
            else
            {
                menu.Items.Add("Pin to list", null, (_, __) => PinBackstageItem(action));
            }

            if (!isRecent)
            {
                menu.Items.Add("Add to recent", null, (_, __) => AddBackstageRecentItem(action));
            }
            else
            {
                menu.Items.Add("Remove from recent", null, (_, __) => RemoveBackstageItemByKey(_backstageRecentItems, key));
            }

            menu.Closed += (_, __) => menu.Dispose();
            menu.Show(button, e.Location);
        }

        private static bool IsBackstageSectionHeader(SimpleItem action)
        {
            if (action == null)
            {
                return false;
            }

            if (!action.IsEnabled && action.Children.Count == 0)
            {
                string text = GetDisplayText(action);
                return text.Equals("Pinned", StringComparison.OrdinalIgnoreCase) ||
                       text.Equals("Recent", StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        private bool IsPinnedBackstageItem(SimpleItem action)
        {
            string key = GetMergeKey(action);
            return FindBackstageItemIndexByKey(_backstagePinnedItems, key) >= 0;
        }

        private bool IsRecentBackstageItem(SimpleItem action)
        {
            string key = GetMergeKey(action);
            return FindBackstageItemIndexByKey(_backstageRecentItems, key) >= 0;
        }

        private static bool TryGetBackstageItemTimestamp(SimpleItem action, out DateTime openedUtc)
        {
            openedUtc = default;
            if (action == null)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(action.SubText3) &&
                DateTime.TryParse(action.SubText3, out var parsed))
            {
                openedUtc = parsed.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(parsed, DateTimeKind.Utc)
                    : parsed.ToUniversalTime();
                return true;
            }

            return false;
        }

        private string FormatBackstageTimestamp(SimpleItem action, DateTime openedUtc)
        {
            if (_backstageTimestampFormatter != null)
            {
                try
                {
                    string custom = _backstageTimestampFormatter.Invoke(action, openedUtc);
                    if (!string.IsNullOrWhiteSpace(custom))
                    {
                        return custom.Trim();
                    }
                }
                catch
                {
                }
            }

            if (!_backstageUseRelativeTimestamps)
            {
                return openedUtc.ToLocalTime().ToString("g");
            }

            return FormatRelativeTime(openedUtc);
        }

        private static string FormatRelativeTime(DateTime openedUtc)
        {
            var elapsed = DateTime.UtcNow - openedUtc;
            if (elapsed < TimeSpan.Zero) elapsed = TimeSpan.Zero;

            if (elapsed.TotalMinutes < 1) return "now";
            if (elapsed.TotalHours < 1) return $"{Math.Max(1, (int)elapsed.TotalMinutes)}m";
            if (elapsed.TotalDays < 1) return $"{Math.Max(1, (int)elapsed.TotalHours)}h";
            if (elapsed.TotalDays < 7) return $"{Math.Max(1, (int)elapsed.TotalDays)}d";
            if (elapsed.TotalDays < 30) return $"{Math.Max(1, (int)(elapsed.TotalDays / 7))}w";
            if (elapsed.TotalDays < 365) return $"{Math.Max(1, (int)(elapsed.TotalDays / 30))}mo";
            return $"{Math.Max(1, (int)(elapsed.TotalDays / 365))}y";
        }

        private void BuildBackstageChildMenu(ToolStripItemCollection parent, SimpleItem section, IEnumerable<SimpleItem> nodes)
        {
            foreach (var node in nodes.Where(IsVisibleNode))
            {
                if (node.IsSeparator)
                {
                    parent.Add(new ToolStripSeparator());
                    continue;
                }

                var item = new ToolStripMenuItem(GetDisplayText(node), CreateBackstageImage(node.ImagePath))
                {
                    Enabled = node.IsEnabled,
                    Checked = node.IsChecked,
                    CheckOnClick = node.IsCheckable,
                    Font = BeepThemesManager.ToFont(_theme.CommandTypography),
                    ToolTipText = BuildToolTip(node)
                };
                item.Click += (_, __) => BackstageCommandInvoked?.Invoke(this, new BackstageCommandInvokedEventArgs(section, node));
                parent.Add(item);
                RibbonAccessibilityHelper.ApplyCommandAccessibility(item, node, GetDisplayText(node), AccessibleRole.MenuItem);

                if (node.Children.Count > 0)
                {
                    BuildBackstageChildMenu(item.DropDownItems, section, node.Children);
                }
            }
        }

        private Image? CreateBackstageImage(string? imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return null;
            }

            int size = 16;
            var bmp = new Bitmap(size, size);
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.Clear(Color.Transparent);
            using var clipPath = new GraphicsPath();
            clipPath.AddRectangle(new Rectangle(0, 0, size, size));
            StyledImagePainter.PaintWithTint(g, clipPath, imagePath, _theme.IconColor, 1f);
            _backstageGeneratedImages.Add(bmp);
            return bmp;
        }

        private void ClearBackstageActions()
        {
            var controls = _backstageActions.Controls.Cast<Control>().ToList();
            foreach (var control in controls)
            {
                foreach (var button in EnumerateButtons(control))
                {
                    button.Click -= BackstageActionButton_Click;
                    button.MouseUp -= BackstageActionButton_MouseUp;
                    button.Click -= BackstageInlinePinButton_Click;
                }
                control.Dispose();
            }
            _backstageActions.Controls.Clear();
            DisposeBackstageImages();
        }

        private static IEnumerable<Button> EnumerateButtons(Control root)
        {
            if (root is Button ownButton)
            {
                yield return ownButton;
            }

            foreach (Control child in root.Controls)
            {
                foreach (var nested in EnumerateButtons(child))
                {
                    yield return nested;
                }
            }
        }

        private void DisposeBackstageImages()
        {
            foreach (var image in _backstageGeneratedImages)
            {
                image.Dispose();
            }
            _backstageGeneratedImages.Clear();
        }

        private void InitializeSearchControls()
        {
            _searchBox.Name = "RibbonSearchBox";
            _searchBox.ToolTipText = "Search commands (Ctrl+F)";
            _searchBox.BorderStyle = BorderStyle.FixedSingle;
            _searchBox.TextChanged += (_, __) =>
            {
                if (_searchMode != RibbonSearchMode.Off)
                {
                    RunSearch(_searchBox.Text);
                }
            };
            _searchBox.Enter += (_, __) =>
            {
                if (_showSearchHistorySuggestions && string.IsNullOrWhiteSpace(_searchBox.Text))
                {
                    ApplySearchHistorySuggestions();
                }
            };
            _searchBox.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Down)
                {
                    if (string.IsNullOrWhiteSpace(_searchBox.Text) && _showSearchHistorySuggestions)
                    {
                        ApplySearchHistorySuggestions();
                    }
                    MoveSearchSelection(1);
                    e.SuppressKeyPress = true;
                    return;
                }

                if (e.KeyCode == Keys.Up)
                {
                    MoveSearchSelection(-1);
                    e.SuppressKeyPress = true;
                    return;
                }

                if (e.KeyCode == Keys.Escape)
                {
                    HideSearchResultsDropDown();
                    e.SuppressKeyPress = true;
                    return;
                }

                if (e.KeyCode == Keys.Enter)
                {
                    if (TryExecuteSelectedSearchResult())
                    {
                        e.SuppressKeyPress = true;
                        return;
                    }

                    RunSearch(_searchBox.Text);
                    e.SuppressKeyPress = true;
                }
            };

            _searchResultsButton.Name = "RibbonSearchResultsButton";
            _searchResultsButton.ToolTipText = "Search results";
            _searchResultsButton.Visible = false;
            _searchResultsButton.DropDownClosed += (_, __) => _searchResultSelectionIndex = -1;
            EnsureSearchControls();
        }

        private void EnsureSearchControls()
        {
            bool enabled = _searchMode != RibbonSearchMode.Off;
            _searchBox.Visible = enabled;
            _searchResultsButton.Visible = enabled;

            if (!enabled)
            {
                HideSearchResultsDropDown();
                _searchBox.Text = string.Empty;
                _searchResultsButton.DropDownItems.Clear();
                _searchResults.Clear();
                _searchResultSelectionIndex = -1;
            }

            RebuildQuickAccessToolbar();
            ApplySearchAccessibility();
            ApplyPaneTabOrder();
        }

        private void ApplySearchAccessibility()
        {
            _searchResultsButton.AccessibleName = "Ribbon search results";
            _searchResultsButton.AccessibleDescription = "Opens ranked search results and recent queries.";
            _searchResultsButton.AccessibleRole = AccessibleRole.ButtonDropDown;
            _searchResultsButton.AccessibleDefaultActionDescription = "Open search results";

            if (_searchBox.Control != null)
            {
                RibbonAccessibilityHelper.ApplyControlAccessibility(
                    _searchBox.Control,
                    "Ribbon search box",
                    "Search commands in the ribbon. Press Enter to execute the top result.",
                    AccessibleRole.Text);
                _searchBox.Control.TabStop = _searchMode != RibbonSearchMode.Off;
                _searchBox.Control.RightToLeft = _ribbonRightToLeft ? RightToLeft.Yes : RightToLeft.No;
            }
        }

        private void ApplyPaneTabOrder()
        {
            _quickAccess.TabStop = true;
            _tabs.TabStop = true;
            _quickAccess.TabIndex = 0;
            _tabs.TabIndex = 1;

            _backstageNavList.TabStop = true;
            _backstageActions.TabStop = true;
            _backstageFooter.TabStop = true;
            _backstageNavList.TabIndex = 0;
            _backstageActions.TabIndex = 1;
            _backstageFooter.TabIndex = 2;

            if (_searchBox.Control != null)
            {
                _searchBox.Control.TabIndex = 2;
            }
        }

        private async void RunSearch(string? rawQuery)
        {
            var sw = Stopwatch.StartNew();
            int version = ++_searchRequestVersion;
            string query = rawQuery?.Trim() ?? string.Empty;

            if (_searchMode == RibbonSearchMode.Off)
            {
                ApplySearchResults([]);
                RaiseSearchExecuted(query, 0, providerUsed: false, providerFailed: false, usedLocalFallback: false, sw.ElapsedMilliseconds);
                return;
            }

            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                if (_showSearchHistorySuggestions)
                {
                    ApplySearchHistorySuggestions();
                }
                else
                {
                    ApplySearchResults([]);
                }
                RaiseSearchExecuted(query, 0, providerUsed: false, providerFailed: false, usedLocalFallback: false, sw.ElapsedMilliseconds);
                return;
            }

            if (_searchMode == RibbonSearchMode.SmartService && _searchProvider != null)
            {
                try
                {
                    var results = await _searchProvider.SearchAsync(query, _commandLookup.Values.Where(n => n.IsVisible));
                    if (version != _searchRequestVersion) return;
                    var list = results?.ToList() ?? [];
                    ApplySearchResults(list);
                    RecordSearchQuery(query);
                    RaiseSearchExecuted(query, list.Count, providerUsed: true, providerFailed: false, usedLocalFallback: false, sw.ElapsedMilliseconds);
                    return;
                }
                catch
                {
                    // Fallback to local search if provider fails.
                    if (version != _searchRequestVersion) return;
                    var fallback = QueryLocalSearch(query);
                    ApplySearchResults(fallback);
                    RecordSearchQuery(query);
                    RaiseSearchExecuted(query, fallback.Count, providerUsed: true, providerFailed: true, usedLocalFallback: true, sw.ElapsedMilliseconds);
                    return;
                }
            }

            var localResults = QueryLocalSearch(query);
            ApplySearchResults(localResults);
            RecordSearchQuery(query);
            RaiseSearchExecuted(query, localResults.Count, providerUsed: false, providerFailed: false, usedLocalFallback: false, sw.ElapsedMilliseconds);
        }

        private void RunLocalSearch(string? rawQuery)
        {
            var sw = Stopwatch.StartNew();
            string query = rawQuery?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                if (_showSearchHistorySuggestions)
                {
                    ApplySearchHistorySuggestions();
                }
                else
                {
                    ApplySearchResults([]);
                }
                RaiseSearchExecuted(query, 0, providerUsed: false, providerFailed: false, usedLocalFallback: false, sw.ElapsedMilliseconds);
                return;
            }

            var results = QueryLocalSearch(query);
            ApplySearchResults(results);
            RecordSearchQuery(query);
            RaiseSearchExecuted(query, results.Count, providerUsed: false, providerFailed: false, usedLocalFallback: false, sw.ElapsedMilliseconds);
        }

        private List<SimpleItem> QueryLocalSearch(string query)
        {
            var map = new Dictionary<string, SimpleItem>(StringComparer.OrdinalIgnoreCase);

            foreach (var node in _commandLookup.Values)
            {
                AddSearchCandidate(map, node);
            }

            if (_searchIncludeBackstage)
            {
                foreach (var node in EnumerateBackstageNodes())
                {
                    AddSearchCandidate(map, node);
                }
            }

            return RibbonSearchIndex.RankCommands(query, map.Values, _searchCommandUsage, _searchMaxResults, ResolveSearchScoreBoost);
        }

        public RibbonSearchBenchmarkReport RunLocalSearchBenchmark(IEnumerable<string> queries, bool? includeBackstage = null)
        {
            var map = new Dictionary<string, SimpleItem>(StringComparer.OrdinalIgnoreCase);
            foreach (var node in _commandLookup.Values)
            {
                AddSearchCandidate(map, node);
            }

            bool includeBackstageNodes = includeBackstage ?? _searchIncludeBackstage;
            if (includeBackstageNodes)
            {
                foreach (var node in EnumerateBackstageNodes())
                {
                    AddSearchCandidate(map, node);
                }
            }

            return RibbonSearchBenchmark.Run(queries, map.Values, _searchCommandUsage, _searchMaxResults, ResolveSearchScoreBoost);
        }

        public RibbonAccessibilityAuditReport RunAccessibilityAudit()
        {
            return RibbonAccessibilityAudit.Audit(this);
        }

        private IEnumerable<SimpleItem> EnumerateBackstageNodes()
        {
            foreach (var root in _backstageItems)
            {
                foreach (var node in EnumerateNodeTree(root))
                {
                    yield return node;
                }
            }

            foreach (var root in _backstageRecentItems)
            {
                foreach (var node in EnumerateNodeTree(root))
                {
                    yield return node;
                }
            }

            foreach (var root in _backstagePinnedItems)
            {
                foreach (var node in EnumerateNodeTree(root))
                {
                    yield return node;
                }
            }
        }

        private static IEnumerable<SimpleItem> EnumerateNodeTree(SimpleItem root)
        {
            if (root == null)
            {
                yield break;
            }

            var stack = new Stack<SimpleItem>();
            stack.Push(root);
            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (!current.IsVisible)
                {
                    continue;
                }

                yield return current;
                for (int i = current.Children.Count - 1; i >= 0; i--)
                {
                    stack.Push(current.Children[i]);
                }
            }
        }

        private static void AddSearchCandidate(IDictionary<string, SimpleItem> map, SimpleItem node)
        {
            if (!IsSearchableNode(node))
            {
                return;
            }

            string key = GetMergeKey(node);
            if (string.IsNullOrWhiteSpace(key))
            {
                key = GetCommandKey(node);
            }

            if (!map.ContainsKey(key))
            {
                map[key] = node;
            }
        }

        private static bool IsSearchableNode(SimpleItem node)
        {
            if (node.IsSeparator || !node.IsVisible)
            {
                return false;
            }

            if (node.Children.Count == 0)
            {
                return true;
            }

            return !string.IsNullOrWhiteSpace(node.ActionID) ||
                   !string.IsNullOrWhiteSpace(node.ReferenceID) ||
                   !string.IsNullOrWhiteSpace(node.MethodName);
        }

        public void SetSearchCategoryBoost(string categoryKey, int scoreBoost)
        {
            if (string.IsNullOrWhiteSpace(categoryKey))
            {
                return;
            }

            _searchCategoryBoosts[categoryKey.Trim()] = Math.Clamp(scoreBoost, -200, 300);
        }

        public void ClearSearchCategoryBoosts()
        {
            _searchCategoryBoosts.Clear();
        }

        private int ResolveSearchScoreBoost(SimpleItem item)
        {
            int boost = 0;
            if (_searchScoreBoostProvider != null)
            {
                boost += _searchScoreBoostProvider(item);
            }

            string categoryKey = item.Category.ToString();
            if (!string.IsNullOrWhiteSpace(categoryKey) &&
                _searchCategoryBoosts.TryGetValue(categoryKey, out int categoryBoost))
            {
                boost += categoryBoost;
            }

            if (!string.IsNullOrWhiteSpace(item.GroupName) &&
                _searchCategoryBoosts.TryGetValue($"group:{item.GroupName}", out int groupBoost))
            {
                boost += groupBoost;
            }

            return boost;
        }

        private void RaiseSearchExecuted(string query, int resultCount, bool providerUsed, bool providerFailed, bool usedLocalFallback, long durationMs = 0)
        {
            SearchExecuted?.Invoke(this, new RibbonSearchExecutedEventArgs(query, _searchMode, resultCount, providerUsed, providerFailed, usedLocalFallback));
            _searchTelemetry?.OnSearchExecuted(new RibbonSearchTelemetryEvent
            {
                Query = query,
                Mode = _searchMode,
                ResultCount = resultCount,
                ProviderUsed = providerUsed,
                ProviderFailed = providerFailed,
                UsedLocalFallback = usedLocalFallback,
                DurationMs = Math.Max(0, durationMs),
                ExecutedAtUtc = DateTime.UtcNow
            });
        }

        private void ApplySearchResults(List<SimpleItem> results)
        {
            _searchResults.Clear();
            _searchResultsButton.DropDownItems.Clear();
            _searchResultSelectionIndex = -1;

            if (results.Count == 0)
            {
                _searchResultsButton.Text = "Find";
                return;
            }

            _searchResults.AddRange(results);
            _searchResultsButton.Text = $"Find ({results.Count})";

            foreach (var command in results)
            {
                var item = new ToolStripMenuItem(GetDisplayText(command), CreateCommandImage(command.ImagePath, true))
                {
                    ToolTipText = BuildToolTip(command),
                    Font = BeepThemesManager.ToFont(_theme.CommandTypography)
                };
                RibbonAccessibilityHelper.ApplyCommandAccessibility(item, command, GetDisplayText(command), AccessibleRole.MenuItem);
                item.Click += (_, __) => InvokeSearchResult(command, item);
                item.MouseEnter += (_, __) => _searchResultSelectionIndex = _searchResultsButton.DropDownItems.IndexOf(item);
                _searchResultsButton.DropDownItems.Add(item);
            }
        }

        private void ApplySearchHistorySuggestions()
        {
            _searchResults.Clear();
            _searchResultsButton.DropDownItems.Clear();
            _searchResultSelectionIndex = -1;

            if (_searchHistory.Count == 0)
            {
                _searchResultsButton.Text = "Find";
                HideSearchResultsDropDown();
                return;
            }

            _searchResultsButton.Text = $"Recent ({_searchHistory.Count})";
            foreach (var query in _searchHistory.Take(Math.Min(10, _searchHistory.Count)))
            {
                var item = new ToolStripMenuItem(query)
                {
                    Font = BeepThemesManager.ToFont(_theme.CommandTypography),
                    ToolTipText = "Run recent search"
                };
                item.AccessibleName = $"Recent query {query}";
                item.AccessibleDescription = "Execute this recent ribbon search query.";
                item.AccessibleRole = AccessibleRole.MenuItem;
                item.Click += (_, __) =>
                {
                    _searchBox.Text = query;
                    _searchBox.SelectionStart = _searchBox.Text.Length;
                    RunSearch(query);
                };
                item.MouseUp += (_, e) =>
                {
                    if (e.Button != MouseButtons.Right)
                    {
                        return;
                    }

                    if (RemoveSearchHistoryItem(query))
                    {
                        ApplySearchHistorySuggestions();
                    }
                };
                item.MouseEnter += (_, __) => _searchResultSelectionIndex = _searchResultsButton.DropDownItems.IndexOf(item);
                _searchResultsButton.DropDownItems.Add(item);
            }

            _searchResultsButton.DropDownItems.Add(new ToolStripSeparator());

            var manage = new ToolStripMenuItem("Manage history...")
            {
                Font = BeepThemesManager.ToFont(_theme.CommandTypography)
            };
            manage.AccessibleName = "Manage search history";
            manage.AccessibleDescription = "Open the dialog to remove or clear search history entries.";
            manage.AccessibleRole = AccessibleRole.MenuItem;
            manage.Click += (_, __) => ShowSearchHistoryManager();
            _searchResultsButton.DropDownItems.Add(manage);

            var clear = new ToolStripMenuItem("Clear history")
            {
                Font = BeepThemesManager.ToFont(_theme.CommandTypography)
            };
            clear.AccessibleName = "Clear search history";
            clear.AccessibleDescription = "Remove all saved search queries.";
            clear.AccessibleRole = AccessibleRole.MenuItem;
            clear.Click += (_, __) =>
            {
                ClearSearchHistory();
                ApplySearchHistorySuggestions();
            };
            _searchResultsButton.DropDownItems.Add(clear);
        }

        private void ShowSearchHistoryManager()
        {
            using var dialog = new Form
            {
                Text = "Search History",
                StartPosition = FormStartPosition.CenterParent,
                Size = new Size(460, 340),
                MinimizeBox = false,
                MaximizeBox = false,
                FormBorderStyle = FormBorderStyle.SizableToolWindow,
                BackColor = _theme.GroupBack,
                ForeColor = _theme.Text,
                Font = BeepThemesManager.ToFont(_theme.CommandTypography)
            };

            var list = new ListBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Font = BeepThemesManager.ToFont(_theme.CommandTypography),
                BackColor = _theme.TabActiveBack,
                ForeColor = _theme.Text
            };
            list.Items.AddRange(_searchHistory.Cast<object>().ToArray());

            var footer = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                Padding = new Padding(6, 4, 6, 4),
                BackColor = _theme.GroupBack
            };

            var close = new Button
            {
                Text = "Close",
                Width = 88,
                Height = 28,
                FlatStyle = FlatStyle.Flat,
                Font = BeepThemesManager.ToFont(_theme.CommandTypography),
                BackColor = _theme.TabActiveBack,
                ForeColor = _theme.Text
            };
            close.FlatAppearance.BorderColor = _theme.GroupBorder;
            close.Click += (_, __) => dialog.Close();

            var clear = new Button
            {
                Text = "Clear All",
                Width = 88,
                Height = 28,
                FlatStyle = FlatStyle.Flat,
                Font = BeepThemesManager.ToFont(_theme.CommandTypography),
                BackColor = _theme.TabActiveBack,
                ForeColor = _theme.Text
            };
            clear.FlatAppearance.BorderColor = _theme.GroupBorder;
            clear.Click += (_, __) =>
            {
                ClearSearchHistory();
                list.Items.Clear();
                ApplySearchHistorySuggestions();
            };

            var remove = new Button
            {
                Text = "Remove",
                Width = 88,
                Height = 28,
                FlatStyle = FlatStyle.Flat,
                Font = BeepThemesManager.ToFont(_theme.CommandTypography),
                BackColor = _theme.TabActiveBack,
                ForeColor = _theme.Text
            };
            remove.FlatAppearance.BorderColor = _theme.GroupBorder;
            remove.Click += (_, __) =>
            {
                if (list.SelectedItem is not string selected)
                {
                    return;
                }

                if (RemoveSearchHistoryItem(selected))
                {
                    list.Items.Remove(selected);
                    ApplySearchHistorySuggestions();
                }
            };

            footer.Controls.Add(close);
            footer.Controls.Add(clear);
            footer.Controls.Add(remove);

            dialog.Controls.Add(list);
            dialog.Controls.Add(footer);

            var owner = FindForm();
            if (owner != null)
            {
                dialog.ShowDialog(owner);
            }
            else
            {
                dialog.ShowDialog();
            }
        }

        public void ClearSearchHistory()
        {
            _searchHistory.Clear();
            _searchCommandUsage.Clear();
        }

        public bool RemoveSearchHistoryItem(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return false;
            }

            int index = _searchHistory.FindIndex(x => x.Equals(query.Trim(), StringComparison.OrdinalIgnoreCase));
            if (index < 0)
            {
                return false;
            }

            _searchHistory.RemoveAt(index);
            return true;
        }

        public void SaveSearchHistoryTo(string file)
        {
            try
            {
                File.WriteAllLines(file, _searchHistory);
            }
            catch
            {
            }
        }

        public void LoadSearchHistoryFrom(string file)
        {
            try
            {
                if (!File.Exists(file))
                {
                    return;
                }

                var lines = File.ReadAllLines(file)
                    .Where(l => !string.IsNullOrWhiteSpace(l))
                    .Select(l => l.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Take(_searchHistoryLimit)
                    .ToList();

                _searchHistory.Clear();
                _searchHistory.AddRange(lines);
            }
            catch
            {
            }
        }

        private void RecordSearchQuery(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return;
            }

            string normalized = query.Trim();
            if (normalized.Length < 2)
            {
                return;
            }

            int existingIndex = _searchHistory.FindIndex(q => q.Equals(normalized, StringComparison.OrdinalIgnoreCase));
            if (existingIndex >= 0)
            {
                _searchHistory.RemoveAt(existingIndex);
            }

            _searchHistory.Insert(0, normalized);
            while (_searchHistory.Count > _searchHistoryLimit)
            {
                _searchHistory.RemoveAt(_searchHistory.Count - 1);
            }
        }

        private void RecordSearchCommandUsage(SimpleItem command)
        {
            string key = GetCommandKey(command);
            if (_searchCommandUsage.TryGetValue(key, out int score))
            {
                _searchCommandUsage[key] = Math.Min(200, score + 1);
            }
            else
            {
                _searchCommandUsage[key] = 1;
            }
        }

        private void InvokeSearchResult(SimpleItem command, ToolStripItem source)
        {
            RecordSearchQuery(_searchBox.Text);
            RecordSearchCommandUsage(command);
            HideSearchResultsDropDown();
            RaiseCommandInvoked(command, source);
        }

        private bool TryExecuteSelectedSearchResult()
        {
            if (_searchResultsButton.DropDownItems.Count == 0)
            {
                return false;
            }

            int index = _searchResultSelectionIndex;
            if (index < 0)
            {
                index = 0;
            }

            if (index >= _searchResultsButton.DropDownItems.Count)
            {
                return false;
            }

            if (_searchResults.Count > index)
            {
                var command = _searchResults[index];
                var source = _searchResultsButton.DropDownItems[index];
                InvokeSearchResult(command, source);
                return true;
            }

            if (_searchResultsButton.DropDownItems[index] is ToolStripMenuItem menuItem)
            {
                menuItem.PerformClick();
                HideSearchResultsDropDown();
                return true;
            }

            return true;
        }

        private void MoveSearchSelection(int delta)
        {
            if (_searchResultsButton.DropDownItems.Count == 0)
            {
                return;
            }

            if (!_searchResultsButton.DropDown.Visible)
            {
                _searchResultsButton.ShowDropDown();
            }

            int count = _searchResultsButton.DropDownItems.Count;
            if (_searchResultSelectionIndex < 0)
            {
                _searchResultSelectionIndex = delta > 0 ? 0 : count - 1;
            }
            else
            {
                _searchResultSelectionIndex += delta;
                if (_searchResultSelectionIndex < 0)
                {
                    _searchResultSelectionIndex = count - 1;
                }
                else if (_searchResultSelectionIndex >= count)
                {
                    _searchResultSelectionIndex = 0;
                }
            }

            if (_searchResultSelectionIndex >= 0 && _searchResultSelectionIndex < count)
            {
                _searchResultsButton.DropDownItems[_searchResultSelectionIndex].Select();
            }
        }

        private void HideSearchResultsDropDown()
        {
            if (_searchResultsButton.DropDown.Visible)
            {
                _searchResultsButton.HideDropDown();
            }
            _searchResultSelectionIndex = -1;
        }

        private void RefreshKeyTips()
        {
            _keyTips.Clear();
            _keyTipLookup.Clear();

            if (!_enableKeyTips)
            {
                return;
            }

            _keyTips[_backstageButton] = "F";
            _keyTipLookup["F"] = _backstageButton;

            int qIndex = 1;
            foreach (ToolStripItem item in _quickAccess.Items)
            {
                if (!CanAssignKeyTip(item) || item == _backstageButton) continue;
                if (qIndex > 9) break;
                string keyTip = qIndex.ToString();
                _keyTips[item] = keyTip;
                _keyTipLookup[keyTip] = item;
                qIndex++;
            }

            var page = _tabs.SelectedTab;
            if (page == null) return;

            int alphaIndex = 0;
            foreach (var group in page.Controls.OfType<BeepRibbonGroup>())
            {
                foreach (ToolStripItem item in group.Items)
                {
                    if (!CanAssignKeyTip(item)) continue;
                    string keyTip = GetAlphaKeyTip(alphaIndex++);
                    _keyTips[item] = keyTip;
                    _keyTipLookup[keyTip] = item;
                }
            }
        }

        private static bool CanAssignKeyTip(ToolStripItem item)
        {
            return item is not ToolStripSeparator &&
                   item.Available &&
                   item.Visible &&
                   item.Enabled &&
                   item is not ToolStripTextBox;
        }

        private static string GetAlphaKeyTip(int index)
        {
            index = Math.Max(0, index);
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            if (index < chars.Length)
            {
                return chars[index].ToString();
            }

            int first = index / chars.Length - 1;
            int second = index % chars.Length;
            first = Math.Clamp(first, 0, chars.Length - 1);
            return $"{chars[first]}{chars[second]}";
        }

        private void ShowKeyTips()
        {
            if (!_enableKeyTips) return;
            RefreshKeyTips();
            if (_keyTips.Count == 0) return;

            _keyTipsVisible = true;
            _keyTipInputBuffer = string.Empty;
            foreach (var kv in _keyTips)
            {
                var item = kv.Key;
                var owner = item.Owner;
                if (owner == null) continue;
                var bounds = item.Bounds;
                var point = new Point(bounds.Left + Math.Max(2, bounds.Width / 2 - 8), Math.Max(0, bounds.Top - 18));
                _keyTipToolTip.Show(kv.Value, owner, point, 30000);
            }
        }

        private void HideKeyTips()
        {
            if (!_keyTipsVisible) return;
            var owners = _keyTips.Keys
                .Select(k => k.Owner)
                .Where(o => o != null)
                .Distinct()
                .ToList();
            foreach (var owner in owners)
            {
                _keyTipToolTip.Hide(owner!);
            }

            _keyTipInputBuffer = string.Empty;
            _keyTipsVisible = false;
        }

        private void RefreshKeyTipsVisibility()
        {
            if (!_keyTipsVisible) return;
            HideKeyTips();
            ShowKeyTips();
        }

        private bool TryInvokeKeyTip(Keys keyData)
        {
            string token = NormalizeKeyToken(keyData);
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            if ((DateTime.UtcNow - _lastKeyTipInput).TotalSeconds > 1.6)
            {
                _keyTipInputBuffer = string.Empty;
            }

            _lastKeyTipInput = DateTime.UtcNow;
            _keyTipInputBuffer += token;

            if (_keyTipLookup.TryGetValue(_keyTipInputBuffer, out var exactItem))
            {
                InvokeToolStripItem(exactItem);
                HideKeyTips();
                return true;
            }

            bool hasPrefix = _keyTipLookup.Keys.Any(k => k.StartsWith(_keyTipInputBuffer, StringComparison.OrdinalIgnoreCase));
            if (hasPrefix)
            {
                return true;
            }

            _keyTipInputBuffer = token;
            if (_keyTipLookup.TryGetValue(_keyTipInputBuffer, out exactItem))
            {
                InvokeToolStripItem(exactItem);
                HideKeyTips();
                return true;
            }

            _keyTipInputBuffer = string.Empty;
            return false;
        }

        private static string NormalizeKeyToken(Keys keyData)
        {
            Keys keyCode = keyData & Keys.KeyCode;
            if (keyCode >= Keys.A && keyCode <= Keys.Z)
            {
                return keyCode.ToString();
            }

            if (keyCode >= Keys.D0 && keyCode <= Keys.D9)
            {
                return ((int)(keyCode - Keys.D0)).ToString();
            }

            if (keyCode >= Keys.NumPad0 && keyCode <= Keys.NumPad9)
            {
                return ((int)(keyCode - Keys.NumPad0)).ToString();
            }

            return string.Empty;
        }

        private void InvokeToolStripItem(ToolStripItem item)
        {
            switch (item)
            {
                case ToolStripButton button:
                    button.PerformClick();
                    break;
                case ToolStripMenuItem menuItem:
                    menuItem.PerformClick();
                    break;
                case ToolStripDropDownButton dropDownButton:
                    if (dropDownButton.HasDropDownItems)
                    {
                        dropDownButton.ShowDropDown();
                    }
                    else
                    {
                        dropDownButton.PerformClick();
                    }
                    break;
                default:
                    item.PerformClick();
                    break;
            }
        }

        private void ContextHeader_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(_theme.Background);

            for (int gi = 0; gi < _contextGroups.Count; gi++)
            {
                var grp = _contextGroups[gi];
                if (!grp.Visible || grp.Pages.Count == 0) continue;

                Rectangle? left = null;
                Rectangle? right = null;
                for (int i = 0; i < _tabs.TabCount; i++)
                {
                    var page = _tabs.TabPages[i];
                    if (!_pageToGroup.TryGetValue(page, out var gref) || gref != grp) continue;
                    var r = _tabs.GetTabRect(i);
                    if (!left.HasValue || r.Left < left.Value.Left) left = r;
                    if (!right.HasValue || r.Right > right.Value.Right) right = r;
                }

                if (!left.HasValue || !right.HasValue) continue;
                var band = new Rectangle(left.Value.Left, 0, right.Value.Right - left.Value.Left, _contextHeader.Height - 1);
                int alpha = Math.Clamp((int)(120 * _contextTransitionProgress), 30, 180);
                using var b = new SolidBrush(Color.FromArgb(alpha, grp.Color));
                using var p = new Pen(grp.Color);
                using var textBrush = new SolidBrush(_theme.Text);
                g.FillRectangle(b, band);
                g.DrawRectangle(p, band);
                g.DrawString(grp.Name, _contextHeader.Font, textBrush, new PointF(band.Left + 6, 2));
            }
        }

        private void ContextTransitionTimer_Tick(object? sender, EventArgs e)
        {
            _contextTransitionProgress += _contextTransitionTimer.Interval / (float)Math.Max(1, _contextTransitionEffectiveDurationMs);
            if (_contextTransitionProgress >= 1f)
            {
                _contextTransitionProgress = 1f;
                _contextTransitionTimer.Stop();
            }

            _contextHeader.Invalidate();
        }

        private void StartContextTransition()
        {
            if (!ShouldAnimateTransitions() || !_enableContextTransitions)
            {
                _contextTransitionProgress = 1f;
                _contextHeader.Invalidate();
                return;
            }

            _contextTransitionEffectiveDurationMs = GetEffectiveTransitionDurationMs(_contextTransitionDurationMs, forBackstage: false);
            _contextTransitionTimer.Interval = Math.Clamp(_contextTransitionEffectiveDurationMs / 12, 10, 24);
            _contextTransitionProgress = 0f;
            _contextTransitionTimer.Stop();
            _contextTransitionTimer.Start();
        }

        private void BackstageButton_DropDownOpening(object? sender, EventArgs e)
        {
            BeginBackstageOpenTransition();
        }

        private void BackstageDropDown_Closed(object? sender, ToolStripDropDownClosedEventArgs e)
        {
            _backstageTransitionTimer.Stop();
            _backstageHost.Size = _backstagePanelContent.Size;
            _backstageDropDown.Size = _backstagePanelContent.Size;
        }

        private void BeginBackstageOpenTransition()
        {
            if (!ShouldAnimateTransitions() || !_enableBackstageTransitions)
            {
                var size = _backstagePanelContent.Size;
                _backstageHost.Size = size;
                _backstageDropDown.Size = size;
                return;
            }

            _backstageTransitionEffectiveDurationMs = GetEffectiveTransitionDurationMs(_backstageTransitionDurationMs, forBackstage: true);
            _backstageTransitionTimer.Interval = Math.Clamp(_backstageTransitionEffectiveDurationMs / 12, 10, 24);
            _backstageTransitionTimer.Stop();
            _backstageTransitionTargetSize = _backstagePanelContent.Size;
            float widthFactor = _density switch
            {
                RibbonDensity.Compact => 0.72f,
                RibbonDensity.Touch => 0.82f,
                _ => 0.76f
            };
            float heightFactor = _density switch
            {
                RibbonDensity.Compact => 0.76f,
                RibbonDensity.Touch => 0.84f,
                _ => 0.78f
            };
            if (_resolvedStylePreset == RibbonStylePreset.HighContrast)
            {
                widthFactor = 0.88f;
                heightFactor = 0.90f;
            }
            _backstageTransitionStartSize = new Size(
                Math.Max(360, (int)(_backstageTransitionTargetSize.Width * widthFactor)),
                Math.Max(220, (int)(_backstageTransitionTargetSize.Height * heightFactor)));

            _backstageHost.Size = _backstageTransitionStartSize;
            _backstageDropDown.Size = _backstageTransitionStartSize;
            _backstageTransitionStartUtc = DateTime.UtcNow;
            _backstageTransitionTimer.Start();
        }

        private void BackstageTransitionTimer_Tick(object? sender, EventArgs e)
        {
            double elapsed = (DateTime.UtcNow - _backstageTransitionStartUtc).TotalMilliseconds;
            double duration = Math.Max(1, _backstageTransitionEffectiveDurationMs);
            double t = Math.Clamp(elapsed / duration, 0, 1);
            // Smooth-step easing for subtle transition.
            double eased = t * t * (3 - 2 * t);

            int width = _backstageTransitionStartSize.Width +
                        (int)Math.Round((_backstageTransitionTargetSize.Width - _backstageTransitionStartSize.Width) * eased);
            int height = _backstageTransitionStartSize.Height +
                         (int)Math.Round((_backstageTransitionTargetSize.Height - _backstageTransitionStartSize.Height) * eased);

            var size = new Size(Math.Max(100, width), Math.Max(100, height));
            _backstageHost.Size = size;
            _backstageDropDown.Size = size;

            if (t >= 1)
            {
                _backstageTransitionTimer.Stop();
                _backstageHost.Size = _backstageTransitionTargetSize;
                _backstageDropDown.Size = _backstageTransitionTargetSize;
            }
        }

        private bool ShouldAnimateTransitions()
        {
            if (_reducedMotion)
            {
                return false;
            }

            if (_respectSystemReducedMotion)
            {
                try
                {
                    if (TryGetSystemAnimationPreference(out bool animationsEnabled) && !animationsEnabled)
                    {
                        return false;
                    }
                }
                catch
                {
                    // no-op: default to enabled
                }
            }

            return true;
        }

        private static bool TryGetSystemAnimationPreference(out bool animationsEnabled)
        {
            animationsEnabled = true;

            // Different WinForms target frameworks expose different animation-related properties.
            // Probe common names via reflection to keep this ribbon control compile-safe everywhere.
            var propertyCandidates = new[]
            {
                "IsMinimizeAnimationEnabled",
                "MinimizeAnimation",
                "IsMenuAnimationEnabled",
                "UIEffects"
            };

            var type = typeof(SystemInformation);
            foreach (var propertyName in propertyCandidates)
            {
                try
                {
                    var prop = type.GetProperty(propertyName);
                    if (prop == null || prop.PropertyType != typeof(bool))
                    {
                        continue;
                    }

                    var value = prop.GetValue(null);
                    if (value is bool enabled)
                    {
                        animationsEnabled = enabled;
                        return true;
                    }
                }
                catch
                {
                    // continue trying other candidates
                }
            }

            return false;
        }

        private int GetEffectiveTransitionDurationMs(int configuredDurationMs, bool forBackstage)
        {
            int baseDuration = Math.Max(50, configuredDurationMs);
            if (!_adaptiveTransitionTiming)
            {
                return baseDuration;
            }

            float densityFactor = _density switch
            {
                RibbonDensity.Compact => 0.86f,
                RibbonDensity.Touch => 1.16f,
                _ => 1f
            };

            float presetFactor = _resolvedStylePreset switch
            {
                RibbonStylePreset.FluentLight => 1.06f,
                RibbonStylePreset.FluentDark => 1.08f,
                RibbonStylePreset.OfficeLight => 0.94f,
                RibbonStylePreset.OfficeDark => 0.96f,
                RibbonStylePreset.HighContrast => 0.78f,
                _ => 1f
            };

            float typeFactor = forBackstage ? 1.08f : 0.94f;
            int effective = (int)Math.Round(baseDuration * densityFactor * presetFactor * typeFactor);
            return Math.Clamp(effective, 40, 420);
        }

        public int AddContextualGroup(string name, Color color)
        {
            var grp = new ContextualGroup { Name = name, Color = color, Visible = false };
            _contextGroups.Add(grp);
            return _contextGroups.Count - 1;
        }

        public TabPage AddContextualTab(int groupId, string title)
        {
            if (groupId < 0 || groupId >= _contextGroups.Count) throw new ArgumentOutOfRangeException(nameof(groupId));
            var grp = _contextGroups[groupId];
            var page = new TabPage(title) { BackColor = _theme.TabActiveBack };
            grp.Pages.Add(page);
            _pageToGroup[page] = grp;
            if (grp.Visible)
            {
                _tabs.TabPages.Add(page);
            }
            _contextHeader.Invalidate();
            return page;
        }

        public void SetContextualGroupVisible(int groupId, bool visible)
        {
            if (groupId < 0 || groupId >= _contextGroups.Count) return;
            var grp = _contextGroups[groupId];
            if (grp.Visible == visible) return;
            grp.Visible = visible;
            if (visible)
            {
                foreach (var p in grp.Pages)
                {
                    if (!_tabs.TabPages.Contains(p)) _tabs.TabPages.Add(p);
                }
            }
            else
            {
                foreach (var p in grp.Pages)
                {
                    if (_tabs.TabPages.Contains(p)) _tabs.TabPages.Remove(p);
                }
            }
            StartContextTransition();
            _contextHeader.Invalidate();
        }

        public void RegisterContextualRule(string contextKey, int groupId)
        {
            if (string.IsNullOrWhiteSpace(contextKey)) return;
            if (groupId < 0 || groupId >= _contextGroups.Count) return;
            _contextualRuleMap[contextKey] = groupId;
        }

        public void ActivateContext(string? contextKey)
        {
            string key = contextKey?.Trim() ?? string.Empty;
            _activeContextKey = key;

            if (_contextGroups.Count == 0)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                for (int i = 0; i < _contextGroups.Count; i++)
                {
                    SetContextualGroupVisible(i, false);
                }
                return;
            }

            bool matched = false;
            if (_contextualRuleMap.TryGetValue(key, out int mappedGroup))
            {
                for (int i = 0; i < _contextGroups.Count; i++)
                {
                    bool visible = i == mappedGroup;
                    SetContextualGroupVisible(i, visible);
                    if (visible) matched = true;
                }
            }

            if (!matched)
            {
                for (int i = 0; i < _contextGroups.Count; i++)
                {
                    bool visible = _contextGroups[i].Name.Equals(key, StringComparison.OrdinalIgnoreCase);
                    SetContextualGroupVisible(i, visible);
                    if (visible) matched = true;
                }
            }

            if (!matched)
            {
                for (int i = 0; i < _contextGroups.Count; i++)
                {
                    SetContextualGroupVisible(i, false);
                }
            }
        }

        public void BeginMergeScope()
        {
            _mergeBaseSnapshot.Clear();
            _mergeBaseSnapshot.AddRange(CloneNodeList(_commandItems));
            _isMerged = false;
        }

        public List<SimpleItem> GetCommandModelSnapshot()
        {
            return CloneNodeList(_commandItems);
        }

        public void MergeFrom(BeepRibbonControl source, RibbonMergeMode mode = RibbonMergeMode.AppendTabs)
        {
            if (source == null) return;
            MergeFrom(source.GetCommandModelSnapshot(), mode);
        }

        public void MergeFrom(IEnumerable<SimpleItem>? sourceTabs, RibbonMergeMode mode = RibbonMergeMode.AppendTabs)
        {
            if (sourceTabs == null) return;
            var incoming = CloneNodeList(sourceTabs);
            if (incoming.Count == 0) return;

            if (_mergeBaseSnapshot.Count == 0)
            {
                _mergeBaseSnapshot.AddRange(CloneNodeList(_commandItems));
            }

            List<SimpleItem> baseModel = _isMerged
                ? CloneNodeList(_commandItems)
                : CloneNodeList(_mergeBaseSnapshot);

            List<SimpleItem> result = mode switch
            {
                RibbonMergeMode.Replace => incoming,
                RibbonMergeMode.MergeByTabName => MergeByName(baseModel, incoming),
                _ => AppendTabs(baseModel, incoming)
            };

            ReplaceCommandItems(result);
            _isMerged = true;
            RibbonMerged?.Invoke(this, new RibbonMergedEventArgs(mode, incoming.Count, result.Count));
        }

        public void EndMergeScope()
        {
            if (_mergeBaseSnapshot.Count == 0)
            {
                _isMerged = false;
                return;
            }

            ReplaceCommandItems(CloneNodeList(_mergeBaseSnapshot));
            _mergeBaseSnapshot.Clear();
            _isMerged = false;
            RibbonMerged?.Invoke(this, new RibbonMergedEventArgs(RibbonMergeMode.Replace, 0, _commandItems.Count));
        }

        public void ApplyThemeFromBeep(IBeepTheme? theme)
        {
            FormStyle style = _followGlobalFormStyle ? BeepThemesManager.CurrentStyle : _ribbonFormStyle;
            ApplyThemeFromBeep(theme, style);
        }

        public void ApplyThemeFromBeep(IBeepTheme? theme, FormStyle formStyle)
        {
            _ribbonFormStyle = formStyle;
            _resolvedStylePreset = RibbonThemeMapper.GetPreset(_ribbonFormStyle, _darkMode);
            Theme = RibbonThemeMapper.Map(theme, _darkMode, _ribbonFormStyle);
        }

        public void SyncWithGlobalThemeAndStyle()
        {
            _ribbonFormStyle = BeepThemesManager.CurrentStyle;
            _resolvedStylePreset = RibbonThemeMapper.GetPreset(_ribbonFormStyle, _darkMode);
            ApplyThemeFromBeep(BeepThemesManager.CurrentTheme, _ribbonFormStyle);
        }

        public bool LoadThemeFromTokenFile(string filePath, string mode = "light")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                {
                    return false;
                }

                var result = RibbonTokenImporter.ImportWithDiagnosticsFromFile(filePath, mode, _theme);
                _lastTokenImportDiagnostics.Clear();
                _lastTokenImportDiagnostics.AddRange(result.Diagnostics);
                Theme = result.Theme;
                return true;
            }
            catch
            {
                _lastTokenImportDiagnostics.Clear();
                _lastTokenImportDiagnostics.Add("Failed to load token file.");
                return false;
            }
        }

        public bool ApplyVariant(string variantName)
        {
            if (!_variantMatrix.TryGet(variantName, out var variant))
            {
                return false;
            }

            LayoutMode = variant.LayoutMode;
            Density = variant.Density;
            DarkMode = variant.DarkMode;
            UseSuperToolTips = variant.UseSuperToolTips;
            SearchIncludeBackstage = variant.SearchIncludeBackstage;
            if (_allowMinimize)
            {
                IsMinimized = variant.IsMinimized;
            }

            return true;
        }

        public bool AddCommandToQuickAccess(SimpleItem command)
        {
            if (command == null) return false;
            return AddCommandToQuickAccess(GetCommandKey(command));
        }

        public bool AddCommandToQuickAccess(string commandKey)
        {
            if (string.IsNullOrWhiteSpace(commandKey)) return false;
            if ((_personalizationOptions & RibbonPersonalizationOptions.QuickAccess) == 0) return false;
            if (_quickAccessCommandKeys.Contains(commandKey, StringComparer.OrdinalIgnoreCase)) return false;

            _quickAccessCommandKeys.Add(commandKey);
            RebuildQuickAccessToolbar();
            return true;
        }

        public bool RemoveCommandFromQuickAccess(SimpleItem command)
        {
            if (command == null) return false;
            return RemoveCommandFromQuickAccess(GetCommandKey(command));
        }

        public bool RemoveCommandFromQuickAccess(string commandKey)
        {
            if (string.IsNullOrWhiteSpace(commandKey)) return false;
            int index = _quickAccessCommandKeys.FindIndex(k => k.Equals(commandKey, StringComparison.OrdinalIgnoreCase));
            if (index < 0) return false;
            _quickAccessCommandKeys.RemoveAt(index);
            RebuildQuickAccessToolbar();
            return true;
        }

        public bool MoveQuickAccessCommand(string commandKey, int newIndex)
        {
            if (string.IsNullOrWhiteSpace(commandKey)) return false;
            int oldIndex = _quickAccessCommandKeys.FindIndex(k => k.Equals(commandKey, StringComparison.OrdinalIgnoreCase));
            if (oldIndex < 0) return false;
            if (newIndex < 0 || newIndex >= _quickAccessCommandKeys.Count) return false;
            if (oldIndex == newIndex) return true;

            var item = _quickAccessCommandKeys[oldIndex];
            _quickAccessCommandKeys.RemoveAt(oldIndex);
            _quickAccessCommandKeys.Insert(newIndex, item);
            RebuildQuickAccessToolbar();
            return true;
        }

        public bool SetTabVisible(string tabKey, bool visible)
        {
            var tab = GetTabNode(tabKey);
            if (tab == null) return false;
            if (tab.IsVisible == visible) return true;
            tab.IsVisible = visible;
            BuildFromSimpleItems();
            return true;
        }

        public bool MoveTab(string tabKey, int newIndex)
        {
            int oldIndex = FindTabIndex(tabKey);
            if (oldIndex < 0) return false;
            if (newIndex < 0 || newIndex >= _commandItems.Count) return false;
            if (oldIndex == newIndex) return true;

            var tab = _commandItems[oldIndex];
            _suspendCommandRebuild = true;
            try
            {
                _commandItems.RemoveAt(oldIndex);
                _commandItems.Insert(newIndex, tab);
            }
            finally
            {
                _suspendCommandRebuild = false;
            }

            BuildFromSimpleItems();
            return true;
        }

        public bool SetGroupVisible(string tabKey, string groupKey, bool visible)
        {
            var group = GetGroupNode(tabKey, groupKey);
            if (group == null) return false;
            if (group.IsVisible == visible) return true;
            group.IsVisible = visible;
            BuildFromSimpleItems();
            return true;
        }

        public bool MoveGroup(string tabKey, string groupKey, int newIndex)
        {
            var tab = GetTabNode(tabKey);
            if (tab == null) return false;
            int oldIndex = FindGroupIndex(tab, groupKey);
            if (oldIndex < 0) return false;
            if (newIndex < 0 || newIndex >= tab.Children.Count) return false;
            if (oldIndex == newIndex) return true;

            var group = tab.Children[oldIndex];
            tab.Children.RemoveAt(oldIndex);
            tab.Children.Insert(newIndex, group);
            BuildFromSimpleItems();
            return true;
        }

        public bool ShowCustomizeRibbonDialog(IWin32Window? owner = null)
        {
            if ((_personalizationOptions & (RibbonPersonalizationOptions.RibbonTabs | RibbonPersonalizationOptions.RibbonGroups | RibbonPersonalizationOptions.CommandOrder | RibbonPersonalizationOptions.QuickAccess)) == 0)
            {
                return false;
            }

            EnsureCustomizationDefaultsCaptured();
            var initialState = BuildCustomizationDialogState();
            using var dialog = new RibbonCustomizationDialog(initialState)
            {
                Font = BeepThemesManager.ToFont(_theme.CommandTypography)
            };

            bool applied = false;

            dialog.ApplyRequested += (_, e) =>
            {
                ApplyCustomizationState(e.State, RibbonCustomizationAction.Apply);
                applied = true;
            };

            dialog.ResetRequested += (_, __) =>
            {
                ResetCustomizationToDefault();
                applied = true;
                dialog.LoadState(BuildCustomizationDialogState());
            };

            dialog.CancelRequested += (_, __) =>
            {
                RibbonCustomizationCanceled?.Invoke(this, EventArgs.Empty);
            };

            if (owner != null)
            {
                dialog.ShowDialog(owner);
            }
            else
            {
                dialog.ShowDialog();
            }

            return applied;
        }

        public RibbonCustomizationDialogState CaptureCustomizationState()
        {
            return BuildCustomizationDialogState().DeepClone();
        }

        public void ApplyCustomizationState(RibbonCustomizationDialogState state, RibbonCustomizationAction action = RibbonCustomizationAction.Apply)
        {
            ApplyCustomizationDialogState(state);
            RibbonCustomizationApplied?.Invoke(this,
                new RibbonCustomizationAppliedEventArgs(action, _commandItems.Count, _quickAccessCommandKeys.Count));
        }

        public void ResetCustomizationToDefault()
        {
            EnsureCustomizationDefaultsCaptured();
            if (_defaultCustomizationSnapshot == null)
            {
                return;
            }

            _pendingCustomizationState = null;
            _quickAccessCommandKeys.Clear();
            if (_defaultQuickAccessSnapshot != null)
            {
                foreach (var key in _defaultQuickAccessSnapshot)
                {
                    if (!string.IsNullOrWhiteSpace(key) &&
                        !_quickAccessCommandKeys.Contains(key, StringComparer.OrdinalIgnoreCase))
                    {
                        _quickAccessCommandKeys.Add(key);
                    }
                }
            }

            ReplaceCommandItems(CloneNodeList(_defaultCustomizationSnapshot));
            RibbonCustomizationReset?.Invoke(this, EventArgs.Empty);
            RibbonCustomizationApplied?.Invoke(this,
                new RibbonCustomizationAppliedEventArgs(RibbonCustomizationAction.Reset, _commandItems.Count, _quickAccessCommandKeys.Count));
        }

        public void MarkCurrentCustomizationAsDefault()
        {
            if (_commandItems.Count == 0)
            {
                _defaultCustomizationSnapshot = null;
                _defaultQuickAccessSnapshot = null;
                return;
            }

            _defaultCustomizationSnapshot = CloneNodeList(_commandItems);
            _defaultQuickAccessSnapshot = [.. _quickAccessCommandKeys];
        }

        private void EnsureCustomizationDefaultsCaptured()
        {
            if (_defaultCustomizationSnapshot != null)
            {
                return;
            }

            if (_commandItems.Count == 0)
            {
                return;
            }

            _defaultCustomizationSnapshot = CloneNodeList(_commandItems);
            _defaultQuickAccessSnapshot = [.. _quickAccessCommandKeys];
        }

        private RibbonCustomizationDialogState BuildCustomizationDialogState()
        {
            var state = new RibbonCustomizationDialogState();

            foreach (var tabNode in _commandItems)
            {
                var tabModel = new RibbonCustomizationTabModel
                {
                    TabKey = GetMergeKey(tabNode),
                    Text = GetDisplayText(tabNode),
                    Visible = tabNode.IsVisible
                };

                foreach (var groupNode in tabNode.Children)
                {
                    tabModel.Groups.Add(new RibbonCustomizationGroupModel
                    {
                        GroupKey = GetMergeKey(groupNode),
                        Text = GetDisplayText(groupNode),
                        Visible = groupNode.IsVisible
                    });
                }

                state.Tabs.Add(tabModel);
            }

            var commandMap = new Dictionary<string, RibbonCustomizationCommandModel>(StringComparer.OrdinalIgnoreCase);
            foreach (var tabNode in _commandItems)
            {
                string tabKey = GetMergeKey(tabNode);
                string tabText = GetDisplayText(tabNode);
                foreach (var groupNode in tabNode.Children)
                {
                    string groupKey = GetMergeKey(groupNode);
                    string groupText = GetDisplayText(groupNode);
                    AddCustomizationCommands(commandMap, tabKey, tabText, groupKey, groupText, groupNode.Children);
                }
            }

            state.AvailableCommands = commandMap.Values
                .OrderBy(c => c.Text, StringComparer.CurrentCultureIgnoreCase)
                .ToList();

            foreach (var key in _quickAccessCommandKeys)
            {
                string? resolved = ResolveQuickAccessKey(key);
                if (!string.IsNullOrWhiteSpace(resolved) &&
                    !state.QuickAccessCommandKeys.Contains(resolved, StringComparer.OrdinalIgnoreCase))
                {
                    state.QuickAccessCommandKeys.Add(resolved);
                }
            }

            return state;
        }

        private static void AddCustomizationCommands(
            IDictionary<string, RibbonCustomizationCommandModel> commandMap,
            string tabKey,
            string tabText,
            string groupKey,
            string groupText,
            IEnumerable<SimpleItem> nodes)
        {
            foreach (var node in nodes)
            {
                if (node.IsSeparator)
                {
                    continue;
                }

                if (CanCustomizeCommand(node))
                {
                    string commandKey = GetCommandKey(node);
                    if (!commandMap.ContainsKey(commandKey))
                    {
                        commandMap[commandKey] = new RibbonCustomizationCommandModel
                        {
                            CommandKey = commandKey,
                            Text = GetDisplayText(node),
                            TabKey = tabKey,
                            GroupKey = groupKey,
                            TabText = tabText,
                            GroupText = groupText
                        };
                    }
                }

                if (node.Children.Count > 0)
                {
                    AddCustomizationCommands(commandMap, tabKey, tabText, groupKey, groupText, node.Children);
                }
            }
        }

        private static bool CanCustomizeCommand(SimpleItem node)
        {
            if (node.IsSeparator)
            {
                return false;
            }

            if (node.Children.Count == 0)
            {
                return true;
            }

            return !string.IsNullOrWhiteSpace(node.ActionID) ||
                   !string.IsNullOrWhiteSpace(node.ReferenceID) ||
                   !string.IsNullOrWhiteSpace(node.MethodName);
        }

        private void ApplyCustomizationDialogState(RibbonCustomizationDialogState state)
        {
            if (state == null)
            {
                return;
            }

            _pendingCustomizationState = null;
            _quickAccessCommandKeys.Clear();
            foreach (var key in state.QuickAccessCommandKeys)
            {
                string? resolved = ResolveQuickAccessKey(key);
                if (!string.IsNullOrWhiteSpace(resolved) &&
                    !_quickAccessCommandKeys.Contains(resolved, StringComparer.OrdinalIgnoreCase))
                {
                    _quickAccessCommandKeys.Add(resolved);
                }
            }

            var tabStates = state.Tabs
                .Select((tab, tabIndex) => new RibbonTabState
                {
                    TabKey = tab.TabKey,
                    Visible = tab.Visible,
                    Order = tabIndex,
                    Groups = tab.Groups
                        .Select((group, groupIndex) => new RibbonGroupState
                        {
                            GroupKey = group.GroupKey,
                            Visible = group.Visible,
                            Order = groupIndex
                        })
                        .ToList()
                })
                .ToList();

            ApplyTabStates(tabStates);
            BuildFromSimpleItems();
        }

        public void SaveCustomizationTo(string file)
        {
            try
            {
                var state = new RibbonCustomizationState
                {
                    LayoutMode = _layoutMode,
                    Density = _density,
                    SearchMode = _searchMode,
                    SearchIncludeBackstage = _searchIncludeBackstage,
                    SearchMaxResults = _searchMaxResults,
                    EnableKeyTips = _enableKeyTips,
                    QuickAccessAboveRibbon = _quickAccessAboveRibbon,
                    IsMinimized = _isMinimized,
                    ShowMinimizedPopupOnTabClick = _showMinimizedPopupOnTabClick,
                    BackstageSelectedIndex = _activeBackstageIndex,
                    QuickAccessCommandKeys = [.. _quickAccessCommandKeys],
                    Tabs = CaptureTabStates()
                };
                var json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(file, json);
            }
            catch
            {
            }
        }

        public void LoadCustomizationFrom(string file)
        {
            try
            {
                if (!File.Exists(file)) return;
                var json = File.ReadAllText(file);
                var state = JsonSerializer.Deserialize<RibbonCustomizationState>(json);
                if (state == null) return;

                LayoutMode = state.LayoutMode;
                Density = state.Density;
                SearchMode = state.SearchMode;
                SearchIncludeBackstage = state.SearchIncludeBackstage;
                SearchMaxResults = state.SearchMaxResults <= 0 ? 12 : state.SearchMaxResults;
                EnableKeyTips = state.EnableKeyTips;
                QuickAccessAboveRibbon = state.QuickAccessAboveRibbon;
                ShowMinimizedPopupOnTabClick = state.ShowMinimizedPopupOnTabClick;
                if (_allowMinimize)
                {
                    IsMinimized = state.IsMinimized;
                }

                _quickAccessCommandKeys.Clear();
                if (state.QuickAccessCommandKeys != null)
                {
                    foreach (var entry in state.QuickAccessCommandKeys)
                    {
                        string? resolved = ResolveQuickAccessKey(entry);
                        if (!string.IsNullOrWhiteSpace(resolved) &&
                            !_quickAccessCommandKeys.Contains(resolved, StringComparer.OrdinalIgnoreCase))
                        {
                            _quickAccessCommandKeys.Add(resolved);
                        }
                    }
                }

                if (_commandItems.Count > 0)
                {
                    EnsureCustomizationDefaultsCaptured();
                    ApplyTabStates(state.Tabs);
                    _pendingCustomizationState = null;
                    BuildFromSimpleItems();
                }
                else
                {
                    _pendingCustomizationState = state;
                    RebuildQuickAccessToolbar();
                }

                if (state.BackstageSelectedIndex >= 0 && state.BackstageSelectedIndex < _backstageNavList.Items.Count)
                {
                    _backstageNavList.SelectedIndex = state.BackstageSelectedIndex;
                }
            }
            catch
            {
            }
        }

        public void SaveThemeTokensTo(string file)
        {
            try
            {
                var tokens = RibbonThemeTokens.FromTheme(_theme);
                var json = JsonSerializer.Serialize(tokens, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(file, json);
            }
            catch
            {
            }
        }

        public void LoadThemeTokensFrom(string file)
        {
            try
            {
                if (!File.Exists(file)) return;
                var json = File.ReadAllText(file);
                var tokens = JsonSerializer.Deserialize<RibbonThemeTokens>(json);
                if (tokens == null) return;
                Theme = tokens.ToTheme(_theme);
            }
            catch
            {
            }
        }

        public void BuildFromSimpleItems()
        {
            BuildFromSimpleItems(_commandItems);
        }

        public void BuildFromSimpleItems(IEnumerable<SimpleItem>? tabNodes)
        {
            SuspendLayout();
            try
            {
                ClearRibbonTree();
                if (tabNodes == null)
                {
                    ApplyTheme();
                    ApplyMinimizedState();
                    return;
                }

                if (ReferenceEquals(tabNodes, _commandItems))
                {
                    EnsureCustomizationDefaultsCaptured();
                }

                if (ReferenceEquals(tabNodes, _commandItems) && _pendingCustomizationState != null)
                {
                    ApplyTabStates(_pendingCustomizationState.Tabs);
                    _pendingCustomizationState = null;
                }

                var tabList = tabNodes.Where(IsVisibleNode).ToList();
                RebuildCommandLookup(tabList);

                foreach (var tabNode in tabList)
                {
                    var page = AddPage(GetDisplayText(tabNode));
                    page.Tag = tabNode;

                    if (_layoutMode == RibbonLayoutMode.Simplified)
                    {
                        var mergedNodes = new List<SimpleItem>();
                        foreach (var groupNode in tabNode.Children.Where(IsVisibleNode))
                        {
                            if (mergedNodes.Count > 0)
                            {
                                mergedNodes.Add(new SimpleItem { IsSeparator = true });
                            }

                            mergedNodes.AddRange(groupNode.Children.Where(IsVisibleNode));
                        }

                        var mergedGroup = AddGroup(page, "Commands");
                        mergedGroup.Tag = tabNode;
                        _groupCommandNodes[mergedGroup] = mergedNodes;
                        BuildGroupCommands(mergedGroup, mergedNodes);
                    }
                    else
                    {
                        foreach (var groupNode in tabNode.Children.Where(IsVisibleNode))
                        {
                            var group = AddGroup(page, GetDisplayText(groupNode));
                            group.Tag = groupNode;
                            group.Density = _density;
                            group.ApplyTheme(_theme);
                            var commands = groupNode.Children.Where(IsVisibleNode).ToList();
                            _groupCommandNodes[group] = commands;
                            BuildGroupCommands(group, commands);
                        }
                    }
                }

                ApplyResponsiveLayout();
                RebuildQuickAccessToolbar();
                if (_searchMode != RibbonSearchMode.Off && !string.IsNullOrWhiteSpace(_searchBox.Text))
                {
                    RunLocalSearch(_searchBox.Text);
                }
                ApplyTheme();
                ApplyMinimizedState();
            }
            finally
            {
                ResumeLayout();
            }
        }

        private void BuildGroupCommands(BeepRibbonGroup group, IEnumerable<SimpleItem> commandNodes)
        {
            var commands = commandNodes.Where(IsVisibleNode).ToList();
            commands = NormalizeSeparators(commands);
            if (commands.Count == 0)
            {
                return;
            }

            bool useLargeButtons = DetermineLayoutSize(commands, group);
            int available = GetAvailableGroupWidth(group);
            int reservedOverflowWidth = EstimateOverflowButtonWidth();
            int used = 0;
            var overflow = new List<SimpleItem>();

            foreach (var command in commands)
            {
                int commandWidth = EstimateCommandWidth(command, useLargeButtons);
                bool fits = used + commandWidth <= available;
                bool reserveOverflow = used + commandWidth <= available - reservedOverflowWidth;

                if (!fits || (_layoutMode == RibbonLayoutMode.Simplified && !reserveOverflow))
                {
                    overflow.Add(command);
                    continue;
                }

                AddCommandToGroup(group, command, useLargeButtons);
                used += commandWidth;
            }

            if (overflow.Count > 0)
            {
                var overflowCommands = NormalizeSeparators(overflow);
                if (overflowCommands.Count > 0)
                {
                    var overflowButton = CreateOverflowButton(overflowCommands);
                    group.Items.Add(overflowButton);
                }
            }
        }

        private void AddCommandToGroup(BeepRibbonGroup group, SimpleItem command, bool useLargeButtons)
        {
            if (command.IsSeparator)
            {
                group.Items.Add(new ToolStripSeparator());
                return;
            }

            if (IsGalleryCommand(command))
            {
                AddGalleryToGroup(group, command, useLargeButtons);
                return;
            }

            if (command.Children.Count > 0)
            {
                var dropdown = CreateDropDownButton(command, useLargeButtons);
                group.Items.Add(dropdown);
                return;
            }

            AddCommandButton(group, command, useLargeButtons);
        }

        private bool IsGalleryCommand(SimpleItem command)
        {
            if (command.Children.Count < 2)
            {
                return false;
            }

            static bool ContainsGalleryToken(string? value)
            {
                return !string.IsNullOrWhiteSpace(value) &&
                       value.Contains("gallery", StringComparison.OrdinalIgnoreCase);
            }

            return ContainsGalleryToken(command.Text) ||
                   ContainsGalleryToken(command.DisplayField) ||
                   ContainsGalleryToken(command.Name) ||
                   ContainsGalleryToken(command.ToolTip) ||
                   ContainsGalleryToken(command.ItemType.ToString());
        }

        private void AddGalleryToGroup(BeepRibbonGroup group, SimpleItem command, bool useLargeButtons)
        {
            string galleryKey = GetCommandKey(command);
            var gallery = new BeepRibbonGallery
            {
                Compact = !useLargeButtons,
                EnableCategoryHeaders = true,
                EnableLargePreviewPopup = true,
                Width = EstimateGalleryWidth(command, useLargeButtons),
                Height = Math.Max(28, GetGroupHeight() - 6),
                Margin = new Padding(0),
                TabStop = true
            };

            gallery.ApplyTheme(_theme, _density);
            gallery.RightToLeft = _ribbonRightToLeft ? RightToLeft.Yes : RightToLeft.No;
            RibbonAccessibilityHelper.ApplyControlAccessibility(
                gallery,
                $"{GetDisplayText(command)} gallery",
                BuildToolTip(command),
                AccessibleRole.List);
            gallery.SetItems(GetGalleryItems(command));
            gallery.SetSelected(GetGallerySelectedItem(command));
            if (_galleryPinnedKeys.TryGetValue(galleryKey, out var pinnedKeys))
            {
                gallery.SetPinnedKeys(pinnedKeys);
            }
            else
            {
                gallery.SetPinnedKeys(GetGalleryPinnedKeysFromMetadata(command));
            }

            if (_galleryRecentKeys.TryGetValue(galleryKey, out var recentKeys))
            {
                gallery.SetRecentKeys(recentKeys);
            }
            else
            {
                gallery.SetRecentKeys(GetGalleryRecentKeysFromMetadata(command));
            }

            var host = new ToolStripControlHost(gallery)
            {
                AutoSize = false,
                Width = gallery.Width,
                Height = Math.Max(30, GetGroupHeight() - 2),
                Margin = new Padding(1),
                Padding = Padding.Empty
            };

            gallery.ItemSelected += (_, e) =>
            {
                _galleryLastSelection[galleryKey] = GetCommandKey(e.Item);
                RecordSearchCommandUsage(e.Item);
                RaiseCommandInvoked(e.Item, host);
            };
            gallery.StateChanged += (_, e) =>
            {
                _galleryPinnedKeys[galleryKey] = e.PinnedKeys
                    .Where(k => !string.IsNullOrWhiteSpace(k))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();
                _galleryRecentKeys[galleryKey] = e.RecentKeys
                    .Where(k => !string.IsNullOrWhiteSpace(k))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Take(12)
                    .ToList();
            };

            ConfigureCommandItem(host, command);
            group.Items.Add(host);
            _commandMap[host] = command;
        }

        private IEnumerable<SimpleItem> GetGalleryItems(SimpleItem command)
        {
            var items = command.Children.Where(IsVisibleNode).ToList();
            if (items.Count <= 1)
            {
                return items;
            }

            string galleryKey = GetCommandKey(command);
            if (!_galleryLastSelection.TryGetValue(galleryKey, out var selectedKey) ||
                string.IsNullOrWhiteSpace(selectedKey))
            {
                return items;
            }

            int index = items.FindIndex(i => GetCommandKey(i).Equals(selectedKey, StringComparison.OrdinalIgnoreCase));
            if (index <= 0)
            {
                return items;
            }

            var selected = items[index];
            items.RemoveAt(index);
            items.Insert(0, selected);
            return items;
        }

        private SimpleItem? GetGallerySelectedItem(SimpleItem command)
        {
            string galleryKey = GetCommandKey(command);
            if (!_galleryLastSelection.TryGetValue(galleryKey, out var selectedKey))
            {
                return null;
            }

            return command.Children.FirstOrDefault(c =>
                GetCommandKey(c).Equals(selectedKey, StringComparison.OrdinalIgnoreCase));
        }

        private static IEnumerable<string> GetGalleryPinnedKeysFromMetadata(SimpleItem command)
        {
            return command.Children
                .Where(c =>
                    c.IsChecked ||
                    (!string.IsNullOrWhiteSpace(c.BadgeText) &&
                     c.BadgeText.Contains("pin", StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrWhiteSpace(c.SubText3) &&
                     c.SubText3.Contains("pin", StringComparison.OrdinalIgnoreCase)))
                .Select(GetCommandKey)
                .Where(k => !string.IsNullOrWhiteSpace(k))
                .Distinct(StringComparer.OrdinalIgnoreCase);
        }

        private static IEnumerable<string> GetGalleryRecentKeysFromMetadata(SimpleItem command)
        {
            return command.Children
                .Where(c =>
                    (!string.IsNullOrWhiteSpace(c.BadgeText) &&
                     c.BadgeText.Contains("recent", StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrWhiteSpace(c.SubText2) &&
                     c.SubText2.Contains("recent", StringComparison.OrdinalIgnoreCase)))
                .Select(GetCommandKey)
                .Where(k => !string.IsNullOrWhiteSpace(k))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(10);
        }

        private int EstimateGalleryWidth(SimpleItem command, bool useLargeButtons)
        {
            int itemCount = command.Children.Count(c => !c.IsSeparator && c.IsVisible);
            itemCount = Math.Max(2, itemCount);
            int tileWidth = _density switch
            {
                RibbonDensity.Compact => useLargeButtons ? 88 : 68,
                RibbonDensity.Touch => useLargeButtons ? 110 : 92,
                _ => useLargeButtons ? 96 : 78
            };
            int visibleTiles = useLargeButtons ? Math.Min(3, itemCount) : Math.Min(4, itemCount);
            return Math.Max(128, visibleTiles * tileWidth + 10);
        }

        private void AddCommandButton(BeepRibbonGroup group, SimpleItem command, bool useLargeButtons)
        {
            string text = GetDisplayText(command);
            Image? image = CreateCommandImage(command.ImagePath, !useLargeButtons);
            ToolStripButton button = useLargeButtons
                ? group.AddLargeButton(text, image)
                : group.AddSmallButton(text, image);

            ConfigureCommandItem(button, command);
            button.CheckOnClick = command.IsCheckable;
            button.Checked = command.IsChecked;
            button.Click += (_, __) => RaiseCommandInvoked(command, button);
            _commandMap[button] = command;
        }

        private ToolStripDropDownButton CreateDropDownButton(SimpleItem command, bool useLargeButtons)
        {
            Image? image = CreateCommandImage(command.ImagePath, !useLargeButtons);
            var button = new ToolStripDropDownButton(GetDisplayText(command), image)
            {
                ImageScaling = useLargeButtons ? ToolStripItemImageScaling.None : ToolStripItemImageScaling.SizeToFit,
                TextImageRelation = useLargeButtons ? TextImageRelation.ImageAboveText : TextImageRelation.ImageBeforeText,
                AutoSize = !useLargeButtons,
                Width = useLargeButtons ? GetLargeItemWidth() : 0,
                Height = GetGroupHeight(),
                Font = BeepThemesManager.ToFont(_theme.CommandTypography)
            };

            ConfigureCommandItem(button, command);
            BuildDropDownMenu(button.DropDownItems, command.Children);
            _commandMap[button] = command;
            return button;
        }

        private ToolStripDropDownButton CreateOverflowButton(IEnumerable<SimpleItem> overflowNodes)
        {
            var button = new ToolStripDropDownButton("More")
            {
                AutoSize = true,
                Height = GetGroupHeight(),
                Font = BeepThemesManager.ToFont(_theme.CommandTypography),
                ForeColor = _theme.Text
            };
            BuildDropDownMenu(button.DropDownItems, overflowNodes);
            return button;
        }

        private void BuildDropDownMenu(ToolStripItemCollection parent, IEnumerable<SimpleItem> nodes)
        {
            foreach (var node in nodes.Where(IsVisibleNode))
            {
                if (node.IsSeparator)
                {
                    parent.Add(new ToolStripSeparator());
                    continue;
                }

                var item = new ToolStripMenuItem(GetDisplayText(node), CreateCommandImage(node.ImagePath, true))
                {
                    Enabled = node.IsEnabled,
                    Checked = node.IsChecked,
                    CheckOnClick = node.IsCheckable,
                    Font = BeepThemesManager.ToFont(_theme.CommandTypography),
                };
                ConfigureCommandItem(item, node);
                item.Click += (_, __) => RaiseCommandInvoked(node, item);
                parent.Add(item);
                _commandMap[item] = node;

                if (node.Children.Count > 0)
                {
                    BuildDropDownMenu(item.DropDownItems, node.Children);
                }
            }
        }

        private void ConfigureCommandItem(ToolStripItem item, SimpleItem command)
        {
            item.Enabled = command.IsEnabled;
            item.Visible = command.IsVisible;
            item.ToolTipText = BuildToolTip(command);
            item.ForeColor = _theme.Text;
            var role = RibbonAccessibilityHelper.GetCommandRole(command, item);
            RibbonAccessibilityHelper.ApplyCommandAccessibility(item, command, GetDisplayText(command), role);
            item.Tag = command;
            if (item is ToolStripControlHost host && host.Control != null)
            {
                RibbonAccessibilityHelper.ApplyControlAccessibility(
                    host.Control,
                    GetDisplayText(command),
                    BuildToolTip(command),
                    AccessibleRole.Grouping);
                host.Control.TabStop = true;
            }

            if (_useSuperToolTips)
            {
                item.MouseHover -= CommandItem_MouseHover;
                item.MouseHover += CommandItem_MouseHover;
                item.MouseLeave -= CommandItem_MouseLeave;
                item.MouseLeave += CommandItem_MouseLeave;
            }
            else
            {
                item.MouseHover -= CommandItem_MouseHover;
                item.MouseLeave -= CommandItem_MouseLeave;
            }

            if ((_personalizationOptions & RibbonPersonalizationOptions.QuickAccess) != 0)
            {
                item.MouseUp -= CommandItem_MouseUp;
                item.MouseUp += CommandItem_MouseUp;
            }
        }

        private static string BuildToolTip(SimpleItem command)
        {
            if (string.IsNullOrWhiteSpace(command.ShortcutText))
            {
                return command.ToolTip;
            }

            if (string.IsNullOrWhiteSpace(command.ToolTip))
            {
                return command.ShortcutText;
            }

            return $"{command.ToolTip} ({command.ShortcutText})";
        }

        private RibbonSuperTooltipModel BuildSuperTooltipModel(SimpleItem command)
        {
            var model = _superTooltipModelProvider?.Invoke(command) ?? RibbonSuperTooltipModel.FromSimpleItem(command);
            if (string.IsNullOrWhiteSpace(model.Description))
            {
                model.Description = BuildToolTip(command);
            }
            return model;
        }

        private void CommandItem_MouseHover(object? sender, EventArgs e)
        {
            if (!_useSuperToolTips)
            {
                return;
            }

            if (sender is not ToolStripItem item)
            {
                return;
            }

            if (!_commandMap.TryGetValue(item, out var command))
            {
                if (item.Tag is not SimpleItem taggedCommand)
                {
                    return;
                }
                command = taggedCommand;
            }

            var owner = item.Owner;
            if (owner == null)
            {
                return;
            }

            var model = BuildSuperTooltipModel(command);
            if (model.IsEmpty)
            {
                return;
            }

            _hoveredTooltipCommand = command;
            _hoveredTooltipModel = model;
            int x = Math.Max(0, item.Bounds.Left + 2);
            int y = Math.Max(0, item.Bounds.Bottom + 2);
            _superTooltip.Show(owner, new Point(x, y), model, _superTooltipDurationMs);
        }

        private void CommandItem_MouseLeave(object? sender, EventArgs e)
        {
            if (sender is not ToolStripItem item)
            {
                return;
            }

            _hoveredTooltipCommand = null;
            _hoveredTooltipModel = null;
            if (item.Owner != null)
            {
                _superTooltip.Hide(item.Owner);
            }
        }

        private void RaiseCommandInvoked(SimpleItem command, ToolStripItem source)
        {
            HideSearchResultsDropDown();
            HideMinimizedPopup();
            HideKeyTips();
            CommandInvoked?.Invoke(this, new RibbonCommandInvokedEventArgs(command, source));
        }

        private void CommandItem_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if ((_personalizationOptions & RibbonPersonalizationOptions.QuickAccess) == 0) return;
            if (sender is not ToolStripItem item) return;
            if (!_commandMap.TryGetValue(item, out var command))
            {
                if (item.Tag is not SimpleItem taggedCommand) return;
                command = taggedCommand;
            }

            string commandKey = GetCommandKey(command);
            bool inQuickAccess = _quickAccessCommandKeys.Contains(commandKey, StringComparer.OrdinalIgnoreCase);
            var menu = new ContextMenuStrip();

            if (inQuickAccess)
            {
                menu.Items.Add("Remove from Quick Access Toolbar", null, (_, __) => RemoveCommandFromQuickAccess(commandKey));
            }
            else
            {
                menu.Items.Add("Add to Quick Access Toolbar", null, (_, __) => AddCommandToQuickAccess(commandKey));
            }

            if ((_personalizationOptions & (RibbonPersonalizationOptions.RibbonTabs | RibbonPersonalizationOptions.RibbonGroups | RibbonPersonalizationOptions.CommandOrder)) != 0)
            {
                menu.Items.Add(new ToolStripSeparator());
                menu.Items.Add(CreateCustomizeRibbonMenuItem());
            }

            menu.Closed += (_, __) => menu.Dispose();
            menu.Show(Cursor.Position);
        }

        private static string GetDisplayText(SimpleItem item)
        {
            if (!string.IsNullOrWhiteSpace(item.DisplayField))
            {
                return item.DisplayField;
            }

            if (!string.IsNullOrWhiteSpace(item.Text))
            {
                return item.Text;
            }

            return item.Name ?? string.Empty;
        }

        private static string GetCommandKey(SimpleItem item)
        {
            if (!string.IsNullOrWhiteSpace(item.ActionID)) return item.ActionID;
            if (!string.IsNullOrWhiteSpace(item.ReferenceID)) return item.ReferenceID;
            if (!string.IsNullOrWhiteSpace(item.GuidId)) return item.GuidId;
            if (!string.IsNullOrWhiteSpace(item.Name)) return item.Name;
            if (!string.IsNullOrWhiteSpace(item.Text)) return item.Text;
            return Guid.NewGuid().ToString("N");
        }

        private void RebuildCommandLookup(IEnumerable<SimpleItem> tabNodes)
        {
            _commandLookup.Clear();
            foreach (var node in tabNodes)
            {
                AddCommandLookupRecursive(node);
            }
        }

        private void AddCommandLookupRecursive(SimpleItem node)
        {
            if (node.IsSeparator) return;
            string key = GetCommandKey(node);
            if (!_commandLookup.ContainsKey(key))
            {
                _commandLookup[key] = node;
            }

            foreach (var child in node.Children.Where(IsVisibleNode))
            {
                AddCommandLookupRecursive(child);
            }
        }

        private string? ResolveQuickAccessKey(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return null;
            if (_commandLookup.Count == 0) return token;
            if (_commandLookup.ContainsKey(token)) return token;

            var match = _commandLookup.FirstOrDefault(kv =>
                string.Equals(GetDisplayText(kv.Value), token, StringComparison.OrdinalIgnoreCase));
            return string.IsNullOrWhiteSpace(match.Key) ? token : match.Key;
        }

        private List<RibbonTabState> CaptureTabStates()
        {
            var states = new List<RibbonTabState>();
            for (int tabIndex = 0; tabIndex < _commandItems.Count; tabIndex++)
            {
                var tab = _commandItems[tabIndex];
                var tabState = new RibbonTabState
                {
                    TabKey = GetMergeKey(tab),
                    Visible = tab.IsVisible,
                    Order = tabIndex
                };

                for (int groupIndex = 0; groupIndex < tab.Children.Count; groupIndex++)
                {
                    var group = tab.Children[groupIndex];
                    tabState.Groups.Add(new RibbonGroupState
                    {
                        GroupKey = GetMergeKey(group),
                        Visible = group.IsVisible,
                        Order = groupIndex
                    });
                }

                states.Add(tabState);
            }

            return states;
        }

        private void ApplyTabStates(IEnumerable<RibbonTabState>? tabStates)
        {
            if (tabStates == null)
            {
                return;
            }

            var states = tabStates.ToList();
            if (states.Count == 0)
            {
                return;
            }

            var currentByKey = CreateNodeMap(_commandItems);
            var orderedTabs = new List<SimpleItem>();

            foreach (var tabState in states.OrderBy(s => s.Order))
            {
                if (string.IsNullOrWhiteSpace(tabState.TabKey)) continue;
                if (!currentByKey.TryGetValue(tabState.TabKey, out var tabNode)) continue;

                tabNode.IsVisible = tabState.Visible;
                ApplyGroupStates(tabNode, tabState.Groups);
                orderedTabs.Add(tabNode);
                currentByKey.Remove(tabState.TabKey);
            }

            orderedTabs.AddRange(currentByKey.Values);

            _suspendCommandRebuild = true;
            try
            {
                _commandItems.Clear();
                foreach (var tab in orderedTabs)
                {
                    _commandItems.Add(tab);
                }
            }
            finally
            {
                _suspendCommandRebuild = false;
            }
        }

        private static void ApplyGroupStates(SimpleItem tabNode, IEnumerable<RibbonGroupState>? groupStates)
        {
            if (groupStates == null)
            {
                return;
            }

            var states = groupStates.ToList();
            if (states.Count == 0)
            {
                return;
            }

            var currentByKey = CreateNodeMap(tabNode.Children);
            var orderedGroups = new List<SimpleItem>();

            foreach (var groupState in states.OrderBy(s => s.Order))
            {
                if (string.IsNullOrWhiteSpace(groupState.GroupKey)) continue;
                if (!currentByKey.TryGetValue(groupState.GroupKey, out var groupNode)) continue;

                groupNode.IsVisible = groupState.Visible;
                orderedGroups.Add(groupNode);
                currentByKey.Remove(groupState.GroupKey);
            }

            orderedGroups.AddRange(currentByKey.Values);

            tabNode.Children.Clear();
            foreach (var group in orderedGroups)
            {
                tabNode.Children.Add(group);
            }
        }

        private SimpleItem? GetTabNode(string tabKey)
        {
            if (string.IsNullOrWhiteSpace(tabKey)) return null;
            return _commandItems.FirstOrDefault(t => GetMergeKey(t).Equals(tabKey, StringComparison.OrdinalIgnoreCase));
        }

        private int FindTabIndex(string tabKey)
        {
            if (string.IsNullOrWhiteSpace(tabKey)) return -1;
            for (int i = 0; i < _commandItems.Count; i++)
            {
                if (GetMergeKey(_commandItems[i]).Equals(tabKey, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            return -1;
        }

        private static SimpleItem? GetGroupNode(string tabKey, string groupKey, IEnumerable<SimpleItem> tabs)
        {
            if (string.IsNullOrWhiteSpace(tabKey) || string.IsNullOrWhiteSpace(groupKey)) return null;
            var tab = tabs.FirstOrDefault(t => GetMergeKey(t).Equals(tabKey, StringComparison.OrdinalIgnoreCase));
            return tab?.Children.FirstOrDefault(g => GetMergeKey(g).Equals(groupKey, StringComparison.OrdinalIgnoreCase));
        }

        private SimpleItem? GetGroupNode(string tabKey, string groupKey)
        {
            return GetGroupNode(tabKey, groupKey, _commandItems);
        }

        private static int FindGroupIndex(SimpleItem tab, string groupKey)
        {
            if (string.IsNullOrWhiteSpace(groupKey)) return -1;
            for (int i = 0; i < tab.Children.Count; i++)
            {
                if (GetMergeKey(tab.Children[i]).Equals(groupKey, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            return -1;
        }

        private static int FindBackstageItemIndexByKey(IList<SimpleItem> items, string itemKey)
        {
            if (string.IsNullOrWhiteSpace(itemKey))
            {
                return -1;
            }

            for (int i = 0; i < items.Count; i++)
            {
                if (GetMergeKey(items[i]).Equals(itemKey, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            return -1;
        }

        private static bool RemoveBackstageItemByKey(IList<SimpleItem> items, string itemKey)
        {
            int index = FindBackstageItemIndexByKey(items, itemKey);
            if (index < 0)
            {
                return false;
            }

            items.RemoveAt(index);
            return true;
        }

        private static Dictionary<string, SimpleItem> CreateNodeMap(IEnumerable<SimpleItem> nodes)
        {
            var map = new Dictionary<string, SimpleItem>(StringComparer.OrdinalIgnoreCase);
            foreach (var node in nodes)
            {
                string key = GetMergeKey(node);
                if (!map.ContainsKey(key))
                {
                    map[key] = node;
                }
            }

            return map;
        }

        private void ReplaceCommandItems(IEnumerable<SimpleItem> tabs)
        {
            _suspendCommandRebuild = true;
            try
            {
                _commandItems.Clear();
                foreach (var tab in tabs)
                {
                    _commandItems.Add(tab);
                }
            }
            finally
            {
                _suspendCommandRebuild = false;
            }

            BuildFromSimpleItems();
        }

        private static List<SimpleItem> AppendTabs(List<SimpleItem> baseTabs, IEnumerable<SimpleItem> sourceTabs)
        {
            var result = CloneNodeList(baseTabs);
            result.AddRange(CloneNodeList(sourceTabs));
            return result;
        }

        private static List<SimpleItem> MergeByName(List<SimpleItem> baseTabs, IEnumerable<SimpleItem> sourceTabs)
        {
            var result = CloneNodeList(baseTabs);
            foreach (var sourceTab in sourceTabs)
            {
                var existingTab = result.FirstOrDefault(t => HasSameMergeKey(t, sourceTab));
                if (existingTab == null)
                {
                    result.Add(CloneNode(sourceTab));
                    continue;
                }

                MergeChildrenByName(existingTab.Children, sourceTab.Children);
            }

            return result;
        }

        private static void MergeChildrenByName(BindingList<SimpleItem> targetChildren, IEnumerable<SimpleItem> sourceChildren)
        {
            foreach (var source in sourceChildren)
            {
                var existing = targetChildren.FirstOrDefault(t => HasSameMergeKey(t, source));
                if (existing == null)
                {
                    targetChildren.Add(CloneNode(source));
                    continue;
                }

                if (source.Children.Count > 0)
                {
                    MergeChildrenByName(existing.Children, source.Children);
                }
            }
        }

        private static bool HasSameMergeKey(SimpleItem left, SimpleItem right)
        {
            return string.Equals(GetMergeKey(left), GetMergeKey(right), StringComparison.OrdinalIgnoreCase);
        }

        private static string GetMergeKey(SimpleItem node)
        {
            if (!string.IsNullOrWhiteSpace(node.ActionID)) return node.ActionID;
            if (!string.IsNullOrWhiteSpace(node.ReferenceID)) return node.ReferenceID;
            if (!string.IsNullOrWhiteSpace(node.Name)) return node.Name;
            if (!string.IsNullOrWhiteSpace(node.Text)) return node.Text;
            if (!string.IsNullOrWhiteSpace(node.DisplayField)) return node.DisplayField;
            if (!string.IsNullOrWhiteSpace(node.GuidId)) return node.GuidId;
            return $"{node.MenuID}:{node.MenuName}:{node.ItemType}";
        }

        private static List<SimpleItem> CloneNodeList(IEnumerable<SimpleItem> nodes)
        {
            return nodes.Select(CloneNode).ToList();
        }

        private static SimpleItem CloneNode(SimpleItem node)
        {
            var clone = new SimpleItem
            {
                ID = node.ID,
                Guid = node.Guid,
                GuidId = node.GuidId,
                Name = node.Name,
                MenuName = node.MenuName,
                Text = node.Text,
                DisplayField = node.DisplayField,
                Description = node.Description,
                SubText = node.SubText,
                SubText2 = node.SubText2,
                SubText3 = node.SubText3,
                ImagePath = node.ImagePath,
                ToolTip = node.ToolTip,
                Shortcut = node.Shortcut,
                ShortcutText = node.ShortcutText,
                KeyCombination = node.KeyCombination,
                BadgeText = node.BadgeText,
                BadgeBackColor = node.BadgeBackColor,
                BadgeForeColor = node.BadgeForeColor,
                BadgeShape = node.BadgeShape,
                IsCheckable = node.IsCheckable,
                IsChecked = node.IsChecked,
                IsEnabled = node.IsEnabled,
                IsVisible = node.IsVisible,
                IsExpanded = node.IsExpanded,
                IsSelected = node.IsSelected,
                IsSeparator = node.IsSeparator,
                MenuID = node.MenuID,
                ActionID = node.ActionID,
                ReferenceID = node.ReferenceID,
                ParentID = node.ParentID,
                OwnerReferenceID = node.OwnerReferenceID,
                OtherReferenceID = node.OtherReferenceID,
                PointType = node.PointType,
                ObjectType = node.ObjectType,
                BranchClass = node.BranchClass,
                BranchName = node.BranchName,
                BranchType = node.BranchType,
                MethodName = node.MethodName,
                ItemType = node.ItemType,
                Category = node.Category,
                Uri = node.Uri,
                AssemblyClassDefinitionID = node.AssemblyClassDefinitionID,
                ClassDefinitionID = node.ClassDefinitionID,
                PackageName = node.PackageName,
                BranchID = node.BranchID,
                ClassName = node.ClassName,
                GroupName = node.GroupName
            };

            foreach (var child in node.Children)
            {
                clone.Children.Add(CloneNode(child));
            }

            return clone;
        }

        private static bool IsVisibleNode(SimpleItem item)
        {
            return item.IsVisible;
        }

        private static List<SimpleItem> NormalizeSeparators(IEnumerable<SimpleItem> nodes)
        {
            var normalized = new List<SimpleItem>();
            bool previousWasSeparator = true;
            foreach (var node in nodes)
            {
                if (node.IsSeparator)
                {
                    if (previousWasSeparator)
                    {
                        continue;
                    }

                    previousWasSeparator = true;
                    normalized.Add(node);
                    continue;
                }

                previousWasSeparator = false;
                normalized.Add(node);
            }

            while (normalized.Count > 0 && normalized[^1].IsSeparator)
            {
                normalized.RemoveAt(normalized.Count - 1);
            }

            return normalized;
        }

        private bool ShouldRenderLargeButtons()
        {
            return _layoutMode == RibbonLayoutMode.Classic && _density != RibbonDensity.Compact;
        }

        private bool DetermineLayoutSize(IReadOnlyCollection<SimpleItem> commands, BeepRibbonGroup group)
        {
            if (_layoutMode == RibbonLayoutMode.Simplified)
            {
                return false;
            }

            bool preferLarge = ShouldRenderLargeButtons();
            if (!preferLarge)
            {
                return false;
            }

            int available = GetAvailableGroupWidth(group);
            int largeWidth = commands.Sum(c => EstimateCommandWidth(c, true));
            if (largeWidth <= available)
            {
                return true;
            }

            return _density == RibbonDensity.Touch;
        }

        private int GetAvailableGroupWidth(BeepRibbonGroup group)
        {
            int width = group.DisplayRectangle.Width > 0 ? group.DisplayRectangle.Width : group.Width;
            return Math.Max(80, width - 12);
        }

        private int EstimateOverflowButtonWidth()
        {
            return 68;
        }

        private int EstimateCommandWidth(SimpleItem command, bool useLargeButtons)
        {
            if (command.IsSeparator)
            {
                return 10;
            }

            if (useLargeButtons)
            {
                return GetLargeItemWidth();
            }

            string text = GetDisplayText(command);
            int textWidth = TextRenderer.MeasureText(text, BeepThemesManager.ToFont(_theme.CommandTypography)).Width;
            int iconWidth = string.IsNullOrWhiteSpace(command.ImagePath) ? 0 : GetIconSize(true) + 8;
            int arrowWidth = command.Children.Count > 0 ? 14 : 0;
            return Math.Max(52, textWidth + iconWidth + arrowWidth + 18);
        }

        private int GetGroupHeight()
        {
            return _density switch
            {
                RibbonDensity.Compact => 40,
                RibbonDensity.Touch => 56,
                _ => 48
            };
        }

        private int GetLargeItemWidth()
        {
            return _density switch
            {
                RibbonDensity.Compact => 64,
                RibbonDensity.Touch => 84,
                _ => 72
            };
        }

        private int GetIconSize(bool small)
        {
            return _density switch
            {
                RibbonDensity.Compact => small ? 14 : 16,
                RibbonDensity.Touch => small ? 18 : 22,
                _ => small ? 16 : 20
            };
        }

        private Image? CreateCommandImage(string? imagePath, bool small)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return null;
            }

            int size = GetIconSize(small);
            var bmp = new Bitmap(size, size);
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.Clear(Color.Transparent);
            using var clipPath = new GraphicsPath();
            clipPath.AddRectangle(new Rectangle(0, 0, size, size));
            StyledImagePainter.PaintWithTint(g, clipPath, imagePath, _theme.IconColor, 1f);
            _generatedImages.Add(bmp);
            return bmp;
        }

        private void ClearRibbonTree()
        {
            HideMinimizedPopup();
            _commandMap.Clear();
            _groupCommandNodes.Clear();
            _commandLookup.Clear();
            _tabs.TabPages.Clear();
            DisposeGeneratedImages();
        }

        private void DisposeGeneratedImages()
        {
            foreach (var image in _generatedImages)
            {
                image.Dispose();
            }
            _generatedImages.Clear();
        }

        private void RefreshCommandView()
        {
            if (_commandItems.Count > 0)
            {
                BuildFromSimpleItems();
            }
            else
            {
                ApplyTheme();
            }
        }

        private void ApplyQuickAccessPlacement()
        {
            _quickAccess.Dock = _quickAccessAboveRibbon ? DockStyle.Top : DockStyle.Bottom;
            if (_isMinimized)
            {
                ApplyMinimizedState();
            }
            PerformLayout();
            Invalidate();
        }

        private void RebuildQuickAccessToolbar()
        {
            _quickAccess.SuspendLayout();
            try
            {
                var qatItems = _commandMap.Keys.Where(i => i.Owner == _quickAccess).ToList();
                foreach (var qatItem in qatItems)
                {
                    _commandMap.Remove(qatItem);
                }

                var existingKeys = _quickAccessCommandKeys
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();
                _quickAccessCommandKeys.Clear();
                _quickAccessCommandKeys.AddRange(existingKeys);

                _quickAccess.Items.Clear();
                _quickAccess.Items.Add(_backstageButton);

                for (int i = 0; i < _quickAccessCommandKeys.Count; i++)
                {
                    var resolvedKey = ResolveQuickAccessKey(_quickAccessCommandKeys[i]);
                    if (string.IsNullOrWhiteSpace(resolvedKey)) continue;
                    _quickAccessCommandKeys[i] = resolvedKey;

                    if (!_commandLookup.TryGetValue(resolvedKey, out var command)) continue;
                    var button = new ToolStripButton(GetDisplayText(command), CreateCommandImage(command.ImagePath, true))
                    {
                        AutoSize = true,
                        DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                        TextImageRelation = TextImageRelation.ImageBeforeText,
                        Font = BeepThemesManager.ToFont(_theme.CommandTypography)
                    };
                    ConfigureCommandItem(button, command);
                    button.Click += (_, __) => RaiseCommandInvoked(command, button);
                    _quickAccess.Items.Add(button);
                    _commandMap[button] = command;
                }

                if (_searchMode != RibbonSearchMode.Off)
                {
                    _searchBox.Font = BeepThemesManager.ToFont(_theme.CommandTypography);
                    _searchResultsButton.Font = BeepThemesManager.ToFont(_theme.CommandTypography);
                    _quickAccess.Items.Add(new ToolStripSeparator());
                    _quickAccess.Items.Add(_searchBox);
                    _quickAccess.Items.Add(_searchResultsButton);
                }
            }
            finally
            {
                _quickAccess.ResumeLayout();
                ApplySearchAccessibility();
                ApplyPaneTabOrder();
                RefreshKeyTipsVisibility();
            }
        }

        private void ApplyResponsiveLayout()
        {
            if (_isApplyingResponsiveLayout || _groupCommandNodes.Count == 0)
            {
                return;
            }

            _isApplyingResponsiveLayout = true;
            try
            {
                var groups = _groupCommandNodes.Keys.Where(g => !g.IsDisposed).ToList();
                foreach (var group in groups)
                {
                    group.Items.Clear();
                }

                _commandMap.Clear();
                DisposeGeneratedImages();

                foreach (var group in groups)
                {
                    if (_groupCommandNodes.TryGetValue(group, out var commands))
                    {
                        BuildGroupCommands(group, commands);
                    }
                }

                RebuildQuickAccessToolbar();
            }
            finally
            {
                _isApplyingResponsiveLayout = false;
            }
        }

        private void ApplyTheme()
        {
            BackColor = _theme.Background;
            ForeColor = _theme.Text;
            Font = BeepThemesManager.ToFont(_theme.CommandTypography);

            _tabs.BackColor = _theme.Background;
            _tabs.ForeColor = _theme.Text;
            _tabs.Font = BeepThemesManager.ToFont(_theme.TabTypography);

            _quickAccess.BackColor = _theme.QuickAccessBack;
            _quickAccess.ForeColor = _theme.Text;
            _quickAccess.Font = BeepThemesManager.ToFont(_theme.CommandTypography);
            _quickAccess.Padding = new Padding(_theme.ItemSpacing, 2, _theme.ItemSpacing, 2);
            _quickAccess.Renderer = new BeepRibbonToolStripRenderer(this);
            _superTooltip.ApplyTheme(_theme);
            _minimizedTabPopup.Font = BeepThemesManager.ToFont(_theme.CommandTypography);
            _minimizedTabPopup.ForeColor = _theme.Text;
            _minimizedTabPopup.BackColor = _theme.GroupBack;
            _minimizedTabPopup.Renderer = new BeepRibbonToolStripRenderer(this);
            _searchBox.BackColor = _theme.TabActiveBack;
            _searchBox.ForeColor = _theme.Text;
            _searchBox.Font = BeepThemesManager.ToFont(_theme.CommandTypography);
            _searchResultsButton.ForeColor = _theme.Text;
            _searchResultsButton.BackColor = _theme.QuickAccessBack;
            _searchResultsButton.Font = BeepThemesManager.ToFont(_theme.CommandTypography);
            _keyTipToolTip.BackColor = _theme.TabActiveBack;
            _keyTipToolTip.ForeColor = _theme.Text;

            _contextHeader.BackColor = _theme.Background;
            _contextHeader.ForeColor = _theme.Text;
            _contextHeader.Font = BeepThemesManager.ToFont(_theme.ContextHeaderTypography);

            _backstagePanelContent.BackColor = _theme.Background;
            _backstageSplit.BackColor = _theme.Background;
            _backstageNavList.BackColor = _theme.GroupBack;
            _backstageNavList.ForeColor = _theme.Text;
            _backstageNavList.Font = BeepThemesManager.ToFont(_theme.GroupTypography);
            _backstageContentHost.BackColor = _theme.Background;
            _backstageTitle.BackColor = _theme.GroupBack;
            _backstageTitle.ForeColor = _theme.Text;
            _backstageTitle.Font = BeepThemesManager.ToFont(_theme.TabTypography);
            _backstageActions.BackColor = _theme.Background;
            _backstageFooter.BackColor = _theme.Background;
            foreach (var control in _backstageActions.Controls.OfType<Button>())
            {
                control.BackColor = _theme.TabActiveBack;
                control.ForeColor = _theme.Text;
                control.Font = BeepThemesManager.ToFont(_theme.CommandTypography);
                control.FlatAppearance.BorderColor = _theme.GroupBorder;
                control.FlatAppearance.MouseDownBackColor = _theme.GroupBack;
                control.FlatAppearance.MouseOverBackColor = ControlPaint.Light(_theme.GroupBack, .1f);
            }
            foreach (var control in _backstageFooter.Controls.OfType<Button>())
            {
                control.BackColor = _theme.TabActiveBack;
                control.ForeColor = _theme.Text;
                control.Font = BeepThemesManager.ToFont(_theme.CommandTypography);
                control.FlatAppearance.BorderColor = _theme.GroupBorder;
                control.FlatAppearance.MouseDownBackColor = _theme.GroupBack;
                control.FlatAppearance.MouseOverBackColor = ControlPaint.Light(_theme.GroupBack, .1f);
            }

            foreach (TabPage page in _tabs.TabPages)
            {
                page.BackColor = _theme.TabActiveBack;
                page.ForeColor = _theme.Text;

                foreach (var group in page.Controls.OfType<BeepRibbonGroup>())
                {
                    group.Renderer = new BeepRibbonToolStripRenderer(this);
                    group.Density = _density;
                    group.ApplyTheme(_theme);

                    foreach (var host in group.Items.OfType<ToolStripControlHost>())
                    {
                        if (host.Control is BeepRibbonGallery gallery)
                        {
                            gallery.ApplyTheme(_theme, _density);
                        }
                    }
                }
            }

            ApplyRightToLeftLayout();
            Invalidate();
        }

        private void ApplyRightToLeftLayout()
        {
            var rtl = _ribbonRightToLeft ? RightToLeft.Yes : RightToLeft.No;
            RightToLeft = rtl;
            _tabs.RightToLeft = rtl;
            _quickAccess.RightToLeft = rtl;
            _contextHeader.RightToLeft = rtl;
            _backstagePanelContent.RightToLeft = rtl;
            _backstageSplit.RightToLeft = rtl;
            _backstageNavList.RightToLeft = rtl;
            _backstageContentHost.RightToLeft = rtl;
            _backstageTitle.RightToLeft = rtl;
            _backstageActions.RightToLeft = rtl;
            _backstageFooter.RightToLeft = rtl;
            _backstageFooter.FlowDirection = _ribbonRightToLeft ? FlowDirection.LeftToRight : FlowDirection.RightToLeft;
            ApplyRightToLeftToRibbonPages(rtl);
            ApplyRightToLeftRecursive(_backstageActions, rtl);
            ApplyRightToLeftRecursive(_backstageFooter, rtl);
            ApplySearchAccessibility();
            ApplyPaneTabOrder();
        }

        private void ApplyRightToLeftToRibbonPages(RightToLeft rtl)
        {
            foreach (TabPage page in _tabs.TabPages)
            {
                page.RightToLeft = rtl;
                ApplyRightToLeftRecursive(page, rtl);
                foreach (var group in page.Controls.OfType<BeepRibbonGroup>())
                {
                    group.RightToLeft = rtl;
                    foreach (var host in group.Items.OfType<ToolStripControlHost>())
                    {
                        if (host.Control != null)
                        {
                            host.Control.RightToLeft = rtl;
                        }
                    }
                }
            }
        }

        private static void ApplyRightToLeftRecursive(Control root, RightToLeft rtl)
        {
            if (root == null)
            {
                return;
            }

            root.RightToLeft = rtl;
            foreach (Control child in root.Controls)
            {
                ApplyRightToLeftRecursive(child, rtl);
            }
        }

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);
            bool rtl = RightToLeft == RightToLeft.Yes;
            if (_ribbonRightToLeft != rtl)
            {
                _ribbonRightToLeft = rtl;
            }
            ApplyRightToLeftLayout();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using var back = new SolidBrush(_theme.Background);
            e.Graphics.FillRectangle(back, ClientRectangle);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (_isMinimized)
            {
                int minimizedHeight = CalculateMinimizedHeight();
                if (Height != minimizedHeight)
                {
                    Height = minimizedHeight;
                }
            }
            else
            {
                _expandedRibbonHeight = Math.Max(_expandedRibbonHeight, Height);
            }
            ApplyResponsiveLayout();
            _tabs.Invalidate();
            _contextHeader.Invalidate();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (_keyboardMap.TryInvoke(keyData))
            {
                return true;
            }

            if (keyData == Keys.F1 && _useSuperToolTips && _hoveredTooltipCommand != null)
            {
                var command = _hoveredTooltipCommand;
                var model = _hoveredTooltipModel ?? BuildSuperTooltipModel(command);
                TooltipActionRequested?.Invoke(this, new RibbonTooltipActionRequestedEventArgs(command, model, "help"));
                return true;
            }

            if (keyData == (Keys.Control | Keys.F) && _searchMode != RibbonSearchMode.Off)
            {
                _searchBox.Focus();
                _searchBox.SelectAll();
                return true;
            }

            if (keyData == Keys.Menu && _enableKeyTips)
            {
                if (_keyTipsVisible)
                {
                    HideKeyTips();
                }
                else
                {
                    ShowKeyTips();
                }
                return true;
            }

            if (_keyTipsVisible)
            {
                if (keyData == Keys.Escape)
                {
                    HideKeyTips();
                    return true;
                }

                if (TryInvokeKeyTip(keyData))
                {
                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private bool FocusRibbonPane(int direction)
        {
            var panes = new List<Control>
            {
                _quickAccess,
                _tabs
            };
            if (_searchMode != RibbonSearchMode.Off && _searchBox.Control != null)
            {
                panes.Add(_searchBox.Control);
            }

            if (_backstageDropDown.Visible)
            {
                panes.Add(_backstageNavList);
                panes.Add(_backstageActions);
                panes.Add(_backstageFooter);
            }

            panes = panes.Where(p => p != null && p.CanFocus && p.Visible && p.Enabled).ToList();
            if (panes.Count == 0)
            {
                return false;
            }

            int currentIndex = panes.FindIndex(p => p.ContainsFocus);
            if (currentIndex < 0)
            {
                currentIndex = 0;
            }

            int nextIndex = direction >= 0
                ? (currentIndex + 1) % panes.Count
                : (currentIndex - 1 + panes.Count) % panes.Count;

            return panes[nextIndex].Focus();
        }

        public TabPage AddPage(string title)
        {
            var page = new TabPage(title) { BackColor = _theme.TabActiveBack, ForeColor = _theme.Text };
            _tabs.TabPages.Add(page);
            return page;
        }

        public BeepRibbonGroup AddGroup(TabPage page, string title)
        {
            var group = new BeepRibbonGroup
            {
                Text = title,
                Density = _density,
                Renderer = new BeepRibbonToolStripRenderer(this)
            };
            group.ApplyTheme(_theme);
            page.Controls.Add(group);
            page.Controls.SetChildIndex(group, 0);
            return group;
        }

        public void SaveQuickAccessTo(string file)
        {
            try
            {
                File.WriteAllLines(file, _quickAccessCommandKeys);
            }
            catch
            {
            }
        }

        public void LoadQuickAccessFrom(string file)
        {
            try
            {
                if (!File.Exists(file)) return;
                var lines = File.ReadAllLines(file);
                _quickAccessCommandKeys.Clear();
                foreach (var l in lines.Where(l => !string.IsNullOrWhiteSpace(l)))
                {
                    string? resolved = ResolveQuickAccessKey(l);
                    if (!string.IsNullOrWhiteSpace(resolved) &&
                        !_quickAccessCommandKeys.Contains(resolved, StringComparer.OrdinalIgnoreCase))
                    {
                        _quickAccessCommandKeys.Add(resolved);
                    }
                }
                RebuildQuickAccessToolbar();
            }
            catch
            {
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _commandItems.ListChanged -= CommandItems_ListChanged;
                _backstageItems.ListChanged -= BackstageItems_ListChanged;
                _backstageRecentItems.ListChanged -= BackstageItems_ListChanged;
                _backstagePinnedItems.ListChanged -= BackstageItems_ListChanged;
                _backstageFooterItems.ListChanged -= BackstageFooterItems_ListChanged;
                _backstageNavList.SelectedIndexChanged -= BackstageNavList_SelectedIndexChanged;
                _backstageActions.SizeChanged -= BackstageActions_SizeChanged;
                _backstageButton.DropDownOpening -= BackstageButton_DropDownOpening;
                _backstageDropDown.Closed -= BackstageDropDown_Closed;
                _tabs.MouseDoubleClick -= Tabs_MouseDoubleClick;
                _tabs.MouseUp -= Tabs_MouseUp;
                _tabs.SelectedIndexChanged -= Tabs_SelectedIndexChanged;
                UnsubscribeThemeManager();
                _contextTransitionTimer.Stop();
                _contextTransitionTimer.Tick -= ContextTransitionTimer_Tick;
                _contextTransitionTimer.Dispose();
                _backstageTransitionTimer.Stop();
                _backstageTransitionTimer.Tick -= BackstageTransitionTimer_Tick;
                _backstageTransitionTimer.Dispose();
                _keyboardMap.Clear();
                HideKeyTips();
                HideMinimizedPopup();
                _minimizedTabPopup.Closed -= MinimizedTabPopup_Closed;
                _minimizedTabPopup.Dispose();
                _superTooltip.Dispose();
                ClearBackstageActions();
                ClearBackstageFooterActions();
                DisposeMinimizedImages();
                DisposeGeneratedImages();
            }
            base.Dispose(disposing);
        }

        private sealed class BeepRibbonToolStripRenderer(BeepRibbonControl owner) : ToolStripProfessionalRenderer(new ProfessionalColorTable())
        {
            protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
            {
                BeepRibbonRenderer.DrawToolStripSurface(
                    e.Graphics,
                    new Rectangle(Point.Empty, e.ToolStrip.Size),
                    owner._theme);
            }

            protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
            {
                var button = e.Item as ToolStripButton;
                if (button == null && e.Item is not ToolStripDropDownButton)
                {
                    base.OnRenderButtonBackground(e);
                    return;
                }

                Rectangle bounds = new Rectangle(Point.Empty, e.Item.Size);
                BeepRibbonRenderer.DrawInteractiveItem(
                    e.Graphics,
                    bounds,
                    owner._theme,
                    hovered: e.Item.Selected,
                    pressed: e.Item.Pressed,
                    enabled: e.Item.Enabled,
                    selected: (e.Item as ToolStripButton)?.Checked == true);
            }

            protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
            {
                using var pen = new Pen(owner._theme.Separator);
                if (e.Vertical)
                {
                    int x = e.Item.Width / 2;
                    e.Graphics.DrawLine(pen, x, 4, x, e.Item.Height - 4);
                }
                else
                {
                    int y = e.Item.Height / 2;
                    e.Graphics.DrawLine(pen, 4, y, e.Item.Width - 4, y);
                }
            }

            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                e.TextColor = e.Item.Enabled ? owner._theme.Text : owner._theme.DisabledText;
                base.OnRenderItemText(e);
            }
        }
    }

    public sealed class RibbonCommandInvokedEventArgs : EventArgs
    {
        public RibbonCommandInvokedEventArgs(SimpleItem command, ToolStripItem source)
        {
            Command = command;
            Source = source;
        }

        public SimpleItem Command { get; }
        public ToolStripItem Source { get; }
    }

    public sealed class BackstageCommandInvokedEventArgs : EventArgs
    {
        public BackstageCommandInvokedEventArgs(SimpleItem section, SimpleItem command)
        {
            Section = section;
            Command = command;
        }

        public SimpleItem Section { get; }
        public SimpleItem Command { get; }
    }

    public sealed class BackstageSectionChangedEventArgs : EventArgs
    {
        public BackstageSectionChangedEventArgs(SimpleItem section, int sectionIndex)
        {
            Section = section;
            SectionIndex = sectionIndex;
        }

        public SimpleItem Section { get; }
        public int SectionIndex { get; }
    }

    public sealed class RibbonMergedEventArgs : EventArgs
    {
        public RibbonMergedEventArgs(RibbonMergeMode mergeMode, int sourceTabCount, int resultTabCount)
        {
            MergeMode = mergeMode;
            SourceTabCount = sourceTabCount;
            ResultTabCount = resultTabCount;
        }

        public RibbonMergeMode MergeMode { get; }
        public int SourceTabCount { get; }
        public int ResultTabCount { get; }
    }

    public sealed class RibbonCustomizationAppliedEventArgs : EventArgs
    {
        public RibbonCustomizationAppliedEventArgs(RibbonCustomizationAction action, int tabCount, int quickAccessCount)
        {
            Action = action;
            TabCount = tabCount;
            QuickAccessCount = quickAccessCount;
        }

        public RibbonCustomizationAction Action { get; }
        public int TabCount { get; }
        public int QuickAccessCount { get; }
    }

    public sealed class RibbonSearchExecutedEventArgs : EventArgs
    {
        public RibbonSearchExecutedEventArgs(string query, RibbonSearchMode mode, int resultCount, bool providerUsed, bool providerFailed, bool usedLocalFallback)
        {
            Query = query;
            Mode = mode;
            ResultCount = resultCount;
            ProviderUsed = providerUsed;
            ProviderFailed = providerFailed;
            UsedLocalFallback = usedLocalFallback;
        }

        public string Query { get; }
        public RibbonSearchMode Mode { get; }
        public int ResultCount { get; }
        public bool ProviderUsed { get; }
        public bool ProviderFailed { get; }
        public bool UsedLocalFallback { get; }
    }

    public sealed class RibbonTooltipActionRequestedEventArgs : EventArgs
    {
        public RibbonTooltipActionRequestedEventArgs(SimpleItem command, RibbonSuperTooltipModel model, string action)
        {
            Command = command;
            Model = model;
            Action = action ?? string.Empty;
        }

        public SimpleItem Command { get; }
        public RibbonSuperTooltipModel Model { get; }
        public string Action { get; }
    }

    public sealed class RibbonCustomizationState
    {
        public int SchemaVersion { get; set; } = 2;
        public RibbonLayoutMode LayoutMode { get; set; } = RibbonLayoutMode.Classic;
        public RibbonDensity Density { get; set; } = RibbonDensity.Comfortable;
        public RibbonSearchMode SearchMode { get; set; } = RibbonSearchMode.Off;
        public bool SearchIncludeBackstage { get; set; }
        public int SearchMaxResults { get; set; } = 12;
        public bool EnableKeyTips { get; set; } = true;
        public bool QuickAccessAboveRibbon { get; set; } = true;
        public bool IsMinimized { get; set; }
        public bool ShowMinimizedPopupOnTabClick { get; set; } = true;
        public int BackstageSelectedIndex { get; set; } = 0;
        public List<string> QuickAccessCommandKeys { get; set; } = [];
        public List<RibbonTabState> Tabs { get; set; } = [];
    }

    public sealed class RibbonTabState
    {
        public string TabKey { get; set; } = string.Empty;
        public bool Visible { get; set; } = true;
        public int Order { get; set; }
        public List<RibbonGroupState> Groups { get; set; } = [];
    }

    public sealed class RibbonGroupState
    {
        public string GroupKey { get; set; } = string.Empty;
        public bool Visible { get; set; } = true;
        public int Order { get; set; }
    }

    public sealed class RibbonThemeTokens
    {
        public int Background { get; set; }
        public int TabActiveBack { get; set; }
        public int TabInactiveBack { get; set; }
        public int TabBorder { get; set; }
        public int GroupBack { get; set; }
        public int GroupBorder { get; set; }
        public int HoverBack { get; set; }
        public int PressedBack { get; set; }
        public int FocusBorder { get; set; }
        public int Separator { get; set; }
        public int Text { get; set; }
        public int IconColor { get; set; }
        public int QuickAccessBack { get; set; }
        public int QuickAccessBorder { get; set; }
        public int DisabledBack { get; set; }
        public int DisabledText { get; set; }
        public int DisabledBorder { get; set; }
        public int SelectionBack { get; set; }
        public int ElevationColor { get; set; }
        public int CornerRadius { get; set; }
        public int GroupSpacing { get; set; }
        public int ItemSpacing { get; set; }
        public int ElevationLevel { get; set; }
        public int ElevationStrongLevel { get; set; }
        public float FocusBorderThickness { get; set; }
        public RibbonTypographyToken? TabTypography { get; set; }
        public RibbonTypographyToken? GroupTypography { get; set; }
        public RibbonTypographyToken? CommandTypography { get; set; }
        public RibbonTypographyToken? ContextHeaderTypography { get; set; }

        public static RibbonThemeTokens FromTheme(RibbonTheme theme)
        {
            return new RibbonThemeTokens
            {
                Background = theme.Background.ToArgb(),
                TabActiveBack = theme.TabActiveBack.ToArgb(),
                TabInactiveBack = theme.TabInactiveBack.ToArgb(),
                TabBorder = theme.TabBorder.ToArgb(),
                GroupBack = theme.GroupBack.ToArgb(),
                GroupBorder = theme.GroupBorder.ToArgb(),
                HoverBack = theme.HoverBack.ToArgb(),
                PressedBack = theme.PressedBack.ToArgb(),
                FocusBorder = theme.FocusBorder.ToArgb(),
                Separator = theme.Separator.ToArgb(),
                Text = theme.Text.ToArgb(),
                IconColor = theme.IconColor.ToArgb(),
                QuickAccessBack = theme.QuickAccessBack.ToArgb(),
                QuickAccessBorder = theme.QuickAccessBorder.ToArgb(),
                DisabledBack = theme.DisabledBack.ToArgb(),
                DisabledText = theme.DisabledText.ToArgb(),
                DisabledBorder = theme.DisabledBorder.ToArgb(),
                SelectionBack = theme.SelectionBack.ToArgb(),
                ElevationColor = theme.ElevationColor.ToArgb(),
                CornerRadius = theme.CornerRadius,
                GroupSpacing = theme.GroupSpacing,
                ItemSpacing = theme.ItemSpacing,
                ElevationLevel = theme.ElevationLevel,
                ElevationStrongLevel = theme.ElevationStrongLevel,
                FocusBorderThickness = theme.FocusBorderThickness,
                TabTypography = RibbonTypographyToken.FromStyle(theme.TabTypography),
                GroupTypography = RibbonTypographyToken.FromStyle(theme.GroupTypography),
                CommandTypography = RibbonTypographyToken.FromStyle(theme.CommandTypography),
                ContextHeaderTypography = RibbonTypographyToken.FromStyle(theme.ContextHeaderTypography)
            };
        }

        public RibbonTheme ToTheme(RibbonTheme? target = null)
        {
            var theme = target ?? new RibbonTheme();
            theme.Background = Color.FromArgb(Background);
            theme.TabActiveBack = Color.FromArgb(TabActiveBack);
            theme.TabInactiveBack = Color.FromArgb(TabInactiveBack);
            theme.TabBorder = Color.FromArgb(TabBorder);
            theme.GroupBack = Color.FromArgb(GroupBack);
            theme.GroupBorder = Color.FromArgb(GroupBorder);
            theme.HoverBack = Color.FromArgb(HoverBack);
            theme.PressedBack = Color.FromArgb(PressedBack);
            theme.FocusBorder = Color.FromArgb(FocusBorder);
            theme.Separator = Color.FromArgb(Separator);
            theme.Text = Color.FromArgb(Text);
            theme.IconColor = Color.FromArgb(IconColor);
            theme.QuickAccessBack = Color.FromArgb(QuickAccessBack);
            theme.QuickAccessBorder = Color.FromArgb(QuickAccessBorder);
            theme.DisabledBack = Color.FromArgb(DisabledBack);
            theme.DisabledText = Color.FromArgb(DisabledText);
            theme.DisabledBorder = Color.FromArgb(DisabledBorder);
            theme.SelectionBack = Color.FromArgb(SelectionBack);
            theme.ElevationColor = Color.FromArgb(ElevationColor);
            theme.CornerRadius = CornerRadius;
            theme.GroupSpacing = GroupSpacing;
            theme.ItemSpacing = ItemSpacing;
            theme.ElevationLevel = ElevationLevel;
            theme.ElevationStrongLevel = ElevationStrongLevel;
            theme.FocusBorderThickness = FocusBorderThickness <= 0 ? theme.FocusBorderThickness : FocusBorderThickness;
            theme.TabTypography = TabTypography?.ToStyle(theme.TabTypography) ?? theme.TabTypography;
            theme.GroupTypography = GroupTypography?.ToStyle(theme.GroupTypography) ?? theme.GroupTypography;
            theme.CommandTypography = CommandTypography?.ToStyle(theme.CommandTypography) ?? theme.CommandTypography;
            theme.ContextHeaderTypography = ContextHeaderTypography?.ToStyle(theme.ContextHeaderTypography) ?? theme.ContextHeaderTypography;
            return theme;
        }
    }

    public sealed class RibbonTypographyToken
    {
        public string FontFamily { get; set; } = "Segoe UI";
        public float FontSize { get; set; } = 9f;
        public int FontWeight { get; set; } = (int)TheTechIdea.Beep.Vis.Modules.FontWeight.Normal;
        public int FontStyle { get; set; } = (int)System.Drawing.FontStyle.Regular;
        public bool IsUnderlined { get; set; }
        public bool IsStrikeout { get; set; }

        public static RibbonTypographyToken FromStyle(TypographyStyle style)
        {
            return new RibbonTypographyToken
            {
                FontFamily = style.FontFamily,
                FontSize = style.FontSize,
                FontWeight = (int)style.FontWeight,
                FontStyle = (int)style.FontStyle,
                IsUnderlined = style.IsUnderlined,
                IsStrikeout = style.IsStrikeout
            };
        }

        public TypographyStyle ToStyle(TypographyStyle fallback)
        {
            var style = new TypographyStyle
            {
                FontFamily = string.IsNullOrWhiteSpace(FontFamily) ? fallback.FontFamily : FontFamily,
                FontSize = FontSize <= 0 ? fallback.FontSize : FontSize,
                FontWeight = Enum.IsDefined(typeof(TheTechIdea.Beep.Vis.Modules.FontWeight), FontWeight)
                    ? (TheTechIdea.Beep.Vis.Modules.FontWeight)FontWeight
                    : fallback.FontWeight,
                FontStyle = Enum.IsDefined(typeof(System.Drawing.FontStyle), FontStyle)
                    ? (System.Drawing.FontStyle)FontStyle
                    : fallback.FontStyle,
                IsUnderlined = IsUnderlined,
                IsStrikeout = IsStrikeout
            };

            return style;
        }
    }
}

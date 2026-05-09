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
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepRibbonControl : Control
    {
        // Core UI controls
        private readonly TabControl _tabs = new() { Dock = DockStyle.Fill };
        private readonly ToolStrip _quickAccess = new() { Dock = DockStyle.Top, GripStyle = ToolStripGripStyle.Hidden, RenderMode = ToolStripRenderMode.System };
        private readonly Panel _contextHeader = new() { Dock = DockStyle.Top, Height = 18 };

        // Command mapping and organization
        private readonly BindingList<SimpleItem> _commandItems = new();
        private readonly Dictionary<ToolStripItem, SimpleItem> _commandMap = new();
        private readonly Dictionary<BeepRibbonGroup, List<SimpleItem>> _groupCommandNodes = new();
        private readonly Dictionary<string, SimpleItem> _commandLookup = new(StringComparer.OrdinalIgnoreCase);

        // Gallery state
        private readonly Dictionary<string, string> _galleryLastSelection = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, List<string>> _galleryPinnedKeys = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, List<string>> _galleryRecentKeys = new(StringComparer.OrdinalIgnoreCase);

        // Quick Access Toolbar
        private readonly List<string> _quickAccessCommandKeys = [];
        private readonly List<Image> _generatedImages = new();

        // Search UI and state
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

        // Keyboard and key tips
        private readonly ToolTip _keyTipToolTip = new() { ShowAlways = true, AutomaticDelay = 0, InitialDelay = 0, ReshowDelay = 0 };
        private readonly Dictionary<ToolStripItem, string> _keyTips = [];
        private readonly Dictionary<string, ToolStripItem> _keyTipLookup = new(StringComparer.OrdinalIgnoreCase);
        private readonly RibbonKeyboardMap _keyboardMap = new();

        // Theme and layout configuration
        private RibbonVariantMatrix _variantMatrix = RibbonVariantMatrix.CreateDefault();
        private readonly List<string> _lastTokenImportDiagnostics = [];

        // Merge and snapshot management
        private readonly List<SimpleItem> _mergeBaseSnapshot = [];

        // Minimized state
        private readonly ContextMenuStrip _minimizedTabPopup = new();
        private readonly List<Image> _minimizedGeneratedImages = [];

        // Theme and appearance settings
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

        // Customization state
        private RibbonCustomizationState? _pendingCustomizationState;
        private List<SimpleItem>? _defaultCustomizationSnapshot;
        private List<string>? _defaultQuickAccessSnapshot;

        // Layout application state
        private bool _isApplyingResponsiveLayout;

        // Search provider and style settings
        private IRibbonSearchProvider? _searchProvider;
        private bool _followGlobalFormStyle = true;
        private FormStyle _ribbonFormStyle = FormStyle.Modern;
        private RibbonStylePreset _resolvedStylePreset = RibbonStylePreset.OfficeLight;
        private bool _subscribedToThemeManager;

        // Backstage UI controls
        private readonly ToolStripDropDownButton _backstageButton;
        private readonly ToolStripDropDown _backstageDropDown;
        private readonly ToolStripControlHost _backstageHost;
        private readonly Panel _backstagePanelContent = new() { BackColor = TheTechIdea.Beep.Winform.Controls.Helpers.ColorUtils.MapSystemColor(SystemColors.ControlLightLight), Size = new Size(600, 400) };

        // Backstage items and collections
        private readonly BindingList<SimpleItem> _backstageItems = new();
        private readonly BindingList<SimpleItem> _backstageRecentItems = new();
        private readonly BindingList<SimpleItem> _backstagePinnedItems = new();
        private int _backstageRecentLimit = 12;
        private bool _backstageShowTimestamps = true;
        private bool _backstageUseRelativeTimestamps = true;
        private Func<SimpleItem, DateTime, string>? _backstageTimestampFormatter;

        // Backstage layout containers
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

        // Backstage transitions
        private readonly Timer _backstageTransitionTimer = new() { Interval = 16 };
        private bool _enableBackstageTransitions = true;
        private int _backstageTransitionDurationMs = 180;
        private int _backstageTransitionEffectiveDurationMs = 180;
        private DateTime _backstageTransitionStartUtc = DateTime.UtcNow;
        private Size _backstageTransitionStartSize = new(600, 400);
        private Size _backstageTransitionTargetSize = new(600, 400);

        // Contextual groups and transitions
        private readonly Dictionary<string, int> _contextualRuleMap = new(StringComparer.OrdinalIgnoreCase);
        private string _activeContextKey = string.Empty;
        private readonly Timer _contextTransitionTimer = new() { Interval = 16 };
        private bool _enableContextTransitions = true;
        private int _contextTransitionDurationMs = 180;
        private int _contextTransitionEffectiveDurationMs = 180;
        private float _contextTransitionProgress = 1f;

        // Right-to-left and accessibility settings
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
    }
}

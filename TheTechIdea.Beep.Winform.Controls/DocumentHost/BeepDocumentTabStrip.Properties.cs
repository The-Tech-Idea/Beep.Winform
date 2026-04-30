// BeepDocumentTabStrip.Properties.cs
// Properties, backing fields and events for BeepDocumentTabStrip.
// All state fields live in BeepDocumentTabStrip.cs (core partial).
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    public partial class BeepDocumentTabStrip
    {
        // ─────────────────────────────────────────────────────────────────────
        // Backing fields for properties declared in this partial
        // ─────────────────────────────────────────────────────────────────────

        private bool _showAddButton   = true;
        private TabCloseMode _closeMode = TabCloseMode.OnHover;
        private DocumentTabStyle _tabStyle = DocumentTabStyle.Chrome;
        private bool _keyboardShortcutsEnabled = true;
        private TabSizeMode _tabSizeMode       = TabSizeMode.Equal;
        private int         _fixedTabWidth     = 160;    // logical px, used in Fixed mode
        private bool        _allowDragFloat    = true;
        private int         _animationDuration = 150;    // ms; 0 = disable animations
        private TabColorMode _tabColorMode     = TabColorMode.None;
        private TabRowMode   _tabRowMode       = TabRowMode.SingleRow;
        private readonly System.Collections.Generic.List<TabGroup> _tabGroups
            = new System.Collections.Generic.List<TabGroup>();
        // Computed during CalculateTabLayout; used by host layout
        internal int RowCount { get; private set; } = 1;

        private TabTooltipMode _tooltipMode       = TabTooltipMode.Simple;
        private int            _tooltipHoverDelay = Tokens.DocTokens.TooltipDelayMs; // ms delay before showing rich tooltip (800)

        // Sprint 18 — Density + Responsive
        private TabDensityMode     _tabDensity           = TabDensityMode.Comfortable;
        private ResponsiveBreakpoints _responsiveBreakpoints = new ResponsiveBreakpoints();
        internal TabResponsiveMode _responsiveMode       = TabResponsiveMode.Normal;
        private bool _preferPainterRendering = true;

        // ─────────────────────────────────────────────────────────────────────
        // Read-only / hidden properties
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Ordered list of tabs.  Mutate via AddTab / RemoveTabAt.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IReadOnlyList<BeepDocumentTab> Tabs => _tabs;

        /// <summary>Current horizontal scroll offset in pixels (0 = fully left).</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal int ScrollOffset
        {
            get => _scrollOffset;
            set { _scrollOffset = Math.Max(0, value); CalculateTabLayout(); Invalidate(); }
        }

        /// <summary>Zero-based index of the active tab, or -1 when empty.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ActiveTabIndex
        {
            get => _activeTabIndex;
            internal set
            {
                if (_activeTabIndex == value) return;

                // Deactivate old
                if (_activeTabIndex >= 0 && _activeTabIndex < _tabs.Count)
                    _tabs[_activeTabIndex].IsActive = false;

                int prev = _activeTabIndex;
                _activeTabIndex = value;

                // Activate new + animate indicator
                if (_activeTabIndex >= 0 && _activeTabIndex < _tabs.Count)
                {
                    _tabs[_activeTabIndex].IsActive = true;
                    StartIndicatorAnimation(prev, _activeTabIndex);
                    ScrollTabIntoView(_activeTabIndex);

                    // Notify accessibility clients that selection changed
                    AccessibilityNotifyClients(
                        System.Windows.Forms.AccessibleEvents.Selection,
                        _activeTabIndex);
                }

                Invalidate();
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Designable properties
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Show the new-document (+) button at the end of the strip.</summary>
        [DefaultValue(true)]
        [Description("Show the new-document (+) button at the end of the tab strip.")]
        public bool ShowAddButton
        {
            get => _showAddButton;
            set
            {
                if (_showAddButton == value) return;
                _showAddButton = value;
                CalculateTabLayout();
                Invalidate();
            }
        }

        /// <summary>Controls when the close (×) glyph is rendered on each tab.</summary>
        [DefaultValue(TabCloseMode.OnHover)]
        [Description("Controls when the close (×) button is visible on each tab.")]
        public TabCloseMode CloseMode
        {
            get => _closeMode;
            set { _closeMode = value; Invalidate(); }
        }

        /// <summary>Visual rendering style of the tab strip (Chrome / VSCode / Underline / Pill).</summary>
        [DefaultValue(DocumentTabStyle.Chrome)]
        [Description("Visual rendering style of the tab strip.")]
        public DocumentTabStyle TabStyle
        {
            get => _tabStyle;
            set { _tabStyle = value; Invalidate(); }
        }

        /// <summary>
        /// When true, built-in keyboard shortcuts are active
        /// (Ctrl+Tab, Ctrl+W, Ctrl+F4, Ctrl+1-9).
        /// </summary>
        [DefaultValue(true)]
        [Description("Enable built-in keyboard shortcuts on the tab strip.")]
        public bool KeyboardShortcutsEnabled
        {
            get => _keyboardShortcutsEnabled;
            set => _keyboardShortcutsEnabled = value;
        }

        /// <summary>
        /// Backward-compatibility flag retained for consumers.
        /// Rendering is now always painter-pipeline based.
        /// </summary>
        [DefaultValue(true)]
        [Description("Backward-compatibility flag. Rendering is always painter-based.")]
        public bool PreferPainterRendering
        {
            get => true;
            set { _preferPainterRendering = true; Invalidate(); }
        }

        /// <summary>How unpinned tabs are sized within the available strip width.</summary>
        [DefaultValue(TabSizeMode.Equal)]
        [Description("How unpinned tabs are sized: Equal, FitToContent, Compact, or Fixed.")]
        public TabSizeMode TabSizeMode
        {
            get => _tabSizeMode;
            set { _tabSizeMode = value; CalculateTabLayout(); Invalidate(); }
        }

        /// <summary>
        /// Width (logical 96-dpi pixels) used for every tab when
        /// <see cref="TabSizeMode"/> is <see cref="TabSizeMode.Fixed"/>.
        /// </summary>
        [DefaultValue(160)]
        [Description("Tab width in logical pixels when TabSizeMode = Fixed.")]
        public int FixedTabWidth
        {
            get => _fixedTabWidth;
            set { _fixedTabWidth = Math.Max(40, value); CalculateTabLayout(); Invalidate(); }
        }

        /// <summary>
        /// When true, dragging a tab far enough vertically detaches it into a float window.
        /// </summary>
        [DefaultValue(true)]
        [Description("Allow dragging a tab off the strip to detach it into a floating window.")]
        public bool AllowDragFloat
        {
            get => _allowDragFloat;
            set => _allowDragFloat = value;
        }

        /// <summary>
        /// Duration in milliseconds of the tab-open slide-in animation.
        /// Set to 0 to disable.
        /// </summary>
        [DefaultValue(150)]
        [Description("Duration in ms of the tab open animation.  Set to 0 to disable.")]
        public int AnimationDuration
        {
            get => _animationDuration;
            set => _animationDuration = Math.Max(0, value);
        }

        /// <summary>Controls how per-document <see cref="BeepDocumentTab.TabColor"/> is rendered.</summary>
        [DefaultValue(TabColorMode.None)]
        [Description("How per-document tab colours are rendered on each tab.")]
        public TabColorMode TabColorMode
        {
            get => _tabColorMode;
            set { _tabColorMode = value; Invalidate(); }
        }

        /// <summary>Name of the Beep theme applied to this control.</summary>
        [DefaultValue("")]
        [Description("Beep theme name applied to this control.")]
        public string ThemeName
        {
            get => _localThemeName;
            set
            {
                _localThemeName = value ?? string.Empty;
                _currentTheme     = BeepThemesManager.GetTheme(_localThemeName)
                             ?? BeepThemesManager.GetDefaultTheme();
                ApplyThemeColors();
                CalculateTabLayout();
                Invalidate();
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Tooltip properties
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Controls whether hovering a tab shows no tooltip, a simple text tooltip, or a rich popup with thumbnail.</summary>
        [DefaultValue(TabTooltipMode.Simple)]
        [Description("Controls tooltip behaviour when hovering over a tab.")]
        public TabTooltipMode TooltipMode
        {
            get => _tooltipMode;
            set
            {
                if (_tooltipMode == value) return;
                _tooltipMode = value;
                // Hide any currently visible tooltip/popup when mode changes
                _tooltip.SetToolTip(this, string.Empty);
            }
        }

        /// <summary>Delay in milliseconds before the rich tooltip popup is displayed.</summary>
        [DefaultValue(500)]
        [Description("Delay (ms) before the rich tooltip popup appears on hover.")]
        public int TooltipHoverDelay
        {
            get => _tooltipHoverDelay;
            set => _tooltipHoverDelay = Math.Max(0, value);
        }

        // ───────────────────────────────────────────────────────────────────
        // Sprint 18.3 — Tab Density
        // ───────────────────────────────────────────────────────────────────

        /// <summary>
        /// Controls the vertical density (row height + font size) of the tab strip.
        /// <list type="bullet">
        /// <item><description><c>Comfortable</c> — 36 px / 12 pt (default)</description></item>
        /// <item><description><c>Compact</c> — 28 px / 11 pt</description></item>
        /// <item><description><c>Dense</c> — 22 px / 10 pt</description></item>
        /// </list>
        /// </summary>
        [DefaultValue(TabDensityMode.Comfortable)]
        [Description("Vertical density of the tab strip: Comfortable (36 px), Compact (28 px) or Dense (22 px).")]
        public TabDensityMode TabDensity
        {
            get => _tabDensity;
            set
            {
                if (_tabDensity == value) return;
                _tabDensity = value;
                ApplyDensityFont();
                if (IsHandleCreated) Height = TabHeight;
                CalculateTabLayout();
                Invalidate();
            }
        }

        // ───────────────────────────────────────────────────────────────────
        // Sprint 18.2 — Responsive Breakpoints
        // ───────────────────────────────────────────────────────────────────

        /// <summary>
        /// Configurable pixel-width thresholds that determine how much information
        /// is shown on each tab as the strip becomes narrower.
        /// Replace the instance or adjust its properties; triggers a layout + repaint.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ResponsiveBreakpoints ResponsiveBreakpoints
        {
            get => _responsiveBreakpoints;
            set
            {
                _responsiveBreakpoints = value ?? new ResponsiveBreakpoints();
                CalculateTabLayout();
                Invalidate();
            }
        }

        /// <summary>
        /// Optional factory that returns a thumbnail bitmap for a given document ID.
        /// Used by the rich tooltip popup.  Return null to omit the thumbnail.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<string, System.Drawing.Bitmap?>? ThumbnailProvider { get; set; }

        // ─────────────────────────────────────────────────────────────────────
        // Multi-row + tab groups (7.3, 7.5)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Controls whether tabs overflow into a scrollable single row (default) or wrap
        /// onto multiple rows with no scrolling.
        /// </summary>
        [DefaultValue(TabRowMode.SingleRow)]
        [Description("SingleRow = scrollable overflow (default). MultiRow = wrap onto extra rows.")]
        public TabRowMode TabRowMode
        {
            get => _tabRowMode;
            set { _tabRowMode = value; CalculateTabLayout(); Invalidate(); }
        }

        /// <summary>Named tab groups; each group appears as a labelled separator in the tab strip.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Collections.Generic.IList<TabGroup> TabGroups => _tabGroups;

        // ─────────────────────────────────────────────────────────────────────
        // Vertical tabs (7.6)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// When true the strip is oriented vertically (<see cref="TabStripPosition.Left"/>
        /// or <see cref="TabStripPosition.Right"/>).  Layout and painting switch to vertical mode.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsVertical { get; internal set; }

        // ─────────────────────────────────────────────────────────────────────
        // Events
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Raised when the user activates a tab.</summary>
        public event System.EventHandler<TabEventArgs>?          TabSelected;

        /// <summary>
        /// Raised just BEFORE the close button action is processed.
        /// Set <see cref="TabClosingEventArgs.Cancel"/> = true to prevent the close.
        /// </summary>
        public event System.EventHandler<TabClosingEventArgs>?   TabClosing;

        /// <summary>Raised when the user clicks the close (×) button on a tab (after TabClosing, if not cancelled).</summary>
        public event System.EventHandler<TabEventArgs>?          TabCloseRequested;

        /// <summary>Raised when the user requests a tab be floated into its own window.</summary>
        public event System.EventHandler<TabEventArgs>?          TabFloatRequested;

        /// <summary>Raised when a tab's pinned state is toggled via context menu.</summary>
        public event System.EventHandler<TabEventArgs>?          TabPinToggled;

        /// <summary>Raised when the user clicks the (+) add button.</summary>
        public event System.EventHandler?                        AddButtonClicked;

        /// <summary>Raised after a successful drag-to-reorder operation.</summary>
        public event System.EventHandler<TabReorderArgs>?        TabReordered;

        /// <summary>
        /// Raised just before the right-click context menu is shown.
        /// Allows adding/removing items or suppressing the menu entirely.
        /// </summary>
        public event System.EventHandler<TabContextMenuEventArgs>? TabContextMenuOpening;

        /// <summary>Raised when the user requests a horizontal split for the tab via context menu.</summary>
        public event System.EventHandler<TabEventArgs>?          TabSplitHorizontalRequested;

        /// <summary>Raised when the user requests a vertical split for the tab via context menu.</summary>
        public event System.EventHandler<TabEventArgs>?          TabSplitVerticalRequested;

        /// <summary>Raised when the user requests a tab be moved to a different tab group.</summary>
        public event System.EventHandler<TabMoveGroupEventArgs>? TabMoveToGroupRequested;
    }
}

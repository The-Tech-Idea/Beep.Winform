// BeepDocumentHost.Properties.cs
// Backing fields, designable properties, and public events for BeepDocumentHost.
// All state fields live in BeepDocumentHost.cs (core partial).
// ─────────────────────────────────────────────────────────────────────────────────────────
using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>Controls the border/elevation appearance of a <see cref="BeepDocumentHost"/>.</summary>
    public enum DocumentHostStyle
    {
        /// <summary>No border — content area merges seamlessly with the parent surface.</summary>
        Flat,
        /// <summary>Thin 1 px border drawn using the theme's BorderColor.</summary>
        Thin,
        /// <summary>Raised 3D border that lifts the host above surrounding controls.</summary>
        Raised
    }

    public partial class BeepDocumentHost
    {
        // ─────────────────────────────────────────────────────────────────────
        // Backing fields  (referenced in the constructor in the core partial)
        // ─────────────────────────────────────────────────────────────────────

        private TabStripPosition _tabPosition   = TabStripPosition.Top;
        private bool             _showAddButton = true;
        private TabCloseMode     _closeMode     = TabCloseMode.OnHover;
        private DocumentTabStyle _tabStyle      = DocumentTabStyle.Chrome;
        private DocumentHostStyle _controlStyle = DocumentHostStyle.Flat;
        private bool             _keyboardShortcutsEnabled = true;
        private TabColorMode     _tabColorMode  = TabColorMode.None;
        private bool             _autoSaveLayout = false;
        private string           _sessionFile   = string.Empty;

        // MRU tracking
        private readonly System.Collections.Generic.LinkedList<string>  _mruList     = new System.Collections.Generic.LinkedList<string>();
        private int _maxRecentHistory = 20;

        // Closed-tab history
        private readonly System.Collections.Generic.Stack<ClosedTabRecord> _closedStack = new System.Collections.Generic.Stack<ClosedTabRecord>();
        private int _maxClosedHistory = 10;

        // ─────────────────────────────────────────────────────────────────────
        // Read-only / hidden properties
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Id of the currently visible (active) document, or null.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? ActiveDocumentId => _activeDocumentId;

        /// <summary>Returns the active <see cref="BeepDocumentPanel"/>, or null.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BeepDocumentPanel? ActivePanel =>
            _activeDocumentId != null && _panels.TryGetValue(_activeDocumentId, out var p) ? p : null;

        /// <summary>Number of docked (non-floated) documents.</summary>
        [Browsable(false)]
        public int DocumentCount => _panels.Count;

        /// <summary>
        /// A lazily-created <see cref="IDocumentHostCommandService"/> that wraps this host.
        /// Use this to wire commands in MVVM or to pass the service via dependency injection.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IDocumentHostCommandService CommandService
            => _commandService ??= new DocumentHostCommandService(this);
        private IDocumentHostCommandService? _commandService;

        private bool _showEmptyState = true;
        /// <summary>
        /// When <see langword="true"/> (default) a centered illustration is painted in the content
        /// area whenever there are no open documents.  Set to <see langword="false"/> to suppress it.
        /// </summary>
        [DefaultValue(true)]
        [Description("Show a centred empty-state illustration when no documents are open.")]
        public bool ShowEmptyState
        {
            get => _showEmptyState;
            set
            {
                _showEmptyState = value;
                if (_contentArea != null) _contentArea.Invalidate();
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Designable properties
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Where the tab strip is positioned relative to the document panels.</summary>
        [DefaultValue(TabStripPosition.Top)]
        [Description("Position of the tab strip relative to the document panels.")]
        public TabStripPosition TabPosition
        {
            get => _tabPosition;
            set { _tabPosition = value; RecalculateLayout(); }
        }

        /// <summary>Show the new-document (+) button in the tab strip.</summary>
        [DefaultValue(true)]
        [Description("Show the new-document (+) button in the tab strip.")]
        public bool ShowAddButton
        {
            get => _showAddButton;
            set { _showAddButton = value; _tabStrip.ShowAddButton = value; }
        }

        /// <summary>Controls when the close (×) button appears on tabs.</summary>
        [DefaultValue(TabCloseMode.OnHover)]
        [Description("Controls when the close (×) button is visible on each tab.")]
        public TabCloseMode CloseMode
        {
            get => _closeMode;
            set { _closeMode = value; _tabStrip.CloseMode = value; }
        }

        /// <summary>Visual rendering style propagated to the tab strip.</summary>
        [DefaultValue(DocumentTabStyle.Chrome)]
        [Description("Visual style of the tab strip (Chrome / VSCode / Underline / Pill).")]
        public DocumentTabStyle TabStyle
        {
            get => _tabStyle;
            set { _tabStyle = value; _tabStrip.TabStyle = value; }
        }

        /// <summary>Specifies the border/elevation style of the document host container.</summary>
        [DefaultValue(DocumentHostStyle.Flat)]
        [Description("Border/elevation style of the document host container.")]
        public DocumentHostStyle ControlStyle
        {
            get => _controlStyle;
            set { _controlStyle = value; ApplyThemeColors(); Invalidate(); }
        }

        /// <summary>
        /// When true, keyboard shortcuts on the tab strip (Ctrl+Tab, Ctrl+W, Ctrl+1-9) are active.
        /// Propagated to the underlying <see cref="BeepDocumentTabStrip"/>.
        /// </summary>
        [DefaultValue(true)]
        [Description("Enable built-in keyboard shortcuts on the tab strip.")]
        public bool KeyboardShortcutsEnabled
        {
            get => _keyboardShortcutsEnabled;
            set { _keyboardShortcutsEnabled = value; _tabStrip.KeyboardShortcutsEnabled = value; }
        }

        /// <summary>Controls how per-document tab colours are applied to tab backgrounds.</summary>
        [DefaultValue(TabColorMode.None)]
        [Description("How per-document tab colours are rendered: None, AccentBar, FullBackground, or BottomBorder.")]
        public TabColorMode TabColorMode
        {
            get => _tabColorMode;
            set { _tabColorMode = value; _tabStrip.TabColorMode = value; _tabStrip.Invalidate(); }
        }

        /// <summary>
        /// When true, the tab layout is automatically saved to <see cref="SessionFile"/> on dispose
        /// and restored on the next load if the file exists.
        /// </summary>
        [DefaultValue(false)]
        [Description("Automatically save and restore the tab layout using SessionFile.")]
        public bool AutoSaveLayout
        {
            get => _autoSaveLayout;
            set => _autoSaveLayout = value;
        }

        /// <summary>Path to the JSON session file used by <see cref="AutoSaveLayout"/>.</summary>
        [DefaultValue("")]
        [Description("File path for the automatic layout save/restore (AutoSaveLayout must be true).")]
        public string SessionFile
        {
            get => _sessionFile;
            set => _sessionFile = value ?? string.Empty;
        }

        /// <summary>Beep theme name propagated to the tab strip and all document panels.</summary>
        [DefaultValue("")]
        [Description("Beep theme name propagated to the tab strip and document panels.")]
        public string ThemeName
        {
            get => _themeName;
            set
            {
                _themeName = value ?? string.Empty;
                _theme = ThemeManagement.BeepThemesManager.GetTheme(_themeName)
                         ?? ThemeManagement.BeepThemesManager.GetDefaultTheme();
                PropagateTheme(_themeName);
                Invalidate();
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Events
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Raised when the active document changes.</summary>
        public event System.EventHandler<DocumentEventArgs>? ActiveDocumentChanged;

        /// <summary>Raised when the user clicks the (+) add button.</summary>
        public event System.EventHandler? NewDocumentRequested;

        /// <summary>
        /// Raised just before a document is closed (after the tab strip fires TabClosing).
        /// Set <see cref="TabClosingEventArgs.Cancel"/> = true to prevent the close.
        /// </summary>
        public event System.EventHandler<TabClosingEventArgs>? DocumentClosing;

        /// <summary>Raised after a document is closed and removed.</summary>
        public event System.EventHandler<DocumentEventArgs>? DocumentClosed;

        /// <summary>Raised after a document is floated into its own window.</summary>
        public event System.EventHandler<DocumentEventArgs>? DocumentFloated;

        /// <summary>Raised after a previously floated document is docked back into the host.</summary>
        public event System.EventHandler<DocumentEventArgs>? DocumentDocked;

        /// <summary>Raised when a document's pinned state changes (from context menu or code).</summary>
        public event System.EventHandler<DocumentEventArgs>? DocumentPinChanged;

        /// <summary>
        /// Raised when <see cref="BeepDocumentHost.ReopenLastClosed"/> is invoked and a closed
        /// document record is available.  The consumer should re-open the panel and may read
        /// <see cref="ClosedTabRecord.RestoreData"/> to restore previous state.
        /// </summary>
        public event System.EventHandler<ClosedTabRecord>? ReopenDocumentRequested;

        // ─────────────────────────────────────────────────────────────────────
        // MRU + Closed-history properties
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Maximum number of document activations to remember in the MRU list.</summary>
        [DefaultValue(20)]
        [Description("Maximum number of recently-used documents tracked for MRU navigation.")]
        public int MaxRecentHistory
        {
            get => _maxRecentHistory;
            set => _maxRecentHistory = Math.Max(1, value);
        }

        /// <summary>Maximum number of closed documents kept for the Reopen Closed Tab feature.</summary>
        [DefaultValue(10)]
        [Description("Maximum number of recently-closed documents available for Ctrl+Shift+T.")]
        public int MaxClosedHistory
        {
            get => _maxClosedHistory;
            set => _maxClosedHistory = Math.Max(0, value);
        }

        /// <summary>Ordered list of document ids by most-recent activation (read-only snapshot).</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Collections.Generic.IReadOnlyList<string> RecentDocuments =>
            new System.Collections.Generic.List<string>(_mruList);

        /// <summary>True when there is at least one closed document available for reopen.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CanReopenClosed => _closedStack.Count > 0;

        // ─────────────────────────────────────────────────────────────────────
        // Cross-host drag
        // ─────────────────────────────────────────────────────────────────────

        private bool _allowDragBetweenHosts = false;

        /// <summary>
        /// When true, a tab dragged past the edge of this host will transfer to another
        /// <see cref="BeepDocumentHost"/> located under the cursor (if one is registered).
        /// </summary>
        [DefaultValue(false)]
        [Description("Allow tabs to be dragged from this host to another BeepDocumentHost in the same process.")]
        public bool AllowDragBetweenHosts
        {
            get => _allowDragBetweenHosts;
            set => _allowDragBetweenHosts = value;
        }

        /// <summary>
        /// Raised before a document is detached and transferred to another host.
        /// Set <see cref="DocumentTransferEventArgs.Cancel"/> = true to block the transfer.
        /// </summary>
        public event System.EventHandler<DocumentTransferEventArgs>? DocumentDetaching;

        /// <summary>Raised after a document has been received from another host.</summary>
        public event System.EventHandler<DocumentEventArgs>? DocumentAttached;

        // ─────────────────────────────────────────────────────────────────────
        // Document Groups — split view (feature 2.1)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Maximum number of simultaneous split groups (default 4).</summary>
        [DefaultValue(4)]
        [Description("Maximum number of side-by-side document groups allowed.")]
        public int MaxGroups
        {
            get => _maxGroups;
            set => _maxGroups = Math.Max(1, value);
        }

        /// <summary>
        /// When true (default), groups are split side-by-side horizontally.
        /// When false, groups are split one above the other.
        /// </summary>
        [DefaultValue(true)]
        [Description("True = side-by-side (horizontal) split; False = top/bottom (vertical) split.")]
        public bool SplitHorizontal
        {
            get => _splitHorizontal;
            set
            {
                if (_splitHorizontal == value) return;
                _splitHorizontal = value;
                if (_splitterBar != null)
                    _splitterBar.Cursor = value ? System.Windows.Forms.Cursors.VSplit
                                                : System.Windows.Forms.Cursors.HSplit;
                RecalculateLayout();
            }
        }

        /// <summary>
        /// Fraction (0.1 – 0.9) of the host's width (or height in vertical split)
        /// allocated to the first group.  Default 0.5 = equal split.
        /// </summary>
        [DefaultValue(0.5f)]
        [Description("Ratio of host space assigned to the first group (0.1 – 0.9).")]
        public float SplitRatio
        {
            get => _splitRatio;
            set
            {
                _splitRatio = Math.Max(0.1f, Math.Min(0.9f, value));
                if (_groups.Count > 1) RecalculateLayout();
            }
        }

        /// <summary>Live snapshot of all active groups (read-only).</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Collections.Generic.IReadOnlyList<BeepDocumentGroup> Groups
            => _groups.AsReadOnly();

        // ─────────────────────────────────────────────────────────────────────
        // Tab-strip pass-through properties
        // These let the Properties grid and smart-tag control the inner strip
        // without requiring consumers to cast or drill into internals.
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>How tab widths are calculated: Equal, FitToContent, Compact, or Fixed.</summary>
        [DefaultValue(TabSizeMode.Equal)]
        [Description("How tab widths are calculated: Equal, FitToContent, Compact, or Fixed.")]
        public TabSizeMode TabSizeMode
        {
            get => _tabStrip.TabSizeMode;
            set { _tabStrip.TabSizeMode = value; }
        }

        /// <summary>Width of each tab in pixels when <see cref="TabSizeMode"/> = Fixed.</summary>
        [DefaultValue(160)]
        [Description("Fixed width per tab in pixels (applies when TabSizeMode = Fixed).")]
        public int FixedTabWidth
        {
            get => _tabStrip.FixedTabWidth;
            set { _tabStrip.FixedTabWidth = value; }
        }

        /// <summary>Controls the tooltip style on tab hover: None, Simple, or Rich (thumbnail).</summary>
        [DefaultValue(TabTooltipMode.Simple)]
        [Description("Tooltip style on tab hover: None = off, Simple = title, Rich = thumbnail panel.")]
        public TabTooltipMode TabTooltipMode
        {
            get => _tabStrip.TooltipMode;
            set { _tabStrip.TooltipMode = value; }
        }

        /// <summary>Allow the user to drag a tab out of the strip to float it in a new window.</summary>
        [DefaultValue(true)]
        [Description("Allow the user to drag a tab out of the strip to open it in a floating window.")]
        public bool AllowDragFloat
        {
            get => _tabStrip.AllowDragFloat;
            set { _tabStrip.AllowDragFloat = value; }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Design-Time Documents collection
        // Seeded at design time; applied to the host in OnHandleCreated.
        // Serialised into InitializeComponent by the designer.
        // ─────────────────────────────────────────────────────────────────────

        private readonly System.Collections.ObjectModel.Collection<DocumentDescriptor>
            _designTimeDocuments = new System.Collections.ObjectModel.Collection<DocumentDescriptor>();

        /// <summary>
        /// Documents to open automatically when the host is first created.
        /// Configure at design time in the Properties grid or via the
        /// "Edit Design-Time Documents…" smart-tag action.
        /// At runtime, if <see cref="AutoSaveLayout"/> is true and a session
        /// file exists, the restored layout takes precedence over this collection.
        /// </summary>
        [Description("Documents opened when the host is first created. Configure at design time.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public System.Collections.ObjectModel.Collection<DocumentDescriptor> DesignTimeDocuments
            => _designTimeDocuments;

        /// <summary>
        /// Applies any entries in <see cref="DesignTimeDocuments"/> that are not already open.
        /// Called automatically from <c>OnHandleCreated</c>; safe to call again manually.
        /// </summary>
        public void ApplyDesignTimeDocuments()
        {
            foreach (var desc in _designTimeDocuments)
            {
                if (string.IsNullOrEmpty(desc.Id) || _panels.ContainsKey(desc.Id)) continue;

                var panel = AddDocument(desc.Id, desc.Title, desc.IconPath, activate: false);

                if (desc.IsPinned)    PinDocument(desc.Id, true);
                if (desc.IsModified)  panel.IsModified = true;
                if (desc.CanClose == false) panel.CanClose = false;

                // Seed placeholder content based on InitialContent
                switch (desc.InitialContent)
                {
                    case DocumentInitialContent.Label:
                        var lbl = new System.Windows.Forms.Label
                        {
                            Text      = desc.Title,
                            Dock      = System.Windows.Forms.DockStyle.Fill,
                            TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                            Font      = new System.Drawing.Font("Segoe UI", 11f)
                        };
                        panel.Controls.Add(lbl);
                        break;
                    case DocumentInitialContent.RichTextBox:
                        panel.Controls.Add(new System.Windows.Forms.RichTextBox
                            { Dock = System.Windows.Forms.DockStyle.Fill, BorderStyle = System.Windows.Forms.BorderStyle.None });
                        break;
                }
            }

            // Activate the first document if nothing is active yet
            if (_activeDocumentId == null && _panels.Count > 0)
                SetActiveDocument(_panels.Keys.First());
        }
    }
}

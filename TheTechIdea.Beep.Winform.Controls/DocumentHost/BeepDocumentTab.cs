using System;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    // ─────────────────────────────────────────────────────────
    // Enumerations
    // ─────────────────────────────────────────────────────────

    /// <summary>How much information is shown in the tab hover tooltip.</summary>
    public enum TabTooltipMode
    {
        /// <summary>No tooltip is shown.</summary>
        None,
        /// <summary>Plain text: title + tooltip-text property only (WinForms <see cref="System.Windows.Forms.ToolTip"/> behaviour).</summary>
        Simple,
        /// <summary>Themed owner-drawn popup with title, metadata and a live panel thumbnail.</summary>
        Rich
    }

    /// <summary>Visual style of the tab strip rendering.</summary>
    public enum DocumentTabStyle
    {
        /// <summary>Rounded top-corner tabs with bottom-edge merge (Google Chrome style).</summary>
        Chrome,
        /// <summary>Flat rectangular tabs with a top accent bar (VS Code style).</summary>
        VSCode,
        /// <summary>No tab background — a sliding underline indicates the active tab.</summary>
        Underline,
        /// <summary>Pill-shaped active indicator (Material / iOS segmented style).</summary>
        Pill,
        /// <summary>Fully flat rectangle — no rounded corners, just a coloured top border.</summary>
        Flat,
        /// <summary>Fully rounded ends — a heavier pill encompassing the entire tab area.</summary>
        Rounded,
        /// <summary>Trapezoidal tabs with angled left/right edges (legacy Chrome / Opera style).</summary>
        Trapezoid,
        /// <summary>Ribbon-style tabs mimicking Microsoft Office (wide, thin top accent, bold title).</summary>
        Office,
        /// <summary>Fluent UI 2 / Windows 11 style: translucent acrylic fill, 4 px bottom accent on active tab.</summary>
        Fluent
    }

    /// <summary>Where the tab strip is rendered relative to the content area.</summary>
    public enum TabStripPosition
    {
        Top,
        Bottom,
        Hidden,
        /// <summary>Vertical tab strip on the left edge of the host.</summary>
        Left,
        /// <summary>Vertical tab strip on the right edge of the host.</summary>
        Right
    }

    /// <summary>When a tab's close button is visible.</summary>
    public enum TabCloseMode
    {
        Always,
        OnHover,
        /// <summary>Shows the close button only on the currently active tab.</summary>
        ActiveOnly,
        Never
    }

    /// <summary>How unpinned tabs are sized within the available strip width.</summary>
    public enum TabSizeMode
    {
        /// <summary>All tabs share equal width within the available space (default).</summary>
        Equal,
        /// <summary>Each tab's width fits its content (icon + title + close + padding); scroll when overflow.</summary>
        FitToContent,
        /// <summary>All tabs use a compact width with ellipsis; good for many open files.</summary>
        Compact,
        /// <summary>All tabs use the pixel width set in <see cref="BeepDocumentTabStrip.FixedTabWidth"/>.</summary>
        Fixed
    }

    /// <summary>How a per-document colour is applied to the tab background (feature 2.2).</summary>
    public enum TabColorMode
    {
        /// <summary>No colour overlay — default appearance (default).</summary>
        None,
        /// <summary>A 3-pixel accent bar painted at the top edge of the tab.</summary>
        AccentBar,
        /// <summary>A semi-transparent tint fills the entire tab background.</summary>
        FullBackground,
        /// <summary>A 3-pixel accent bar painted at the bottom edge of the tab.</summary>
        BottomBorder
    }

    /// <summary>Tab strip row wrapping mode (feature 7.5).</summary>
    public enum TabRowMode
    {
        /// <summary>All tabs in a single scrollable row (default).</summary>
        SingleRow,
        /// <summary>Tabs wrap onto multiple rows; no scroll overflow.</summary>
        MultiRow
    }

    /// <summary>Which side of a <see cref="BeepDocumentHost"/> an auto-hidden document slides in from (feature 3.3).</summary>
    public enum AutoHideSide { Left, Right, Top, Bottom }

    // ── Sprint 18 ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Vertical density of the tab strip — controls row height and font size (Sprint 18.3).
    /// </summary>
    public enum TabDensityMode
    {
        /// <summary>Default — 36 px row height, 12 pt font.</summary>
        Comfortable,
        /// <summary>Tighter — 28 px row height, 11 pt font.</summary>
        Compact,
        /// <summary>Minimal — 22 px row height, 10 pt font.</summary>
        Dense
    }

    /// <summary>
    /// Computed display mode of the tab strip based on available width (Sprint 18.2).
    /// </summary>
    internal enum TabResponsiveMode
    {
        /// <summary>Full display: title + icon + close button.</summary>
        Normal,
        /// <summary>240–480 px: title only; close on hover.</summary>
        Compact,
        /// <summary>120–240 px: icon-only per tab; overflow button for extras.</summary>
        IconOnly,
        /// <summary>Below 120 px: only the active tab; all others in overflow.</summary>
        ActiveOnly
    }

    /// <summary>
    /// Configurable breakpoint thresholds (in logical pixels) that drive
    /// <see cref="TabResponsiveMode"/> switching (Sprint 18.2).
    /// </summary>
    public sealed class ResponsiveBreakpoints
    {
        /// <summary>Above this width the strip is in <c>Normal</c> mode.  Default 480.</summary>
        public int Normal   { get; set; } = 480;
        /// <summary>Above this width the strip is in <c>Compact</c> mode.  Default 240.</summary>
        public int Compact  { get; set; } = 240;
        /// <summary>Above this width the strip is in <c>IconOnly</c> mode.  Default 120.</summary>
        public int IconOnly { get; set; } = 120;
        // Below IconOnly → ActiveOnly
    }

    // ─────────────────────────────────────────────────────────
    // TabGroup — named group with collapsible tab section (feature 7.3)
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// Describes a named group of tabs in <see cref="BeepDocumentTabStrip"/>.
    /// Tabs whose <see cref="BeepDocumentTab.Group"/> matches <see cref="Id"/>
    /// are rendered together under a visual separator header.
    /// </summary>
    public sealed class TabGroup
    {
        /// <summary>Unique key that tabs reference via <see cref="BeepDocumentTab.Group"/>.</summary>
        public string Id { get; set; } = System.Guid.NewGuid().ToString();

        /// <summary>Human-readable label rendered above the group.</summary>
        public string GroupName { get; set; } = "Group";

        /// <summary>Accent colour for the group separator line.  Empty = theme PrimaryColor.</summary>
        public System.Drawing.Color GroupColor { get; set; } = System.Drawing.Color.Empty;

        /// <summary>When true, the group's tabs are hidden (the header remains visible).</summary>
        public bool IsCollapsed { get; set; }

        /// <summary>Bounding rectangle of the group header (updated by CalculateTabLayout).</summary>
        internal System.Drawing.Rectangle HeaderRect { get; set; }

        public override string ToString() => $"[Group] {GroupName} (Id={Id})";
    }

    // ─────────────────────────────────────────────────────────
    // Event argument types
    // ─────────────────────────────────────────────────────────

    /// <summary>Arguments for any event that identifies a single tab by index.</summary>
    public class TabEventArgs : EventArgs
    {
        /// <summary>Zero-based index of the tab in the strip.</summary>
        public int TabIndex { get; }

        /// <summary>The tab data model that raised the event.</summary>
        public BeepDocumentTab Tab { get; }

        public TabEventArgs(int tabIndex, BeepDocumentTab tab)
        {
            TabIndex = tabIndex;
            Tab = tab ?? throw new ArgumentNullException(nameof(tab));
        }
    }

    /// <summary>Arguments raised when a tab is dragged to a new position.</summary>
    public class TabReorderArgs : EventArgs
    {
        public int OldIndex { get; }
        public int NewIndex { get; }
        public BeepDocumentTab Tab { get; }

        public TabReorderArgs(int oldIndex, int newIndex, BeepDocumentTab tab)
        {
            OldIndex = oldIndex;
            NewIndex = newIndex;
            Tab = tab ?? throw new ArgumentNullException(nameof(tab));
        }
    }

    /// <summary>Arguments raised when an entire document (panel + tab) changes state.</summary>
    public class DocumentEventArgs : EventArgs
    {
        /// <summary>Unique string identifier of the document.</summary>
        public string DocumentId { get; }

        /// <summary>Human-readable title of the document.</summary>
        public string Title { get; }

        public DocumentEventArgs(string documentId, string title)
        {
            DocumentId = documentId ?? throw new ArgumentNullException(nameof(documentId));
            Title = title ?? string.Empty;
        }
    }

    /// <summary>
    /// Cancellable event arguments raised just before a tab is closed,
    /// allowing the host or consumer to prevent the close.
    /// </summary>
    public class TabClosingEventArgs : EventArgs
    {
        /// <summary>Zero-based index of the tab about to be closed.</summary>
        public int TabIndex { get; }

        /// <summary>The tab data model that is about to close.</summary>
        public BeepDocumentTab Tab { get; }

        /// <summary>Set to <c>true</c> to cancel the close operation.</summary>
        public bool Cancel { get; set; }

        public TabClosingEventArgs(int tabIndex, BeepDocumentTab tab)
        {
            TabIndex = tabIndex;
            Tab      = tab ?? throw new ArgumentNullException(nameof(tab));
        }
    }

    /// <summary>
    /// Event arguments raised when a context menu is about to be displayed for a tab.
    /// Set <see cref="Cancel"/> to true to suppress the default menu.
    /// </summary>
    public class TabContextMenuEventArgs : EventArgs
    {
        /// <summary>Zero-based index of the right-clicked tab, or -1 if the empty strip area was clicked.</summary>
        public int TabIndex { get; }

        /// <summary>The right-clicked tab, or null when the empty area was clicked.</summary>
        public BeepDocumentTab? Tab { get; }

        /// <summary>The pre-built context menu; add, remove, or modify items before display.</summary>
        public System.Windows.Forms.ContextMenuStrip Menu { get; }

        /// <summary>Set to <c>true</c> to prevent the menu from being shown.</summary>
        public bool Cancel { get; set; }

        public TabContextMenuEventArgs(int tabIndex, BeepDocumentTab? tab,
                                       System.Windows.Forms.ContextMenuStrip menu)
        {
            TabIndex = tabIndex;
            Tab      = tab;
            Menu     = menu ?? throw new ArgumentNullException(nameof(menu));
        }
    }

    // ─────────────────────────────────────────────────────────
    // TabMoveGroupEventArgs — move tab to a tab group
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// Arguments raised when the user requests a tab be moved to a different <see cref="TabGroup"/>.
    /// </summary>
    public class TabMoveGroupEventArgs : EventArgs
    {
        /// <summary>Zero-based index of the tab being moved.</summary>
        public int TabIndex { get; }

        /// <summary>The tab being moved.</summary>
        public BeepDocumentTab Tab { get; }

        /// <summary>Id of the target <see cref="TabGroup"/>, or null to remove the tab from all groups.</summary>
        public string? TargetGroupId { get; }

        public TabMoveGroupEventArgs(int tabIndex, BeepDocumentTab tab, string? targetGroupId)
        {
            TabIndex      = tabIndex;
            Tab           = tab ?? throw new ArgumentNullException(nameof(tab));
            TargetGroupId = targetGroupId;
        }
    }

    // ─────────────────────────────────────────────────────────
    // DocumentLayoutEventArgs — serialisation / restore notifications
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// Arguments raised during layout serialisation (saving) or layout restoration
    /// for a single document entry.  Set <see cref="Cancel"/> to true in the
    /// <c>LayoutRestoring</c> event to skip re-opening a specific document.
    /// </summary>
    public class DocumentLayoutEventArgs : EventArgs
    {
        /// <summary>The document id from the saved layout.</summary>
        public string  DocumentId   { get; }

        /// <summary>The document title from the saved layout.</summary>
        public string  Title        { get; }

        /// <summary>Optional icon path from the saved layout.</summary>
        public string? IconPath     { get; }

        /// <summary>True if the tab was pinned at save time.</summary>
        public bool    IsPinned     { get; }

        /// <summary>True if the tab had unsaved changes at save time.</summary>
        public bool    IsModified   { get; }

        /// <summary>
        /// Set to <c>true</c> inside <c>LayoutRestoring</c> to prevent this
        /// document from being re-created during <see cref="BeepDocumentHost.RestoreLayout"/>.
        /// </summary>
        public bool    Cancel       { get; set; }

        /// <summary>
        /// Arbitrary data the consumer can attach during <c>LayoutSerialising</c>
        /// and read back during <c>LayoutRestoring</c>.
        /// </summary>
        public object? Tag          { get; set; }

        /// <summary>
        /// Typed key/value pairs that are serialised into the v2 layout JSON
        /// <c>customData</c> object and restored verbatim.  Only string values
        /// are supported for JSON round-trip compatibility.
        /// </summary>
        public System.Collections.Generic.Dictionary<string, string> CustomData { get; }
            = new System.Collections.Generic.Dictionary<string, string>();

        public DocumentLayoutEventArgs(string documentId, string title,
                               string? iconPath, bool isPinned, bool isModified)
        {
            DocumentId = documentId ?? throw new System.ArgumentNullException(nameof(documentId));
            Title      = title      ?? string.Empty;
            IconPath   = iconPath;
            IsPinned   = isPinned;
            IsModified = isModified;
        }
    }

    // ─────────────────────────────────────────────────────────
    // BeepDocumentTab — pure data model (never a Control)
    // ─────────────────────────────────────────────────────────

    /// <summary>
    /// Immutable-ish data record that represents one tab entry inside
    /// <see cref="BeepDocumentTabStrip"/>.  This is NOT a WinForms control.
    /// </summary>
    public class BeepDocumentTab
    {
        // ── Identity ──────────────────────────────────────────────────────────

        /// <summary>Globally-unique identifier that ties this tab to its panel.</summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>Text displayed on the tab.</summary>
        public string Title { get; set; } = "Document";

        /// <summary>Optional path to an icon image (same convention as BeepButton.ImagePath).</summary>
        public string? IconPath { get; set; }

        // ── State ─────────────────────────────────────────────────────────────

        /// <summary>When true, a dirty indicator (●) is drawn next to the title.</summary>
        public bool IsModified { get; set; }

        /// <summary>When false, the close button is hidden for this tab.</summary>
        public bool CanClose { get; set; } = true;

        /// <summary>Pinned tabs cannot be reordered or closed via drag.</summary>
        public bool IsPinned { get; set; }

        /// <summary>True when this tab is the currently active/selected one.</summary>
        public bool IsActive { get; set; }

        // ── Visual overrides ──────────────────────────────────────────────────

        /// <summary>
        /// Optional accent colour shown as a small bar on the active tab.
        /// When <see cref="Color.Empty"/> the theme's PrimaryColor is used.
        /// </summary>
        public Color AccentColor { get; set; } = Color.Empty;

        /// <summary>Tooltip text shown on mouse hover.  Uses Title when empty.</summary>
        public string? TooltipText { get; set; }

        // ── ImageList icons ───────────────────────────────────────────────────

        /// <summary>
        /// Optional <see cref="System.Windows.Forms.ImageList"/> shared across tabs.
        /// When set together with <see cref="ImageIndex"/>, the icon is taken from
        /// this list instead of <see cref="IconPath"/>.
        /// </summary>
        public System.Windows.Forms.ImageList? ImageList { get; set; }

        /// <summary>
        /// Zero-based index into <see cref="ImageList"/>.
        /// Ignored when <see cref="ImageList"/> is null or the index is out of range.
        /// </summary>
        public int ImageIndex { get; set; } = -1;

        /// <summary>Arbitrary user data associated with the tab.</summary>
        public object? Tag { get; set; }

        // ── Notification badge (7.4) ──────────────────────────────────────────

        /// <summary>
        /// Short string displayed as a coloured badge pill over the tab's top-right corner.
        /// Typical values: "3", "!", "●", or null/empty to hide the badge.
        /// </summary>
        public string? BadgeText { get; set; }

        /// <summary>
        /// Background colour of the badge pill.
        /// When <see cref="Color.Empty"/> the theme's <c>ErrorColor</c> is used.
        /// </summary>
        public Color BadgeColor { get; set; } = Color.Empty;

        // ── Tab colour and category (2.2) ──────────────────────────────────────

        /// <summary>
        /// Per-document background tint colour rendered according to the host's
        /// <see cref="BeepDocumentHost.TabColorMode"/> property.
        /// When <see cref="Color.Empty"/> no overlay is drawn.
        /// </summary>
        public Color TabColor { get; set; } = Color.Empty;

        /// <summary>Optional grouping category (shown as a separator in the overflow dropdown).</summary>
        public string? DocumentCategory { get; set; }

        // ── Tab group (7.3) ───────────────────────────────────────────────────

        /// <summary>
        /// Id matching a <see cref="TabGroup.Id"/> in the strip's <c>TabGroups</c> collection.
        /// Null/empty = no group (default section).
        /// </summary>
        public string? Group { get; set; }

        // ── Multi-row layout cache (7.5) ──────────────────────────────────────

        /// <summary>Zero-based row index assigned by the multi-row layout pass (0 in single-row mode).</summary>
        internal int RowIndex { get; set; }

        // ── Layout cache (updated by BeepDocumentTabStrip.CalculateTabLayout) ───

        /// <summary>Bounding rectangle of the entire tab including padding.</summary>
        public Rectangle TabRect { get; internal set; }

        /// <summary>Bounding rectangle of the close (×) button.</summary>
        public Rectangle CloseRect { get; internal set; }

        /// <summary>Bounding rectangle of the dirty indicator dot.</summary>
        public Rectangle DirtyRect { get; internal set; }

        /// <summary>Bounding rectangle reserved for the icon image.</summary>
        public Rectangle IconRect { get; internal set; }

        /// <summary>Bounding rectangle available for the title text.</summary>
        public Rectangle TitleRect { get; internal set; }

        // ── Constructor ───────────────────────────────────────────────────────

        /// <summary>Creates a new tab with the given title.</summary>
        public BeepDocumentTab(string title)
        {
            Title = title ?? "Document";
        }

        /// <summary>Creates a new tab with the given title and id.</summary>
        public BeepDocumentTab(string id, string title) : this(title)
        {
            Id = id ?? Guid.NewGuid().ToString();
        }

        /// <inheritdoc/>
        public override string ToString() => $"[Tab] {Title} (Id={Id})";
    }

    // ─────────────────────────────────────────────────────
    // ClosedTabRecord — snapshot for the "Reopen Closed Tab" feature
    // ─────────────────────────────────────────────────────

    /// <summary>
    /// Lightweight snapshot of a closed document kept in the
    /// <see cref="BeepDocumentHost"/> closed-tab history stack.
    /// </summary>
    public sealed class ClosedTabRecord
    {
        /// <summary>The original document id.</summary>
        public string  DocumentId  { get; }

        /// <summary>The document title at the time of closing.</summary>
        public string  Title       { get; }

        /// <summary>Path to the icon image, or null.</summary>
        public string? IconPath    { get; }

        /// <summary>
        /// Opaque state blob that consumer code can populate inside the
        /// <see cref="BeepDocumentHost.DocumentClosing"/> event handler.
        /// Returned via <see cref="BeepDocumentHost.ReopenDocumentRequested"/>.
        /// </summary>
        public object? RestoreData { get; set; }

        public ClosedTabRecord(string documentId, string title, string? iconPath)
        {
            DocumentId = documentId ?? throw new ArgumentNullException(nameof(documentId));
            Title      = title      ?? string.Empty;
            IconPath   = iconPath;
        }
    }
}

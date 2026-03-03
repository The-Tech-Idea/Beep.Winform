// DocumentDescriptor.cs
// Data-Transfer Object used for the Data-Binding API of BeepDocumentHost.
// A BindingList<DocumentDescriptor> drives the open documents collection
// so that consumer code never needs to call AddDocument/CloseDocument directly.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// Lightweight, bindable descriptor for one document in a
    /// <see cref="BeepDocumentHost"/>.  Implements <see cref="INotifyPropertyChanged"/>
    /// so tab titles, icons and dirty state update automatically.
    /// </summary>
    public sealed class DocumentDescriptor : INotifyPropertyChanged
    {
        // ── Identity ─────────────────────────────────────────────────────────

        private string _id = Guid.NewGuid().ToString();
        /// <summary>Globally-unique identifier.  Changing it after binding is unsupported.</summary>
        public string Id
        {
            get => _id;
            set { _id = value ?? Guid.NewGuid().ToString(); OnPropertyChanged(nameof(Id)); }
        }

        private string _title = "Document";
        /// <summary>Text shown in the tab header.</summary>
        public string Title
        {
            get => _title;
            set { _title = value ?? "Document"; OnPropertyChanged(nameof(Title)); }
        }

        private string? _iconPath;
        /// <summary>Path to an icon image (same convention as BeepButton.ImagePath); null = no icon.</summary>
        public string? IconPath
        {
            get => _iconPath;
            set { _iconPath = value; OnPropertyChanged(nameof(IconPath)); }
        }

        // ── State ─────────────────────────────────────────────────────────────

        private bool _isModified;
        /// <summary>When true, the dirty indicator (●) is shown on the tab.</summary>
        public bool IsModified
        {
            get => _isModified;
            set { _isModified = value; OnPropertyChanged(nameof(IsModified)); }
        }

        private bool _isPinned;
        /// <summary>When true, the tab is pinned (icon-only, left-anchored).</summary>
        public bool IsPinned
        {
            get => _isPinned;
            set { _isPinned = value; OnPropertyChanged(nameof(IsPinned)); }
        }

        private bool _canClose = true;
        /// <summary>When false, the close button is hidden.</summary>
        public bool CanClose
        {
            get => _canClose;
            set { _canClose = value; OnPropertyChanged(nameof(CanClose)); }
        }

        private string? _category;
        /// <summary>
        /// Optional grouping category shown in the overflow dropdown header.
        /// When null or empty, the document belongs to the default (uncategorised) group.
        /// </summary>
        public string? Category
        {
            get => _category;
            set { _category = value; OnPropertyChanged(nameof(Category)); }
        }

        private string? _tooltipText;
        /// <summary>Tooltip text shown on mouse hover; falls back to <see cref="Title"/> when null.</summary>
        public string? TooltipText
        {
            get => _tooltipText;
            set { _tooltipText = value; OnPropertyChanged(nameof(TooltipText)); }
        }

        private object? _tag;
        /// <summary>Arbitrary user data; not interpreted by the host.</summary>
        public object? Tag
        {
            get => _tag;
            set { _tag = value; OnPropertyChanged(nameof(Tag)); }
        }

        // ── Notification badge ────────────────────────────────────────────────

        private string? _badgeText;
        /// <summary>
        /// Short string shown as a badge pill on the tab (e.g. "3", "!").
        /// Set to null or empty to hide the badge.
        /// </summary>
        public string? BadgeText
        {
            get => _badgeText;
            set { _badgeText = value; OnPropertyChanged(nameof(BadgeText)); }
        }

        private System.Drawing.Color _badgeColor = System.Drawing.Color.Empty;
        /// <summary>Background colour of the badge pill.  Empty = use theme ErrorColor.</summary>
        public System.Drawing.Color BadgeColor
        {
            get => _badgeColor;
            set { _badgeColor = value; OnPropertyChanged(nameof(BadgeColor)); }
        }
        // ── Tab colour (2.2) ─────────────────────────────────────────────────────

        private System.Drawing.Color _tabColor = System.Drawing.Color.Empty;
        /// <summary>Per-document tab tint colour; applied according to <see cref="BeepDocumentHost.TabColorMode"/>.</summary>
        public System.Drawing.Color TabColor
        {
            get => _tabColor;
            set { _tabColor = value; OnPropertyChanged(nameof(TabColor)); }
        }

        // ── Phase 2 additions ─────────────────────────────────────────────────

        private System.Drawing.Color _accentColor = System.Drawing.Color.Empty;
        /// <summary>
        /// Per-document accent colour for the active-indicator bar (e.g. git branch colour).
        /// When <see cref="System.Drawing.Color.Empty"/>, the theme <c>PrimaryColor</c> is used.
        /// </summary>
        public System.Drawing.Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; OnPropertyChanged(nameof(AccentColor)); }
        }

        /// <summary>
        /// Optional design-time placeholder content type.  Only used when
        /// the document is created from a <c>DesignTimeDocuments</c> seed collection.
        /// </summary>
        public DocumentInitialContent InitialContent { get; set; } = DocumentInitialContent.Empty;

        /// <summary>
        /// Typed key/value metadata attached to this descriptor.  Serialised into
        /// the v2 layout JSON <c>customData</c> object; only string values survive
        /// the JSON round-trip.
        /// </summary>
        public System.Collections.Generic.Dictionary<string, string> CustomData { get; }
            = new System.Collections.Generic.Dictionary<string, string>();

        // ── INotifyPropertyChanged ────────────────────────────────────────────

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        // ── Convenience factory ───────────────────────────────────────────────

        /// <summary>Creates a descriptor with an explicit id and title.</summary>
        public static DocumentDescriptor Create(string id, string title,
                                                string? iconPath = null)
            => new() { Id = id, Title = title, IconPath = iconPath };

        /// <inheritdoc/>
        public override string ToString() => $"[Doc] {Title} (Id={Id})";
    }

    /// <summary>
    /// Specifies the placeholder content type to render inside a
    /// <see cref="BeepDocumentPanel"/> when a design-time document seed
    /// (<c>DesignTimeDocuments</c>) is instantiated.
    /// </summary>
    public enum DocumentInitialContent
    {
        /// <summary>No placeholder — panel is empty.</summary>
        Empty,
        /// <summary>A centered <see cref="System.Windows.Forms.Label"/> showing the document title.</summary>
        Label,
        /// <summary>A <see cref="System.Windows.Forms.RichTextBox"/> filling the panel.</summary>
        RichTextBox,
        /// <summary>A WebView2 component filling the panel (requires WebView2 runtime).</summary>
        WebView
    }
}

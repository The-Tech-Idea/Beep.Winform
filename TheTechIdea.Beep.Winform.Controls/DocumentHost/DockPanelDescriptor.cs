// DockPanelDescriptor.cs
// Data model for a named tool-window panel managed by BeepDockManager.
// A DockPanelDescriptor describes one tool panel (Solution Explorer, Properties,
// Output, etc.) that lives on an edge of a BeepDocumentHost rather than in the
// central document tab area.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    // ─────────────────────────────────────────────────────────────────────────
    // Enumerations
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Which edge of a <see cref="BeepDocumentHost"/> a dock panel is attached to,
    /// or <see cref="Floating"/> if it has been torn off into its own window.
    /// </summary>
    public enum DockEdge
    {
        Left,
        Right,
        Top,
        Bottom,
        Floating
    }

    // ─────────────────────────────────────────────────────────────────────────
    // DockPanelDescriptor
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Describes one tool-window panel owned by a <see cref="BeepDockManager"/>.
    /// Implements <see cref="INotifyPropertyChanged"/> so the manager can react to
    /// property changes (e.g. user hides panel at runtime).
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class DockPanelDescriptor : INotifyPropertyChanged
    {
        // ── Identity ─────────────────────────────────────────────────────────

        private string _id = Guid.NewGuid().ToString();

        /// <summary>Globally-unique identifier. Changing it after binding is unsupported.</summary>
        [Category("Identity")]
        [Description("Globally-unique identifier for this dock panel.")]
        public string Id
        {
            get => _id;
            set { _id = value ?? Guid.NewGuid().ToString(); OnChanged(); }
        }

        /// <summary>
        /// Stable key used as the primary key when saving/restoring layout JSON.
        /// Set once on creation; never changes after that.
        /// </summary>
        [Category("Identity")]
        [Description("Stable persistence key used in layout JSON. Do not change after creation.")]
        public string PersistenceKey { get; set; } = Guid.NewGuid().ToString();

        // ── Appearance ───────────────────────────────────────────────────────

        private string _title = "Panel";

        /// <summary>Display name shown in the panel header and the auto-hide strip button.</summary>
        [Category("Appearance")]
        [Description("Display title shown in the panel header and the auto-hide strip button.")]
        [DefaultValue("Panel")]
        public string Title
        {
            get => _title;
            set { _title = string.IsNullOrWhiteSpace(value) ? "Panel" : value; OnChanged(); }
        }

        private string? _iconPath;

        /// <summary>
        /// Path to a 16 px icon image (same convention as <see cref="BeepDocumentTab.IconPath"/>).
        /// <see langword="null"/> = no icon.
        /// </summary>
        [Category("Appearance")]
        [Description("Path to a 16 px icon image. Null = no icon.")]
        [DefaultValue(null)]
        public string? IconPath
        {
            get => _iconPath;
            set { _iconPath = value; OnChanged(); }
        }

        // ── Layout ───────────────────────────────────────────────────────────

        private DockEdge _edge = DockEdge.Left;

        /// <summary>Which edge the panel is docked to, or <see cref="DockEdge.Floating"/>.</summary>
        [Category("Layout")]
        [Description("Which edge the panel is docked to, or Floating.")]
        [DefaultValue(DockEdge.Left)]
        public DockEdge Edge
        {
            get => _edge;
            set { _edge = value; OnChanged(); }
        }

        private double _sizePercent = 0.22;

        /// <summary>
        /// Fraction of the host's relevant dimension (0.05–0.95) consumed by this panel
        /// when it is pinned. For Left/Right edges this is a width fraction; for
        /// Top/Bottom it is a height fraction.
        /// </summary>
        [Category("Layout")]
        [Description("Fraction (0.05 – 0.95) of the host dimension occupied by this panel when pinned.")]
        [DefaultValue(0.22)]
        public double SizePercent
        {
            get => _sizePercent;
            set
            {
                _sizePercent = Math.Min(0.95, Math.Max(0.05, value));
                OnChanged();
            }
        }

        // ── State ─────────────────────────────────────────────────────────────

        private bool _isAutoHidden;

        /// <summary>
        /// When <see langword="true"/> the panel is collapsed into the edge strip
        /// (slides out on hover/click).  When <see langword="false"/> and
        /// <see cref="IsVisible"/> is <see langword="true"/>, the panel occupies its
        /// edge permanently (DevExpress "pinned" state).
        /// </summary>
        [Category("State")]
        [Description("True = auto-hidden (shows on hover); False = pinned (always visible).")]
        [DefaultValue(false)]
        public bool IsAutoHidden
        {
            get => _isAutoHidden;
            set { _isAutoHidden = value; OnChanged(); }
        }

        private bool _isVisible = true;

        /// <summary>When <see langword="false"/> the panel is completely hidden from the UI.</summary>
        [Category("State")]
        [Description("When false the panel is completely hidden from the UI.")]
        [DefaultValue(true)]
        public bool IsVisible
        {
            get => _isVisible;
            set { _isVisible = value; OnChanged(); }
        }

        // ── Content ───────────────────────────────────────────────────────────

        /// <summary>
        /// The <see cref="Control"/> hosted inside this panel.  Set at runtime or in
        /// <see cref="ISupportInitialize.EndInit"/> of the owning form.  Not persisted
        /// in the designer because controls have their own designer identity.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Control? Content { get; set; }

        // ── INotifyPropertyChanged ────────────────────────────────────────────

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnChanged([System.Runtime.CompilerServices.CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public override string ToString() => $"[DockPanel] {Title} ({Edge}, {(IsAutoHidden ? "auto-hide" : "pinned")})";
    }
}

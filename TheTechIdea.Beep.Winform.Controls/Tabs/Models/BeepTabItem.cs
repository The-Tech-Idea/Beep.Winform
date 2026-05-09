using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Models
{
    /// <summary>
    /// Canonical tab item model used by the custom host pipeline.
    /// </summary>
    public sealed class BeepTabItem
    {
        // ── Identity ─────────────────────────────────────────────────────────
        public int Index { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public Control? Content { get; set; }

        // ── Interaction state ────────────────────────────────────────────────
        public bool IsSelected { get; set; }
        public bool IsHovered { get; set; }
        public bool IsPressed { get; set; }
        public bool IsCloseButtonHovered { get; set; }
        public bool IsCloseButtonPressed { get; set; }
        public bool IsFocused { get; set; }
        public bool IsDragging { get; set; }
        public bool IsEnabled { get; set; } = true;
        public bool IsVisible { get; set; } = true;
        public bool CanClose { get; set; } = true;
        public bool CanSelect { get; set; } = true;
        public bool CanReorder { get; set; } = true;
        public BeepTabWorkspaceState WorkspaceState { get; set; } = new BeepTabWorkspaceState();

        // ── Layout ───────────────────────────────────────────────────────────
        public Rectangle Bounds { get; set; } = Rectangle.Empty;

        // ── Phase 2: rich metadata ────────────────────────────────────────────
        /// <summary>Path or embedded resource key for the tab icon.</summary>
        public string IconPath { get; set; } = string.Empty;

        /// <summary>Optional secondary label shown below or alongside the title.</summary>
        public string SubText { get; set; } = string.Empty;

        /// <summary>Text shown inside the badge glyph (e.g. "3" for a count badge).</summary>
        public string BadgeText { get; set; } = string.Empty;

        /// <summary>Visual kind of the badge. <see cref="BeepTabBadgeKind.None"/> disables the badge.</summary>
        public BeepTabBadgeKind BadgeKind { get; set; } = BeepTabBadgeKind.None;

        /// <summary>True when the tab content has unsaved changes (enables the dirty-dot marker).</summary>
        public bool IsDirty
        {
            get => WorkspaceState.IsDirty;
            set => WorkspaceState.IsDirty = value;
        }

        /// <summary>True while an async operation tied to this tab is in progress.</summary>
        public bool IsBusy { get; set; }

        /// <summary>True when the tab is pinned ahead of the normal workspace run.</summary>
        public bool IsPinned
        {
            get => WorkspaceState.IsPinned;
            set => WorkspaceState.IsPinned = value;
        }

        /// <summary>True when the tab participates in preview-slot behavior.</summary>
        public bool IsPreview
        {
            get => WorkspaceState.IsPreview;
            set => WorkspaceState.IsPreview = value;
        }

        /// <summary>Workspace/document grouping hint for future host integrations.</summary>
        public string GroupKey
        {
            get => WorkspaceState.GroupKey;
            set => WorkspaceState.GroupKey = value ?? string.Empty;
        }

        /// <summary>
        /// Overrides the host-level close-button visibility for this specific tab.
        /// Null means inherit the host setting.
        /// </summary>
        public bool? CloseVisible { get; set; }

        // ── Helpers ──────────────────────────────────────────────────────────

        /// <summary>Whether this item has a non-empty icon path.</summary>
        public bool HasIcon => !string.IsNullOrWhiteSpace(IconPath);

        /// <summary>Whether a visible badge should be rendered.</summary>
        public bool HasBadge => BadgeKind != BeepTabBadgeKind.None;

        /// <summary>Whether a secondary-text line should be rendered.</summary>
        public bool HasSubText => !string.IsNullOrWhiteSpace(SubText);

        /// <summary>Snapshot of this item's adornment inputs for use by layout helpers.</summary>
        public BeepTabAdornmentState GetAdornmentState() =>
            new BeepTabAdornmentState
            {
                BadgeText = BadgeText,
                BadgeKind = BadgeKind,
                IsDirty = IsDirty,
                IsBusy = IsBusy,
                HasIcon = HasIcon,
                HasSubText = HasSubText
            };

        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(Title) ? Name : Title;
        }
    }
}
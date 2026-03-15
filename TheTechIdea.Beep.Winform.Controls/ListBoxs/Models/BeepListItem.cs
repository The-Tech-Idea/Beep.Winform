using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Models
{
    /// <summary>
    /// Rich list item model extending SimpleItem with 2026 commercial-standard fields.
    /// Supports sub-text, badges, grouping, pinning, disabling, separators, accent colours.
    /// Backward-compatible with SimpleItem: cast BeepListItem to SimpleItem freely.
    /// </summary>
    public class BeepListItem : SimpleItem
    {
        // ── Secondary content ────────────────────────────────────────────────────

        /// <summary>Secondary/metadata line shown below the main title (nullable).</summary>
        public string? SubText { get; set; }

        // ── Badge / notification chip ─────────────────────────────────────────────

        /// <summary>Short badge label, e.g. "3", "New", "●". Empty = no badge.</summary>
        public string? BadgeText { get; set; }

        /// <summary>Background colour for the badge pill. Color.Empty uses theme PrimaryColor.</summary>
        public Color BadgeColor { get; set; } = Color.Empty;

        // ── Grouping ──────────────────────────────────────────────────────────────

        /// <summary>Group / category key. Items sharing this value appear under the same header.</summary>
        public string? Category { get; set; }

        /// <summary>Marks this item as a synthetic group header row.</summary>
        public bool IsGroupHeader { get; set; }

        /// <summary>Visible child item count for a group header row.</summary>
        public int GroupItemCount { get; set; }

        // ── Pinning ───────────────────────────────────────────────────────────────

        /// <summary>Pinned items are promoted to the top section and never scroll away.</summary>
        public bool IsPinned { get; set; }

        // ── State flags ───────────────────────────────────────────────────────────

        /// <summary>Disabled items are rendered at reduced opacity and ignore mouse / keyboard.</summary>
        public bool IsDisabled { get; set; }

        /// <summary>When true the item renders as a 1 px horizontal separator rule (height = 20 px).</summary>
        public bool IsSeparator { get; set; }

        // ── Visual accent ─────────────────────────────────────────────────────────

        /// <summary>Optional 3 px left-edge accent colour bar. Color.Empty = none.</summary>
        public Color ItemAccentColor { get; set; } = Color.Empty;

        /// <summary>Optional trailing metadata text (e.g., shortcut, count, status).</summary>
        public string? TrailingMeta { get; set; }

        /// <summary>Preferred row composition preset for this item.</summary>
        public ListRowPreset RowPreset { get; set; } = ListRowPreset.Auto;

        // ── User payload ──────────────────────────────────────────────────────────

        /// <summary>Arbitrary consumer object (not serialised).</summary>
        public object? Tag { get; set; }

        // ── Constructors ──────────────────────────────────────────────────────────

        public BeepListItem() { }

        public BeepListItem(string text) { Text = text; }

        public BeepListItem(string text, string subText) { Text = text; SubText = subText; }

        public BeepListItem(string text, string subText, string? imagePath)
        {
            Text = text;
            SubText = subText;
            ImagePath = imagePath;
        }

        public BeepListItem(string text, string subText, string? imagePath, string? category)
        {
            Text = text;
            SubText = subText;
            ImagePath = imagePath;
            Category = category;
        }

        /// <summary>Separator factory.</summary>
        public static BeepListItem Separator(string? groupLabel = null)
            => new BeepListItem(groupLabel ?? string.Empty) { IsSeparator = true };
    }
}

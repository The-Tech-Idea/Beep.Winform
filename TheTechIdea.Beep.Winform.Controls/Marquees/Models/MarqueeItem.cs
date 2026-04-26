using System;
using System.ComponentModel;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Marquees.Models
{
    /// <summary>
    /// Sprint 2 — Rich data item for the marquee.
    /// Supports text, an optional image/icon, an optional badge overlay, and per-item colours.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class MarqueeItem
    {
        /// <summary>Unique identifier for hit-testing and event routing.</summary>
        public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];

        /// <summary>Primary display text.</summary>
        [Category("Content")]
        [Description("Primary label text rendered inside the item.")]
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Path or resource key for a leading icon/image.
        /// Follows the BeepImage path convention (relative path or embedded resource name).
        /// </summary>
        [Category("Content")]
        [Description("Path to the item's leading icon or image.")]
        public string ImagePath { get; set; } = string.Empty;

        /// <summary>Optional short label shown in a badge/pill overlay (e.g. "NEW", "3").</summary>
        [Category("Content")]
        [Description("Short text rendered in a badge overlay on the item.")]
        public string BadgeText { get; set; } = string.Empty;

        /// <summary>Background fill colour of the badge pill.</summary>
        [Category("Appearance")]
        public Color BadgeColor { get; set; } = Color.FromArgb(220, 53, 69);

        /// <summary>Foreground text colour. Transparent = inherit from theme.</summary>
        [Category("Appearance")]
        public Color TextColor { get; set; } = Color.Transparent;

        /// <summary>Background fill colour. Transparent = inherit from theme.</summary>
        [Category("Appearance")]
        public Color BackgroundColor { get; set; } = Color.Transparent;

        /// <summary>Arbitrary data associated with this item.</summary>
        [Browsable(false)]
        public object Tag { get; set; }

        /// <summary>When false the item is skipped during layout and rendering.</summary>
        [Category("Behaviour")]
        [DefaultValue(true)]
        public bool IsVisible { get; set; } = true;

        public override string ToString() =>
            string.IsNullOrEmpty(Text) ? $"[{Id}]" : Text;
    }
}

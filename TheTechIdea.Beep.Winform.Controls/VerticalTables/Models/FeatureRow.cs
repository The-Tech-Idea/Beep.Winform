using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls.VerticalTables.Models
{
    /// <summary>
    /// Icon type for feature/plan comparison cells.
    /// </summary>
    public enum FeatureIconType
    {
        /// <summary>Render the cell text value as-is.</summary>
        Text,
        /// <summary>Green checkmark — feature is included.</summary>
        Check,
        /// <summary>Red cross — feature is NOT included.</summary>
        Cross,
        /// <summary>Gray dash — feature is not applicable.</summary>
        Dash,
        /// <summary>Star rating (numeric value 0-5).</summary>
        Star,
        /// <summary>Progress/percentage meter (numeric value 0-100).</summary>
        Meter
    }

    /// <summary>
    /// Represents a single feature row in a comparison/pricing table.
    /// Maps to one row across all columns.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class FeatureRow
    {
        /// <summary>Feature/row display name.</summary>
        [Category("Data")]
        [Description("Feature display name shown in the row label column.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>Optional category for grouping features.</summary>
        [Category("Data")]
        [Description("Category name for grouping related features together.")]
        public string? Category { get; set; }

        /// <summary>Per-column values. Index matches the column order.</summary>
        [Browsable(false)]
        public Dictionary<int, object?> Values { get; set; } = new();

        /// <summary>How this row should be rendered in each cell.</summary>
        [Category("Appearance")]
        [Description("Icon/indicator type for this feature row.")]
        [DefaultValue(FeatureIconType.Text)]
        public FeatureIconType IconType { get; set; } = FeatureIconType.Text;

        /// <summary>Tooltip shown when hovering over the row label.</summary>
        [Category("Appearance")]
        [Description("Tooltip text shown when hovering over this feature row.")]
        public string? Tooltip { get; set; }

        /// <summary>Whether this row is a category separator (group header).</summary>
        [Category("Appearance")]
        [Description("When true, renders as a bold category group header instead of a regular row.")]
        [DefaultValue(false)]
        public bool IsCategoryHeader { get; set; } = false;

        public override string ToString() => Name;
    }

    /// <summary>
    /// Represents a pricing plan row with price, period, and CTA information.
    /// Extends FeatureRow with pricing-specific properties.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class PricingRow : FeatureRow
    {
        /// <summary>Price string (e.g., "$29", "Free", "Custom").</summary>
        [Category("Pricing")]
        [Description("Price display string, e.g. '$29/mo'.")]
        public string? Price { get; set; }

        /// <summary>Billing period (e.g., "/month", "/year", "one-time").</summary>
        [Category("Pricing")]
        [Description("Billing period label, e.g. '/month'.")]
        public string? Period { get; set; }

        /// <summary>Call-to-action button label.</summary>
        [Category("Pricing")]
        [Description("CTA button text, e.g. 'Get Started', 'Try Free'.")]
        public string? CtaLabel { get; set; }

        /// <summary>Whether this is the highlighted/recommended plan.</summary>
        [Category("Pricing")]
        [Description("Marks this column as the recommended/highlighted plan.")]
        [DefaultValue(false)]
        public bool Highlighted { get; set; } = false;
    }

    /// <summary>
    /// Determines how data is bound to the vertical table.
    /// </summary>
    public enum ComparisonMode
    {
        /// <summary>Default: each SimpleItem is a column with its Children as rows.</summary>
        SingleSource,
        /// <summary>Multiple data sources bound side-by-side for comparison.</summary>
        MultiSource
    }

}

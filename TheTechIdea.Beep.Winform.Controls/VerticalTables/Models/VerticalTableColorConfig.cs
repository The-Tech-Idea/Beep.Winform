using System;
using System.ComponentModel;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.VerticalTables.Models
{
    /// <summary>
    /// Color configuration model for vertical table control
    /// Stores all color properties for theme and customization
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class VerticalTableColorConfig
    {
        [Category("Colors")]
        [Description("Table background color")]
        public Color TableBackgroundColor { get; set; } = Color.White;

        [Category("Colors")]
        [Description("Header background color (normal)")]
        public Color HeaderBackgroundColor { get; set; } = Color.FromArgb(245, 247, 250);

        [Category("Colors")]
        [Description("Header background color (selected)")]
        public Color HeaderSelectedBackgroundColor { get; set; } = Color.FromArgb(52, 168, 83);

        [Category("Colors")]
        [Description("Header background color (featured)")]
        public Color HeaderFeaturedBackgroundColor { get; set; } = Color.FromArgb(52, 168, 83);

        [Category("Colors")]
        [Description("Cell background color (normal)")]
        public Color CellBackgroundColor { get; set; } = Color.White;

        [Category("Colors")]
        [Description("Cell background color (hovered)")]
        public Color CellHoveredBackgroundColor { get; set; } = Color.FromArgb(250, 250, 250);

        [Category("Colors")]
        [Description("Cell background color (selected)")]
        public Color CellSelectedBackgroundColor { get; set; } = Color.FromArgb(240, 248, 255);

        [Category("Colors")]
        [Description("Cell background color (alternate)")]
        public Color CellAlternateBackgroundColor { get; set; } = Color.FromArgb(248, 249, 250);

        [Category("Colors")]
        [Description("Border color (normal)")]
        public Color BorderColor { get; set; } = Color.FromArgb(220, 225, 230);

        [Category("Colors")]
        [Description("Border color (selected)")]
        public Color BorderSelectedColor { get; set; } = Color.FromArgb(52, 168, 83);

        [Category("Colors")]
        [Description("Header text color")]
        public Color HeaderTextColor { get; set; } = Color.FromArgb(33, 37, 41);

        [Category("Colors")]
        [Description("Header text color (selected)")]
        public Color HeaderSelectedTextColor { get; set; } = Color.White;

        [Category("Colors")]
        [Description("Cell text color")]
        public Color CellTextColor { get; set; } = Color.FromArgb(73, 80, 87);

        [Category("Colors")]
        [Description("Cell text color (selected)")]
        public Color CellSelectedTextColor { get; set; } = Color.FromArgb(33, 37, 41);

        [Category("Colors")]
        [Description("Shadow color")]
        public Color ShadowColor { get; set; } = Color.FromArgb(40, Color.Black);

        public override string ToString() => $"Header: {HeaderBackgroundColor}, Cell: {CellBackgroundColor}";
    }
}

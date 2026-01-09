using System;
using System.ComponentModel;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Menus.Models
{
    /// <summary>
    /// Color configuration model for menu bar control
    /// Stores all color properties for theme and customization
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class MenuColorConfig
    {
        [Category("Colors")]
        [Description("Menu bar background color")]
        public Color MenuBarBackgroundColor { get; set; } = Color.FromArgb(240, 240, 240);

        [Category("Colors")]
        [Description("Menu bar foreground/text color")]
        public Color MenuBarForegroundColor { get; set; } = Color.FromArgb(33, 33, 33);

        [Category("Colors")]
        [Description("Menu bar border color")]
        public Color MenuBarBorderColor { get; set; } = Color.FromArgb(200, 200, 200);

        [Category("Colors")]
        [Description("Menu item background color (normal)")]
        public Color MenuItemBackgroundColor { get; set; } = Color.Transparent;

        [Category("Colors")]
        [Description("Menu item background color (hovered)")]
        public Color MenuItemHoverBackgroundColor { get; set; } = Color.FromArgb(245, 245, 245);

        [Category("Colors")]
        [Description("Menu item background color (selected)")]
        public Color MenuItemSelectedBackgroundColor { get; set; } = Color.FromArgb(0, 120, 215);

        [Category("Colors")]
        [Description("Menu item foreground/text color (normal)")]
        public Color MenuItemForegroundColor { get; set; } = Color.FromArgb(33, 33, 33);

        [Category("Colors")]
        [Description("Menu item foreground/text color (hovered)")]
        public Color MenuItemHoverForegroundColor { get; set; } = Color.FromArgb(33, 33, 33);

        [Category("Colors")]
        [Description("Menu item foreground/text color (selected)")]
        public Color MenuItemSelectedForegroundColor { get; set; } = Color.White;

        [Category("Colors")]
        [Description("Menu item border color")]
        public Color MenuItemBorderColor { get; set; } = Color.Transparent;

        [Category("Colors")]
        [Description("Gradient start color (if using gradient)")]
        public Color GradientStartColor { get; set; } = Color.Empty;

        [Category("Colors")]
        [Description("Gradient end color (if using gradient)")]
        public Color GradientEndColor { get; set; } = Color.Empty;

        public override string ToString() => $"BG: {MenuBarBackgroundColor}, Text: {MenuBarForegroundColor}";
    }
}

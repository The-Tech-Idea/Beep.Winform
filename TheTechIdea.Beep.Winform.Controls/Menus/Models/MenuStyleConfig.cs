using System;
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Menus;

namespace TheTechIdea.Beep.Winform.Controls.Menus.Models
{
    /// <summary>
    /// Configuration model for menu bar style properties
    /// Defines visual properties for each control style
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class MenuStyleConfig
    {
        [Category("Style")]
        [Description("Menu bar style")]
        public MenuBarStyle MenuBarStyle { get; set; } = MenuBarStyle.Modern;

        [Category("Style")]
        [Description("Associated control style")]
        public BeepControlStyle ControlStyle { get; set; } = BeepControlStyle.Material3;

        [Category("Style")]
        [Description("Show shadow effect")]
        public bool ShowShadow { get; set; } = true;

        [Category("Layout")]
        [Description("Recommended minimum height")]
        public int RecommendedMinimumHeight { get; set; } = 40;

        [Category("Layout")]
        [Description("Recommended menu item height")]
        public int RecommendedMenuItemHeight { get; set; } = 32;

        [Category("Layout")]
        [Description("Recommended menu item spacing")]
        public int RecommendedMenuItemSpacing { get; set; } = 4;

        [Category("Layout")]
        [Description("Recommended padding")]
        public Padding RecommendedPadding { get; set; } = new Padding(8, 4, 8, 4);

        [Category("Icons")]
        [Description("Icon size ratio (as percentage of menu item height)")]
        public float IconSizeRatio { get; set; } = 0.6f;

        public override string ToString() => $"Style: {MenuBarStyle}, Item Height: {RecommendedMenuItemHeight}";
    }
}

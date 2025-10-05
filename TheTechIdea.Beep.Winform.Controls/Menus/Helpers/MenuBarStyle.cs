using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls.Menus
{
    /// <summary>
    /// Defines the visual style of the BeepMenuBar control
    /// </summary>
    public enum MenuBarStyle
    {
        [Description("Traditional horizontal menu bar with button-like items")]
        Classic = 0,

        [Description("Modern flat design with subtle hover effects")]
        Modern = 1,

        [Description("Material Design 3.0 style with elevation and rounded corners")]
        Material = 2,

        [Description("Minimalist compact design with reduced spacing")]
        Compact = 3,

        [Description("Breadcrumb-style navigation with chevron separators")]
        Breadcrumb = 4,

        [Description("Tab-style menu bar with active tab highlighting")]
        Tab = 5,

        [Description("Fluent-style with pill selection and soft reveal hover")]
        Fluent = 6,

        [Description("Bubble-style with circular/rounded item backgrounds")]
        Bubble = 7,

        [Description("Floating bar with rounded container and subtle shadow")]
        Floating = 8,

        [Description("Dropdown category menu with hierarchical navigation")]
        DropdownCategory = 9,

        [Description("Card-based menu layout with icons and descriptions")]
        CardLayout = 10,

        [Description("Grid-based icon menu with organized sections")]
        IconGrid = 11,

        [Description("Multi-row menu layout with mixed content types")]
        MultiRow = 12
    }
}
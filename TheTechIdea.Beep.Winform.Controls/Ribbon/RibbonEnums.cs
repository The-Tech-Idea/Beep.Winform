namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Defines ribbon command layout mode.
    /// </summary>
    public enum RibbonLayoutMode
    {
        Classic,
        Simplified
    }

    /// <summary>
    /// Defines ribbon density profile.
    /// </summary>
    public enum RibbonDensity
    {
        Comfortable,
        Compact,
        Touch
    }

    /// <summary>
    /// Defines command prominence and overflow behavior hints.
    /// </summary>
    public enum RibbonCommandPriority
    {
        Primary,
        Secondary,
        OverflowOnly
    }

    /// <summary>
    /// Defines ribbon search behavior.
    /// </summary>
    public enum RibbonSearchMode
    {
        Off,
        Local,
        SmartService
    }

    /// <summary>
    /// Defines runtime customization capabilities.
    /// </summary>
    [Flags]
    public enum RibbonPersonalizationOptions
    {
        None = 0,
        QuickAccess = 1,
        RibbonTabs = 2,
        RibbonGroups = 4,
        CommandOrder = 8,
        All = QuickAccess | RibbonTabs | RibbonGroups | CommandOrder
    }

    /// <summary>
    /// Defines how external ribbon models are merged into this ribbon.
    /// </summary>
    public enum RibbonMergeMode
    {
        AppendTabs,
        MergeByTabName,
        Replace
    }

    /// <summary>
    /// Defines the origin action used by runtime ribbon customization.
    /// </summary>
    public enum RibbonCustomizationAction
    {
        Apply,
        Reset,
        Cancel
    }

    /// <summary>
    /// Defines built-in ribbon visual presets.
    /// </summary>
    public enum RibbonStylePreset
    {
        OfficeLight,
        OfficeDark,
        FluentLight,
        FluentDark,
        HighContrast
    }
}

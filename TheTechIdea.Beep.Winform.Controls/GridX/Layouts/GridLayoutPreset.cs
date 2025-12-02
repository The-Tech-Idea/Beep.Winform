using System;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    /// <summary>
    /// Available layout presets for BeepGridPro
    /// </summary>
    public enum GridLayoutPreset
    {
        // Original presets (12)
        Default,
        Clean,
        Dense,
        Striped,
        Borderless,
        HeaderBold,
        MaterialHeader,
        Card,
        ComparisonTable,
        MatrixSimple,
        MatrixStriped,
        PricingTable,
        
        // Material Design 3 presets (3)
        Material3Surface,
        Material3Compact,
        Material3List,
        
        // Fluent 2 presets (2)
        Fluent2Standard,
        Fluent2Card,
        
        // Tailwind presets (2)
        TailwindProse,
        TailwindDashboard,
        
        // AG Grid presets (2)
        AGGridAlpine,
        AGGridBalham,
        
        // Ant Design presets (2)
        AntDesignStandard,
        AntDesignCompact,
        
        // DataTables preset (1)
        DataTablesStandard
    }
}

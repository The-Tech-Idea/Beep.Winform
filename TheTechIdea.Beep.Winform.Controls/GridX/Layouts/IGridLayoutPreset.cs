using System;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    /// <summary>
    /// Enhanced contract for applying a layout preset on BeepGridPro
    /// Supports automatic painter integration and advanced features
    /// </summary>
    public interface IGridLayoutPreset
    {
        // Core application method
        void Apply(BeepGridPro grid);
        
        // Metadata
        string Name { get; }
        string Description { get; }
        string Version { get; }
        LayoutCategory Category { get; }
        
        // Painter integration
        IPaintGridHeader GetHeaderPainter();
        INavigationPainter GetNavigationPainter();
        
        // Height calculations
        int CalculateHeaderHeight(BeepGridPro grid);
        int CalculateNavigatorHeight(BeepGridPro grid);
        
        // Compatibility
        bool IsCompatibleWith(BeepGridStyle gridStyle);
        bool IsCompatibleWith(IBeepTheme theme);
    }
}

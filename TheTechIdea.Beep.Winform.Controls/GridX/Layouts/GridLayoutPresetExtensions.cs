using System;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    public static class GridLayoutPresetExtensions
    {
        /// <summary>
        /// Apply a layout preset and perform common recalculation/refresh.
        /// </summary>
        public static void ApplyLayoutPreset(this BeepGridPro grid, IGridLayoutPreset preset)
        {
            if (grid == null || preset == null) return;
            preset.Apply(grid);
            grid?.Layout?.Recalculate();
            grid?.Invalidate();
        }
    }
}

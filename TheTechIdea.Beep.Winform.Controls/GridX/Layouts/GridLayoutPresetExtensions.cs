using System;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    public static class GridLayoutPresetExtensions
    {
        /// <summary>
        /// Apply a layout preset and perform common recalculation/refresh.
        /// Recalculates heights using painter-based calculations to account for font sizes.
        /// </summary>
        public static void ApplyLayoutPreset(this BeepGridPro grid, IGridLayoutPreset preset)
        {
            if (grid == null || preset == null) return;
            
            // Apply the layout preset settings
            preset.Apply(grid);
            
            // Recalculate all heights using painter-based calculations (font-aware)
            grid?.RecalculateHeightsFromPainters();
            
            // Recalculate layout and refresh
            grid?.Layout?.Recalculate();
            grid?.Invalidate();
        }
    }
}

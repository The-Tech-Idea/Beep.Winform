using System;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    /// <summary>
    /// Small contract for applying a layout preset on BeepGridPro
    /// </summary>
    public interface IGridLayoutPreset
    {
        void Apply(BeepGridPro grid);
    }
}

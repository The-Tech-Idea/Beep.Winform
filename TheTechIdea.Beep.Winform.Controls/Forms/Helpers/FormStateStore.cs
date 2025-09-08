using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Helpers
{
    /// <summary>
    /// Central container for mutable runtime state previously scattered across the form.
    /// Keeps logic-free properties to simplify testing and helper coordination.
    /// </summary>
    internal sealed class FormStateStore
    {
        public bool IsDragging { get; set; }
        public bool InMoveOrResize { get; set; }
        public Point DragStartScreen { get; set; }
        public Point DragStartForm { get; set; }
        public int SavedBorderRadius { get; set; }
        public int SavedBorderThickness { get; set; }
        public bool ShadowDirty { get; set; } = true;
        public bool RegionDirty { get; set; } = true;
        public bool LayoutDirty { get; set; } = true;
    }
}

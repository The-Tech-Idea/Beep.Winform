using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Models
{
    /// <summary>
    /// Drag-reorder feedback resolved from the current header snapshot.
    /// </summary>
    public sealed class BeepTabHeaderDragFeedback
    {
        public static BeepTabHeaderDragFeedback Empty { get; } = new BeepTabHeaderDragFeedback();

        public int TargetIndex { get; init; } = -1;
        public int InsertIndex { get; init; } = -1;
        public bool WouldReorder { get; init; }
        public PointF MarkerStart { get; init; } = PointF.Empty;
        public PointF MarkerEnd { get; init; } = PointF.Empty;

        public bool HasDropTarget => TargetIndex >= 0;
        public bool HasMarker => !MarkerStart.IsEmpty || !MarkerEnd.IsEmpty;
    }
}
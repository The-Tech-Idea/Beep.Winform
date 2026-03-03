using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Steppers.Models
{
    public sealed class StepPainterContext
    {
        public Graphics Graphics { get; init; }
        public Rectangle DrawingRect { get; init; }
        public IBeepTheme Theme { get; init; }
        public bool UseThemeColors { get; init; }
        public IReadOnlyList<StepModel> Steps { get; init; }
        public IReadOnlyList<Rectangle> StepRects { get; init; }
        public IReadOnlyList<Rectangle> ConnectorRects { get; init; }
        public IReadOnlyList<StepAnimationState> AnimationStates { get; init; }
        public int SelectedIndex { get; init; }
        public int HoveredIndex { get; init; } = -1;
        public int PressedIndex { get; init; } = -1;
        public int FocusedIndex { get; init; } = -1;
        public Orientation Orientation { get; init; }
        public StepperStyleConfig StyleConfig { get; init; }
        public Font StepFont { get; init; }
        public Font LabelFont { get; init; }
        public Font NumberFont { get; init; }
    }
}

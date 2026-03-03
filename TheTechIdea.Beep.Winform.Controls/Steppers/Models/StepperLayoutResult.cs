using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Steppers.Models
{
    public sealed class StepperLayoutResult
    {
        public List<Rectangle> StepRects { get; } = new();
        public List<Rectangle> ConnectorRects { get; } = new();
        public Rectangle ContentRect { get; set; }
    }
}

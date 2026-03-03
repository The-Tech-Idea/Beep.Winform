using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Steppers.Models
{
    public sealed class StepAnimationState
    {
        public float HoverProgress { get; set; }
        public float PressProgress { get; set; }
        public float NodeScale { get; set; } = 1f;
        public float ConnectorFillProgress { get; set; }
        public bool RippleActive { get; set; }
        public Point RippleCenter { get; set; }
        public float RippleRadius { get; set; }
        public int RippleAlpha { get; set; }
    }
}

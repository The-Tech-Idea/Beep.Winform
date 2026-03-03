using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.Steppers.Models
{
    public sealed class StepModel
    {
        public string Text { get; set; }
        public string Subtitle { get; set; }
        public string Tooltip { get; set; }
        public string ImagePath { get; set; }
        public StepState State { get; set; } = StepState.Pending;
        public bool IsEnabled { get; set; } = true;
        public object Tag { get; set; }
        public int BadgeCount { get; set; }
        public bool HasSubSteps { get; set; }
        public IReadOnlyList<StepModel> SubSteps { get; set; }
    }
}

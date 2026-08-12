using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.Steppers.Models
{
    public sealed class StepModel
    {
        /// <summary>The step's title. This is the caption a stepper shows.</summary>
        public string Text { get; set; }

        /// <summary>
        /// The optional second line under the title.
        /// </summary>
        /// <remarks>
        /// Named <c>SubText</c> to match <c>SimpleItem.SubText</c>, which is where it comes from.
        /// It was <c>SubText</c>, which matched nothing and made the mapping easy to invert - and
        /// it had been inverted: the controls read <c>SimpleItem.Name</c> for the title and
        /// <c>SimpleItem.Text</c> for this line, the opposite of every other Beep control.
        /// </remarks>
        public string SubText { get; set; }
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

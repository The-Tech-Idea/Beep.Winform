using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Steppers.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepStepperBar
    {
        private int GetScaledStepSpacing() => DpiScalingHelper.ScaleValue(20, this);

        private Point ComputeStepStartPoint(int spacing)
        {
            return TheTechIdea.Beep.Winform.Controls.Steppers.Helpers.StepperLayoutHelper.ComputeCenteredStartPoint(
                GetStepperContentBounds(),
                orientation,
                buttonSize,
                stepCount,
                spacing);
        }
    }
}

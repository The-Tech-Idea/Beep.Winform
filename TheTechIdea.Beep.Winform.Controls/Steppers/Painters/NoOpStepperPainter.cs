using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Steppers.Models;

namespace TheTechIdea.Beep.Winform.Controls.Steppers.Painters
{
    internal sealed class NoOpStepperPainter : IStepperPainter
    {
        public string Name => "NoOp";

        public void Initialize(BaseControl owner, IBeepTheme theme, Font stepFont, Font labelFont, Font numberFont)
        {
        }

        public StepperLayoutResult ComputeLayout(
            Rectangle clientRect,
            IReadOnlyList<StepModel> steps,
            Orientation orientation,
            StepperStyleConfig styleConfig)
        {
            return new StepperLayoutResult { ContentRect = clientRect };
        }

        public void Paint(Graphics g, StepPainterContext context)
        {
        }

        public void PaintStep(Graphics g, StepPainterContext context, int stepIndex, Rectangle stepRect)
        {
        }

        public void PaintConnector(Graphics g, StepPainterContext context, int fromIndex, Rectangle connectorRect)
        {
        }
    }
}

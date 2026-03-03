using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Steppers.Models;

namespace TheTechIdea.Beep.Winform.Controls.Steppers.Painters
{
    public interface IStepperPainter
    {
        string Name { get; }

        void Initialize(BaseControl owner, IBeepTheme theme, Font stepFont, Font labelFont, Font numberFont);

        StepperLayoutResult ComputeLayout(
            Rectangle clientRect,
            IReadOnlyList<StepModel> steps,
            Orientation orientation,
            StepperStyleConfig styleConfig);

        void Paint(Graphics g, StepPainterContext context);
        void PaintStep(Graphics g, StepPainterContext context, int stepIndex, Rectangle stepRect);
        void PaintConnector(Graphics g, StepPainterContext context, int fromIndex, Rectangle connectorRect);
    }
}

using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplyStepper()
        {
            this.StepperBackColor = PanelGradiantMiddleColor;
            this.StepperForeColor = ForeColor;
            this.StepperBorderColor = InactiveBorderColor;
            this.StepperItemForeColor = ForeColor;
            this.StepperItemHoverForeColor = ForeColor;
            this.StepperItemHoverBackColor = PanelGradiantMiddleColor;
            this.StepperItemSelectedForeColor = ForeColor;
            this.StepperItemSelectedBackColor = PanelGradiantMiddleColor;
            this.StepperItemSelectedBorderColor = InactiveBorderColor;
            this.StepperItemBorderColor = InactiveBorderColor;
            this.StepperItemHoverBorderColor = InactiveBorderColor;
            this.StepperItemCheckedBoxForeColor = ForeColor;
            this.StepperItemCheckedBoxBackColor = PanelGradiantMiddleColor;
            this.StepperItemCheckedBoxBorderColor = InactiveBorderColor;
        }
    }
}
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GruvBoxTheme
    {
        private void ApplyStepper()
        {
            this.StepperBackColor = PanelBackColor;
            this.StepperForeColor = ForeColor;
            this.StepperBorderColor = InactiveBorderColor;
            this.StepperItemForeColor = ForeColor;
            this.StepperItemHoverForeColor = ForeColor;
            this.StepperItemHoverBackColor = PanelBackColor;
            this.StepperItemSelectedForeColor = ForeColor;
            this.StepperItemSelectedBackColor = PanelBackColor;
            this.StepperItemSelectedBorderColor = InactiveBorderColor;
            this.StepperItemBorderColor = InactiveBorderColor;
            this.StepperItemHoverBorderColor = InactiveBorderColor;
            this.StepperItemCheckedBoxForeColor = ForeColor;
            this.StepperItemCheckedBoxBackColor = PanelBackColor;
            this.StepperItemCheckedBoxBorderColor = InactiveBorderColor;
        }
    }
}
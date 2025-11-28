using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GNOMETheme
    {
        private void ApplyStepper()
        {
            this.StepperBackColor = SurfaceColor;
            this.StepperForeColor = ForeColor;
            this.StepperBorderColor = BorderColor;
            this.StepperItemForeColor = ForeColor;
            this.StepperItemHoverForeColor = ForeColor;
            this.StepperItemHoverBackColor = PanelGradiantMiddleColor;
            this.StepperItemSelectedForeColor = ForeColor;
            this.StepperItemSelectedBackColor = SurfaceColor;
            this.StepperItemSelectedBorderColor = BorderColor;
            this.StepperItemBorderColor = BorderColor;
            this.StepperItemHoverBorderColor = BorderColor;
            this.StepperItemCheckedBoxForeColor = ForeColor;
            this.StepperItemCheckedBoxBackColor = SurfaceColor;
            this.StepperItemCheckedBoxBorderColor = BorderColor;
        }
    }
}
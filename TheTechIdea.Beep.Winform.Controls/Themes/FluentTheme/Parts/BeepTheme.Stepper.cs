using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyStepper()
        {
            this.StepperBackColor = SurfaceColor;
            this.StepperForeColor = ForeColor;
            this.StepperBorderColor = BorderColor;
            this.StepperItemForeColor = ForeColor;
            this.StepperItemHoverForeColor = ForeColor;
            this.StepperItemHoverBackColor = SecondaryColor;
            this.StepperItemSelectedForeColor = OnPrimaryColor;
            this.StepperItemSelectedBackColor = PrimaryColor;
            this.StepperItemSelectedBorderColor = PrimaryColor;
            this.StepperItemBorderColor = BorderColor;
            this.StepperItemHoverBorderColor = ActiveBorderColor;
            this.StepperItemCheckedBoxForeColor = OnPrimaryColor;
            this.StepperItemCheckedBoxBackColor = PrimaryColor;
            this.StepperItemCheckedBoxBorderColor = PrimaryColor;
        }
    }
}
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class iOSTheme
    {
        private void ApplyStepper()
        {
            this.StepperBackColor = BackgroundColor;
            this.StepperForeColor = ForeColor;
            this.StepperBorderColor = BorderColor;
            this.StepperItemForeColor = ForeColor;
            this.StepperItemHoverForeColor = ForeColor;
            this.StepperItemHoverBackColor = BackgroundColor;
            this.StepperItemSelectedForeColor = ForeColor;
            this.StepperItemSelectedBackColor = BackgroundColor;
            this.StepperItemSelectedBorderColor = BorderColor;
            this.StepperItemBorderColor = BorderColor;
            this.StepperItemHoverBorderColor = BorderColor;
            this.StepperItemCheckedBoxForeColor = ForeColor;
            this.StepperItemCheckedBoxBackColor = BackgroundColor;
            this.StepperItemCheckedBoxBorderColor = BorderColor;
        }
    }
}
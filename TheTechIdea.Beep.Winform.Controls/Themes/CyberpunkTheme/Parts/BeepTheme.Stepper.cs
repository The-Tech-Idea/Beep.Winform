using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyStepper()
        {
            this.StepperBackColor = BackgroundColor;
            this.StepperForeColor = ForeColor;
            this.StepperBorderColor = BorderColor;
            this.StepperItemForeColor = ForeColor;
            this.StepperItemHoverForeColor = ForeColor;
            this.StepperItemHoverBackColor = PanelGradiantMiddleColor;
            this.StepperItemSelectedForeColor = ForeColor;
            this.StepperItemSelectedBackColor = PanelGradiantMiddleColor;
            this.StepperItemSelectedBorderColor = ActiveBorderColor;
            this.StepperItemBorderColor = BorderColor;
            this.StepperItemHoverBorderColor = ActiveBorderColor;
            this.StepperItemCheckedBoxForeColor = OnPrimaryColor;
            this.StepperItemCheckedBoxBackColor = PrimaryColor;
            this.StepperItemCheckedBoxBorderColor = ActiveBorderColor;
        }
    }
}
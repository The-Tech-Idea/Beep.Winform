using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyStepper()
        {
            this.StepperBackColor = Color.FromArgb(0,43,54);
            this.StepperForeColor = Color.FromArgb(147,161,161);
            this.StepperBorderColor = Color.FromArgb(88,110,117);
            this.StepperItemForeColor = Color.FromArgb(147,161,161);
            this.StepperItemHoverForeColor = Color.FromArgb(147,161,161);
            this.StepperItemHoverBackColor = Color.FromArgb(0,43,54);
            this.StepperItemSelectedForeColor = Color.FromArgb(147,161,161);
            this.StepperItemSelectedBackColor = Color.FromArgb(0,43,54);
            this.StepperItemSelectedBorderColor = Color.FromArgb(88,110,117);
            this.StepperItemBorderColor = Color.FromArgb(88,110,117);
            this.StepperItemHoverBorderColor = Color.FromArgb(88,110,117);
            this.StepperItemCheckedBoxForeColor = Color.FromArgb(147,161,161);
            this.StepperItemCheckedBoxBackColor = Color.FromArgb(0,43,54);
            this.StepperItemCheckedBoxBorderColor = Color.FromArgb(88,110,117);
        }
    }
}
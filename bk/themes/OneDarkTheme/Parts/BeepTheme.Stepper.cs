using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class OneDarkTheme
    {
        private void ApplyStepper()
        {
            this.StepperBackColor = Color.FromArgb(40,44,52);
            this.StepperForeColor = Color.FromArgb(171,178,191);
            this.StepperBorderColor = Color.FromArgb(92,99,112);
            this.StepperItemForeColor = Color.FromArgb(171,178,191);
            this.StepperItemHoverForeColor = Color.FromArgb(171,178,191);
            this.StepperItemHoverBackColor = Color.FromArgb(40,44,52);
            this.StepperItemSelectedForeColor = Color.FromArgb(171,178,191);
            this.StepperItemSelectedBackColor = Color.FromArgb(40,44,52);
            this.StepperItemSelectedBorderColor = Color.FromArgb(92,99,112);
            this.StepperItemBorderColor = Color.FromArgb(92,99,112);
            this.StepperItemHoverBorderColor = Color.FromArgb(92,99,112);
            this.StepperItemCheckedBoxForeColor = Color.FromArgb(171,178,191);
            this.StepperItemCheckedBoxBackColor = Color.FromArgb(40,44,52);
            this.StepperItemCheckedBoxBorderColor = Color.FromArgb(92,99,112);
        }
    }
}
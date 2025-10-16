using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MetroTheme
    {
        private void ApplyStepper()
        {
            this.StepperBackColor = Color.FromArgb(243,242,241);
            this.StepperForeColor = Color.FromArgb(32,31,30);
            this.StepperBorderColor = Color.FromArgb(225,225,225);
            this.StepperItemForeColor = Color.FromArgb(32,31,30);
            this.StepperItemHoverForeColor = Color.FromArgb(32,31,30);
            this.StepperItemHoverBackColor = Color.FromArgb(243,242,241);
            this.StepperItemSelectedForeColor = Color.FromArgb(32,31,30);
            this.StepperItemSelectedBackColor = Color.FromArgb(243,242,241);
            this.StepperItemSelectedBorderColor = Color.FromArgb(225,225,225);
            this.StepperItemBorderColor = Color.FromArgb(225,225,225);
            this.StepperItemHoverBorderColor = Color.FromArgb(225,225,225);
            this.StepperItemCheckedBoxForeColor = Color.FromArgb(32,31,30);
            this.StepperItemCheckedBoxBackColor = Color.FromArgb(243,242,241);
            this.StepperItemCheckedBoxBorderColor = Color.FromArgb(225,225,225);
        }
    }
}
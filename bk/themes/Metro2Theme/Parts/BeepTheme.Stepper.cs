using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class Metro2Theme
    {
        private void ApplyStepper()
        {
            this.StepperBackColor = Color.FromArgb(243,242,241);
            this.StepperForeColor = Color.FromArgb(32,31,30);
            this.StepperBorderColor = Color.FromArgb(220,220,220);
            this.StepperItemForeColor = Color.FromArgb(32,31,30);
            this.StepperItemHoverForeColor = Color.FromArgb(32,31,30);
            this.StepperItemHoverBackColor = Color.FromArgb(243,242,241);
            this.StepperItemSelectedForeColor = Color.FromArgb(32,31,30);
            this.StepperItemSelectedBackColor = Color.FromArgb(243,242,241);
            this.StepperItemSelectedBorderColor = Color.FromArgb(220,220,220);
            this.StepperItemBorderColor = Color.FromArgb(220,220,220);
            this.StepperItemHoverBorderColor = Color.FromArgb(220,220,220);
            this.StepperItemCheckedBoxForeColor = Color.FromArgb(32,31,30);
            this.StepperItemCheckedBoxBackColor = Color.FromArgb(243,242,241);
            this.StepperItemCheckedBoxBorderColor = Color.FromArgb(220,220,220);
        }
    }
}
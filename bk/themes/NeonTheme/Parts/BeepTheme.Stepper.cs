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
            this.StepperBackColor = Color.FromArgb(10,12,20);
            this.StepperForeColor = Color.FromArgb(235,245,255);
            this.StepperBorderColor = Color.FromArgb(60,70,100);
            this.StepperItemForeColor = Color.FromArgb(235,245,255);
            this.StepperItemHoverForeColor = Color.FromArgb(235,245,255);
            this.StepperItemHoverBackColor = Color.FromArgb(10,12,20);
            this.StepperItemSelectedForeColor = Color.FromArgb(235,245,255);
            this.StepperItemSelectedBackColor = Color.FromArgb(10,12,20);
            this.StepperItemSelectedBorderColor = Color.FromArgb(60,70,100);
            this.StepperItemBorderColor = Color.FromArgb(60,70,100);
            this.StepperItemHoverBorderColor = Color.FromArgb(60,70,100);
            this.StepperItemCheckedBoxForeColor = Color.FromArgb(235,245,255);
            this.StepperItemCheckedBoxBackColor = Color.FromArgb(10,12,20);
            this.StepperItemCheckedBoxBorderColor = Color.FromArgb(60,70,100);
        }
    }
}
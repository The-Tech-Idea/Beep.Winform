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
            this.StepperBackColor = Color.FromArgb(10,8,20);
            this.StepperForeColor = Color.FromArgb(228,244,255);
            this.StepperBorderColor = Color.FromArgb(90,20,110);
            this.StepperItemForeColor = Color.FromArgb(228,244,255);
            this.StepperItemHoverForeColor = Color.FromArgb(228,244,255);
            this.StepperItemHoverBackColor = Color.FromArgb(10,8,20);
            this.StepperItemSelectedForeColor = Color.FromArgb(228,244,255);
            this.StepperItemSelectedBackColor = Color.FromArgb(10,8,20);
            this.StepperItemSelectedBorderColor = Color.FromArgb(90,20,110);
            this.StepperItemBorderColor = Color.FromArgb(90,20,110);
            this.StepperItemHoverBorderColor = Color.FromArgb(90,20,110);
            this.StepperItemCheckedBoxForeColor = Color.FromArgb(228,244,255);
            this.StepperItemCheckedBoxBackColor = Color.FromArgb(10,8,20);
            this.StepperItemCheckedBoxBorderColor = Color.FromArgb(90,20,110);
        }
    }
}
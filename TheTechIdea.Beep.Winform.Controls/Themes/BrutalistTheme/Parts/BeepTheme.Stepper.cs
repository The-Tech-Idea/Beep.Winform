using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class BrutalistTheme
    {
        private void ApplyStepper()
        {
            this.StepperBackColor = Color.FromArgb(250,250,250);
            this.StepperForeColor = Color.FromArgb(20,20,20);
            this.StepperBorderColor = Color.FromArgb(0,0,0);
            this.StepperItemForeColor = Color.FromArgb(20,20,20);
            this.StepperItemHoverForeColor = Color.FromArgb(20,20,20);
            this.StepperItemHoverBackColor = Color.FromArgb(250,250,250);
            this.StepperItemSelectedForeColor = Color.FromArgb(20,20,20);
            this.StepperItemSelectedBackColor = Color.FromArgb(250,250,250);
            this.StepperItemSelectedBorderColor = Color.FromArgb(0,0,0);
            this.StepperItemBorderColor = Color.FromArgb(0,0,0);
            this.StepperItemHoverBorderColor = Color.FromArgb(0,0,0);
            this.StepperItemCheckedBoxForeColor = Color.FromArgb(20,20,20);
            this.StepperItemCheckedBoxBackColor = Color.FromArgb(250,250,250);
            this.StepperItemCheckedBoxBorderColor = Color.FromArgb(0,0,0);
        }
    }
}
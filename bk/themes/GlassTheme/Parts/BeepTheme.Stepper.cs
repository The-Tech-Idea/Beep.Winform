using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GlassTheme
    {
        private void ApplyStepper()
        {
            this.StepperBackColor = Color.FromArgb(236,244,255);
            this.StepperForeColor = Color.FromArgb(17,24,39);
            this.StepperBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.StepperItemForeColor = Color.FromArgb(17,24,39);
            this.StepperItemHoverForeColor = Color.FromArgb(17,24,39);
            this.StepperItemHoverBackColor = Color.FromArgb(236,244,255);
            this.StepperItemSelectedForeColor = Color.FromArgb(17,24,39);
            this.StepperItemSelectedBackColor = Color.FromArgb(236,244,255);
            this.StepperItemSelectedBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.StepperItemBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.StepperItemHoverBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.StepperItemCheckedBoxForeColor = Color.FromArgb(17,24,39);
            this.StepperItemCheckedBoxBackColor = Color.FromArgb(236,244,255);
            this.StepperItemCheckedBoxBorderColor = Color.FromArgb(140, 255, 255, 255);
        }
    }
}
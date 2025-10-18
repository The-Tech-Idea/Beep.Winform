using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplyStepper()
        {
            this.StepperBackColor = Color.FromArgb(15,16,32);
            this.StepperForeColor = Color.FromArgb(245,247,255);
            this.StepperBorderColor = Color.FromArgb(74,79,123);
            this.StepperItemForeColor = Color.FromArgb(245,247,255);
            this.StepperItemHoverForeColor = Color.FromArgb(245,247,255);
            this.StepperItemHoverBackColor = Color.FromArgb(15,16,32);
            this.StepperItemSelectedForeColor = Color.FromArgb(245,247,255);
            this.StepperItemSelectedBackColor = Color.FromArgb(15,16,32);
            this.StepperItemSelectedBorderColor = Color.FromArgb(74,79,123);
            this.StepperItemBorderColor = Color.FromArgb(74,79,123);
            this.StepperItemHoverBorderColor = Color.FromArgb(74,79,123);
            this.StepperItemCheckedBoxForeColor = Color.FromArgb(245,247,255);
            this.StepperItemCheckedBoxBackColor = Color.FromArgb(15,16,32);
            this.StepperItemCheckedBoxBorderColor = Color.FromArgb(74,79,123);
        }
    }
}
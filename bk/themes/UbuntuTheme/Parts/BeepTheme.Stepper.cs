using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplyStepper()
        {
            this.StepperBackColor = Color.FromArgb(242,242,245);
            this.StepperForeColor = Color.FromArgb(44,44,44);
            this.StepperBorderColor = Color.FromArgb(218,218,222);
            this.StepperItemForeColor = Color.FromArgb(44,44,44);
            this.StepperItemHoverForeColor = Color.FromArgb(44,44,44);
            this.StepperItemHoverBackColor = Color.FromArgb(242,242,245);
            this.StepperItemSelectedForeColor = Color.FromArgb(44,44,44);
            this.StepperItemSelectedBackColor = Color.FromArgb(242,242,245);
            this.StepperItemSelectedBorderColor = Color.FromArgb(218,218,222);
            this.StepperItemBorderColor = Color.FromArgb(218,218,222);
            this.StepperItemHoverBorderColor = Color.FromArgb(218,218,222);
            this.StepperItemCheckedBoxForeColor = Color.FromArgb(44,44,44);
            this.StepperItemCheckedBoxBackColor = Color.FromArgb(242,242,245);
            this.StepperItemCheckedBoxBorderColor = Color.FromArgb(218,218,222);
        }
    }
}
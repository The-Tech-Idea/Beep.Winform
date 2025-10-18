using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GruvBoxTheme
    {
        private void ApplyStepper()
        {
            this.StepperBackColor = Color.FromArgb(40,40,40);
            this.StepperForeColor = Color.FromArgb(235,219,178);
            this.StepperBorderColor = Color.FromArgb(168,153,132);
            this.StepperItemForeColor = Color.FromArgb(235,219,178);
            this.StepperItemHoverForeColor = Color.FromArgb(235,219,178);
            this.StepperItemHoverBackColor = Color.FromArgb(40,40,40);
            this.StepperItemSelectedForeColor = Color.FromArgb(235,219,178);
            this.StepperItemSelectedBackColor = Color.FromArgb(40,40,40);
            this.StepperItemSelectedBorderColor = Color.FromArgb(168,153,132);
            this.StepperItemBorderColor = Color.FromArgb(168,153,132);
            this.StepperItemHoverBorderColor = Color.FromArgb(168,153,132);
            this.StepperItemCheckedBoxForeColor = Color.FromArgb(235,219,178);
            this.StepperItemCheckedBoxBackColor = Color.FromArgb(40,40,40);
            this.StepperItemCheckedBoxBorderColor = Color.FromArgb(168,153,132);
        }
    }
}
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyStepper()
        {
            this.StepperBackColor = Color.FromArgb(245,246,248);
            this.StepperForeColor = Color.FromArgb(32,32,32);
            this.StepperBorderColor = Color.FromArgb(218,223,230);
            this.StepperItemForeColor = Color.FromArgb(32,32,32);
            this.StepperItemHoverForeColor = Color.FromArgb(32,32,32);
            this.StepperItemHoverBackColor = Color.FromArgb(245,246,248);
            this.StepperItemSelectedForeColor = Color.FromArgb(32,32,32);
            this.StepperItemSelectedBackColor = Color.FromArgb(245,246,248);
            this.StepperItemSelectedBorderColor = Color.FromArgb(218,223,230);
            this.StepperItemBorderColor = Color.FromArgb(218,223,230);
            this.StepperItemHoverBorderColor = Color.FromArgb(218,223,230);
            this.StepperItemCheckedBoxForeColor = Color.FromArgb(32,32,32);
            this.StepperItemCheckedBoxBackColor = Color.FromArgb(245,246,248);
            this.StepperItemCheckedBoxBorderColor = Color.FromArgb(218,223,230);
        }
    }
}
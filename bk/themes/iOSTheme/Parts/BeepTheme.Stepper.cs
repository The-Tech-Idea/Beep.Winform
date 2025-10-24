using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class iOSTheme
    {
        private void ApplyStepper()
        {
            this.StepperBackColor = Color.FromArgb(242,242,247);
            this.StepperForeColor = Color.FromArgb(28,28,30);
            this.StepperBorderColor = Color.FromArgb(198,198,207);
            this.StepperItemForeColor = Color.FromArgb(28,28,30);
            this.StepperItemHoverForeColor = Color.FromArgb(28,28,30);
            this.StepperItemHoverBackColor = Color.FromArgb(242,242,247);
            this.StepperItemSelectedForeColor = Color.FromArgb(28,28,30);
            this.StepperItemSelectedBackColor = Color.FromArgb(242,242,247);
            this.StepperItemSelectedBorderColor = Color.FromArgb(198,198,207);
            this.StepperItemBorderColor = Color.FromArgb(198,198,207);
            this.StepperItemHoverBorderColor = Color.FromArgb(198,198,207);
            this.StepperItemCheckedBoxForeColor = Color.FromArgb(28,28,30);
            this.StepperItemCheckedBoxBackColor = Color.FromArgb(242,242,247);
            this.StepperItemCheckedBoxBorderColor = Color.FromArgb(198,198,207);
        }
    }
}
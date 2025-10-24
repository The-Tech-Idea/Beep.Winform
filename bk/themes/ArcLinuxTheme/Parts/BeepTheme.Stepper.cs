using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyStepper()
        {
            this.StepperBackColor = Color.FromArgb(245,246,247);
            this.StepperForeColor = Color.FromArgb(43,45,48);
            this.StepperBorderColor = Color.FromArgb(220,223,230);
            this.StepperItemForeColor = Color.FromArgb(43,45,48);
            this.StepperItemHoverForeColor = Color.FromArgb(43,45,48);
            this.StepperItemHoverBackColor = Color.FromArgb(245,246,247);
            this.StepperItemSelectedForeColor = Color.FromArgb(43,45,48);
            this.StepperItemSelectedBackColor = Color.FromArgb(245,246,247);
            this.StepperItemSelectedBorderColor = Color.FromArgb(220,223,230);
            this.StepperItemBorderColor = Color.FromArgb(220,223,230);
            this.StepperItemHoverBorderColor = Color.FromArgb(220,223,230);
            this.StepperItemCheckedBoxForeColor = Color.FromArgb(43,45,48);
            this.StepperItemCheckedBoxBackColor = Color.FromArgb(245,246,247);
            this.StepperItemCheckedBoxBorderColor = Color.FromArgb(220,223,230);
        }
    }
}
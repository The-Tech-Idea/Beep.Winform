using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MacOSTheme
    {
        private void ApplyStepper()
        {
            this.StepperBackColor = Color.FromArgb(250,250,252);
            this.StepperForeColor = Color.FromArgb(28,28,30);
            this.StepperBorderColor = Color.FromArgb(229,229,234);
            this.StepperItemForeColor = Color.FromArgb(28,28,30);
            this.StepperItemHoverForeColor = Color.FromArgb(28,28,30);
            this.StepperItemHoverBackColor = Color.FromArgb(250,250,252);
            this.StepperItemSelectedForeColor = Color.FromArgb(28,28,30);
            this.StepperItemSelectedBackColor = Color.FromArgb(250,250,252);
            this.StepperItemSelectedBorderColor = Color.FromArgb(229,229,234);
            this.StepperItemBorderColor = Color.FromArgb(229,229,234);
            this.StepperItemHoverBorderColor = Color.FromArgb(229,229,234);
            this.StepperItemCheckedBoxForeColor = Color.FromArgb(28,28,30);
            this.StepperItemCheckedBoxBackColor = Color.FromArgb(250,250,252);
            this.StepperItemCheckedBoxBorderColor = Color.FromArgb(229,229,234);
        }
    }
}
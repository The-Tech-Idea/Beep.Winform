using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class DraculaTheme
    {
        private void ApplyStepper()
        {
            this.StepperBackColor = Color.FromArgb(40,42,54);
            this.StepperForeColor = Color.FromArgb(248,248,242);
            this.StepperBorderColor = Color.FromArgb(98,114,164);
            this.StepperItemForeColor = Color.FromArgb(248,248,242);
            this.StepperItemHoverForeColor = Color.FromArgb(248,248,242);
            this.StepperItemHoverBackColor = Color.FromArgb(40,42,54);
            this.StepperItemSelectedForeColor = Color.FromArgb(248,248,242);
            this.StepperItemSelectedBackColor = Color.FromArgb(40,42,54);
            this.StepperItemSelectedBorderColor = Color.FromArgb(98,114,164);
            this.StepperItemBorderColor = Color.FromArgb(98,114,164);
            this.StepperItemHoverBorderColor = Color.FromArgb(98,114,164);
            this.StepperItemCheckedBoxForeColor = Color.FromArgb(248,248,242);
            this.StepperItemCheckedBoxBackColor = Color.FromArgb(40,42,54);
            this.StepperItemCheckedBoxBorderColor = Color.FromArgb(98,114,164);
        }
    }
}
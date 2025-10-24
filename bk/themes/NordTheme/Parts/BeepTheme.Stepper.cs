using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordTheme
    {
        private void ApplyStepper()
        {
            this.StepperBackColor = Color.FromArgb(46,52,64);
            this.StepperForeColor = Color.FromArgb(216,222,233);
            this.StepperBorderColor = Color.FromArgb(76,86,106);
            this.StepperItemForeColor = Color.FromArgb(216,222,233);
            this.StepperItemHoverForeColor = Color.FromArgb(216,222,233);
            this.StepperItemHoverBackColor = Color.FromArgb(46,52,64);
            this.StepperItemSelectedForeColor = Color.FromArgb(216,222,233);
            this.StepperItemSelectedBackColor = Color.FromArgb(46,52,64);
            this.StepperItemSelectedBorderColor = Color.FromArgb(76,86,106);
            this.StepperItemBorderColor = Color.FromArgb(76,86,106);
            this.StepperItemHoverBorderColor = Color.FromArgb(76,86,106);
            this.StepperItemCheckedBoxForeColor = Color.FromArgb(216,222,233);
            this.StepperItemCheckedBoxBackColor = Color.FromArgb(46,52,64);
            this.StepperItemCheckedBoxBorderColor = Color.FromArgb(76,86,106);
        }
    }
}
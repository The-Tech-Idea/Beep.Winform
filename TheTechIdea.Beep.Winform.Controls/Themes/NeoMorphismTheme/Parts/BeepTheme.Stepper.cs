using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplyStepper()
        {
            this.StepperBackColor = Color.FromArgb(236,240,243);
            this.StepperForeColor = Color.FromArgb(58,66,86);
            this.StepperBorderColor = Color.FromArgb(221,228,235);
            this.StepperItemForeColor = Color.FromArgb(58,66,86);
            this.StepperItemHoverForeColor = Color.FromArgb(58,66,86);
            this.StepperItemHoverBackColor = Color.FromArgb(236,240,243);
            this.StepperItemSelectedForeColor = Color.FromArgb(58,66,86);
            this.StepperItemSelectedBackColor = Color.FromArgb(236,240,243);
            this.StepperItemSelectedBorderColor = Color.FromArgb(221,228,235);
            this.StepperItemBorderColor = Color.FromArgb(221,228,235);
            this.StepperItemHoverBorderColor = Color.FromArgb(221,228,235);
            this.StepperItemCheckedBoxForeColor = Color.FromArgb(58,66,86);
            this.StepperItemCheckedBoxBackColor = Color.FromArgb(236,240,243);
            this.StepperItemCheckedBoxBorderColor = Color.FromArgb(221,228,235);
        }
    }
}
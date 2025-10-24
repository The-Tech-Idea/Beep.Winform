using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class PaperTheme
    {
        private void ApplyStepper()
        {
            this.StepperBackColor = Color.FromArgb(250,250,250);
            this.StepperForeColor = Color.FromArgb(33,33,33);
            this.StepperBorderColor = Color.FromArgb(224,224,224);
            this.StepperItemForeColor = Color.FromArgb(33,33,33);
            this.StepperItemHoverForeColor = Color.FromArgb(33,33,33);
            this.StepperItemHoverBackColor = Color.FromArgb(250,250,250);
            this.StepperItemSelectedForeColor = Color.FromArgb(33,33,33);
            this.StepperItemSelectedBackColor = Color.FromArgb(250,250,250);
            this.StepperItemSelectedBorderColor = Color.FromArgb(224,224,224);
            this.StepperItemBorderColor = Color.FromArgb(224,224,224);
            this.StepperItemHoverBorderColor = Color.FromArgb(224,224,224);
            this.StepperItemCheckedBoxForeColor = Color.FromArgb(33,33,33);
            this.StepperItemCheckedBoxBackColor = Color.FromArgb(250,250,250);
            this.StepperItemCheckedBoxBorderColor = Color.FromArgb(224,224,224);
        }
    }
}
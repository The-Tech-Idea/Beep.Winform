using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplyStepper()
        {
            this.StepperBackColor = Color.FromArgb(255,251,235);
            this.StepperForeColor = Color.FromArgb(33,37,41);
            this.StepperBorderColor = Color.FromArgb(247,208,136);
            this.StepperItemForeColor = Color.FromArgb(33,37,41);
            this.StepperItemHoverForeColor = Color.FromArgb(33,37,41);
            this.StepperItemHoverBackColor = Color.FromArgb(255,251,235);
            this.StepperItemSelectedForeColor = Color.FromArgb(33,37,41);
            this.StepperItemSelectedBackColor = Color.FromArgb(255,251,235);
            this.StepperItemSelectedBorderColor = Color.FromArgb(247,208,136);
            this.StepperItemBorderColor = Color.FromArgb(247,208,136);
            this.StepperItemHoverBorderColor = Color.FromArgb(247,208,136);
            this.StepperItemCheckedBoxForeColor = Color.FromArgb(33,37,41);
            this.StepperItemCheckedBoxBackColor = Color.FromArgb(255,251,235);
            this.StepperItemCheckedBoxBorderColor = Color.FromArgb(247,208,136);
        }
    }
}
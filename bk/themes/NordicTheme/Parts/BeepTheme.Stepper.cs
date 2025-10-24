using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordicTheme
    {
        private void ApplyStepper()
        {
            this.StepperBackColor = Color.FromArgb(250,250,251);
            this.StepperForeColor = Color.FromArgb(31,41,55);
            this.StepperBorderColor = Color.FromArgb(229,231,235);
            this.StepperItemForeColor = Color.FromArgb(31,41,55);
            this.StepperItemHoverForeColor = Color.FromArgb(31,41,55);
            this.StepperItemHoverBackColor = Color.FromArgb(250,250,251);
            this.StepperItemSelectedForeColor = Color.FromArgb(31,41,55);
            this.StepperItemSelectedBackColor = Color.FromArgb(250,250,251);
            this.StepperItemSelectedBorderColor = Color.FromArgb(229,231,235);
            this.StepperItemBorderColor = Color.FromArgb(229,231,235);
            this.StepperItemHoverBorderColor = Color.FromArgb(229,231,235);
            this.StepperItemCheckedBoxForeColor = Color.FromArgb(31,41,55);
            this.StepperItemCheckedBoxBackColor = Color.FromArgb(250,250,251);
            this.StepperItemCheckedBoxBorderColor = Color.FromArgb(229,231,235);
        }
    }
}
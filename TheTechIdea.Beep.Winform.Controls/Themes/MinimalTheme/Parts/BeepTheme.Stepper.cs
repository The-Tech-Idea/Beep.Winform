using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyStepper()
        {
            this.StepperBackColor = Color.FromArgb(255,255,255);
            this.StepperForeColor = Color.FromArgb(31,41,55);
            this.StepperBorderColor = Color.FromArgb(209,213,219);
            this.StepperItemForeColor = Color.FromArgb(31,41,55);
            this.StepperItemHoverForeColor = Color.FromArgb(31,41,55);
            this.StepperItemHoverBackColor = Color.FromArgb(255,255,255);
            this.StepperItemSelectedForeColor = Color.FromArgb(31,41,55);
            this.StepperItemSelectedBackColor = Color.FromArgb(255,255,255);
            this.StepperItemSelectedBorderColor = Color.FromArgb(209,213,219);
            this.StepperItemBorderColor = Color.FromArgb(209,213,219);
            this.StepperItemHoverBorderColor = Color.FromArgb(209,213,219);
            this.StepperItemCheckedBoxForeColor = Color.FromArgb(31,41,55);
            this.StepperItemCheckedBoxBackColor = Color.FromArgb(255,255,255);
            this.StepperItemCheckedBoxBorderColor = Color.FromArgb(209,213,219);
        }
    }
}
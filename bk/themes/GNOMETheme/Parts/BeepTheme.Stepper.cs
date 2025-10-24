using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GNOMETheme
    {
        private void ApplyStepper()
        {
            this.StepperBackColor = Color.FromArgb(246,245,244);
            this.StepperForeColor = Color.FromArgb(46,52,54);
            this.StepperBorderColor = Color.FromArgb(205,207,212);
            this.StepperItemForeColor = Color.FromArgb(46,52,54);
            this.StepperItemHoverForeColor = Color.FromArgb(46,52,54);
            this.StepperItemHoverBackColor = Color.FromArgb(246,245,244);
            this.StepperItemSelectedForeColor = Color.FromArgb(46,52,54);
            this.StepperItemSelectedBackColor = Color.FromArgb(246,245,244);
            this.StepperItemSelectedBorderColor = Color.FromArgb(205,207,212);
            this.StepperItemBorderColor = Color.FromArgb(205,207,212);
            this.StepperItemHoverBorderColor = Color.FromArgb(205,207,212);
            this.StepperItemCheckedBoxForeColor = Color.FromArgb(46,52,54);
            this.StepperItemCheckedBoxBackColor = Color.FromArgb(246,245,244);
            this.StepperItemCheckedBoxBorderColor = Color.FromArgb(205,207,212);
        }
    }
}
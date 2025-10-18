using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class KDETheme
    {
        private void ApplyStepper()
        {
            this.StepperBackColor = Color.FromArgb(248,249,250);
            this.StepperForeColor = Color.FromArgb(33,37,41);
            this.StepperBorderColor = Color.FromArgb(222,226,230);
            this.StepperItemForeColor = Color.FromArgb(33,37,41);
            this.StepperItemHoverForeColor = Color.FromArgb(33,37,41);
            this.StepperItemHoverBackColor = Color.FromArgb(248,249,250);
            this.StepperItemSelectedForeColor = Color.FromArgb(33,37,41);
            this.StepperItemSelectedBackColor = Color.FromArgb(248,249,250);
            this.StepperItemSelectedBorderColor = Color.FromArgb(222,226,230);
            this.StepperItemBorderColor = Color.FromArgb(222,226,230);
            this.StepperItemHoverBorderColor = Color.FromArgb(222,226,230);
            this.StepperItemCheckedBoxForeColor = Color.FromArgb(33,37,41);
            this.StepperItemCheckedBoxBackColor = Color.FromArgb(248,249,250);
            this.StepperItemCheckedBoxBorderColor = Color.FromArgb(222,226,230);
        }
    }
}
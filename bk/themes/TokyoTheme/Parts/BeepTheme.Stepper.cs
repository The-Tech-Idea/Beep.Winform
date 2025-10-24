using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class TokyoTheme
    {
        private void ApplyStepper()
        {
            this.StepperBackColor = Color.FromArgb(26,27,38);
            this.StepperForeColor = Color.FromArgb(192,202,245);
            this.StepperBorderColor = Color.FromArgb(86,95,137);
            this.StepperItemForeColor = Color.FromArgb(192,202,245);
            this.StepperItemHoverForeColor = Color.FromArgb(192,202,245);
            this.StepperItemHoverBackColor = Color.FromArgb(26,27,38);
            this.StepperItemSelectedForeColor = Color.FromArgb(192,202,245);
            this.StepperItemSelectedBackColor = Color.FromArgb(26,27,38);
            this.StepperItemSelectedBorderColor = Color.FromArgb(86,95,137);
            this.StepperItemBorderColor = Color.FromArgb(86,95,137);
            this.StepperItemHoverBorderColor = Color.FromArgb(86,95,137);
            this.StepperItemCheckedBoxForeColor = Color.FromArgb(192,202,245);
            this.StepperItemCheckedBoxBackColor = Color.FromArgb(26,27,38);
            this.StepperItemCheckedBoxBorderColor = Color.FromArgb(86,95,137);
        }
    }
}
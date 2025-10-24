using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme
    {
        private void ApplyStepper()
        {
            this.StepperBackColor = Color.FromArgb(245,248,255);
            this.StepperForeColor = Color.FromArgb(24,28,35);
            this.StepperBorderColor = Color.FromArgb(210,220,235);
            this.StepperItemForeColor = Color.FromArgb(24,28,35);
            this.StepperItemHoverForeColor = Color.FromArgb(24,28,35);
            this.StepperItemHoverBackColor = Color.FromArgb(245,248,255);
            this.StepperItemSelectedForeColor = Color.FromArgb(24,28,35);
            this.StepperItemSelectedBackColor = Color.FromArgb(245,248,255);
            this.StepperItemSelectedBorderColor = Color.FromArgb(210,220,235);
            this.StepperItemBorderColor = Color.FromArgb(210,220,235);
            this.StepperItemHoverBorderColor = Color.FromArgb(210,220,235);
            this.StepperItemCheckedBoxForeColor = Color.FromArgb(24,28,35);
            this.StepperItemCheckedBoxBackColor = Color.FromArgb(245,248,255);
            this.StepperItemCheckedBoxBorderColor = Color.FromArgb(210,220,235);
        }
    }
}
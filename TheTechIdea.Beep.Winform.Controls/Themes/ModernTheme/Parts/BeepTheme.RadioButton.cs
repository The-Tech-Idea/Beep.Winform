using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ModernTheme
    {
        private void ApplyRadioButton()
        {
            this.RadioButtonBackColor = Color.FromArgb(255,255,255);
            this.RadioButtonForeColor = Color.FromArgb(17,24,39);
            this.RadioButtonBorderColor = Color.FromArgb(203,213,225);
            this.RadioButtonCheckedBackColor = Color.FromArgb(255,255,255);
            this.RadioButtonCheckedForeColor = Color.FromArgb(17,24,39);
            this.RadioButtonCheckedBorderColor = Color.FromArgb(203,213,225);
            this.RadioButtonHoverBackColor = Color.FromArgb(255,255,255);
            this.RadioButtonHoverForeColor = Color.FromArgb(17,24,39);
            this.RadioButtonHoverBorderColor = Color.FromArgb(203,213,225);
            this.RadioButtonSelectedForeColor = Color.FromArgb(17,24,39);
            this.RadioButtonSelectedBackColor = Color.FromArgb(255,255,255);
        }
    }
}
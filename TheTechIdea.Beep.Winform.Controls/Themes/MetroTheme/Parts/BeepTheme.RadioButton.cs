using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MetroTheme
    {
        private void ApplyRadioButton()
        {
            this.RadioButtonBackColor = Color.FromArgb(243,242,241);
            this.RadioButtonForeColor = Color.FromArgb(32,31,30);
            this.RadioButtonBorderColor = Color.FromArgb(225,225,225);
            this.RadioButtonCheckedBackColor = Color.FromArgb(243,242,241);
            this.RadioButtonCheckedForeColor = Color.FromArgb(32,31,30);
            this.RadioButtonCheckedBorderColor = Color.FromArgb(225,225,225);
            this.RadioButtonHoverBackColor = Color.FromArgb(243,242,241);
            this.RadioButtonHoverForeColor = Color.FromArgb(32,31,30);
            this.RadioButtonHoverBorderColor = Color.FromArgb(225,225,225);
            this.RadioButtonSelectedForeColor = Color.FromArgb(32,31,30);
            this.RadioButtonSelectedBackColor = Color.FromArgb(243,242,241);
        }
    }
}
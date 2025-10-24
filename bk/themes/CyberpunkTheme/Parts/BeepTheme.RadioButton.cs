using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyRadioButton()
        {
            this.RadioButtonBackColor = Color.FromArgb(10,8,20);
            this.RadioButtonForeColor = Color.FromArgb(228,244,255);
            this.RadioButtonBorderColor = Color.FromArgb(90,20,110);
            this.RadioButtonCheckedBackColor = Color.FromArgb(10,8,20);
            this.RadioButtonCheckedForeColor = Color.FromArgb(228,244,255);
            this.RadioButtonCheckedBorderColor = Color.FromArgb(90,20,110);
            this.RadioButtonHoverBackColor = Color.FromArgb(10,8,20);
            this.RadioButtonHoverForeColor = Color.FromArgb(228,244,255);
            this.RadioButtonHoverBorderColor = Color.FromArgb(90,20,110);
            this.RadioButtonSelectedForeColor = Color.FromArgb(228,244,255);
            this.RadioButtonSelectedBackColor = Color.FromArgb(10,8,20);
        }
    }
}
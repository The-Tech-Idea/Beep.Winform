using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GlassTheme
    {
        private void ApplyRadioButton()
        {
            this.RadioButtonBackColor = Color.FromArgb(236,244,255);
            this.RadioButtonForeColor = Color.FromArgb(17,24,39);
            this.RadioButtonBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.RadioButtonCheckedBackColor = Color.FromArgb(236,244,255);
            this.RadioButtonCheckedForeColor = Color.FromArgb(17,24,39);
            this.RadioButtonCheckedBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.RadioButtonHoverBackColor = Color.FromArgb(236,244,255);
            this.RadioButtonHoverForeColor = Color.FromArgb(17,24,39);
            this.RadioButtonHoverBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.RadioButtonSelectedForeColor = Color.FromArgb(17,24,39);
            this.RadioButtonSelectedBackColor = Color.FromArgb(236,244,255);
        }
    }
}
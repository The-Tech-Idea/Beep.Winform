using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyRadioButton()
        {
            this.RadioButtonBackColor = Color.FromArgb(0,43,54);
            this.RadioButtonForeColor = Color.FromArgb(147,161,161);
            this.RadioButtonBorderColor = Color.FromArgb(88,110,117);
            this.RadioButtonCheckedBackColor = Color.FromArgb(0,43,54);
            this.RadioButtonCheckedForeColor = Color.FromArgb(147,161,161);
            this.RadioButtonCheckedBorderColor = Color.FromArgb(88,110,117);
            this.RadioButtonHoverBackColor = Color.FromArgb(0,43,54);
            this.RadioButtonHoverForeColor = Color.FromArgb(147,161,161);
            this.RadioButtonHoverBorderColor = Color.FromArgb(88,110,117);
            this.RadioButtonSelectedForeColor = Color.FromArgb(147,161,161);
            this.RadioButtonSelectedBackColor = Color.FromArgb(0,43,54);
        }
    }
}
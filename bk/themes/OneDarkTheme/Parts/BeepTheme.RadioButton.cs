using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class OneDarkTheme
    {
        private void ApplyRadioButton()
        {
            this.RadioButtonBackColor = Color.FromArgb(40,44,52);
            this.RadioButtonForeColor = Color.FromArgb(171,178,191);
            this.RadioButtonBorderColor = Color.FromArgb(92,99,112);
            this.RadioButtonCheckedBackColor = Color.FromArgb(40,44,52);
            this.RadioButtonCheckedForeColor = Color.FromArgb(171,178,191);
            this.RadioButtonCheckedBorderColor = Color.FromArgb(92,99,112);
            this.RadioButtonHoverBackColor = Color.FromArgb(40,44,52);
            this.RadioButtonHoverForeColor = Color.FromArgb(171,178,191);
            this.RadioButtonHoverBorderColor = Color.FromArgb(92,99,112);
            this.RadioButtonSelectedForeColor = Color.FromArgb(171,178,191);
            this.RadioButtonSelectedBackColor = Color.FromArgb(40,44,52);
        }
    }
}
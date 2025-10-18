using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyRadioButton()
        {
            this.RadioButtonBackColor = Color.FromArgb(245,246,247);
            this.RadioButtonForeColor = Color.FromArgb(43,45,48);
            this.RadioButtonBorderColor = Color.FromArgb(220,223,230);
            this.RadioButtonCheckedBackColor = Color.FromArgb(245,246,247);
            this.RadioButtonCheckedForeColor = Color.FromArgb(43,45,48);
            this.RadioButtonCheckedBorderColor = Color.FromArgb(220,223,230);
            this.RadioButtonHoverBackColor = Color.FromArgb(245,246,247);
            this.RadioButtonHoverForeColor = Color.FromArgb(43,45,48);
            this.RadioButtonHoverBorderColor = Color.FromArgb(220,223,230);
            this.RadioButtonSelectedForeColor = Color.FromArgb(43,45,48);
            this.RadioButtonSelectedBackColor = Color.FromArgb(245,246,247);
        }
    }
}
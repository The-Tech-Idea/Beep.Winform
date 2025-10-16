using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyRadioButton()
        {
            this.RadioButtonBackColor = Color.FromArgb(245,246,248);
            this.RadioButtonForeColor = Color.FromArgb(32,32,32);
            this.RadioButtonBorderColor = Color.FromArgb(218,223,230);
            this.RadioButtonCheckedBackColor = Color.FromArgb(245,246,248);
            this.RadioButtonCheckedForeColor = Color.FromArgb(32,32,32);
            this.RadioButtonCheckedBorderColor = Color.FromArgb(218,223,230);
            this.RadioButtonHoverBackColor = Color.FromArgb(245,246,248);
            this.RadioButtonHoverForeColor = Color.FromArgb(32,32,32);
            this.RadioButtonHoverBorderColor = Color.FromArgb(218,223,230);
            this.RadioButtonSelectedForeColor = Color.FromArgb(32,32,32);
            this.RadioButtonSelectedBackColor = Color.FromArgb(245,246,248);
        }
    }
}
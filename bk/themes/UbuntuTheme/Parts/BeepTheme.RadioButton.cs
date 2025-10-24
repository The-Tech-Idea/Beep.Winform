using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplyRadioButton()
        {
            this.RadioButtonBackColor = Color.FromArgb(242,242,245);
            this.RadioButtonForeColor = Color.FromArgb(44,44,44);
            this.RadioButtonBorderColor = Color.FromArgb(218,218,222);
            this.RadioButtonCheckedBackColor = Color.FromArgb(242,242,245);
            this.RadioButtonCheckedForeColor = Color.FromArgb(44,44,44);
            this.RadioButtonCheckedBorderColor = Color.FromArgb(218,218,222);
            this.RadioButtonHoverBackColor = Color.FromArgb(242,242,245);
            this.RadioButtonHoverForeColor = Color.FromArgb(44,44,44);
            this.RadioButtonHoverBorderColor = Color.FromArgb(218,218,222);
            this.RadioButtonSelectedForeColor = Color.FromArgb(44,44,44);
            this.RadioButtonSelectedBackColor = Color.FromArgb(242,242,245);
        }
    }
}
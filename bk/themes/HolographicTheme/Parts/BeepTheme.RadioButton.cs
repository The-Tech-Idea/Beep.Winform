using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplyRadioButton()
        {
            this.RadioButtonBackColor = Color.FromArgb(15,16,32);
            this.RadioButtonForeColor = Color.FromArgb(245,247,255);
            this.RadioButtonBorderColor = Color.FromArgb(74,79,123);
            this.RadioButtonCheckedBackColor = Color.FromArgb(15,16,32);
            this.RadioButtonCheckedForeColor = Color.FromArgb(245,247,255);
            this.RadioButtonCheckedBorderColor = Color.FromArgb(74,79,123);
            this.RadioButtonHoverBackColor = Color.FromArgb(15,16,32);
            this.RadioButtonHoverForeColor = Color.FromArgb(245,247,255);
            this.RadioButtonHoverBorderColor = Color.FromArgb(74,79,123);
            this.RadioButtonSelectedForeColor = Color.FromArgb(245,247,255);
            this.RadioButtonSelectedBackColor = Color.FromArgb(15,16,32);
        }
    }
}
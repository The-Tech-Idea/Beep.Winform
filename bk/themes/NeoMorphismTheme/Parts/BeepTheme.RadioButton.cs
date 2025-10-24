using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplyRadioButton()
        {
            this.RadioButtonBackColor = Color.FromArgb(236,240,243);
            this.RadioButtonForeColor = Color.FromArgb(58,66,86);
            this.RadioButtonBorderColor = Color.FromArgb(221,228,235);
            this.RadioButtonCheckedBackColor = Color.FromArgb(236,240,243);
            this.RadioButtonCheckedForeColor = Color.FromArgb(58,66,86);
            this.RadioButtonCheckedBorderColor = Color.FromArgb(221,228,235);
            this.RadioButtonHoverBackColor = Color.FromArgb(236,240,243);
            this.RadioButtonHoverForeColor = Color.FromArgb(58,66,86);
            this.RadioButtonHoverBorderColor = Color.FromArgb(221,228,235);
            this.RadioButtonSelectedForeColor = Color.FromArgb(58,66,86);
            this.RadioButtonSelectedBackColor = Color.FromArgb(236,240,243);
        }
    }
}
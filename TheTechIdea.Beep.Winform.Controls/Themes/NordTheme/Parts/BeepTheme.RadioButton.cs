using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordTheme
    {
        private void ApplyRadioButton()
        {
            this.RadioButtonBackColor = Color.FromArgb(46,52,64);
            this.RadioButtonForeColor = Color.FromArgb(216,222,233);
            this.RadioButtonBorderColor = Color.FromArgb(76,86,106);
            this.RadioButtonCheckedBackColor = Color.FromArgb(46,52,64);
            this.RadioButtonCheckedForeColor = Color.FromArgb(216,222,233);
            this.RadioButtonCheckedBorderColor = Color.FromArgb(76,86,106);
            this.RadioButtonHoverBackColor = Color.FromArgb(46,52,64);
            this.RadioButtonHoverForeColor = Color.FromArgb(216,222,233);
            this.RadioButtonHoverBorderColor = Color.FromArgb(76,86,106);
            this.RadioButtonSelectedForeColor = Color.FromArgb(216,222,233);
            this.RadioButtonSelectedBackColor = Color.FromArgb(46,52,64);
        }
    }
}
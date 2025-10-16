using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GNOMETheme
    {
        private void ApplyRadioButton()
        {
            this.RadioButtonBackColor = Color.FromArgb(246,245,244);
            this.RadioButtonForeColor = Color.FromArgb(46,52,54);
            this.RadioButtonBorderColor = Color.FromArgb(205,207,212);
            this.RadioButtonCheckedBackColor = Color.FromArgb(246,245,244);
            this.RadioButtonCheckedForeColor = Color.FromArgb(46,52,54);
            this.RadioButtonCheckedBorderColor = Color.FromArgb(205,207,212);
            this.RadioButtonHoverBackColor = Color.FromArgb(246,245,244);
            this.RadioButtonHoverForeColor = Color.FromArgb(46,52,54);
            this.RadioButtonHoverBorderColor = Color.FromArgb(205,207,212);
            this.RadioButtonSelectedForeColor = Color.FromArgb(46,52,54);
            this.RadioButtonSelectedBackColor = Color.FromArgb(246,245,244);
        }
    }
}
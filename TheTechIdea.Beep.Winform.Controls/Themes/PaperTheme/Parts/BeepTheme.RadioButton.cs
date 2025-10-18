using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class PaperTheme
    {
        private void ApplyRadioButton()
        {
            this.RadioButtonBackColor = Color.FromArgb(250,250,250);
            this.RadioButtonForeColor = Color.FromArgb(33,33,33);
            this.RadioButtonBorderColor = Color.FromArgb(224,224,224);
            this.RadioButtonCheckedBackColor = Color.FromArgb(250,250,250);
            this.RadioButtonCheckedForeColor = Color.FromArgb(33,33,33);
            this.RadioButtonCheckedBorderColor = Color.FromArgb(224,224,224);
            this.RadioButtonHoverBackColor = Color.FromArgb(250,250,250);
            this.RadioButtonHoverForeColor = Color.FromArgb(33,33,33);
            this.RadioButtonHoverBorderColor = Color.FromArgb(224,224,224);
            this.RadioButtonSelectedForeColor = Color.FromArgb(33,33,33);
            this.RadioButtonSelectedBackColor = Color.FromArgb(250,250,250);
        }
    }
}
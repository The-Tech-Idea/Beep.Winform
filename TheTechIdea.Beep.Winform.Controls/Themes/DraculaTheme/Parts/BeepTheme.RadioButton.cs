using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class DraculaTheme
    {
        private void ApplyRadioButton()
        {
            this.RadioButtonBackColor = Color.FromArgb(40,42,54);
            this.RadioButtonForeColor = Color.FromArgb(248,248,242);
            this.RadioButtonBorderColor = Color.FromArgb(98,114,164);
            this.RadioButtonCheckedBackColor = Color.FromArgb(40,42,54);
            this.RadioButtonCheckedForeColor = Color.FromArgb(248,248,242);
            this.RadioButtonCheckedBorderColor = Color.FromArgb(98,114,164);
            this.RadioButtonHoverBackColor = Color.FromArgb(40,42,54);
            this.RadioButtonHoverForeColor = Color.FromArgb(248,248,242);
            this.RadioButtonHoverBorderColor = Color.FromArgb(98,114,164);
            this.RadioButtonSelectedForeColor = Color.FromArgb(248,248,242);
            this.RadioButtonSelectedBackColor = Color.FromArgb(40,42,54);
        }
    }
}
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MacOSTheme
    {
        private void ApplyRadioButton()
        {
            this.RadioButtonBackColor = Color.FromArgb(250,250,252);
            this.RadioButtonForeColor = Color.FromArgb(28,28,30);
            this.RadioButtonBorderColor = Color.FromArgb(229,229,234);
            this.RadioButtonCheckedBackColor = Color.FromArgb(250,250,252);
            this.RadioButtonCheckedForeColor = Color.FromArgb(28,28,30);
            this.RadioButtonCheckedBorderColor = Color.FromArgb(229,229,234);
            this.RadioButtonHoverBackColor = Color.FromArgb(250,250,252);
            this.RadioButtonHoverForeColor = Color.FromArgb(28,28,30);
            this.RadioButtonHoverBorderColor = Color.FromArgb(229,229,234);
            this.RadioButtonSelectedForeColor = Color.FromArgb(28,28,30);
            this.RadioButtonSelectedBackColor = Color.FromArgb(250,250,252);
        }
    }
}
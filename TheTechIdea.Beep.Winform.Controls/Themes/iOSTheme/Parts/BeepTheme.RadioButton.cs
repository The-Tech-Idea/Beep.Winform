using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class iOSTheme
    {
        private void ApplyRadioButton()
        {
            this.RadioButtonBackColor = Color.FromArgb(242,242,247);
            this.RadioButtonForeColor = Color.FromArgb(28,28,30);
            this.RadioButtonBorderColor = Color.FromArgb(198,198,207);
            this.RadioButtonCheckedBackColor = Color.FromArgb(242,242,247);
            this.RadioButtonCheckedForeColor = Color.FromArgb(28,28,30);
            this.RadioButtonCheckedBorderColor = Color.FromArgb(198,198,207);
            this.RadioButtonHoverBackColor = Color.FromArgb(242,242,247);
            this.RadioButtonHoverForeColor = Color.FromArgb(28,28,30);
            this.RadioButtonHoverBorderColor = Color.FromArgb(198,198,207);
            this.RadioButtonSelectedForeColor = Color.FromArgb(28,28,30);
            this.RadioButtonSelectedBackColor = Color.FromArgb(242,242,247);
        }
    }
}
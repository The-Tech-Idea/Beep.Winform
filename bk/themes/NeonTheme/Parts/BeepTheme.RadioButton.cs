using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplyRadioButton()
        {
            this.RadioButtonBackColor = Color.FromArgb(10,12,20);
            this.RadioButtonForeColor = Color.FromArgb(235,245,255);
            this.RadioButtonBorderColor = Color.FromArgb(60,70,100);
            this.RadioButtonCheckedBackColor = Color.FromArgb(10,12,20);
            this.RadioButtonCheckedForeColor = Color.FromArgb(235,245,255);
            this.RadioButtonCheckedBorderColor = Color.FromArgb(60,70,100);
            this.RadioButtonHoverBackColor = Color.FromArgb(10,12,20);
            this.RadioButtonHoverForeColor = Color.FromArgb(235,245,255);
            this.RadioButtonHoverBorderColor = Color.FromArgb(60,70,100);
            this.RadioButtonSelectedForeColor = Color.FromArgb(235,245,255);
            this.RadioButtonSelectedBackColor = Color.FromArgb(10,12,20);
        }
    }
}
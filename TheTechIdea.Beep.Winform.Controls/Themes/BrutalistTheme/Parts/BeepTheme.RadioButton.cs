using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class BrutalistTheme
    {
        private void ApplyRadioButton()
        {
            this.RadioButtonBackColor = Color.FromArgb(250,250,250);
            this.RadioButtonForeColor = Color.FromArgb(20,20,20);
            this.RadioButtonBorderColor = Color.FromArgb(0,0,0);
            this.RadioButtonCheckedBackColor = Color.FromArgb(250,250,250);
            this.RadioButtonCheckedForeColor = Color.FromArgb(20,20,20);
            this.RadioButtonCheckedBorderColor = Color.FromArgb(0,0,0);
            this.RadioButtonHoverBackColor = Color.FromArgb(250,250,250);
            this.RadioButtonHoverForeColor = Color.FromArgb(20,20,20);
            this.RadioButtonHoverBorderColor = Color.FromArgb(0,0,0);
            this.RadioButtonSelectedForeColor = Color.FromArgb(20,20,20);
            this.RadioButtonSelectedBackColor = Color.FromArgb(250,250,250);
        }
    }
}
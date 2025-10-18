using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GruvBoxTheme
    {
        private void ApplyRadioButton()
        {
            this.RadioButtonBackColor = Color.FromArgb(40,40,40);
            this.RadioButtonForeColor = Color.FromArgb(235,219,178);
            this.RadioButtonBorderColor = Color.FromArgb(168,153,132);
            this.RadioButtonCheckedBackColor = Color.FromArgb(40,40,40);
            this.RadioButtonCheckedForeColor = Color.FromArgb(235,219,178);
            this.RadioButtonCheckedBorderColor = Color.FromArgb(168,153,132);
            this.RadioButtonHoverBackColor = Color.FromArgb(40,40,40);
            this.RadioButtonHoverForeColor = Color.FromArgb(235,219,178);
            this.RadioButtonHoverBorderColor = Color.FromArgb(168,153,132);
            this.RadioButtonSelectedForeColor = Color.FromArgb(235,219,178);
            this.RadioButtonSelectedBackColor = Color.FromArgb(40,40,40);
        }
    }
}
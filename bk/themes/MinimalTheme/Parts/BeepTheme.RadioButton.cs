using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyRadioButton()
        {
            this.RadioButtonBackColor = Color.FromArgb(255,255,255);
            this.RadioButtonForeColor = Color.FromArgb(31,41,55);
            this.RadioButtonBorderColor = Color.FromArgb(209,213,219);
            this.RadioButtonCheckedBackColor = Color.FromArgb(255,255,255);
            this.RadioButtonCheckedForeColor = Color.FromArgb(31,41,55);
            this.RadioButtonCheckedBorderColor = Color.FromArgb(209,213,219);
            this.RadioButtonHoverBackColor = Color.FromArgb(255,255,255);
            this.RadioButtonHoverForeColor = Color.FromArgb(31,41,55);
            this.RadioButtonHoverBorderColor = Color.FromArgb(209,213,219);
            this.RadioButtonSelectedForeColor = Color.FromArgb(31,41,55);
            this.RadioButtonSelectedBackColor = Color.FromArgb(255,255,255);
        }
    }
}
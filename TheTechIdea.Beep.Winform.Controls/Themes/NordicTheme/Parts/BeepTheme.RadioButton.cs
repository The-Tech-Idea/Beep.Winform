using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordicTheme
    {
        private void ApplyRadioButton()
        {
            this.RadioButtonBackColor = Color.FromArgb(250,250,251);
            this.RadioButtonForeColor = Color.FromArgb(31,41,55);
            this.RadioButtonBorderColor = Color.FromArgb(229,231,235);
            this.RadioButtonCheckedBackColor = Color.FromArgb(250,250,251);
            this.RadioButtonCheckedForeColor = Color.FromArgb(31,41,55);
            this.RadioButtonCheckedBorderColor = Color.FromArgb(229,231,235);
            this.RadioButtonHoverBackColor = Color.FromArgb(250,250,251);
            this.RadioButtonHoverForeColor = Color.FromArgb(31,41,55);
            this.RadioButtonHoverBorderColor = Color.FromArgb(229,231,235);
            this.RadioButtonSelectedForeColor = Color.FromArgb(31,41,55);
            this.RadioButtonSelectedBackColor = Color.FromArgb(250,250,251);
        }
    }
}
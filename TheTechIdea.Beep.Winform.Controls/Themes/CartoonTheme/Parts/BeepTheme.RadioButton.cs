using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplyRadioButton()
        {
            this.RadioButtonBackColor = Color.FromArgb(255,251,235);
            this.RadioButtonForeColor = Color.FromArgb(33,37,41);
            this.RadioButtonBorderColor = Color.FromArgb(247,208,136);
            this.RadioButtonCheckedBackColor = Color.FromArgb(255,251,235);
            this.RadioButtonCheckedForeColor = Color.FromArgb(33,37,41);
            this.RadioButtonCheckedBorderColor = Color.FromArgb(247,208,136);
            this.RadioButtonHoverBackColor = Color.FromArgb(255,251,235);
            this.RadioButtonHoverForeColor = Color.FromArgb(33,37,41);
            this.RadioButtonHoverBorderColor = Color.FromArgb(247,208,136);
            this.RadioButtonSelectedForeColor = Color.FromArgb(33,37,41);
            this.RadioButtonSelectedBackColor = Color.FromArgb(255,251,235);
        }
    }
}
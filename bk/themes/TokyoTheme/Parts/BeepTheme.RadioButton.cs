using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class TokyoTheme
    {
        private void ApplyRadioButton()
        {
            this.RadioButtonBackColor = Color.FromArgb(26,27,38);
            this.RadioButtonForeColor = Color.FromArgb(192,202,245);
            this.RadioButtonBorderColor = Color.FromArgb(86,95,137);
            this.RadioButtonCheckedBackColor = Color.FromArgb(26,27,38);
            this.RadioButtonCheckedForeColor = Color.FromArgb(192,202,245);
            this.RadioButtonCheckedBorderColor = Color.FromArgb(86,95,137);
            this.RadioButtonHoverBackColor = Color.FromArgb(26,27,38);
            this.RadioButtonHoverForeColor = Color.FromArgb(192,202,245);
            this.RadioButtonHoverBorderColor = Color.FromArgb(86,95,137);
            this.RadioButtonSelectedForeColor = Color.FromArgb(192,202,245);
            this.RadioButtonSelectedBackColor = Color.FromArgb(26,27,38);
        }
    }
}
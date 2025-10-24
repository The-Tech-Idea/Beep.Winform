using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class KDETheme
    {
        private void ApplyRadioButton()
        {
            this.RadioButtonBackColor = Color.FromArgb(248,249,250);
            this.RadioButtonForeColor = Color.FromArgb(33,37,41);
            this.RadioButtonBorderColor = Color.FromArgb(222,226,230);
            this.RadioButtonCheckedBackColor = Color.FromArgb(248,249,250);
            this.RadioButtonCheckedForeColor = Color.FromArgb(33,37,41);
            this.RadioButtonCheckedBorderColor = Color.FromArgb(222,226,230);
            this.RadioButtonHoverBackColor = Color.FromArgb(248,249,250);
            this.RadioButtonHoverForeColor = Color.FromArgb(33,37,41);
            this.RadioButtonHoverBorderColor = Color.FromArgb(222,226,230);
            this.RadioButtonSelectedForeColor = Color.FromArgb(33,37,41);
            this.RadioButtonSelectedBackColor = Color.FromArgb(248,249,250);
        }
    }
}
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
            this.RadioButtonBackColor = BackgroundColor;
            this.RadioButtonForeColor = ForeColor;
            this.RadioButtonBorderColor = BorderColor;
            this.RadioButtonCheckedBackColor = BackgroundColor;
            this.RadioButtonCheckedForeColor = ForeColor;
            this.RadioButtonCheckedBorderColor = BorderColor;
            this.RadioButtonHoverBackColor = BackgroundColor;
            this.RadioButtonHoverForeColor = ForeColor;
            this.RadioButtonHoverBorderColor = BorderColor;
            this.RadioButtonSelectedForeColor = ForeColor;
            this.RadioButtonSelectedBackColor = BackgroundColor;
        }
    }
}
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
            this.RadioButtonBackColor = PanelBackColor;
            this.RadioButtonForeColor = ForeColor;
            this.RadioButtonBorderColor = BorderColor;
            this.RadioButtonCheckedBackColor = PanelBackColor;
            this.RadioButtonCheckedForeColor = ForeColor;
            this.RadioButtonCheckedBorderColor = BorderColor;
            this.RadioButtonHoverBackColor = PanelBackColor;
            this.RadioButtonHoverForeColor = ForeColor;
            this.RadioButtonHoverBorderColor = BorderColor;
            this.RadioButtonSelectedForeColor = ForeColor;
            this.RadioButtonSelectedBackColor = PanelBackColor;
        }
    }
}
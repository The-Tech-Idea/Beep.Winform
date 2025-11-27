using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class Metro2Theme
    {
        private void ApplyRadioButton()
        {
            this.RadioButtonBackColor = PanelBackColor;
            this.RadioButtonForeColor = ForeColor;
            this.RadioButtonBorderColor = InactiveBorderColor;
            this.RadioButtonCheckedBackColor = PrimaryColor;
            this.RadioButtonCheckedForeColor = OnPrimaryColor;
            this.RadioButtonCheckedBorderColor = ActiveBorderColor;
            this.RadioButtonHoverBackColor = PanelGradiantMiddleColor;
            this.RadioButtonHoverForeColor = ForeColor;
            this.RadioButtonHoverBorderColor = InactiveBorderColor;
            this.RadioButtonSelectedForeColor = OnPrimaryColor;
            this.RadioButtonSelectedBackColor = PrimaryColor;
        }
    }
}
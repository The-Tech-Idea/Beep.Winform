using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class OneDarkTheme
    {
        private void ApplyRadioButton()
        {
            this.RadioButtonBackColor = PanelBackColor;
            this.RadioButtonForeColor = ForeColor;
            this.RadioButtonBorderColor = InactiveBorderColor;
            this.RadioButtonCheckedBackColor = PanelBackColor;
            this.RadioButtonCheckedForeColor = ForeColor;
            this.RadioButtonCheckedBorderColor = InactiveBorderColor;
            this.RadioButtonHoverBackColor = PanelGradiantMiddleColor;
            this.RadioButtonHoverForeColor = ForeColor;
            this.RadioButtonHoverBorderColor = InactiveBorderColor;
            this.RadioButtonSelectedForeColor = ForeColor;
            this.RadioButtonSelectedBackColor = PanelBackColor;
        }
    }
}
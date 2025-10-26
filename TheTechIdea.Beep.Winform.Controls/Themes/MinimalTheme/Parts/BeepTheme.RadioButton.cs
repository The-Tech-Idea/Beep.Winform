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
            this.RadioButtonBackColor = BackgroundColor;
            this.RadioButtonForeColor = ForeColor;
            this.RadioButtonBorderColor = BorderColor;
            this.RadioButtonCheckedBackColor = PrimaryColor;
            this.RadioButtonCheckedForeColor = OnPrimaryColor;
            this.RadioButtonCheckedBorderColor = ActiveBorderColor;
            this.RadioButtonHoverBackColor = SurfaceColor;
            this.RadioButtonHoverForeColor = ForeColor;
            this.RadioButtonHoverBorderColor = ActiveBorderColor;
            this.RadioButtonSelectedForeColor = ForeColor;
            this.RadioButtonSelectedBackColor = ThemeUtil.Lighten(SurfaceColor, 0.04);
        }
    }
}

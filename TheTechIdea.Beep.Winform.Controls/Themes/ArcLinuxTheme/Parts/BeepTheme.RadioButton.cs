using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyRadioButton()
        {
            this.RadioButtonBackColor = SurfaceColor;
            this.RadioButtonForeColor = ForeColor;
            this.RadioButtonBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.RadioButtonCheckedBackColor = SurfaceColor;
            this.RadioButtonCheckedForeColor = ForeColor;
            this.RadioButtonCheckedBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.RadioButtonHoverBackColor = SurfaceColor;
            this.RadioButtonHoverForeColor = ForeColor;
            this.RadioButtonHoverBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.RadioButtonSelectedForeColor = ForeColor;
            this.RadioButtonSelectedBackColor = SurfaceColor;
        }
    }
}

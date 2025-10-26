using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyCheckBox()
        {
            this.CheckBoxBackColor = SurfaceColor;
            this.CheckBoxForeColor = ForeColor;
            this.CheckBoxBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.2);
            this.CheckBoxCheckedBackColor = PrimaryColor;
            this.CheckBoxCheckedForeColor = OnPrimaryColor;
            this.CheckBoxCheckedBorderColor = ActiveBorderColor;
            this.CheckBoxHoverBackColor = ThemeUtil.Darken(SurfaceColor, 0.04);
            this.CheckBoxHoverForeColor = ForeColor;
            this.CheckBoxHoverBorderColor = ActiveBorderColor;
        }
    }
}

using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyButtons()
        {
            this.ButtonHoverBackColor = ThemeUtil.Lighten(SurfaceColor, 0.08);
            this.ButtonHoverForeColor = ForeColor;
            this.ButtonHoverBorderColor = ActiveBorderColor;
            this.ButtonSelectedBorderColor = ActiveBorderColor;
            this.ButtonSelectedBackColor = ThemeUtil.Lighten(SurfaceColor, 0.05);
            this.ButtonSelectedForeColor = ForeColor;
            this.ButtonSelectedHoverBackColor = ThemeUtil.Lighten(SurfaceColor, 0.08);
            this.ButtonSelectedHoverForeColor = ForeColor;
            this.ButtonSelectedHoverBorderColor = ActiveBorderColor;
            this.ButtonBackColor = SurfaceColor;
            this.ButtonForeColor = ForeColor;
            this.ButtonBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.2);
            this.ButtonErrorBackColor = ErrorColor;
            this.ButtonErrorForeColor = OnPrimaryColor;
            this.ButtonErrorBorderColor = ErrorColor;
            this.ButtonPressedBackColor = ThemeUtil.Darken(SurfaceColor, 0.06);
            this.ButtonPressedForeColor = ForeColor;
            this.ButtonPressedBorderColor = ActiveBorderColor;
        }
    }
}

using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplySwitch()
        {
            this.SwitchBackColor = SurfaceColor;
            this.SwitchBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.SwitchForeColor = ForeColor;
            this.SwitchSelectedBackColor = SurfaceColor;
            this.SwitchSelectedBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.SwitchSelectedForeColor = ForeColor;
            this.SwitchHoverBackColor = SurfaceColor;
            this.SwitchHoverBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.SwitchHoverForeColor = ForeColor;
        }
    }
}

using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplySideMenu()
        {
            this.SideMenuBackColor = SurfaceColor;
            this.SideMenuHoverBackColor = SurfaceColor;
            this.SideMenuSelectedBackColor = SurfaceColor;
            this.SideMenuForeColor = ForeColor;
            this.SideMenuSelectedForeColor = ForeColor;
            this.SideMenuHoverForeColor = ForeColor;
            this.SideMenuBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.SideMenuTitleTextColor = ForeColor;
            this.SideMenuTitleBackColor = SurfaceColor;
            this.SideMenuSubTitleTextColor = ForeColor;
            this.SideMenuSubTitleBackColor = SurfaceColor;
            this.SideMenuGradiantStartColor = SurfaceColor;
            this.SideMenuGradiantEndColor = SurfaceColor;
            this.SideMenuGradiantMiddleColor = SurfaceColor;
            this.SideMenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}

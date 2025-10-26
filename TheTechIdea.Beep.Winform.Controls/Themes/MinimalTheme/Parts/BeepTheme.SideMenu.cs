using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplySideMenu()
        {
            this.SideMenuBackColor = BackgroundColor;
            this.SideMenuHoverBackColor = SurfaceColor;
            this.SideMenuSelectedBackColor = ThemeUtil.Lighten(SurfaceColor, 0.05);
            this.SideMenuForeColor = ForeColor;
            this.SideMenuSelectedForeColor = ForeColor;
            this.SideMenuHoverForeColor = ForeColor;
            this.SideMenuBorderColor = BorderColor;
            this.SideMenuTitleTextColor = ForeColor;
            this.SideMenuTitleBackColor = BackgroundColor;
            this.SideMenuSubTitleTextColor = ForeColor;
            this.SideMenuSubTitleBackColor = BackgroundColor;
            this.SideMenuGradiantStartColor = BackgroundColor;
            this.SideMenuGradiantEndColor = BackgroundColor;
            this.SideMenuGradiantMiddleColor = BackgroundColor;
            this.SideMenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}

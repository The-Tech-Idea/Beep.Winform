using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplySideMenu()
        {
            this.SideMenuBackColor = SurfaceColor;
            this.SideMenuHoverBackColor = PanelGradiantStartColor;
            this.SideMenuSelectedBackColor = PrimaryColor;
                this.SideMenuForeColor = ForeColor;
                this.SideMenuSelectedForeColor = OnPrimaryColor;
                this.SideMenuHoverForeColor = ForeColor;
                this.SideMenuBorderColor = InactiveBorderColor;
                this.SideMenuTitleTextColor = ForeColor;
                this.SideMenuTitleBackColor = SurfaceColor;
                this.SideMenuSubTitleTextColor = AccentColor;
                this.SideMenuSubTitleBackColor = SurfaceColor;
                this.SideMenuGradiantStartColor = PanelGradiantStartColor;
                this.SideMenuGradiantEndColor = PanelGradiantEndColor;
                this.SideMenuGradiantMiddleColor = PanelGradiantMiddleColor;
            this.SideMenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}
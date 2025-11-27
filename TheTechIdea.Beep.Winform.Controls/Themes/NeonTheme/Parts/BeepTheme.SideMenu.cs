using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplySideMenu()
        {
            this.SideMenuBackColor = PanelGradiantMiddleColor;
            this.SideMenuHoverBackColor = PanelGradiantMiddleColor;
            this.SideMenuSelectedBackColor = PanelGradiantMiddleColor;
            this.SideMenuForeColor = ForeColor;
            this.SideMenuSelectedForeColor = ForeColor;
            this.SideMenuHoverForeColor = ForeColor;
            this.SideMenuBorderColor = InactiveBorderColor;
            this.SideMenuTitleTextColor = ForeColor;
            this.SideMenuTitleBackColor = PanelGradiantMiddleColor;
            this.SideMenuSubTitleTextColor = ForeColor;
            this.SideMenuSubTitleBackColor = PanelGradiantMiddleColor;
            this.SideMenuGradiantStartColor = PanelGradiantMiddleColor;
            this.SideMenuGradiantEndColor = PanelGradiantMiddleColor;
            this.SideMenuGradiantMiddleColor = PanelGradiantMiddleColor;
            this.SideMenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplySideMenu()
        {
            this.SideMenuBackColor = BackgroundColor;
            this.SideMenuHoverBackColor = PanelGradiantMiddleColor;
            this.SideMenuSelectedBackColor = PanelBackColor;
            this.SideMenuForeColor = ForeColor;
            this.SideMenuSelectedForeColor = ForeColor;
            this.SideMenuHoverForeColor = ForeColor;
            this.SideMenuBorderColor = BorderColor;
            this.SideMenuTitleTextColor = ForeColor;
            this.SideMenuTitleBackColor = BackgroundColor;
            this.SideMenuSubTitleTextColor = ForeColor;
            this.SideMenuSubTitleBackColor = BackgroundColor;
            this.SideMenuGradiantStartColor = PanelBackColor;
            this.SideMenuGradiantEndColor = PanelBackColor;
            this.SideMenuGradiantMiddleColor = PanelGradiantMiddleColor;
            this.SideMenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
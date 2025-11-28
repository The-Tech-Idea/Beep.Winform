using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme
    {
        private void ApplySideMenu()
        {
            this.SideMenuBackColor = PanelGradiantStartColor;
            this.SideMenuHoverBackColor = PanelGradiantMiddleColor;
            this.SideMenuSelectedBackColor = PanelGradiantMiddleColor;
            this.SideMenuForeColor = ForeColor;
            this.SideMenuSelectedForeColor = ForeColor;
            this.SideMenuHoverForeColor = ForeColor;
            this.SideMenuBorderColor = BorderColor;
            this.SideMenuTitleTextColor = ForeColor;
            this.SideMenuTitleBackColor = PanelGradiantStartColor;
            this.SideMenuSubTitleTextColor = ForeColor;
            this.SideMenuSubTitleBackColor = PanelGradiantStartColor;
            this.SideMenuGradiantStartColor = PanelGradiantStartColor;
            this.SideMenuGradiantEndColor = PanelGradiantEndColor;
            this.SideMenuGradiantMiddleColor = PanelGradiantMiddleColor;
            this.SideMenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
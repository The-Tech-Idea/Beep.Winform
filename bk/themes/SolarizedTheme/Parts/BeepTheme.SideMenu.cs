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
            this.SideMenuBackColor = Color.FromArgb(0,43,54);
            this.SideMenuHoverBackColor = Color.FromArgb(0,43,54);
            this.SideMenuSelectedBackColor = Color.FromArgb(0,43,54);
            this.SideMenuForeColor = Color.FromArgb(147,161,161);
            this.SideMenuSelectedForeColor = Color.FromArgb(147,161,161);
            this.SideMenuHoverForeColor = Color.FromArgb(147,161,161);
            this.SideMenuBorderColor = Color.FromArgb(88,110,117);
            this.SideMenuTitleTextColor = Color.FromArgb(147,161,161);
            this.SideMenuTitleBackColor = Color.FromArgb(0,43,54);
            this.SideMenuSubTitleTextColor = Color.FromArgb(147,161,161);
            this.SideMenuSubTitleBackColor = Color.FromArgb(0,43,54);
            this.SideMenuGradiantStartColor = Color.FromArgb(0,43,54);
            this.SideMenuGradiantEndColor = Color.FromArgb(0,43,54);
            this.SideMenuGradiantMiddleColor = Color.FromArgb(0,43,54);
            this.SideMenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
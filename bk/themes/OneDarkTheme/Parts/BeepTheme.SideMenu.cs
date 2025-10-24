using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class OneDarkTheme
    {
        private void ApplySideMenu()
        {
            this.SideMenuBackColor = Color.FromArgb(40,44,52);
            this.SideMenuHoverBackColor = Color.FromArgb(40,44,52);
            this.SideMenuSelectedBackColor = Color.FromArgb(40,44,52);
            this.SideMenuForeColor = Color.FromArgb(171,178,191);
            this.SideMenuSelectedForeColor = Color.FromArgb(171,178,191);
            this.SideMenuHoverForeColor = Color.FromArgb(171,178,191);
            this.SideMenuBorderColor = Color.FromArgb(92,99,112);
            this.SideMenuTitleTextColor = Color.FromArgb(171,178,191);
            this.SideMenuTitleBackColor = Color.FromArgb(40,44,52);
            this.SideMenuSubTitleTextColor = Color.FromArgb(171,178,191);
            this.SideMenuSubTitleBackColor = Color.FromArgb(40,44,52);
            this.SideMenuGradiantStartColor = Color.FromArgb(40,44,52);
            this.SideMenuGradiantEndColor = Color.FromArgb(40,44,52);
            this.SideMenuGradiantMiddleColor = Color.FromArgb(40,44,52);
            this.SideMenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
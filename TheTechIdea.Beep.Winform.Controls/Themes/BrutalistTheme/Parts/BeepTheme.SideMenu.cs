using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class BrutalistTheme
    {
        private void ApplySideMenu()
        {
            this.SideMenuBackColor = Color.FromArgb(250,250,250);
            this.SideMenuHoverBackColor = Color.FromArgb(250,250,250);
            this.SideMenuSelectedBackColor = Color.FromArgb(250,250,250);
            this.SideMenuForeColor = Color.FromArgb(20,20,20);
            this.SideMenuSelectedForeColor = Color.FromArgb(20,20,20);
            this.SideMenuHoverForeColor = Color.FromArgb(20,20,20);
            this.SideMenuBorderColor = Color.FromArgb(0,0,0);
            this.SideMenuTitleTextColor = Color.FromArgb(20,20,20);
            this.SideMenuTitleBackColor = Color.FromArgb(250,250,250);
            this.SideMenuSubTitleTextColor = Color.FromArgb(20,20,20);
            this.SideMenuSubTitleBackColor = Color.FromArgb(250,250,250);
            this.SideMenuGradiantStartColor = Color.FromArgb(250,250,250);
            this.SideMenuGradiantEndColor = Color.FromArgb(250,250,250);
            this.SideMenuGradiantMiddleColor = Color.FromArgb(250,250,250);
            this.SideMenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
        }
    }
}
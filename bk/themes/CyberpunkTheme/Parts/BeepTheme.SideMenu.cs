using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplySideMenu()
        {
            this.SideMenuBackColor = Color.FromArgb(10,8,20);
            this.SideMenuHoverBackColor = Color.FromArgb(10,8,20);
            this.SideMenuSelectedBackColor = Color.FromArgb(10,8,20);
            this.SideMenuForeColor = Color.FromArgb(228,244,255);
            this.SideMenuSelectedForeColor = Color.FromArgb(228,244,255);
            this.SideMenuHoverForeColor = Color.FromArgb(228,244,255);
            this.SideMenuBorderColor = Color.FromArgb(90,20,110);
            this.SideMenuTitleTextColor = Color.FromArgb(228,244,255);
            this.SideMenuTitleBackColor = Color.FromArgb(10,8,20);
            this.SideMenuSubTitleTextColor = Color.FromArgb(228,244,255);
            this.SideMenuSubTitleBackColor = Color.FromArgb(10,8,20);
            this.SideMenuGradiantStartColor = Color.FromArgb(10,8,20);
            this.SideMenuGradiantEndColor = Color.FromArgb(10,8,20);
            this.SideMenuGradiantMiddleColor = Color.FromArgb(10,8,20);
            this.SideMenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}
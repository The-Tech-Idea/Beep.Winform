using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GlassTheme
    {
        private void ApplySideMenu()
        {
            this.SideMenuBackColor = Color.FromArgb(236,244,255);
            this.SideMenuHoverBackColor = Color.FromArgb(236,244,255);
            this.SideMenuSelectedBackColor = Color.FromArgb(236,244,255);
            this.SideMenuForeColor = Color.FromArgb(17,24,39);
            this.SideMenuSelectedForeColor = Color.FromArgb(17,24,39);
            this.SideMenuHoverForeColor = Color.FromArgb(17,24,39);
            this.SideMenuBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.SideMenuTitleTextColor = Color.FromArgb(17,24,39);
            this.SideMenuTitleBackColor = Color.FromArgb(236,244,255);
            this.SideMenuSubTitleTextColor = Color.FromArgb(17,24,39);
            this.SideMenuSubTitleBackColor = Color.FromArgb(236,244,255);
            this.SideMenuGradiantStartColor = Color.FromArgb(236,244,255);
            this.SideMenuGradiantEndColor = Color.FromArgb(236,244,255);
            this.SideMenuGradiantMiddleColor = Color.FromArgb(236,244,255);
            this.SideMenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
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
            this.SideMenuBackColor = Color.FromArgb(255,255,255);
            this.SideMenuHoverBackColor = Color.FromArgb(255,255,255);
            this.SideMenuSelectedBackColor = Color.FromArgb(255,255,255);
            this.SideMenuForeColor = Color.FromArgb(31,41,55);
            this.SideMenuSelectedForeColor = Color.FromArgb(31,41,55);
            this.SideMenuHoverForeColor = Color.FromArgb(31,41,55);
            this.SideMenuBorderColor = Color.FromArgb(209,213,219);
            this.SideMenuTitleTextColor = Color.FromArgb(31,41,55);
            this.SideMenuTitleBackColor = Color.FromArgb(255,255,255);
            this.SideMenuSubTitleTextColor = Color.FromArgb(31,41,55);
            this.SideMenuSubTitleBackColor = Color.FromArgb(255,255,255);
            this.SideMenuGradiantStartColor = Color.FromArgb(255,255,255);
            this.SideMenuGradiantEndColor = Color.FromArgb(255,255,255);
            this.SideMenuGradiantMiddleColor = Color.FromArgb(255,255,255);
            this.SideMenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
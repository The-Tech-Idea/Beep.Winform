using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplySideMenu()
        {
            this.SideMenuBackColor = Color.FromArgb(255,251,235);
            this.SideMenuHoverBackColor = Color.FromArgb(255,251,235);
            this.SideMenuSelectedBackColor = Color.FromArgb(255,251,235);
            this.SideMenuForeColor = Color.FromArgb(33,37,41);
            this.SideMenuSelectedForeColor = Color.FromArgb(33,37,41);
            this.SideMenuHoverForeColor = Color.FromArgb(33,37,41);
            this.SideMenuBorderColor = Color.FromArgb(247,208,136);
            this.SideMenuTitleTextColor = Color.FromArgb(33,37,41);
            this.SideMenuTitleBackColor = Color.FromArgb(255,251,235);
            this.SideMenuSubTitleTextColor = Color.FromArgb(33,37,41);
            this.SideMenuSubTitleBackColor = Color.FromArgb(255,251,235);
            this.SideMenuGradiantStartColor = Color.FromArgb(255,251,235);
            this.SideMenuGradiantEndColor = Color.FromArgb(255,251,235);
            this.SideMenuGradiantMiddleColor = Color.FromArgb(255,251,235);
            this.SideMenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}
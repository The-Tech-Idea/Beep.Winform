using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplySideMenu()
        {
            this.SideMenuBackColor = Color.FromArgb(242,242,245);
            this.SideMenuHoverBackColor = Color.FromArgb(242,242,245);
            this.SideMenuSelectedBackColor = Color.FromArgb(242,242,245);
            this.SideMenuForeColor = Color.FromArgb(44,44,44);
            this.SideMenuSelectedForeColor = Color.FromArgb(44,44,44);
            this.SideMenuHoverForeColor = Color.FromArgb(44,44,44);
            this.SideMenuBorderColor = Color.FromArgb(218,218,222);
            this.SideMenuTitleTextColor = Color.FromArgb(44,44,44);
            this.SideMenuTitleBackColor = Color.FromArgb(242,242,245);
            this.SideMenuSubTitleTextColor = Color.FromArgb(44,44,44);
            this.SideMenuSubTitleBackColor = Color.FromArgb(242,242,245);
            this.SideMenuGradiantStartColor = Color.FromArgb(242,242,245);
            this.SideMenuGradiantEndColor = Color.FromArgb(242,242,245);
            this.SideMenuGradiantMiddleColor = Color.FromArgb(242,242,245);
            this.SideMenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
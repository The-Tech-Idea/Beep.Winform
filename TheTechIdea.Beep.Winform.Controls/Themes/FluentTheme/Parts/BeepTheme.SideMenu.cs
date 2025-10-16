using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplySideMenu()
        {
            this.SideMenuBackColor = Color.FromArgb(245,246,248);
            this.SideMenuHoverBackColor = Color.FromArgb(245,246,248);
            this.SideMenuSelectedBackColor = Color.FromArgb(245,246,248);
            this.SideMenuForeColor = Color.FromArgb(32,32,32);
            this.SideMenuSelectedForeColor = Color.FromArgb(32,32,32);
            this.SideMenuHoverForeColor = Color.FromArgb(32,32,32);
            this.SideMenuBorderColor = Color.FromArgb(218,223,230);
            this.SideMenuTitleTextColor = Color.FromArgb(32,32,32);
            this.SideMenuTitleBackColor = Color.FromArgb(245,246,248);
            this.SideMenuSubTitleTextColor = Color.FromArgb(32,32,32);
            this.SideMenuSubTitleBackColor = Color.FromArgb(245,246,248);
            this.SideMenuGradiantStartColor = Color.FromArgb(245,246,248);
            this.SideMenuGradiantEndColor = Color.FromArgb(245,246,248);
            this.SideMenuGradiantMiddleColor = Color.FromArgb(245,246,248);
            this.SideMenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
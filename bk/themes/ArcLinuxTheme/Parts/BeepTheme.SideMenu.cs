using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplySideMenu()
        {
            this.SideMenuBackColor = Color.FromArgb(245,246,247);
            this.SideMenuHoverBackColor = Color.FromArgb(245,246,247);
            this.SideMenuSelectedBackColor = Color.FromArgb(245,246,247);
            this.SideMenuForeColor = Color.FromArgb(43,45,48);
            this.SideMenuSelectedForeColor = Color.FromArgb(43,45,48);
            this.SideMenuHoverForeColor = Color.FromArgb(43,45,48);
            this.SideMenuBorderColor = Color.FromArgb(220,223,230);
            this.SideMenuTitleTextColor = Color.FromArgb(43,45,48);
            this.SideMenuTitleBackColor = Color.FromArgb(245,246,247);
            this.SideMenuSubTitleTextColor = Color.FromArgb(43,45,48);
            this.SideMenuSubTitleBackColor = Color.FromArgb(245,246,247);
            this.SideMenuGradiantStartColor = Color.FromArgb(245,246,247);
            this.SideMenuGradiantEndColor = Color.FromArgb(245,246,247);
            this.SideMenuGradiantMiddleColor = Color.FromArgb(245,246,247);
            this.SideMenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
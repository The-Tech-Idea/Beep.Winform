using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MacOSTheme
    {
        private void ApplySideMenu()
        {
            this.SideMenuBackColor = Color.FromArgb(250,250,252);
            this.SideMenuHoverBackColor = Color.FromArgb(250,250,252);
            this.SideMenuSelectedBackColor = Color.FromArgb(250,250,252);
            this.SideMenuForeColor = Color.FromArgb(28,28,30);
            this.SideMenuSelectedForeColor = Color.FromArgb(28,28,30);
            this.SideMenuHoverForeColor = Color.FromArgb(28,28,30);
            this.SideMenuBorderColor = Color.FromArgb(229,229,234);
            this.SideMenuTitleTextColor = Color.FromArgb(28,28,30);
            this.SideMenuTitleBackColor = Color.FromArgb(250,250,252);
            this.SideMenuSubTitleTextColor = Color.FromArgb(28,28,30);
            this.SideMenuSubTitleBackColor = Color.FromArgb(250,250,252);
            this.SideMenuGradiantStartColor = Color.FromArgb(250,250,252);
            this.SideMenuGradiantEndColor = Color.FromArgb(250,250,252);
            this.SideMenuGradiantMiddleColor = Color.FromArgb(250,250,252);
            this.SideMenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
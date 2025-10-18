using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class iOSTheme
    {
        private void ApplySideMenu()
        {
            this.SideMenuBackColor = Color.FromArgb(242,242,247);
            this.SideMenuHoverBackColor = Color.FromArgb(242,242,247);
            this.SideMenuSelectedBackColor = Color.FromArgb(242,242,247);
            this.SideMenuForeColor = Color.FromArgb(28,28,30);
            this.SideMenuSelectedForeColor = Color.FromArgb(28,28,30);
            this.SideMenuHoverForeColor = Color.FromArgb(28,28,30);
            this.SideMenuBorderColor = Color.FromArgb(198,198,207);
            this.SideMenuTitleTextColor = Color.FromArgb(28,28,30);
            this.SideMenuTitleBackColor = Color.FromArgb(242,242,247);
            this.SideMenuSubTitleTextColor = Color.FromArgb(28,28,30);
            this.SideMenuSubTitleBackColor = Color.FromArgb(242,242,247);
            this.SideMenuGradiantStartColor = Color.FromArgb(242,242,247);
            this.SideMenuGradiantEndColor = Color.FromArgb(242,242,247);
            this.SideMenuGradiantMiddleColor = Color.FromArgb(242,242,247);
            this.SideMenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
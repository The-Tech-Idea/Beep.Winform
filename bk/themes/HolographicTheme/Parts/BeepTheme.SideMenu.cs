using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplySideMenu()
        {
            this.SideMenuBackColor = Color.FromArgb(15,16,32);
            this.SideMenuHoverBackColor = Color.FromArgb(15,16,32);
            this.SideMenuSelectedBackColor = Color.FromArgb(15,16,32);
            this.SideMenuForeColor = Color.FromArgb(245,247,255);
            this.SideMenuSelectedForeColor = Color.FromArgb(245,247,255);
            this.SideMenuHoverForeColor = Color.FromArgb(245,247,255);
            this.SideMenuBorderColor = Color.FromArgb(74,79,123);
            this.SideMenuTitleTextColor = Color.FromArgb(245,247,255);
            this.SideMenuTitleBackColor = Color.FromArgb(15,16,32);
            this.SideMenuSubTitleTextColor = Color.FromArgb(245,247,255);
            this.SideMenuSubTitleBackColor = Color.FromArgb(15,16,32);
            this.SideMenuGradiantStartColor = Color.FromArgb(15,16,32);
            this.SideMenuGradiantEndColor = Color.FromArgb(15,16,32);
            this.SideMenuGradiantMiddleColor = Color.FromArgb(15,16,32);
            this.SideMenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}
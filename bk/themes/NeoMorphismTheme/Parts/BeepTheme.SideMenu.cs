using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplySideMenu()
        {
            this.SideMenuBackColor = Color.FromArgb(236,240,243);
            this.SideMenuHoverBackColor = Color.FromArgb(236,240,243);
            this.SideMenuSelectedBackColor = Color.FromArgb(236,240,243);
            this.SideMenuForeColor = Color.FromArgb(58,66,86);
            this.SideMenuSelectedForeColor = Color.FromArgb(58,66,86);
            this.SideMenuHoverForeColor = Color.FromArgb(58,66,86);
            this.SideMenuBorderColor = Color.FromArgb(221,228,235);
            this.SideMenuTitleTextColor = Color.FromArgb(58,66,86);
            this.SideMenuTitleBackColor = Color.FromArgb(236,240,243);
            this.SideMenuSubTitleTextColor = Color.FromArgb(58,66,86);
            this.SideMenuSubTitleBackColor = Color.FromArgb(236,240,243);
            this.SideMenuGradiantStartColor = Color.FromArgb(236,240,243);
            this.SideMenuGradiantEndColor = Color.FromArgb(236,240,243);
            this.SideMenuGradiantMiddleColor = Color.FromArgb(236,240,243);
            this.SideMenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class TokyoTheme
    {
        private void ApplySideMenu()
        {
            this.SideMenuBackColor = Color.FromArgb(26,27,38);
            this.SideMenuHoverBackColor = Color.FromArgb(26,27,38);
            this.SideMenuSelectedBackColor = Color.FromArgb(26,27,38);
            this.SideMenuForeColor = Color.FromArgb(192,202,245);
            this.SideMenuSelectedForeColor = Color.FromArgb(192,202,245);
            this.SideMenuHoverForeColor = Color.FromArgb(192,202,245);
            this.SideMenuBorderColor = Color.FromArgb(86,95,137);
            this.SideMenuTitleTextColor = Color.FromArgb(192,202,245);
            this.SideMenuTitleBackColor = Color.FromArgb(26,27,38);
            this.SideMenuSubTitleTextColor = Color.FromArgb(192,202,245);
            this.SideMenuSubTitleBackColor = Color.FromArgb(26,27,38);
            this.SideMenuGradiantStartColor = Color.FromArgb(26,27,38);
            this.SideMenuGradiantEndColor = Color.FromArgb(26,27,38);
            this.SideMenuGradiantMiddleColor = Color.FromArgb(26,27,38);
            this.SideMenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
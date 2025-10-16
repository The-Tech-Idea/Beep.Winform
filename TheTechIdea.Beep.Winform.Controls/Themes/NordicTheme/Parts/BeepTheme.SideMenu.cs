using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordicTheme
    {
        private void ApplySideMenu()
        {
            this.SideMenuBackColor = Color.FromArgb(250,250,251);
            this.SideMenuHoverBackColor = Color.FromArgb(250,250,251);
            this.SideMenuSelectedBackColor = Color.FromArgb(250,250,251);
            this.SideMenuForeColor = Color.FromArgb(31,41,55);
            this.SideMenuSelectedForeColor = Color.FromArgb(31,41,55);
            this.SideMenuHoverForeColor = Color.FromArgb(31,41,55);
            this.SideMenuBorderColor = Color.FromArgb(229,231,235);
            this.SideMenuTitleTextColor = Color.FromArgb(31,41,55);
            this.SideMenuTitleBackColor = Color.FromArgb(250,250,251);
            this.SideMenuSubTitleTextColor = Color.FromArgb(31,41,55);
            this.SideMenuSubTitleBackColor = Color.FromArgb(250,250,251);
            this.SideMenuGradiantStartColor = Color.FromArgb(250,250,251);
            this.SideMenuGradiantEndColor = Color.FromArgb(250,250,251);
            this.SideMenuGradiantMiddleColor = Color.FromArgb(250,250,251);
            this.SideMenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
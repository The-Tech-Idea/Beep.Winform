using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyLogin()
        {
            this.LoginPopoverBackgroundColor = Color.FromArgb(0,43,54);
            this.LoginTitleColor = Color.FromArgb(0,43,54);
            this.LoginSubtitleColor = Color.FromArgb(0,43,54);
            this.LoginDescriptionColor = Color.FromArgb(0,43,54);
            this.LoginLinkColor = Color.FromArgb(0,43,54);
            this.LoginButtonBackgroundColor = Color.FromArgb(0,43,54);
            this.LoginButtonTextColor = Color.FromArgb(147,161,161);
            this.LoginDropdownBackgroundColor = Color.FromArgb(0,43,54);
            this.LoginDropdownTextColor = Color.FromArgb(147,161,161);
            this.LoginLogoBackgroundColor = Color.FromArgb(0,43,54);
        }
    }
}
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class OneDarkTheme
    {
        private void ApplyLogin()
        {
            this.LoginPopoverBackgroundColor = Color.FromArgb(40,44,52);
            this.LoginTitleColor = Color.FromArgb(40,44,52);
            this.LoginSubtitleColor = Color.FromArgb(40,44,52);
            this.LoginDescriptionColor = Color.FromArgb(40,44,52);
            this.LoginLinkColor = Color.FromArgb(40,44,52);
            this.LoginButtonBackgroundColor = Color.FromArgb(40,44,52);
            this.LoginButtonTextColor = Color.FromArgb(171,178,191);
            this.LoginDropdownBackgroundColor = Color.FromArgb(40,44,52);
            this.LoginDropdownTextColor = Color.FromArgb(171,178,191);
            this.LoginLogoBackgroundColor = Color.FromArgb(40,44,52);
        }
    }
}
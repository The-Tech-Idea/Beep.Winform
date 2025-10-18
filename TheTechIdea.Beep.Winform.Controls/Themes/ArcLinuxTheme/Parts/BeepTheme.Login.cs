using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyLogin()
        {
            this.LoginPopoverBackgroundColor = Color.FromArgb(245,246,247);
            this.LoginTitleColor = Color.FromArgb(245,246,247);
            this.LoginSubtitleColor = Color.FromArgb(245,246,247);
            this.LoginDescriptionColor = Color.FromArgb(245,246,247);
            this.LoginLinkColor = Color.FromArgb(245,246,247);
            this.LoginButtonBackgroundColor = Color.FromArgb(245,246,247);
            this.LoginButtonTextColor = Color.FromArgb(43,45,48);
            this.LoginDropdownBackgroundColor = Color.FromArgb(245,246,247);
            this.LoginDropdownTextColor = Color.FromArgb(43,45,48);
            this.LoginLogoBackgroundColor = Color.FromArgb(245,246,247);
        }
    }
}
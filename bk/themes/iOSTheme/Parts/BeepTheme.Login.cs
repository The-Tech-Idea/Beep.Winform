using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class iOSTheme
    {
        private void ApplyLogin()
        {
            this.LoginPopoverBackgroundColor = Color.FromArgb(242,242,247);
            this.LoginTitleColor = Color.FromArgb(242,242,247);
            this.LoginSubtitleColor = Color.FromArgb(242,242,247);
            this.LoginDescriptionColor = Color.FromArgb(242,242,247);
            this.LoginLinkColor = Color.FromArgb(242,242,247);
            this.LoginButtonBackgroundColor = Color.FromArgb(242,242,247);
            this.LoginButtonTextColor = Color.FromArgb(28,28,30);
            this.LoginDropdownBackgroundColor = Color.FromArgb(242,242,247);
            this.LoginDropdownTextColor = Color.FromArgb(28,28,30);
            this.LoginLogoBackgroundColor = Color.FromArgb(242,242,247);
        }
    }
}
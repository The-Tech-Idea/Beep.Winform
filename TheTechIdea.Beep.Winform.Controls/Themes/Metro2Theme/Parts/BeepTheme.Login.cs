using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class Metro2Theme
    {
        private void ApplyLogin()
        {
            this.LoginPopoverBackgroundColor = Color.FromArgb(243,242,241);
            this.LoginTitleColor = Color.FromArgb(243,242,241);
            this.LoginSubtitleColor = Color.FromArgb(243,242,241);
            this.LoginDescriptionColor = Color.FromArgb(243,242,241);
            this.LoginLinkColor = Color.FromArgb(243,242,241);
            this.LoginButtonBackgroundColor = Color.FromArgb(243,242,241);
            this.LoginButtonTextColor = Color.FromArgb(32,31,30);
            this.LoginDropdownBackgroundColor = Color.FromArgb(243,242,241);
            this.LoginDropdownTextColor = Color.FromArgb(32,31,30);
            this.LoginLogoBackgroundColor = Color.FromArgb(243,242,241);
        }
    }
}
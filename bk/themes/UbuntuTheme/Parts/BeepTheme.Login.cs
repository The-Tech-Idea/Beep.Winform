using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplyLogin()
        {
            this.LoginPopoverBackgroundColor = Color.FromArgb(242,242,245);
            this.LoginTitleColor = Color.FromArgb(242,242,245);
            this.LoginSubtitleColor = Color.FromArgb(242,242,245);
            this.LoginDescriptionColor = Color.FromArgb(242,242,245);
            this.LoginLinkColor = Color.FromArgb(242,242,245);
            this.LoginButtonBackgroundColor = Color.FromArgb(242,242,245);
            this.LoginButtonTextColor = Color.FromArgb(44,44,44);
            this.LoginDropdownBackgroundColor = Color.FromArgb(242,242,245);
            this.LoginDropdownTextColor = Color.FromArgb(44,44,44);
            this.LoginLogoBackgroundColor = Color.FromArgb(242,242,245);
        }
    }
}
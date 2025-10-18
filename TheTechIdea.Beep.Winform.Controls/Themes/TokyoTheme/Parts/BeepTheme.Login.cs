using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class TokyoTheme
    {
        private void ApplyLogin()
        {
            this.LoginPopoverBackgroundColor = Color.FromArgb(26,27,38);
            this.LoginTitleColor = Color.FromArgb(26,27,38);
            this.LoginSubtitleColor = Color.FromArgb(26,27,38);
            this.LoginDescriptionColor = Color.FromArgb(26,27,38);
            this.LoginLinkColor = Color.FromArgb(26,27,38);
            this.LoginButtonBackgroundColor = Color.FromArgb(26,27,38);
            this.LoginButtonTextColor = Color.FromArgb(192,202,245);
            this.LoginDropdownBackgroundColor = Color.FromArgb(26,27,38);
            this.LoginDropdownTextColor = Color.FromArgb(192,202,245);
            this.LoginLogoBackgroundColor = Color.FromArgb(26,27,38);
        }
    }
}
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GruvBoxTheme
    {
        private void ApplyLogin()
        {
            this.LoginPopoverBackgroundColor = Color.FromArgb(40,40,40);
            this.LoginTitleColor = Color.FromArgb(40,40,40);
            this.LoginSubtitleColor = Color.FromArgb(40,40,40);
            this.LoginDescriptionColor = Color.FromArgb(40,40,40);
            this.LoginLinkColor = Color.FromArgb(40,40,40);
            this.LoginButtonBackgroundColor = Color.FromArgb(40,40,40);
            this.LoginButtonTextColor = Color.FromArgb(235,219,178);
            this.LoginDropdownBackgroundColor = Color.FromArgb(40,40,40);
            this.LoginDropdownTextColor = Color.FromArgb(235,219,178);
            this.LoginLogoBackgroundColor = Color.FromArgb(40,40,40);
        }
    }
}
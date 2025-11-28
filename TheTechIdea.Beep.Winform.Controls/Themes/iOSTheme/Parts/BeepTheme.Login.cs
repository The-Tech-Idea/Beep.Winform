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
            this.LoginPopoverBackgroundColor = BackgroundColor;
            this.LoginTitleColor = BackgroundColor;
            this.LoginSubtitleColor = BackgroundColor;
            this.LoginDescriptionColor = BackgroundColor;
            this.LoginLinkColor = PrimaryColor;
            this.LoginButtonBackgroundColor = BackgroundColor;
            this.LoginButtonTextColor = ForeColor;
            this.LoginDropdownBackgroundColor = BackgroundColor;
            this.LoginDropdownTextColor = ForeColor;
            this.LoginLogoBackgroundColor = BackgroundColor;
        }
    }
}
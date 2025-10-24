using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class BrutalistTheme
    {
        private void ApplyLogin()
        {
            this.LoginPopoverBackgroundColor = Color.FromArgb(250,250,250);
            this.LoginTitleColor = Color.FromArgb(250,250,250);
            this.LoginSubtitleColor = Color.FromArgb(250,250,250);
            this.LoginDescriptionColor = Color.FromArgb(250,250,250);
            this.LoginLinkColor = Color.FromArgb(250,250,250);
            this.LoginButtonBackgroundColor = Color.FromArgb(250,250,250);
            this.LoginButtonTextColor = Color.FromArgb(20,20,20);
            this.LoginDropdownBackgroundColor = Color.FromArgb(250,250,250);
            this.LoginDropdownTextColor = Color.FromArgb(20,20,20);
            this.LoginLogoBackgroundColor = Color.FromArgb(250,250,250);
        }
    }
}
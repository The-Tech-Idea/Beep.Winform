using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordicTheme
    {
        private void ApplyLogin()
        {
            this.LoginPopoverBackgroundColor = Color.FromArgb(250,250,251);
            this.LoginTitleColor = Color.FromArgb(250,250,251);
            this.LoginSubtitleColor = Color.FromArgb(250,250,251);
            this.LoginDescriptionColor = Color.FromArgb(250,250,251);
            this.LoginLinkColor = Color.FromArgb(250,250,251);
            this.LoginButtonBackgroundColor = Color.FromArgb(250,250,251);
            this.LoginButtonTextColor = Color.FromArgb(31,41,55);
            this.LoginDropdownBackgroundColor = Color.FromArgb(250,250,251);
            this.LoginDropdownTextColor = Color.FromArgb(31,41,55);
            this.LoginLogoBackgroundColor = Color.FromArgb(250,250,251);
        }
    }
}
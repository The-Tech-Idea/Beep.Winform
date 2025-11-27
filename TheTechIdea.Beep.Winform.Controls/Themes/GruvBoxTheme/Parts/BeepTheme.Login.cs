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
            this.LoginPopoverBackgroundColor = SurfaceColor;
            this.LoginTitleColor = ForeColor;
            this.LoginSubtitleColor = ForeColor;
            this.LoginDescriptionColor = ForeColor;
            this.LoginLinkColor = AccentColor;
            this.LoginButtonBackgroundColor = SurfaceColor;
            this.LoginButtonTextColor = ForeColor;
            this.LoginDropdownBackgroundColor = SurfaceColor;
            this.LoginDropdownTextColor = ForeColor;
            this.LoginLogoBackgroundColor = SurfaceColor;
        }
    }
}
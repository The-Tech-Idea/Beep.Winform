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
            this.LoginPopoverBackgroundColor = SurfaceColor;
            this.LoginTitleColor = SurfaceColor;
            this.LoginSubtitleColor = SurfaceColor;
            this.LoginDescriptionColor = SurfaceColor;
            this.LoginLinkColor = SurfaceColor;
            this.LoginButtonBackgroundColor = SurfaceColor;
            this.LoginButtonTextColor = ForeColor;
            this.LoginDropdownBackgroundColor = SurfaceColor;
            this.LoginDropdownTextColor = ForeColor;
            this.LoginLogoBackgroundColor = SurfaceColor;
        }
    }
}

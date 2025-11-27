using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyLogin()
        {
            this.LoginPopoverBackgroundColor = BackgroundColor;
            this.LoginTitleColor = ForeColor;
            this.LoginSubtitleColor = ForeColor;
            this.LoginDescriptionColor = InactiveBorderColor;
            this.LoginLinkColor = PrimaryColor;
            this.LoginButtonBackgroundColor = PrimaryColor;
            this.LoginButtonTextColor = OnPrimaryColor;
            this.LoginDropdownBackgroundColor = SurfaceColor;
            this.LoginDropdownTextColor = ForeColor;
            this.LoginLogoBackgroundColor = PanelBackColor;
        }
    }
}
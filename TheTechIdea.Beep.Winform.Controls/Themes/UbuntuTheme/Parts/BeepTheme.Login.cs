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
            this.LoginPopoverBackgroundColor = PanelBackColor;
            this.LoginTitleColor = ForeColor;
            this.LoginSubtitleColor = ForeColor;
            this.LoginDescriptionColor = ForeColor;
            this.LoginLinkColor = PrimaryColor;
            this.LoginButtonBackgroundColor = PanelBackColor;
            this.LoginButtonTextColor = ForeColor;
            this.LoginDropdownBackgroundColor = PanelBackColor;
            this.LoginDropdownTextColor = ForeColor;
            this.LoginLogoBackgroundColor = PanelBackColor;
        }
    }
}
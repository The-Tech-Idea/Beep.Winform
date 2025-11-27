using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplyLogin()
        {
            this.LoginPopoverBackgroundColor = PanelGradiantMiddleColor;
            this.LoginTitleColor = PanelGradiantMiddleColor;
            this.LoginSubtitleColor = PanelGradiantMiddleColor;
            this.LoginDescriptionColor = PanelGradiantMiddleColor;
            this.LoginLinkColor = PanelGradiantMiddleColor;
            this.LoginButtonBackgroundColor = PanelGradiantMiddleColor;
            this.LoginButtonTextColor = ForeColor;
            this.LoginDropdownBackgroundColor = PanelGradiantMiddleColor;
            this.LoginDropdownTextColor = ForeColor;
            this.LoginLogoBackgroundColor = PanelGradiantMiddleColor;
        }
    }
}
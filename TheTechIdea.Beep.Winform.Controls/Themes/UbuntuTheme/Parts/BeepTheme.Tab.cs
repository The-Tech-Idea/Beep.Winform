using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplyTab()
        {
            this.TabBackColor = SurfaceColor;
            this.TabForeColor = ForeColor;
            this.TabBorderColor = BorderColor;
            this.TabHoverBackColor = SecondaryColor;
            this.TabHoverForeColor = ForeColor;
            this.TabSelectedBackColor = PrimaryColor;
            this.TabSelectedForeColor = OnPrimaryColor;
            this.TabSelectedBorderColor = PrimaryColor;
            this.TabHoverBorderColor = ActiveBorderColor;
        }
    }
}
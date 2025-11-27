using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplyStatusBar()
        {
            this.StatusBarBackColor = SurfaceColor;
            this.StatusBarForeColor = ForeColor;
            this.StatusBarBorderColor = BorderColor;
            this.StatusBarHoverBackColor = SecondaryColor;
            this.StatusBarHoverForeColor = ForeColor;
            this.StatusBarHoverBorderColor = ActiveBorderColor;
        }
    }
}
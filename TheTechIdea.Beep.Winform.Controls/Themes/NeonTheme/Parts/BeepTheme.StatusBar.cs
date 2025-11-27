using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplyStatusBar()
        {
            this.StatusBarBackColor = PanelGradiantMiddleColor;
            this.StatusBarForeColor = ForeColor;
            this.StatusBarBorderColor = InactiveBorderColor;
            this.StatusBarHoverBackColor = PanelGradiantMiddleColor;
            this.StatusBarHoverForeColor = ForeColor;
            this.StatusBarHoverBorderColor = InactiveBorderColor;
        }
    }
}
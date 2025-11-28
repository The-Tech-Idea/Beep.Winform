using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class KDETheme
    {
        private void ApplyStatusBar()
        {
            this.StatusBarBackColor = PanelBackColor;
            this.StatusBarForeColor = ForeColor;
            this.StatusBarBorderColor = BorderColor;
            this.StatusBarHoverBackColor = PanelBackColor;
            this.StatusBarHoverForeColor = ForeColor;
            this.StatusBarHoverBorderColor = BorderColor;
        }
    }
}
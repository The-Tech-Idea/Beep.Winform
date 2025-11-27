using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyTab()
        {
            this.TabBackColor = BackgroundColor;
            this.TabForeColor = ForeColor;
            this.TabBorderColor = SecondaryColor;
            this.TabHoverBackColor = PanelGradiantMiddleColor;
            this.TabHoverForeColor = ForeColor;
            this.TabSelectedBackColor = PanelBackColor;
            this.TabSelectedForeColor = ForeColor;
            this.TabSelectedBorderColor = SecondaryColor;
            this.TabHoverBorderColor = SecondaryColor;
        }
    }
}
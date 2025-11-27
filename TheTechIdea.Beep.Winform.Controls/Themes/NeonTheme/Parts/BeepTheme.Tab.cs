using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplyTab()
        {
            this.TabBackColor = PanelGradiantMiddleColor;
            this.TabForeColor = ForeColor;
            this.TabBorderColor = InactiveBorderColor;
            this.TabHoverBackColor = PanelGradiantMiddleColor;
            this.TabHoverForeColor = ForeColor;
            this.TabSelectedBackColor = PanelGradiantMiddleColor;
            this.TabSelectedForeColor = ForeColor;
            this.TabSelectedBorderColor = InactiveBorderColor;
            this.TabHoverBorderColor = InactiveBorderColor;
        }
    }
}
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class OneDarkTheme
    {
        private void ApplyTab()
        {
            this.TabBackColor = PanelBackColor;
            this.TabForeColor = ForeColor;
            this.TabBorderColor = BorderColor;
            this.TabHoverBackColor = PanelGradiantMiddleColor;
            this.TabHoverForeColor = ForeColor;
            this.TabSelectedBackColor = PanelGradiantMiddleColor;
            this.TabSelectedForeColor = ForeColor;
            this.TabSelectedBorderColor = ActiveBorderColor;
            this.TabHoverBorderColor = ActiveBorderColor;
        }
    }
}
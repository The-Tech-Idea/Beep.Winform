using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class Metro2Theme
    {
        private void ApplyTab()
        {
            this.TabBackColor = PanelBackColor;
            this.TabForeColor = ForeColor;
            this.TabBorderColor = InactiveBorderColor;
            this.TabHoverBackColor = PanelGradiantMiddleColor;
            this.TabHoverForeColor = ForeColor;
            this.TabSelectedBackColor = PrimaryColor;
            this.TabSelectedForeColor = OnPrimaryColor;
            this.TabSelectedBorderColor = ActiveBorderColor;
            this.TabHoverBorderColor = InactiveBorderColor;
        }
    }
}
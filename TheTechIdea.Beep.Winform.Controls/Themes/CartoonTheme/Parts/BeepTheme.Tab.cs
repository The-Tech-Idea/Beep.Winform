using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplyTab()
        {
            this.TabBackColor = SurfaceColor;
            this.TabForeColor = ForeColor;
            this.TabBorderColor = InactiveBorderColor;
            this.TabHoverBackColor = PanelGradiantStartColor;
            this.TabHoverForeColor = ForeColor;
            this.TabSelectedBackColor = PrimaryColor;
            this.TabSelectedForeColor = OnPrimaryColor;
            this.TabSelectedBorderColor = PrimaryColor;
            this.TabHoverBorderColor = ActiveBorderColor;
        }
    }
}
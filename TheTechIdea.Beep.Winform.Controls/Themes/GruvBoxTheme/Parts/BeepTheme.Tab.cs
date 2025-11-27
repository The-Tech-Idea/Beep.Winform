using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GruvBoxTheme
    {
        private void ApplyTab()
        {
            this.TabBackColor = SurfaceColor;
            this.TabForeColor = ForeColor;
            this.TabBorderColor = BorderColor;
            this.TabHoverBackColor = SecondaryColor;
            this.TabHoverForeColor = ForeColor;
            this.TabSelectedBackColor = SecondaryColor;
            this.TabSelectedForeColor = ForeColor;
            this.TabSelectedBorderColor = BorderColor;
            this.TabHoverBorderColor = BorderColor;
        }
    }
}
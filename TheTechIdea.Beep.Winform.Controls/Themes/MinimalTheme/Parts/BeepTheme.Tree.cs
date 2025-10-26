using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyTree()
        {
            this.TreeBackColor = BackgroundColor;
            this.TreeForeColor = ForeColor;
            this.TreeBorderColor = BorderColor;
            this.TreeNodeForeColor = ForeColor;
            this.TreeNodeHoverForeColor = ForeColor;
            this.TreeNodeHoverBackColor = SurfaceColor;
            this.TreeNodeSelectedForeColor = ForeColor;
            this.TreeNodeSelectedBackColor = ThemeUtil.Lighten(SurfaceColor, 0.04);
            this.TreeNodeCheckedBoxForeColor = OnPrimaryColor;
            this.TreeNodeCheckedBoxBackColor = PrimaryColor;
        }
    }
}

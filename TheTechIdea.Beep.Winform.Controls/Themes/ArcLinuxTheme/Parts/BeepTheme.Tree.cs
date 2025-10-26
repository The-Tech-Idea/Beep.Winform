using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyTree()
        {
            this.TreeBackColor = SurfaceColor;
            this.TreeForeColor = ForeColor;
            this.TreeBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.TreeNodeForeColor = ForeColor;
            this.TreeNodeHoverForeColor = ForeColor;
            this.TreeNodeHoverBackColor = SurfaceColor;
            this.TreeNodeSelectedForeColor = ForeColor;
            this.TreeNodeSelectedBackColor = SurfaceColor;
            this.TreeNodeCheckedBoxForeColor = ForeColor;
            this.TreeNodeCheckedBoxBackColor = SurfaceColor;
        }
    }
}

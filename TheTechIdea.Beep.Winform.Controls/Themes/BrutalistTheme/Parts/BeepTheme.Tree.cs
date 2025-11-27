using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class BrutalistTheme
    {
        private void ApplyTree()
        {
            this.TreeBackColor = SurfaceColor;
            this.TreeForeColor = ForeColor;
            this.TreeBorderColor = BorderColor;
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
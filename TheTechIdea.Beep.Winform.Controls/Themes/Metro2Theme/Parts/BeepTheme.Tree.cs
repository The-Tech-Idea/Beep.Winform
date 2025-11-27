using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class Metro2Theme
    {
        private void ApplyTree()
        {
            this.TreeBackColor = PanelBackColor;
            this.TreeForeColor = ForeColor;
            this.TreeBorderColor = InactiveBorderColor;
            this.TreeNodeForeColor = ForeColor;
            this.TreeNodeHoverForeColor = ForeColor;
            this.TreeNodeHoverBackColor = PanelGradiantMiddleColor;
            this.TreeNodeSelectedForeColor = OnPrimaryColor;
            this.TreeNodeSelectedBackColor = PrimaryColor;
            this.TreeNodeCheckedBoxForeColor = ForeColor;
            this.TreeNodeCheckedBoxBackColor = PanelBackColor;
        }
    }
}
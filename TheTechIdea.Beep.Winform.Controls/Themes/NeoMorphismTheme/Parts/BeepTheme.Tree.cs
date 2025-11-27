using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplyTree()
        {
            this.TreeBackColor = PanelBackColor;
            this.TreeForeColor = ForeColor;
            this.TreeBorderColor = BorderColor;
            this.TreeNodeForeColor = ForeColor;
            this.TreeNodeHoverForeColor = ForeColor;
            this.TreeNodeHoverBackColor = PanelGradiantMiddleColor;
            this.TreeNodeSelectedForeColor = ForeColor;
            this.TreeNodeSelectedBackColor = PanelGradiantMiddleColor;
            this.TreeNodeCheckedBoxForeColor = ForeColor;
            this.TreeNodeCheckedBoxBackColor = SecondaryColor;
        }
    }
}
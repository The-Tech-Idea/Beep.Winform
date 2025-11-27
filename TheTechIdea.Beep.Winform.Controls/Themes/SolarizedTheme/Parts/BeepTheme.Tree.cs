using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyTree()
        {
            this.TreeBackColor = BackgroundColor;
            this.TreeForeColor = ForeColor;
            this.TreeBorderColor = BorderColor;
            this.TreeNodeForeColor = ForeColor;
            this.TreeNodeHoverForeColor = ForeColor;
            this.TreeNodeHoverBackColor = PanelGradiantMiddleColor;
            this.TreeNodeSelectedForeColor = ForeColor;
            this.TreeNodeSelectedBackColor = PrimaryColor;
            this.TreeNodeCheckedBoxForeColor = ForeColor;
            this.TreeNodeCheckedBoxBackColor = PrimaryColor;
        }
    }
}
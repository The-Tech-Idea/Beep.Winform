using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplyTree()
        {
            this.TreeBackColor = Color.FromArgb(15,16,32);
            this.TreeForeColor = Color.FromArgb(245,247,255);
            this.TreeBorderColor = Color.FromArgb(74,79,123);
            this.TreeNodeForeColor = Color.FromArgb(245,247,255);
            this.TreeNodeHoverForeColor = Color.FromArgb(245,247,255);
            this.TreeNodeHoverBackColor = Color.FromArgb(15,16,32);
            this.TreeNodeSelectedForeColor = Color.FromArgb(245,247,255);
            this.TreeNodeSelectedBackColor = Color.FromArgb(15,16,32);
            this.TreeNodeCheckedBoxForeColor = Color.FromArgb(245,247,255);
            this.TreeNodeCheckedBoxBackColor = Color.FromArgb(15,16,32);
        }
    }
}
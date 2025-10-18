using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplyTree()
        {
            this.TreeBackColor = Color.FromArgb(10,12,20);
            this.TreeForeColor = Color.FromArgb(235,245,255);
            this.TreeBorderColor = Color.FromArgb(60,70,100);
            this.TreeNodeForeColor = Color.FromArgb(235,245,255);
            this.TreeNodeHoverForeColor = Color.FromArgb(235,245,255);
            this.TreeNodeHoverBackColor = Color.FromArgb(10,12,20);
            this.TreeNodeSelectedForeColor = Color.FromArgb(235,245,255);
            this.TreeNodeSelectedBackColor = Color.FromArgb(10,12,20);
            this.TreeNodeCheckedBoxForeColor = Color.FromArgb(235,245,255);
            this.TreeNodeCheckedBoxBackColor = Color.FromArgb(10,12,20);
        }
    }
}
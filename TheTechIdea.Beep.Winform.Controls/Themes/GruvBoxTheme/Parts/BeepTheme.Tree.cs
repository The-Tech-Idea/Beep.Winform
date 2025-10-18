using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GruvBoxTheme
    {
        private void ApplyTree()
        {
            this.TreeBackColor = Color.FromArgb(40,40,40);
            this.TreeForeColor = Color.FromArgb(235,219,178);
            this.TreeBorderColor = Color.FromArgb(168,153,132);
            this.TreeNodeForeColor = Color.FromArgb(235,219,178);
            this.TreeNodeHoverForeColor = Color.FromArgb(235,219,178);
            this.TreeNodeHoverBackColor = Color.FromArgb(40,40,40);
            this.TreeNodeSelectedForeColor = Color.FromArgb(235,219,178);
            this.TreeNodeSelectedBackColor = Color.FromArgb(40,40,40);
            this.TreeNodeCheckedBoxForeColor = Color.FromArgb(235,219,178);
            this.TreeNodeCheckedBoxBackColor = Color.FromArgb(40,40,40);
        }
    }
}
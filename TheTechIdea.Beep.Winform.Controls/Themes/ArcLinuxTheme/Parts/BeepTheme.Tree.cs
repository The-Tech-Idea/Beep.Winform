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
            this.TreeBackColor = Color.FromArgb(245,246,247);
            this.TreeForeColor = Color.FromArgb(43,45,48);
            this.TreeBorderColor = Color.FromArgb(220,223,230);
            this.TreeNodeForeColor = Color.FromArgb(43,45,48);
            this.TreeNodeHoverForeColor = Color.FromArgb(43,45,48);
            this.TreeNodeHoverBackColor = Color.FromArgb(245,246,247);
            this.TreeNodeSelectedForeColor = Color.FromArgb(43,45,48);
            this.TreeNodeSelectedBackColor = Color.FromArgb(245,246,247);
            this.TreeNodeCheckedBoxForeColor = Color.FromArgb(43,45,48);
            this.TreeNodeCheckedBoxBackColor = Color.FromArgb(245,246,247);
        }
    }
}
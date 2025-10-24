using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyTree()
        {
            this.TreeBackColor = Color.FromArgb(245,246,248);
            this.TreeForeColor = Color.FromArgb(32,32,32);
            this.TreeBorderColor = Color.FromArgb(218,223,230);
            this.TreeNodeForeColor = Color.FromArgb(32,32,32);
            this.TreeNodeHoverForeColor = Color.FromArgb(32,32,32);
            this.TreeNodeHoverBackColor = Color.FromArgb(245,246,248);
            this.TreeNodeSelectedForeColor = Color.FromArgb(32,32,32);
            this.TreeNodeSelectedBackColor = Color.FromArgb(245,246,248);
            this.TreeNodeCheckedBoxForeColor = Color.FromArgb(32,32,32);
            this.TreeNodeCheckedBoxBackColor = Color.FromArgb(245,246,248);
        }
    }
}
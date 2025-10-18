using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class OneDarkTheme
    {
        private void ApplyTree()
        {
            this.TreeBackColor = Color.FromArgb(40,44,52);
            this.TreeForeColor = Color.FromArgb(171,178,191);
            this.TreeBorderColor = Color.FromArgb(92,99,112);
            this.TreeNodeForeColor = Color.FromArgb(171,178,191);
            this.TreeNodeHoverForeColor = Color.FromArgb(171,178,191);
            this.TreeNodeHoverBackColor = Color.FromArgb(40,44,52);
            this.TreeNodeSelectedForeColor = Color.FromArgb(171,178,191);
            this.TreeNodeSelectedBackColor = Color.FromArgb(40,44,52);
            this.TreeNodeCheckedBoxForeColor = Color.FromArgb(171,178,191);
            this.TreeNodeCheckedBoxBackColor = Color.FromArgb(40,44,52);
        }
    }
}
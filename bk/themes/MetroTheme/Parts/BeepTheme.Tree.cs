using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MetroTheme
    {
        private void ApplyTree()
        {
            this.TreeBackColor = Color.FromArgb(243,242,241);
            this.TreeForeColor = Color.FromArgb(32,31,30);
            this.TreeBorderColor = Color.FromArgb(225,225,225);
            this.TreeNodeForeColor = Color.FromArgb(32,31,30);
            this.TreeNodeHoverForeColor = Color.FromArgb(32,31,30);
            this.TreeNodeHoverBackColor = Color.FromArgb(243,242,241);
            this.TreeNodeSelectedForeColor = Color.FromArgb(32,31,30);
            this.TreeNodeSelectedBackColor = Color.FromArgb(243,242,241);
            this.TreeNodeCheckedBoxForeColor = Color.FromArgb(32,31,30);
            this.TreeNodeCheckedBoxBackColor = Color.FromArgb(243,242,241);
        }
    }
}
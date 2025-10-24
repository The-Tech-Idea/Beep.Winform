using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplyTree()
        {
            this.TreeBackColor = Color.FromArgb(242,242,245);
            this.TreeForeColor = Color.FromArgb(44,44,44);
            this.TreeBorderColor = Color.FromArgb(218,218,222);
            this.TreeNodeForeColor = Color.FromArgb(44,44,44);
            this.TreeNodeHoverForeColor = Color.FromArgb(44,44,44);
            this.TreeNodeHoverBackColor = Color.FromArgb(242,242,245);
            this.TreeNodeSelectedForeColor = Color.FromArgb(44,44,44);
            this.TreeNodeSelectedBackColor = Color.FromArgb(242,242,245);
            this.TreeNodeCheckedBoxForeColor = Color.FromArgb(44,44,44);
            this.TreeNodeCheckedBoxBackColor = Color.FromArgb(242,242,245);
        }
    }
}
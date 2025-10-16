using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class BrutalistTheme
    {
        private void ApplyTree()
        {
            this.TreeBackColor = Color.FromArgb(250,250,250);
            this.TreeForeColor = Color.FromArgb(20,20,20);
            this.TreeBorderColor = Color.FromArgb(0,0,0);
            this.TreeNodeForeColor = Color.FromArgb(20,20,20);
            this.TreeNodeHoverForeColor = Color.FromArgb(20,20,20);
            this.TreeNodeHoverBackColor = Color.FromArgb(250,250,250);
            this.TreeNodeSelectedForeColor = Color.FromArgb(20,20,20);
            this.TreeNodeSelectedBackColor = Color.FromArgb(250,250,250);
            this.TreeNodeCheckedBoxForeColor = Color.FromArgb(20,20,20);
            this.TreeNodeCheckedBoxBackColor = Color.FromArgb(250,250,250);
        }
    }
}
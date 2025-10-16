using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyTree()
        {
            this.TreeBackColor = Color.FromArgb(10,8,20);
            this.TreeForeColor = Color.FromArgb(228,244,255);
            this.TreeBorderColor = Color.FromArgb(90,20,110);
            this.TreeNodeForeColor = Color.FromArgb(228,244,255);
            this.TreeNodeHoverForeColor = Color.FromArgb(228,244,255);
            this.TreeNodeHoverBackColor = Color.FromArgb(10,8,20);
            this.TreeNodeSelectedForeColor = Color.FromArgb(228,244,255);
            this.TreeNodeSelectedBackColor = Color.FromArgb(10,8,20);
            this.TreeNodeCheckedBoxForeColor = Color.FromArgb(228,244,255);
            this.TreeNodeCheckedBoxBackColor = Color.FromArgb(10,8,20);
        }
    }
}
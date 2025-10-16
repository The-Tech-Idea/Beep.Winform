using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GlassTheme
    {
        private void ApplyTree()
        {
            this.TreeBackColor = Color.FromArgb(236,244,255);
            this.TreeForeColor = Color.FromArgb(17,24,39);
            this.TreeBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.TreeNodeForeColor = Color.FromArgb(17,24,39);
            this.TreeNodeHoverForeColor = Color.FromArgb(17,24,39);
            this.TreeNodeHoverBackColor = Color.FromArgb(236,244,255);
            this.TreeNodeSelectedForeColor = Color.FromArgb(17,24,39);
            this.TreeNodeSelectedBackColor = Color.FromArgb(236,244,255);
            this.TreeNodeCheckedBoxForeColor = Color.FromArgb(17,24,39);
            this.TreeNodeCheckedBoxBackColor = Color.FromArgb(236,244,255);
        }
    }
}
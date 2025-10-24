using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyTree()
        {
            this.TreeBackColor = Color.FromArgb(255,255,255);
            this.TreeForeColor = Color.FromArgb(31,41,55);
            this.TreeBorderColor = Color.FromArgb(209,213,219);
            this.TreeNodeForeColor = Color.FromArgb(31,41,55);
            this.TreeNodeHoverForeColor = Color.FromArgb(31,41,55);
            this.TreeNodeHoverBackColor = Color.FromArgb(255,255,255);
            this.TreeNodeSelectedForeColor = Color.FromArgb(31,41,55);
            this.TreeNodeSelectedBackColor = Color.FromArgb(255,255,255);
            this.TreeNodeCheckedBoxForeColor = Color.FromArgb(31,41,55);
            this.TreeNodeCheckedBoxBackColor = Color.FromArgb(255,255,255);
        }
    }
}
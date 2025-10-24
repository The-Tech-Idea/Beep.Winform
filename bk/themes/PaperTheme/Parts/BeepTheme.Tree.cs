using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class PaperTheme
    {
        private void ApplyTree()
        {
            this.TreeBackColor = Color.FromArgb(250,250,250);
            this.TreeForeColor = Color.FromArgb(33,33,33);
            this.TreeBorderColor = Color.FromArgb(224,224,224);
            this.TreeNodeForeColor = Color.FromArgb(33,33,33);
            this.TreeNodeHoverForeColor = Color.FromArgb(33,33,33);
            this.TreeNodeHoverBackColor = Color.FromArgb(250,250,250);
            this.TreeNodeSelectedForeColor = Color.FromArgb(33,33,33);
            this.TreeNodeSelectedBackColor = Color.FromArgb(250,250,250);
            this.TreeNodeCheckedBoxForeColor = Color.FromArgb(33,33,33);
            this.TreeNodeCheckedBoxBackColor = Color.FromArgb(250,250,250);
        }
    }
}
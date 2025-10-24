using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class DraculaTheme
    {
        private void ApplyTree()
        {
            this.TreeBackColor = Color.FromArgb(40,42,54);
            this.TreeForeColor = Color.FromArgb(248,248,242);
            this.TreeBorderColor = Color.FromArgb(98,114,164);
            this.TreeNodeForeColor = Color.FromArgb(248,248,242);
            this.TreeNodeHoverForeColor = Color.FromArgb(248,248,242);
            this.TreeNodeHoverBackColor = Color.FromArgb(40,42,54);
            this.TreeNodeSelectedForeColor = Color.FromArgb(248,248,242);
            this.TreeNodeSelectedBackColor = Color.FromArgb(40,42,54);
            this.TreeNodeCheckedBoxForeColor = Color.FromArgb(248,248,242);
            this.TreeNodeCheckedBoxBackColor = Color.FromArgb(40,42,54);
        }
    }
}
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplyTree()
        {
            this.TreeBackColor = Color.FromArgb(236,240,243);
            this.TreeForeColor = Color.FromArgb(58,66,86);
            this.TreeBorderColor = Color.FromArgb(221,228,235);
            this.TreeNodeForeColor = Color.FromArgb(58,66,86);
            this.TreeNodeHoverForeColor = Color.FromArgb(58,66,86);
            this.TreeNodeHoverBackColor = Color.FromArgb(236,240,243);
            this.TreeNodeSelectedForeColor = Color.FromArgb(58,66,86);
            this.TreeNodeSelectedBackColor = Color.FromArgb(236,240,243);
            this.TreeNodeCheckedBoxForeColor = Color.FromArgb(58,66,86);
            this.TreeNodeCheckedBoxBackColor = Color.FromArgb(236,240,243);
        }
    }
}
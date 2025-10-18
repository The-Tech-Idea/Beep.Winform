using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordTheme
    {
        private void ApplyTree()
        {
            this.TreeBackColor = Color.FromArgb(46,52,64);
            this.TreeForeColor = Color.FromArgb(216,222,233);
            this.TreeBorderColor = Color.FromArgb(76,86,106);
            this.TreeNodeForeColor = Color.FromArgb(216,222,233);
            this.TreeNodeHoverForeColor = Color.FromArgb(216,222,233);
            this.TreeNodeHoverBackColor = Color.FromArgb(46,52,64);
            this.TreeNodeSelectedForeColor = Color.FromArgb(216,222,233);
            this.TreeNodeSelectedBackColor = Color.FromArgb(46,52,64);
            this.TreeNodeCheckedBoxForeColor = Color.FromArgb(216,222,233);
            this.TreeNodeCheckedBoxBackColor = Color.FromArgb(46,52,64);
        }
    }
}
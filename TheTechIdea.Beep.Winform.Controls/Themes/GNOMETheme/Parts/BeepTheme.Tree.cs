using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GNOMETheme
    {
        private void ApplyTree()
        {
            this.TreeBackColor = Color.FromArgb(246,245,244);
            this.TreeForeColor = Color.FromArgb(46,52,54);
            this.TreeBorderColor = Color.FromArgb(205,207,212);
            this.TreeNodeForeColor = Color.FromArgb(46,52,54);
            this.TreeNodeHoverForeColor = Color.FromArgb(46,52,54);
            this.TreeNodeHoverBackColor = Color.FromArgb(246,245,244);
            this.TreeNodeSelectedForeColor = Color.FromArgb(46,52,54);
            this.TreeNodeSelectedBackColor = Color.FromArgb(246,245,244);
            this.TreeNodeCheckedBoxForeColor = Color.FromArgb(46,52,54);
            this.TreeNodeCheckedBoxBackColor = Color.FromArgb(246,245,244);
        }
    }
}
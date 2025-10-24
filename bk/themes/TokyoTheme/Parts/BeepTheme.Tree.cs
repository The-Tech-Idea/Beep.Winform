using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class TokyoTheme
    {
        private void ApplyTree()
        {
            this.TreeBackColor = Color.FromArgb(26,27,38);
            this.TreeForeColor = Color.FromArgb(192,202,245);
            this.TreeBorderColor = Color.FromArgb(86,95,137);
            this.TreeNodeForeColor = Color.FromArgb(192,202,245);
            this.TreeNodeHoverForeColor = Color.FromArgb(192,202,245);
            this.TreeNodeHoverBackColor = Color.FromArgb(26,27,38);
            this.TreeNodeSelectedForeColor = Color.FromArgb(192,202,245);
            this.TreeNodeSelectedBackColor = Color.FromArgb(26,27,38);
            this.TreeNodeCheckedBoxForeColor = Color.FromArgb(192,202,245);
            this.TreeNodeCheckedBoxBackColor = Color.FromArgb(26,27,38);
        }
    }
}
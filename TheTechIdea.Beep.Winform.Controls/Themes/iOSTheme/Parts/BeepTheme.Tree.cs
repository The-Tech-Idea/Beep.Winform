using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class iOSTheme
    {
        private void ApplyTree()
        {
            this.TreeBackColor = Color.FromArgb(242,242,247);
            this.TreeForeColor = Color.FromArgb(28,28,30);
            this.TreeBorderColor = Color.FromArgb(198,198,207);
            this.TreeNodeForeColor = Color.FromArgb(28,28,30);
            this.TreeNodeHoverForeColor = Color.FromArgb(28,28,30);
            this.TreeNodeHoverBackColor = Color.FromArgb(242,242,247);
            this.TreeNodeSelectedForeColor = Color.FromArgb(28,28,30);
            this.TreeNodeSelectedBackColor = Color.FromArgb(242,242,247);
            this.TreeNodeCheckedBoxForeColor = Color.FromArgb(28,28,30);
            this.TreeNodeCheckedBoxBackColor = Color.FromArgb(242,242,247);
        }
    }
}
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme
    {
        private void ApplyTree()
        {
            this.TreeBackColor = Color.FromArgb(245,248,255);
            this.TreeForeColor = Color.FromArgb(24,28,35);
            this.TreeBorderColor = Color.FromArgb(210,220,235);
            this.TreeNodeForeColor = Color.FromArgb(24,28,35);
            this.TreeNodeHoverForeColor = Color.FromArgb(24,28,35);
            this.TreeNodeHoverBackColor = Color.FromArgb(245,248,255);
            this.TreeNodeSelectedForeColor = Color.FromArgb(24,28,35);
            this.TreeNodeSelectedBackColor = Color.FromArgb(245,248,255);
            this.TreeNodeCheckedBoxForeColor = Color.FromArgb(24,28,35);
            this.TreeNodeCheckedBoxBackColor = Color.FromArgb(245,248,255);
        }
    }
}
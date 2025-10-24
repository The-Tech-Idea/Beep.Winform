using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class KDETheme
    {
        private void ApplyTree()
        {
            this.TreeBackColor = Color.FromArgb(248,249,250);
            this.TreeForeColor = Color.FromArgb(33,37,41);
            this.TreeBorderColor = Color.FromArgb(222,226,230);
            this.TreeNodeForeColor = Color.FromArgb(33,37,41);
            this.TreeNodeHoverForeColor = Color.FromArgb(33,37,41);
            this.TreeNodeHoverBackColor = Color.FromArgb(248,249,250);
            this.TreeNodeSelectedForeColor = Color.FromArgb(33,37,41);
            this.TreeNodeSelectedBackColor = Color.FromArgb(248,249,250);
            this.TreeNodeCheckedBoxForeColor = Color.FromArgb(33,37,41);
            this.TreeNodeCheckedBoxBackColor = Color.FromArgb(248,249,250);
        }
    }
}
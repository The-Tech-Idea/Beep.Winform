using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyTree()
        {
            this.TreeBackColor = Color.FromArgb(0,43,54);
            this.TreeForeColor = Color.FromArgb(147,161,161);
            this.TreeBorderColor = Color.FromArgb(88,110,117);
            this.TreeNodeForeColor = Color.FromArgb(147,161,161);
            this.TreeNodeHoverForeColor = Color.FromArgb(147,161,161);
            this.TreeNodeHoverBackColor = Color.FromArgb(0,43,54);
            this.TreeNodeSelectedForeColor = Color.FromArgb(147,161,161);
            this.TreeNodeSelectedBackColor = Color.FromArgb(0,43,54);
            this.TreeNodeCheckedBoxForeColor = Color.FromArgb(147,161,161);
            this.TreeNodeCheckedBoxBackColor = Color.FromArgb(0,43,54);
        }
    }
}
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordicTheme
    {
        private void ApplyTree()
        {
            this.TreeBackColor = Color.FromArgb(250,250,251);
            this.TreeForeColor = Color.FromArgb(31,41,55);
            this.TreeBorderColor = Color.FromArgb(229,231,235);
            this.TreeNodeForeColor = Color.FromArgb(31,41,55);
            this.TreeNodeHoverForeColor = Color.FromArgb(31,41,55);
            this.TreeNodeHoverBackColor = Color.FromArgb(250,250,251);
            this.TreeNodeSelectedForeColor = Color.FromArgb(31,41,55);
            this.TreeNodeSelectedBackColor = Color.FromArgb(250,250,251);
            this.TreeNodeCheckedBoxForeColor = Color.FromArgb(31,41,55);
            this.TreeNodeCheckedBoxBackColor = Color.FromArgb(250,250,251);
        }
    }
}
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GruvBoxTheme
    {
        private void ApplyList()
        {
            this.ListBackColor = Color.FromArgb(40,40,40);
            this.ListForeColor = Color.FromArgb(235,219,178);
            this.ListBorderColor = Color.FromArgb(168,153,132);
            this.ListItemForeColor = Color.FromArgb(235,219,178);
            this.ListItemHoverForeColor = Color.FromArgb(235,219,178);
            this.ListItemHoverBackColor = Color.FromArgb(40,40,40);
            this.ListItemSelectedForeColor = Color.FromArgb(235,219,178);
            this.ListItemSelectedBackColor = Color.FromArgb(40,40,40);
            this.ListItemSelectedBorderColor = Color.FromArgb(168,153,132);
            this.ListItemBorderColor = Color.FromArgb(168,153,132);
            this.ListItemHoverBorderColor = Color.FromArgb(168,153,132);
            this.ListItemSpacing = 0f;
            this.ListIndentation = 0f;
        }
    }
}
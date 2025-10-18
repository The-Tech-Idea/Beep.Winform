using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyList()
        {
            this.ListBackColor = Color.FromArgb(245,246,247);
            this.ListForeColor = Color.FromArgb(43,45,48);
            this.ListBorderColor = Color.FromArgb(220,223,230);
            this.ListItemForeColor = Color.FromArgb(43,45,48);
            this.ListItemHoverForeColor = Color.FromArgb(43,45,48);
            this.ListItemHoverBackColor = Color.FromArgb(245,246,247);
            this.ListItemSelectedForeColor = Color.FromArgb(43,45,48);
            this.ListItemSelectedBackColor = Color.FromArgb(245,246,247);
            this.ListItemSelectedBorderColor = Color.FromArgb(220,223,230);
            this.ListItemBorderColor = Color.FromArgb(220,223,230);
            this.ListItemHoverBorderColor = Color.FromArgb(220,223,230);
            this.ListItemSpacing = 0f;
            this.ListIndentation = 0f;
        }
    }
}
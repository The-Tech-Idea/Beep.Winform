using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class Metro2Theme
    {
        private void ApplyList()
        {
            this.ListBackColor = Color.FromArgb(243,242,241);
            this.ListForeColor = Color.FromArgb(32,31,30);
            this.ListBorderColor = Color.FromArgb(220,220,220);
            this.ListItemForeColor = Color.FromArgb(32,31,30);
            this.ListItemHoverForeColor = Color.FromArgb(32,31,30);
            this.ListItemHoverBackColor = Color.FromArgb(243,242,241);
            this.ListItemSelectedForeColor = Color.FromArgb(32,31,30);
            this.ListItemSelectedBackColor = Color.FromArgb(243,242,241);
            this.ListItemSelectedBorderColor = Color.FromArgb(220,220,220);
            this.ListItemBorderColor = Color.FromArgb(220,220,220);
            this.ListItemHoverBorderColor = Color.FromArgb(220,220,220);
            this.ListItemSpacing = 0f;
            this.ListIndentation = 0f;
        }
    }
}
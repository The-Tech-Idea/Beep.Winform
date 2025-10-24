using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class OneDarkTheme
    {
        private void ApplyList()
        {
            this.ListBackColor = Color.FromArgb(40,44,52);
            this.ListForeColor = Color.FromArgb(171,178,191);
            this.ListBorderColor = Color.FromArgb(92,99,112);
            this.ListItemForeColor = Color.FromArgb(171,178,191);
            this.ListItemHoverForeColor = Color.FromArgb(171,178,191);
            this.ListItemHoverBackColor = Color.FromArgb(40,44,52);
            this.ListItemSelectedForeColor = Color.FromArgb(171,178,191);
            this.ListItemSelectedBackColor = Color.FromArgb(40,44,52);
            this.ListItemSelectedBorderColor = Color.FromArgb(92,99,112);
            this.ListItemBorderColor = Color.FromArgb(92,99,112);
            this.ListItemHoverBorderColor = Color.FromArgb(92,99,112);
            this.ListItemSpacing = 0f;
            this.ListIndentation = 0f;
        }
    }
}
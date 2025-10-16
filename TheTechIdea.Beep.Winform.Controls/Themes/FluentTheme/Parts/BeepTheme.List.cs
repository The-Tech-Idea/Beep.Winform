using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyList()
        {
            this.ListBackColor = Color.FromArgb(245,246,248);
            this.ListForeColor = Color.FromArgb(32,32,32);
            this.ListBorderColor = Color.FromArgb(218,223,230);
            this.ListItemForeColor = Color.FromArgb(32,32,32);
            this.ListItemHoverForeColor = Color.FromArgb(32,32,32);
            this.ListItemHoverBackColor = Color.FromArgb(245,246,248);
            this.ListItemSelectedForeColor = Color.FromArgb(32,32,32);
            this.ListItemSelectedBackColor = Color.FromArgb(245,246,248);
            this.ListItemSelectedBorderColor = Color.FromArgb(218,223,230);
            this.ListItemBorderColor = Color.FromArgb(218,223,230);
            this.ListItemHoverBorderColor = Color.FromArgb(218,223,230);
            this.ListItemSpacing = 0f;
            this.ListIndentation = 0f;
        }
    }
}
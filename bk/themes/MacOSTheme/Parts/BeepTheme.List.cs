using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MacOSTheme
    {
        private void ApplyList()
        {
            this.ListBackColor = Color.FromArgb(250,250,252);
            this.ListForeColor = Color.FromArgb(28,28,30);
            this.ListBorderColor = Color.FromArgb(229,229,234);
            this.ListItemForeColor = Color.FromArgb(28,28,30);
            this.ListItemHoverForeColor = Color.FromArgb(28,28,30);
            this.ListItemHoverBackColor = Color.FromArgb(250,250,252);
            this.ListItemSelectedForeColor = Color.FromArgb(28,28,30);
            this.ListItemSelectedBackColor = Color.FromArgb(250,250,252);
            this.ListItemSelectedBorderColor = Color.FromArgb(229,229,234);
            this.ListItemBorderColor = Color.FromArgb(229,229,234);
            this.ListItemHoverBorderColor = Color.FromArgb(229,229,234);
            this.ListItemSpacing = 0f;
            this.ListIndentation = 0f;
        }
    }
}
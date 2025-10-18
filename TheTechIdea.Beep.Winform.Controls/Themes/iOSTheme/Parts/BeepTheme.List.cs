using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class iOSTheme
    {
        private void ApplyList()
        {
            this.ListBackColor = Color.FromArgb(242,242,247);
            this.ListForeColor = Color.FromArgb(28,28,30);
            this.ListBorderColor = Color.FromArgb(198,198,207);
            this.ListItemForeColor = Color.FromArgb(28,28,30);
            this.ListItemHoverForeColor = Color.FromArgb(28,28,30);
            this.ListItemHoverBackColor = Color.FromArgb(242,242,247);
            this.ListItemSelectedForeColor = Color.FromArgb(28,28,30);
            this.ListItemSelectedBackColor = Color.FromArgb(242,242,247);
            this.ListItemSelectedBorderColor = Color.FromArgb(198,198,207);
            this.ListItemBorderColor = Color.FromArgb(198,198,207);
            this.ListItemHoverBorderColor = Color.FromArgb(198,198,207);
            this.ListItemSpacing = 0f;
            this.ListIndentation = 0f;
        }
    }
}
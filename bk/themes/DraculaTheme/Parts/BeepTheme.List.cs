using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class DraculaTheme
    {
        private void ApplyList()
        {
            this.ListBackColor = Color.FromArgb(40,42,54);
            this.ListForeColor = Color.FromArgb(248,248,242);
            this.ListBorderColor = Color.FromArgb(98,114,164);
            this.ListItemForeColor = Color.FromArgb(248,248,242);
            this.ListItemHoverForeColor = Color.FromArgb(248,248,242);
            this.ListItemHoverBackColor = Color.FromArgb(40,42,54);
            this.ListItemSelectedForeColor = Color.FromArgb(248,248,242);
            this.ListItemSelectedBackColor = Color.FromArgb(40,42,54);
            this.ListItemSelectedBorderColor = Color.FromArgb(98,114,164);
            this.ListItemBorderColor = Color.FromArgb(98,114,164);
            this.ListItemHoverBorderColor = Color.FromArgb(98,114,164);
            this.ListItemSpacing = 0f;
            this.ListIndentation = 0f;
        }
    }
}
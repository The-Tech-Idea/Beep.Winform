using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordTheme
    {
        private void ApplyList()
        {
            this.ListBackColor = Color.FromArgb(46,52,64);
            this.ListForeColor = Color.FromArgb(216,222,233);
            this.ListBorderColor = Color.FromArgb(76,86,106);
            this.ListItemForeColor = Color.FromArgb(216,222,233);
            this.ListItemHoverForeColor = Color.FromArgb(216,222,233);
            this.ListItemHoverBackColor = Color.FromArgb(46,52,64);
            this.ListItemSelectedForeColor = Color.FromArgb(216,222,233);
            this.ListItemSelectedBackColor = Color.FromArgb(46,52,64);
            this.ListItemSelectedBorderColor = Color.FromArgb(76,86,106);
            this.ListItemBorderColor = Color.FromArgb(76,86,106);
            this.ListItemHoverBorderColor = Color.FromArgb(76,86,106);
            this.ListItemSpacing = 0f;
            this.ListIndentation = 0f;
        }
    }
}
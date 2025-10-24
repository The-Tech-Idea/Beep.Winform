using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplyList()
        {
            this.ListBackColor = Color.FromArgb(10,12,20);
            this.ListForeColor = Color.FromArgb(235,245,255);
            this.ListBorderColor = Color.FromArgb(60,70,100);
            this.ListItemForeColor = Color.FromArgb(235,245,255);
            this.ListItemHoverForeColor = Color.FromArgb(235,245,255);
            this.ListItemHoverBackColor = Color.FromArgb(10,12,20);
            this.ListItemSelectedForeColor = Color.FromArgb(235,245,255);
            this.ListItemSelectedBackColor = Color.FromArgb(10,12,20);
            this.ListItemSelectedBorderColor = Color.FromArgb(60,70,100);
            this.ListItemBorderColor = Color.FromArgb(60,70,100);
            this.ListItemHoverBorderColor = Color.FromArgb(60,70,100);
            this.ListItemSpacing = 0f;
            this.ListIndentation = 0f;
        }
    }
}
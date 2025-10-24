using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class BrutalistTheme
    {
        private void ApplyList()
        {
            this.ListBackColor = Color.FromArgb(250,250,250);
            this.ListForeColor = Color.FromArgb(20,20,20);
            this.ListBorderColor = Color.FromArgb(0,0,0);
            this.ListItemForeColor = Color.FromArgb(20,20,20);
            this.ListItemHoverForeColor = Color.FromArgb(20,20,20);
            this.ListItemHoverBackColor = Color.FromArgb(250,250,250);
            this.ListItemSelectedForeColor = Color.FromArgb(20,20,20);
            this.ListItemSelectedBackColor = Color.FromArgb(250,250,250);
            this.ListItemSelectedBorderColor = Color.FromArgb(0,0,0);
            this.ListItemBorderColor = Color.FromArgb(0,0,0);
            this.ListItemHoverBorderColor = Color.FromArgb(0,0,0);
            this.ListItemSpacing = 0f;
            this.ListIndentation = 0f;
        }
    }
}
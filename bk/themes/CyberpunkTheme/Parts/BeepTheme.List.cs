using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyList()
        {
            this.ListBackColor = Color.FromArgb(10,8,20);
            this.ListForeColor = Color.FromArgb(228,244,255);
            this.ListBorderColor = Color.FromArgb(90,20,110);
            this.ListItemForeColor = Color.FromArgb(228,244,255);
            this.ListItemHoverForeColor = Color.FromArgb(228,244,255);
            this.ListItemHoverBackColor = Color.FromArgb(10,8,20);
            this.ListItemSelectedForeColor = Color.FromArgb(228,244,255);
            this.ListItemSelectedBackColor = Color.FromArgb(10,8,20);
            this.ListItemSelectedBorderColor = Color.FromArgb(90,20,110);
            this.ListItemBorderColor = Color.FromArgb(90,20,110);
            this.ListItemHoverBorderColor = Color.FromArgb(90,20,110);
            this.ListItemSpacing = 0f;
            this.ListIndentation = 0f;
        }
    }
}
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyList()
        {
            this.ListBackColor = Color.FromArgb(0,43,54);
            this.ListForeColor = Color.FromArgb(147,161,161);
            this.ListBorderColor = Color.FromArgb(88,110,117);
            this.ListItemForeColor = Color.FromArgb(147,161,161);
            this.ListItemHoverForeColor = Color.FromArgb(147,161,161);
            this.ListItemHoverBackColor = Color.FromArgb(0,43,54);
            this.ListItemSelectedForeColor = Color.FromArgb(147,161,161);
            this.ListItemSelectedBackColor = Color.FromArgb(0,43,54);
            this.ListItemSelectedBorderColor = Color.FromArgb(88,110,117);
            this.ListItemBorderColor = Color.FromArgb(88,110,117);
            this.ListItemHoverBorderColor = Color.FromArgb(88,110,117);
            this.ListItemSpacing = 0f;
            this.ListIndentation = 0f;
        }
    }
}
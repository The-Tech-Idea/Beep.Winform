using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplyList()
        {
            this.ListBackColor = Color.FromArgb(242,242,245);
            this.ListForeColor = Color.FromArgb(44,44,44);
            this.ListBorderColor = Color.FromArgb(218,218,222);
            this.ListItemForeColor = Color.FromArgb(44,44,44);
            this.ListItemHoverForeColor = Color.FromArgb(44,44,44);
            this.ListItemHoverBackColor = Color.FromArgb(242,242,245);
            this.ListItemSelectedForeColor = Color.FromArgb(44,44,44);
            this.ListItemSelectedBackColor = Color.FromArgb(242,242,245);
            this.ListItemSelectedBorderColor = Color.FromArgb(218,218,222);
            this.ListItemBorderColor = Color.FromArgb(218,218,222);
            this.ListItemHoverBorderColor = Color.FromArgb(218,218,222);
            this.ListItemSpacing = 0f;
            this.ListIndentation = 0f;
        }
    }
}
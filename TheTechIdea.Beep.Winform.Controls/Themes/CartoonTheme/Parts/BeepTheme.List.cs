using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplyList()
        {
            this.ListBackColor = Color.FromArgb(255,251,235);
            this.ListForeColor = Color.FromArgb(33,37,41);
            this.ListBorderColor = Color.FromArgb(247,208,136);
            this.ListItemForeColor = Color.FromArgb(33,37,41);
            this.ListItemHoverForeColor = Color.FromArgb(33,37,41);
            this.ListItemHoverBackColor = Color.FromArgb(255,251,235);
            this.ListItemSelectedForeColor = Color.FromArgb(33,37,41);
            this.ListItemSelectedBackColor = Color.FromArgb(255,251,235);
            this.ListItemSelectedBorderColor = Color.FromArgb(247,208,136);
            this.ListItemBorderColor = Color.FromArgb(247,208,136);
            this.ListItemHoverBorderColor = Color.FromArgb(247,208,136);
            this.ListItemSpacing = 0f;
            this.ListIndentation = 0f;
        }
    }
}
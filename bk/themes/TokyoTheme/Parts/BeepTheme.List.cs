using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class TokyoTheme
    {
        private void ApplyList()
        {
            this.ListBackColor = Color.FromArgb(26,27,38);
            this.ListForeColor = Color.FromArgb(192,202,245);
            this.ListBorderColor = Color.FromArgb(86,95,137);
            this.ListItemForeColor = Color.FromArgb(192,202,245);
            this.ListItemHoverForeColor = Color.FromArgb(192,202,245);
            this.ListItemHoverBackColor = Color.FromArgb(26,27,38);
            this.ListItemSelectedForeColor = Color.FromArgb(192,202,245);
            this.ListItemSelectedBackColor = Color.FromArgb(26,27,38);
            this.ListItemSelectedBorderColor = Color.FromArgb(86,95,137);
            this.ListItemBorderColor = Color.FromArgb(86,95,137);
            this.ListItemHoverBorderColor = Color.FromArgb(86,95,137);
            this.ListItemSpacing = 0f;
            this.ListIndentation = 0f;
        }
    }
}
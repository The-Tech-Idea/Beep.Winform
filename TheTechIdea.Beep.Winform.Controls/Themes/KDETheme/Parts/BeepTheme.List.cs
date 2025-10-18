using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class KDETheme
    {
        private void ApplyList()
        {
            this.ListBackColor = Color.FromArgb(248,249,250);
            this.ListForeColor = Color.FromArgb(33,37,41);
            this.ListBorderColor = Color.FromArgb(222,226,230);
            this.ListItemForeColor = Color.FromArgb(33,37,41);
            this.ListItemHoverForeColor = Color.FromArgb(33,37,41);
            this.ListItemHoverBackColor = Color.FromArgb(248,249,250);
            this.ListItemSelectedForeColor = Color.FromArgb(33,37,41);
            this.ListItemSelectedBackColor = Color.FromArgb(248,249,250);
            this.ListItemSelectedBorderColor = Color.FromArgb(222,226,230);
            this.ListItemBorderColor = Color.FromArgb(222,226,230);
            this.ListItemHoverBorderColor = Color.FromArgb(222,226,230);
            this.ListItemSpacing = 0f;
            this.ListIndentation = 0f;
        }
    }
}
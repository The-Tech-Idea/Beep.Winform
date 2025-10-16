using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GNOMETheme
    {
        private void ApplyList()
        {
            this.ListBackColor = Color.FromArgb(246,245,244);
            this.ListForeColor = Color.FromArgb(46,52,54);
            this.ListBorderColor = Color.FromArgb(205,207,212);
            this.ListItemForeColor = Color.FromArgb(46,52,54);
            this.ListItemHoverForeColor = Color.FromArgb(46,52,54);
            this.ListItemHoverBackColor = Color.FromArgb(246,245,244);
            this.ListItemSelectedForeColor = Color.FromArgb(46,52,54);
            this.ListItemSelectedBackColor = Color.FromArgb(246,245,244);
            this.ListItemSelectedBorderColor = Color.FromArgb(205,207,212);
            this.ListItemBorderColor = Color.FromArgb(205,207,212);
            this.ListItemHoverBorderColor = Color.FromArgb(205,207,212);
            this.ListItemSpacing = 0f;
            this.ListIndentation = 0f;
        }
    }
}
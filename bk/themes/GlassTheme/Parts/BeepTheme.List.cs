using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GlassTheme
    {
        private void ApplyList()
        {
            this.ListBackColor = Color.FromArgb(236,244,255);
            this.ListForeColor = Color.FromArgb(17,24,39);
            this.ListBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.ListItemForeColor = Color.FromArgb(17,24,39);
            this.ListItemHoverForeColor = Color.FromArgb(17,24,39);
            this.ListItemHoverBackColor = Color.FromArgb(236,244,255);
            this.ListItemSelectedForeColor = Color.FromArgb(17,24,39);
            this.ListItemSelectedBackColor = Color.FromArgb(236,244,255);
            this.ListItemSelectedBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.ListItemBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.ListItemHoverBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.ListItemSpacing = 0f;
            this.ListIndentation = 0f;
        }
    }
}
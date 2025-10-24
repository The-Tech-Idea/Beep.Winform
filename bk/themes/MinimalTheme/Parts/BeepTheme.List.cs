using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyList()
        {
            this.ListBackColor = Color.FromArgb(255,255,255);
            this.ListForeColor = Color.FromArgb(31,41,55);
            this.ListBorderColor = Color.FromArgb(209,213,219);
            this.ListItemForeColor = Color.FromArgb(31,41,55);
            this.ListItemHoverForeColor = Color.FromArgb(31,41,55);
            this.ListItemHoverBackColor = Color.FromArgb(255,255,255);
            this.ListItemSelectedForeColor = Color.FromArgb(31,41,55);
            this.ListItemSelectedBackColor = Color.FromArgb(255,255,255);
            this.ListItemSelectedBorderColor = Color.FromArgb(209,213,219);
            this.ListItemBorderColor = Color.FromArgb(209,213,219);
            this.ListItemHoverBorderColor = Color.FromArgb(209,213,219);
            this.ListItemSpacing = 0f;
            this.ListIndentation = 0f;
        }
    }
}
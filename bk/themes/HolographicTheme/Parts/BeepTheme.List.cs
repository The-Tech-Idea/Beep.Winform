using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplyList()
        {
            this.ListBackColor = Color.FromArgb(15,16,32);
            this.ListForeColor = Color.FromArgb(245,247,255);
            this.ListBorderColor = Color.FromArgb(74,79,123);
            this.ListItemForeColor = Color.FromArgb(245,247,255);
            this.ListItemHoverForeColor = Color.FromArgb(245,247,255);
            this.ListItemHoverBackColor = Color.FromArgb(15,16,32);
            this.ListItemSelectedForeColor = Color.FromArgb(245,247,255);
            this.ListItemSelectedBackColor = Color.FromArgb(15,16,32);
            this.ListItemSelectedBorderColor = Color.FromArgb(74,79,123);
            this.ListItemBorderColor = Color.FromArgb(74,79,123);
            this.ListItemHoverBorderColor = Color.FromArgb(74,79,123);
            this.ListItemSpacing = 0f;
            this.ListIndentation = 0f;
        }
    }
}
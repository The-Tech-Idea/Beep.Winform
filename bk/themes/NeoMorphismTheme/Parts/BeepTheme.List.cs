using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplyList()
        {
            this.ListBackColor = Color.FromArgb(236,240,243);
            this.ListForeColor = Color.FromArgb(58,66,86);
            this.ListBorderColor = Color.FromArgb(221,228,235);
            this.ListItemForeColor = Color.FromArgb(58,66,86);
            this.ListItemHoverForeColor = Color.FromArgb(58,66,86);
            this.ListItemHoverBackColor = Color.FromArgb(236,240,243);
            this.ListItemSelectedForeColor = Color.FromArgb(58,66,86);
            this.ListItemSelectedBackColor = Color.FromArgb(236,240,243);
            this.ListItemSelectedBorderColor = Color.FromArgb(221,228,235);
            this.ListItemBorderColor = Color.FromArgb(221,228,235);
            this.ListItemHoverBorderColor = Color.FromArgb(221,228,235);
            this.ListItemSpacing = 0f;
            this.ListIndentation = 0f;
        }
    }
}
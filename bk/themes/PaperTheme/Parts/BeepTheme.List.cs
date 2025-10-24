using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class PaperTheme
    {
        private void ApplyList()
        {
            this.ListBackColor = Color.FromArgb(250,250,250);
            this.ListForeColor = Color.FromArgb(33,33,33);
            this.ListBorderColor = Color.FromArgb(224,224,224);
            this.ListItemForeColor = Color.FromArgb(33,33,33);
            this.ListItemHoverForeColor = Color.FromArgb(33,33,33);
            this.ListItemHoverBackColor = Color.FromArgb(250,250,250);
            this.ListItemSelectedForeColor = Color.FromArgb(33,33,33);
            this.ListItemSelectedBackColor = Color.FromArgb(250,250,250);
            this.ListItemSelectedBorderColor = Color.FromArgb(224,224,224);
            this.ListItemBorderColor = Color.FromArgb(224,224,224);
            this.ListItemHoverBorderColor = Color.FromArgb(224,224,224);
            this.ListItemSpacing = 0f;
            this.ListIndentation = 0f;
        }
    }
}
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme
    {
        private void ApplyList()
        {
            this.ListBackColor = Color.FromArgb(245,248,255);
            this.ListForeColor = Color.FromArgb(24,28,35);
            this.ListBorderColor = Color.FromArgb(210,220,235);
            this.ListItemForeColor = Color.FromArgb(24,28,35);
            this.ListItemHoverForeColor = Color.FromArgb(24,28,35);
            this.ListItemHoverBackColor = Color.FromArgb(245,248,255);
            this.ListItemSelectedForeColor = Color.FromArgb(24,28,35);
            this.ListItemSelectedBackColor = Color.FromArgb(245,248,255);
            this.ListItemSelectedBorderColor = Color.FromArgb(210,220,235);
            this.ListItemBorderColor = Color.FromArgb(210,220,235);
            this.ListItemHoverBorderColor = Color.FromArgb(210,220,235);
            this.ListItemSpacing = 0f;
            this.ListIndentation = 0f;
        }
    }
}
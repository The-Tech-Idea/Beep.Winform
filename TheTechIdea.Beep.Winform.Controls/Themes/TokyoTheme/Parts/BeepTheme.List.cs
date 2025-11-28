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
            this.ListBackColor = BackgroundColor;
            this.ListForeColor = ForeColor;
            this.ListBorderColor = BorderColor;
            this.ListItemForeColor = ForeColor;
            this.ListItemHoverForeColor = ForeColor;
            this.ListItemHoverBackColor = BackgroundColor;
            this.ListItemSelectedForeColor = ForeColor;
            this.ListItemSelectedBackColor = BackgroundColor;
            this.ListItemSelectedBorderColor = BorderColor;
            this.ListItemBorderColor = BorderColor;
            this.ListItemHoverBorderColor = BorderColor;
            this.ListItemSpacing = 0f;
            this.ListIndentation = 0f;
        }
    }
}
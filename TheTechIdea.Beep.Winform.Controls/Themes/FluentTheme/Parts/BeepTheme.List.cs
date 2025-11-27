using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyList()
        {
            this.ListBackColor = SurfaceColor;
            this.ListForeColor = ForeColor;
            this.ListBorderColor = BorderColor;
            this.ListItemForeColor = ForeColor;
            this.ListItemHoverForeColor = ForeColor;
            this.ListItemHoverBackColor = SecondaryColor;
            this.ListItemSelectedForeColor = OnPrimaryColor;
            this.ListItemSelectedBackColor = PrimaryColor;
            this.ListItemSelectedBorderColor = PrimaryColor;
            this.ListItemBorderColor = BorderColor;
            this.ListItemHoverBorderColor = ActiveBorderColor;
            this.ListItemSpacing = 0f;
            this.ListIndentation = 0f;
        }
    }
}
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class OneDarkTheme
    {
        private void ApplyList()
        {
            this.ListBackColor = PanelBackColor;
            this.ListForeColor = ForeColor;
            this.ListBorderColor = BorderColor;
            this.ListItemForeColor = ForeColor;
            this.ListItemHoverForeColor = ForeColor;
            this.ListItemHoverBackColor = PanelGradiantMiddleColor;
            this.ListItemSelectedForeColor = ForeColor;
            this.ListItemSelectedBackColor = PanelGradiantMiddleColor;
            this.ListItemSelectedBorderColor = ActiveBorderColor;
            this.ListItemBorderColor = BorderColor;
            this.ListItemHoverBorderColor = ActiveBorderColor;
            this.ListItemSpacing = 0f;
            this.ListIndentation = 0f;
        }
    }
}
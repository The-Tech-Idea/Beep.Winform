using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
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
            this.ListItemSelectedBackColor = PanelBackColor;
            this.ListItemSelectedBorderColor = BorderColor;
            this.ListItemBorderColor = BorderColor;
            this.ListItemHoverBorderColor = BorderColor;
            this.ListItemSpacing = 0f;
            this.ListIndentation = 0f;
        }
    }
}
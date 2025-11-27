using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplyList()
        {
            this.ListBackColor = PanelGradiantMiddleColor;
            this.ListForeColor = ForeColor;
            this.ListBorderColor = InactiveBorderColor;
            this.ListItemForeColor = ForeColor;
            this.ListItemHoverForeColor = ForeColor;
            this.ListItemHoverBackColor = PanelGradiantMiddleColor;
            this.ListItemSelectedForeColor = ForeColor;
            this.ListItemSelectedBackColor = PanelGradiantMiddleColor;
            this.ListItemSelectedBorderColor = InactiveBorderColor;
            this.ListItemBorderColor = InactiveBorderColor;
            this.ListItemHoverBorderColor = InactiveBorderColor;
            this.ListItemSpacing = 0f;
            this.ListIndentation = 0f;
        }
    }
}
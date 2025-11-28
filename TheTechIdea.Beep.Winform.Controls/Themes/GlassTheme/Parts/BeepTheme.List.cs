using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GlassTheme
    {
        private void ApplyList()
        {
            this.ListBackColor = SurfaceColor;
            this.ListForeColor = ForeColor;
            this.ListBorderColor = InactiveBorderColor;
            this.ListItemForeColor = ForeColor;
            this.ListItemHoverForeColor = ForeColor;
            this.ListItemHoverBackColor = PanelGradiantStartColor;
            this.ListItemSelectedForeColor = OnPrimaryColor;
            this.ListItemSelectedBackColor = PrimaryColor;
            this.ListItemSelectedBorderColor = PrimaryColor;
            this.ListItemBorderColor = InactiveBorderColor;
            this.ListItemHoverBorderColor = ActiveBorderColor;
            this.ListItemSpacing = 0f;
            this.ListIndentation = 0f;
        }
    }
}
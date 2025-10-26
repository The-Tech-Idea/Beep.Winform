using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyMenu()
        {
            this.MenuBackColor = BackgroundColor;
            this.MenuForeColor = ForeColor;
            this.MenuBorderColor = BorderColor;
            this.MenuMainItemForeColor = ForeColor;
            this.MenuMainItemHoverForeColor = ForeColor;
            this.MenuMainItemHoverBackColor = SurfaceColor;
            this.MenuMainItemSelectedForeColor = ForeColor;
            this.MenuMainItemSelectedBackColor = ThemeUtil.Lighten(SurfaceColor, 0.05);
            this.MenuItemForeColor = ForeColor;
            this.MenuItemHoverForeColor = ForeColor;
            this.MenuItemHoverBackColor = SurfaceColor;
            this.MenuItemSelectedForeColor = ForeColor;
            this.MenuItemSelectedBackColor = ThemeUtil.Lighten(SurfaceColor, 0.05);
            this.MenuGradiantStartColor = BackgroundColor;
            this.MenuGradiantEndColor = BackgroundColor;
            this.MenuGradiantMiddleColor = BackgroundColor;
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}

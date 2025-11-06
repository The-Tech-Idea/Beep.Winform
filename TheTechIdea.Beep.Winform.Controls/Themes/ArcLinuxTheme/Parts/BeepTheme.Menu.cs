using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyMenu()
        {
            this.MenuTitleFont = ThemeUtils.ConvertFontToTypographyStyle("Ubuntu", 14F, FontStyle.Bold);
            this.MenuItemSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Ubuntu", 12F, FontStyle.Bold);
            this.MenuItemUnSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Ubuntu", 12F, FontStyle.Regular);
            
            this.MenuBackColor = SurfaceColor;
            this.MenuForeColor = ForeColor;
            this.MenuBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.MenuMainItemForeColor = ForeColor;
            this.MenuMainItemHoverForeColor = ForeColor;
            this.MenuMainItemHoverBackColor = SurfaceColor;
            this.MenuMainItemSelectedForeColor = ForeColor;
            this.MenuMainItemSelectedBackColor = SurfaceColor;
            this.MenuItemForeColor = ForeColor;
            this.MenuItemHoverForeColor = ForeColor;
            this.MenuItemHoverBackColor = SurfaceColor;
            this.MenuItemSelectedForeColor = ForeColor;
            this.MenuItemSelectedBackColor = SurfaceColor;
            this.MenuGradiantStartColor = SurfaceColor;
            this.MenuGradiantEndColor = SurfaceColor;
            this.MenuGradiantMiddleColor = SurfaceColor;
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}

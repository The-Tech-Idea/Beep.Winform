using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GruvBoxTheme
    {
        private void ApplyMenu()
        {
            // GruvBox menu - warm retro theme
            this.MenuTitleFont = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 14F, FontStyle.Bold);
            this.MenuItemSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 12F, FontStyle.Bold);
            this.MenuItemUnSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 12F, FontStyle.Regular);

            this.MenuBackColor = SurfaceColor;
            this.MenuForeColor = ForeColor;
            this.MenuBorderColor = BorderColor;

            // Main menu items
            this.MenuMainItemForeColor = ForeColor;
            this.MenuMainItemHoverForeColor = AccentColor;
            this.MenuMainItemHoverBackColor = SecondaryColor;
            this.MenuMainItemSelectedForeColor = AccentColor;
            this.MenuMainItemSelectedBackColor = SecondaryColor;

            // Sub menu items
            this.MenuItemForeColor = ForeColor;
            this.MenuItemHoverForeColor = AccentColor;
            this.MenuItemHoverBackColor = SecondaryColor;
            this.MenuItemSelectedForeColor = AccentColor;
            this.MenuItemSelectedBackColor = SecondaryColor;

            // Dark gradient
            this.MenuGradiantStartColor = SecondaryColor;
            this.MenuGradiantEndColor = SurfaceColor;
            this.MenuGradiantMiddleColor = SurfaceColor;
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
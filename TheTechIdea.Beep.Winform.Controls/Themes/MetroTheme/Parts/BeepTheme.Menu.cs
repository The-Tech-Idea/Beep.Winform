using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MetroTheme
    {
        private void ApplyMenu()
        {
            // Metro menu - Windows Metro theme
            this.MenuTitleFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14F, FontStyle.Bold);
            this.MenuItemSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Bold);
            this.MenuItemUnSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Regular);
            
            this.MenuBackColor = SurfaceColor;
            this.MenuForeColor = ForeColor;
            this.MenuBorderColor = BorderColor;
            // Main menu items
            this.MenuMainItemForeColor = ForeColor;
            this.MenuMainItemHoverForeColor = ForeColor;
            this.MenuMainItemHoverBackColor = SecondaryColor;
            this.MenuMainItemSelectedForeColor = OnPrimaryColor;
            this.MenuMainItemSelectedBackColor = PrimaryColor;
            // Sub menu items
            this.MenuItemForeColor = ForeColor;
            this.MenuItemHoverForeColor = ForeColor;
            this.MenuItemHoverBackColor = SecondaryColor;
            this.MenuItemSelectedForeColor = OnPrimaryColor;
            this.MenuItemSelectedBackColor = PrimaryColor;
            // Clean gradient
            this.MenuGradiantStartColor = SurfaceColor;
            this.MenuGradiantEndColor = SurfaceColor;
            this.MenuGradiantMiddleColor = SurfaceColor;
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
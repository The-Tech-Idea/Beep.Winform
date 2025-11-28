using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GlassTheme
    {
        private void ApplyMenu()
        {
            // Glass menu - frosted glass theme
            this.MenuTitleFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14F, FontStyle.Bold);
            this.MenuItemSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Bold);
            this.MenuItemUnSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Regular);
            
            this.MenuBackColor = SurfaceColor;  // Light surface
            this.MenuForeColor = ForeColor;  // Dark text
            this.MenuBorderColor = InactiveBorderColor;  // Visible border
            
            // Main menu items
            this.MenuMainItemForeColor = ForeColor;
            this.MenuMainItemHoverForeColor = ForeColor;
            this.MenuMainItemHoverBackColor = PanelGradiantStartColor;  // Hover
            this.MenuMainItemSelectedForeColor = OnPrimaryColor;
            this.MenuMainItemSelectedBackColor = PrimaryColor;  // Selected
            
            // Sub menu items
            this.MenuItemForeColor = ForeColor;
            this.MenuItemHoverForeColor = ForeColor;
            this.MenuItemHoverBackColor = PanelGradiantStartColor;  // Hover
            this.MenuItemSelectedForeColor = OnPrimaryColor;
            this.MenuItemSelectedBackColor = PrimaryColor;  // Selected
            
            // Frosted gradient
            this.MenuGradiantStartColor = PanelGradiantStartColor;
            this.MenuGradiantEndColor = PanelGradiantEndColor;
            this.MenuGradiantMiddleColor = PanelGradiantMiddleColor;
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
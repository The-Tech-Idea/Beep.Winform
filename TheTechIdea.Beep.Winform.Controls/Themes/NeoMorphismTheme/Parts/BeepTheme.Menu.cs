using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplyMenu()
        {
            // NeoMorphism menu - soft neomorphic theme
            this.MenuTitleFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14F, FontStyle.Bold);
            this.MenuItemSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Bold);
            this.MenuItemUnSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Regular);
            
            this.MenuBackColor = BackgroundColor;  // Light background
            this.MenuForeColor = ForeColor;  // Dark gray text
            this.MenuBorderColor = BorderColor;  // Soft border
            
            // Main menu items
            this.MenuMainItemForeColor = ForeColor;
            this.MenuMainItemHoverForeColor = ForeColor;
            this.MenuMainItemHoverBackColor = PanelGradiantMiddleColor;  // Darker hover
            this.MenuMainItemSelectedForeColor = OnPrimaryColor;  // White text on blue
            this.MenuMainItemSelectedBackColor = PrimaryColor;  // Blue
            
            // Sub menu items
            this.MenuItemForeColor = ForeColor;
            this.MenuItemHoverForeColor = ForeColor;
            this.MenuItemHoverBackColor = PanelGradiantMiddleColor;  // Darker hover
            this.MenuItemSelectedForeColor = OnPrimaryColor;  // White text
            this.MenuItemSelectedBackColor = PrimaryColor;  // Blue
            
            // Soft gradient
            this.MenuGradiantStartColor = ThemeUtil.Lighten(BackgroundColor, 0.06);
            this.MenuGradiantEndColor = ThemeUtil.Darken(BackgroundColor, 0.07);
            this.MenuGradiantMiddleColor = ThemeUtil.Lighten(BackgroundColor, 0.03);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}
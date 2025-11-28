using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class PaperTheme
    {
        private void ApplyMenu()
        {
            // Paper menu - Material Design aesthetic
            this.MenuTitleFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14F, FontStyle.Bold);
            this.MenuItemSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Bold);
            this.MenuItemUnSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Regular);
            
            this.MenuBackColor = BackgroundColor;  // Light paper
            this.MenuForeColor = ForeColor;  // Dark grey text
            this.MenuBorderColor = InactiveBorderColor;  // Light grey
            
            // Main menu items
            this.MenuMainItemForeColor = ForeColor;
            this.MenuMainItemHoverForeColor = ForeColor;
            this.MenuMainItemHoverBackColor = PanelGradiantStartColor;  // Darker hover
            this.MenuMainItemSelectedForeColor = OnPrimaryColor;  // White-like on blue
            this.MenuMainItemSelectedBackColor = PrimaryColor;  // Material blue
            
            // Sub menu items
                this.MenuItemForeColor = ForeColor;
                this.MenuItemHoverForeColor = ForeColor;
                this.MenuItemHoverBackColor = PanelGradiantStartColor;
                this.MenuItemSelectedForeColor = OnPrimaryColor;
                this.MenuItemSelectedBackColor = PrimaryColor;
          
                this.MenuItemSelectedBackColor = PanelGradiantStartColor;
                this.MenuItemSelectedForeColor = ForeColor;
                this.MenuItemHoverBackColor = PanelGradiantStartColor;
                this.MenuItemHoverForeColor = ForeColor;
            
            // Clean gradient
            this.MenuGradiantStartColor = ThemeUtil.Lighten(BackgroundColor, 0.02);
            this.MenuGradiantEndColor = ThemeUtil.Darken(BackgroundColor, 0.03);
            this.MenuGradiantMiddleColor = ThemeUtil.Lighten(BackgroundColor, 0.01);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
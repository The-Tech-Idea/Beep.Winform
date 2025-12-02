using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyMenu()
        {
            // Solarized menu - scientifically crafted color palette
            this.MenuTitleFont = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 14F, FontStyle.Bold);
            this.MenuItemSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 12F, FontStyle.Bold);
            this.MenuItemUnSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 12F, FontStyle.Regular);
            
            // Menu colors - Solarized LIGHT mode (matches BeepStyling light backgrounds)
            this.MenuBackColor = BackgroundColor;  // Light beige (253, 246, 227)
            this.MenuForeColor = ForeColor;  // Dark gray-blue (88, 110, 117)
            this.MenuBorderColor = BorderColor;  // Medium gray (147, 161, 161)
            
            // Main menu items - Dark text on light backgrounds
            this.MenuMainItemForeColor = ForeColor;  // Dark gray-blue (88, 110, 117)
            this.MenuMainItemHoverForeColor = OnPrimaryColor;  // Light beige on colored hover
            this.MenuMainItemHoverBackColor = PrimaryColor;  // Blue (38, 139, 210)
            this.MenuMainItemSelectedForeColor = OnPrimaryColor;  // Light beige
            this.MenuMainItemSelectedBackColor = AccentColor;  // Orange (203, 75, 22)
            
            // Sub menu items - Dark text on light painted backgrounds (matches BeepStyling)
            this.MenuItemForeColor = ForeColor;  // Dark gray-blue (88, 110, 117) on light
            this.MenuItemHoverForeColor = OnPrimaryColor;  // Light beige on colored background
            this.MenuItemHoverBackColor = SecondaryColor;  // Cyan (42, 161, 152)
            this.MenuItemSelectedForeColor = OnPrimaryColor;  // Light beige on colored
            this.MenuItemSelectedBackColor = AccentColor;  // Orange (203, 75, 22)
            
            // Dark gradient based on background
            this.MenuGradiantStartColor = BackgroundColor;
            this.MenuGradiantEndColor = BackgroundColor;
            this.MenuGradiantMiddleColor = ThemeUtil.Lighten(BackgroundColor, 0.04);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
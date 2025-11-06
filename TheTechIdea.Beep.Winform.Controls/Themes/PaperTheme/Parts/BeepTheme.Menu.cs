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
            
            this.MenuBackColor = Color.FromArgb(250, 250, 250);  // Light paper
            this.MenuForeColor = Color.FromArgb(33, 33, 33);  // Dark grey text
            this.MenuBorderColor = Color.FromArgb(224, 224, 224);  // Light grey
            
            // Main menu items
            this.MenuMainItemForeColor = Color.FromArgb(33, 33, 33);
            this.MenuMainItemHoverForeColor = Color.FromArgb(33, 33, 33);
            this.MenuMainItemHoverBackColor = Color.FromArgb(240, 240, 240);  // Darker hover
            this.MenuMainItemSelectedForeColor = Color.White;  // White text
            this.MenuMainItemSelectedBackColor = Color.FromArgb(33, 150, 243);  // Material blue
            
            // Sub menu items
            this.MenuItemForeColor = Color.FromArgb(33, 33, 33);
            this.MenuItemHoverForeColor = Color.FromArgb(33, 33, 33);
            this.MenuItemHoverBackColor = Color.FromArgb(240, 240, 240);  // Darker hover
            this.MenuItemSelectedForeColor = Color.White;  // White text
            this.MenuItemSelectedBackColor = Color.FromArgb(33, 150, 243);  // Material blue
            
            // Clean gradient
            this.MenuGradiantStartColor = ThemeUtil.Lighten(BackgroundColor, 0.02);
            this.MenuGradiantEndColor = ThemeUtil.Darken(BackgroundColor, 0.03);
            this.MenuGradiantMiddleColor = ThemeUtil.Lighten(BackgroundColor, 0.01);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
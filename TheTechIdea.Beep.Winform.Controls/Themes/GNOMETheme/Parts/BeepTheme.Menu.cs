using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GNOMETheme
    {
        private void ApplyMenu()
        {
            // GNOME menu - clean light theme
            this.MenuTitleFont = ThemeUtils.ConvertFontToTypographyStyle("Ubuntu", 14F, FontStyle.Bold);
            this.MenuItemSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Ubuntu", 12F, FontStyle.Bold);
            this.MenuItemUnSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Ubuntu", 12F, FontStyle.Regular);
            
            this.MenuBackColor = Color.FromArgb(255, 255, 255);  // White
            this.MenuForeColor = Color.FromArgb(35, 38, 41);  // Dark gray text
            this.MenuBorderColor = Color.FromArgb(200, 200, 200);  // Light gray
            
            // Main menu items
            this.MenuMainItemForeColor = Color.FromArgb(35, 38, 41);
            this.MenuMainItemHoverForeColor = Color.FromArgb(35, 38, 41);
            this.MenuMainItemHoverBackColor = Color.FromArgb(240, 240, 240);  // Light gray hover
            this.MenuMainItemSelectedForeColor = Color.FromArgb(35, 38, 41);
            this.MenuMainItemSelectedBackColor = Color.FromArgb(220, 220, 220);  // Medium gray selected
            
            // Sub menu items
            this.MenuItemForeColor = Color.FromArgb(35, 38, 41);
            this.MenuItemHoverForeColor = Color.FromArgb(35, 38, 41);
            this.MenuItemHoverBackColor = Color.FromArgb(240, 240, 240);  // Light gray hover
            this.MenuItemSelectedForeColor = Color.FromArgb(35, 38, 41);
            this.MenuItemSelectedBackColor = Color.FromArgb(220, 220, 220);  // Medium gray selected
            
            // Clean gradient
            this.MenuGradiantStartColor = Color.FromArgb(245, 245, 245);
            this.MenuGradiantEndColor = Color.FromArgb(239, 240, 241);
            this.MenuGradiantMiddleColor = Color.FromArgb(242, 243, 244);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
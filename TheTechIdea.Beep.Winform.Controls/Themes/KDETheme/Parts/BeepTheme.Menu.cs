using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class KDETheme
    {
        private void ApplyMenu()
        {
            // KDE menu - clean Linux theme
            this.MenuTitleFont = ThemeUtils.ConvertFontToTypographyStyle("Noto Sans", 14F, FontStyle.Bold);
            this.MenuItemSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Noto Sans", 12F, FontStyle.Bold);
            this.MenuItemUnSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Noto Sans", 12F, FontStyle.Regular);
            
            this.MenuBackColor = Color.FromArgb(248, 249, 250);  // Light gray
            this.MenuForeColor = Color.FromArgb(33, 37, 41);  // Dark gray text
            this.MenuBorderColor = Color.FromArgb(222, 226, 230);  // Medium gray
            
            // Main menu items
            this.MenuMainItemForeColor = Color.FromArgb(33, 37, 41);
            this.MenuMainItemHoverForeColor = Color.FromArgb(33, 37, 41);
            this.MenuMainItemHoverBackColor = Color.FromArgb(230, 235, 240);  // Darker gray hover
            this.MenuMainItemSelectedForeColor = Color.FromArgb(255, 255, 255);  // White text
            this.MenuMainItemSelectedBackColor = Color.FromArgb(61, 174, 233);  // KDE blue
            
            // Sub menu items
            this.MenuItemForeColor = Color.FromArgb(33, 37, 41);
            this.MenuItemHoverForeColor = Color.FromArgb(33, 37, 41);
            this.MenuItemHoverBackColor = Color.FromArgb(230, 235, 240);  // Darker gray hover
            this.MenuItemSelectedForeColor = Color.FromArgb(255, 255, 255);  // White text
            this.MenuItemSelectedBackColor = Color.FromArgb(61, 174, 233);  // KDE blue
            
            // Clean gradient
            this.MenuGradiantStartColor = Color.FromArgb(255, 255, 255);
            this.MenuGradiantEndColor = Color.FromArgb(248, 249, 250);
            this.MenuGradiantMiddleColor = Color.FromArgb(252, 252, 253);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
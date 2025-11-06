using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MacOSTheme
    {
        private void ApplyMenu()
        {
            // MacOS menu - clean macOS theme
            this.MenuTitleFont = ThemeUtils.ConvertFontToTypographyStyle("SF Pro", 14F, FontStyle.Bold);
            this.MenuItemSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("SF Pro", 12F, FontStyle.Bold);
            this.MenuItemUnSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("SF Pro", 12F, FontStyle.Regular);
            
            this.MenuBackColor = Color.FromArgb(250, 250, 252);  // Light gray
            this.MenuForeColor = Color.FromArgb(28, 28, 30);  // Dark gray text
            this.MenuBorderColor = Color.FromArgb(229, 229, 234);  // Medium gray
            
            // Main menu items
            this.MenuMainItemForeColor = Color.FromArgb(28, 28, 30);
            this.MenuMainItemHoverForeColor = Color.FromArgb(28, 28, 30);
            this.MenuMainItemHoverBackColor = Color.FromArgb(235, 235, 242);  // Darker gray hover
            this.MenuMainItemSelectedForeColor = Color.FromArgb(28, 28, 30);
            this.MenuMainItemSelectedBackColor = Color.FromArgb(220, 220, 230);  // Medium gray
            
            // Sub menu items
            this.MenuItemForeColor = Color.FromArgb(28, 28, 30);
            this.MenuItemHoverForeColor = Color.FromArgb(28, 28, 30);
            this.MenuItemHoverBackColor = Color.FromArgb(235, 235, 242);  // Darker gray hover
            this.MenuItemSelectedForeColor = Color.FromArgb(28, 28, 30);
            this.MenuItemSelectedBackColor = Color.FromArgb(220, 220, 230);  // Medium gray
            
            // Clean gradient
            this.MenuGradiantStartColor = Color.FromArgb(255, 255, 255);
            this.MenuGradiantEndColor = Color.FromArgb(250, 250, 252);
            this.MenuGradiantMiddleColor = Color.FromArgb(252, 252, 254);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
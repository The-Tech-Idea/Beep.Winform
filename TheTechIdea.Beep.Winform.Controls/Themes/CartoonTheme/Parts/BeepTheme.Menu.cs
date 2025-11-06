using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplyMenu()
        {
            // Cartoon menu - playful purple theme
            this.MenuTitleFont = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 14F, FontStyle.Bold);
            this.MenuItemSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 12F, FontStyle.Bold);
            this.MenuItemUnSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 12F, FontStyle.Regular);
            
            this.MenuBackColor = Color.FromArgb(255, 240, 255);  // Pink-tinted background
            this.MenuForeColor = Color.FromArgb(80, 0, 120);  // Purple text
            this.MenuBorderColor = Color.FromArgb(150, 100, 200);  // Purple border
            
            // Main menu items
            this.MenuMainItemForeColor = Color.FromArgb(80, 0, 120);
            this.MenuMainItemHoverForeColor = Color.FromArgb(80, 0, 120);
            this.MenuMainItemHoverBackColor = Color.FromArgb(240, 220, 255);  // Light purple hover
            this.MenuMainItemSelectedForeColor = Color.FromArgb(80, 0, 120);
            this.MenuMainItemSelectedBackColor = Color.FromArgb(220, 180, 255);  // Medium purple selected
            
            // Sub menu items
            this.MenuItemForeColor = Color.FromArgb(80, 0, 120);
            this.MenuItemHoverForeColor = Color.FromArgb(80, 0, 120);
            this.MenuItemHoverBackColor = Color.FromArgb(240, 220, 255);  // Light purple hover
            this.MenuItemSelectedForeColor = Color.FromArgb(80, 0, 120);
            this.MenuItemSelectedBackColor = Color.FromArgb(220, 180, 255);  // Medium purple selected
            
            // Playful gradient
            this.MenuGradiantStartColor = Color.FromArgb(255, 250, 255);
            this.MenuGradiantEndColor = Color.FromArgb(255, 230, 250);
            this.MenuGradiantMiddleColor = Color.FromArgb(255, 240, 255);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}
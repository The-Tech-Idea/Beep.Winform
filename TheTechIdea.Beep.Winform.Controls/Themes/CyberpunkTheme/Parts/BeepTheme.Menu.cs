using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyMenu()
        {
            // Cyberpunk menu - dark with neon cyan
            this.MenuTitleFont = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 14F, FontStyle.Bold);
            this.MenuItemSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 12F, FontStyle.Bold);
            this.MenuItemUnSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 12F, FontStyle.Regular);
            
            this.MenuBackColor = Color.FromArgb(10, 10, 20);  // Dark background
            this.MenuForeColor = Color.FromArgb(0, 255, 255);  // Cyan text
            this.MenuBorderColor = Color.FromArgb(0, 255, 255);  // Cyan border
            
            // Main menu items
            this.MenuMainItemForeColor = Color.FromArgb(0, 255, 255);
            this.MenuMainItemHoverForeColor = Color.FromArgb(100, 255, 255);
            this.MenuMainItemHoverBackColor = Color.FromArgb(20, 40, 40);  // Dark cyan glow
            this.MenuMainItemSelectedForeColor = Color.FromArgb(0, 255, 255);
            this.MenuMainItemSelectedBackColor = Color.FromArgb(30, 60, 60);  // Medium cyan
            
            // Sub menu items
            this.MenuItemForeColor = Color.FromArgb(0, 255, 255);
            this.MenuItemHoverForeColor = Color.FromArgb(100, 255, 255);
            this.MenuItemHoverBackColor = Color.FromArgb(20, 40, 40);  // Dark cyan glow
            this.MenuItemSelectedForeColor = Color.FromArgb(0, 255, 255);
            this.MenuItemSelectedBackColor = Color.FromArgb(30, 60, 60);  // Medium cyan
            
            // Neon gradient
            this.MenuGradiantStartColor = Color.FromArgb(10, 10, 20);
            this.MenuGradiantEndColor = Color.FromArgb(20, 20, 40);
            this.MenuGradiantMiddleColor = Color.FromArgb(15, 15, 30);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}
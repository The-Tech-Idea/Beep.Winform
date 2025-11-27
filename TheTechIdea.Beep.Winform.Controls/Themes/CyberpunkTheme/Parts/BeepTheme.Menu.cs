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
            
            this.MenuBackColor = BackgroundColor;  // Dark background
            this.MenuForeColor = ForeColor;  // Cyan text
            this.MenuBorderColor = BorderColor;  // Cyan border
            
            // Main menu items
            this.MenuMainItemForeColor = ForeColor;
            this.MenuMainItemHoverForeColor = PrimaryColor;
            this.MenuMainItemHoverBackColor = PanelGradiantMiddleColor;  // Dark cyan glow
            this.MenuMainItemSelectedForeColor = PrimaryColor;
            this.MenuMainItemSelectedBackColor = PanelBackColor;  // Medium cyan
            
            // Sub menu items
            this.MenuItemForeColor = ForeColor;
            this.MenuItemHoverForeColor = PrimaryColor;
            this.MenuItemHoverBackColor = PanelGradiantMiddleColor;  // Dark cyan glow
            this.MenuItemSelectedForeColor = PrimaryColor;
            this.MenuItemSelectedBackColor = PanelBackColor;  // Medium cyan
            
            // Neon gradient
            this.MenuGradiantStartColor = PanelGradiantStartColor;
            this.MenuGradiantEndColor = PanelGradiantEndColor;
            this.MenuGradiantMiddleColor = PanelGradiantMiddleColor;
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplyMenu()
        {
            // Holographic menu - futuristic gradient theme
            this.MenuTitleFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14F, FontStyle.Bold);
            this.MenuItemSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Bold);
            this.MenuItemUnSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Regular);
            
            this.MenuBackColor = BackgroundColor;  // Very dark purple
            this.MenuForeColor = ForeColor;  // Light purple text
            this.MenuBorderColor = BorderColor;  // Purple glow
            
            // Main menu items
            this.MenuMainItemForeColor = ForeColor;
            this.MenuMainItemHoverForeColor = PrimaryColor;
            this.MenuMainItemHoverBackColor = PanelGradiantStartColor;
            this.MenuMainItemSelectedForeColor = SecondaryColor;
            this.MenuMainItemSelectedBackColor = PanelGradiantMiddleColor;
            
            // Sub menu items
            this.MenuItemForeColor = ForeColor;
            this.MenuItemHoverForeColor = PrimaryColor;
            this.MenuItemHoverBackColor = PanelGradiantStartColor;
            this.MenuItemSelectedForeColor = SecondaryColor;
            this.MenuItemSelectedBackColor = PanelGradiantMiddleColor;
            
            // Gradient effect
            this.MenuGradiantStartColor = PanelGradiantStartColor;  // Pink
            this.MenuGradiantEndColor = PanelGradiantEndColor;  // Cyan
            this.MenuGradiantMiddleColor = PanelGradiantMiddleColor;  // Purple
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}
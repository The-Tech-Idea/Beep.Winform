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
            
            this.MenuBackColor = BackgroundColor;  // White
            this.MenuForeColor = ForeColor;  // Dark gray text
            this.MenuBorderColor = InactiveBorderColor;  // Light gray
            
            // Main menu items
            this.MenuMainItemForeColor = ForeColor;
            this.MenuMainItemHoverForeColor = ForeColor;
            this.MenuMainItemHoverBackColor = PanelGradiantStartColor;  // Light gray hover
            this.MenuMainItemSelectedForeColor = ForeColor;
            this.MenuMainItemSelectedBackColor = PanelGradiantMiddleColor;  // Medium gray selected
            
            // Sub menu items
            this.MenuItemForeColor = ForeColor;
            this.MenuItemHoverForeColor = ForeColor;
            this.MenuItemHoverBackColor = PanelGradiantStartColor;  // Light gray hover
            this.MenuItemSelectedForeColor = ForeColor;
            this.MenuItemSelectedBackColor = PanelGradiantMiddleColor;  // Medium gray selected
            
            // Clean gradient
            this.MenuGradiantStartColor = PanelGradiantStartColor;
            this.MenuGradiantEndColor = PanelGradiantEndColor;
            this.MenuGradiantMiddleColor = PanelGradiantMiddleColor;
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
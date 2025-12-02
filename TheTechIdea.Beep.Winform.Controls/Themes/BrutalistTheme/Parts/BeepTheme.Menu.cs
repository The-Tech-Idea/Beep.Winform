using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class BrutalistTheme
    {
        private void ApplyMenu()
        {
            // Brutalist menu - palette driven
            this.MenuTitleFont = ThemeUtils.ConvertFontToTypographyStyle("Courier New", 14F, FontStyle.Bold);
            this.MenuItemSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Courier New", 12F, FontStyle.Bold);
            this.MenuItemUnSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Courier New", 12F, FontStyle.Regular);
            this.MenuBackColor = SurfaceColor;
            this.MenuForeColor = ForeColor;
            this.MenuBorderColor = BorderColor;
            // Main menu items
            this.MenuMainItemForeColor = ForeColor;
            this.MenuMainItemHoverForeColor = ForeColor;
            this.MenuMainItemHoverBackColor = SecondaryColor;
            this.MenuMainItemSelectedForeColor = ForeColor;
            this.MenuMainItemSelectedBackColor = SecondaryColor;
            // Sub menu items - FIXED: Text must contrast with BeepStyling painted backgrounds
            // BeepStyling Brutalist paints Light gray background (242,242,242) in Normal state
            this.MenuItemForeColor = PrimaryColor;  // Black text on light gray (good contrast!)
            this.MenuItemHoverForeColor = PrimaryColor;  // Black text on gray hover
            this.MenuItemHoverBackColor = SecondaryColor;  // Medium gray (100,100,100)
            this.MenuItemSelectedForeColor = OnPrimaryColor;  // White on black when selected
            this.MenuItemSelectedBackColor = PrimaryColor;  // Black (0,0,0)
            // No gradient for brutalist aesthetic
            this.MenuGradiantStartColor = SurfaceColor;
            this.MenuGradiantEndColor = SurfaceColor;
            this.MenuGradiantMiddleColor = SurfaceColor;
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
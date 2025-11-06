using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme
    {
        private void ApplyMenu()
        {
            // ChatBubble menu - soft cyan theme
            this.MenuTitleFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14F, FontStyle.Bold);
            this.MenuItemSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Bold);
            this.MenuItemUnSelectedFont = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12F, FontStyle.Regular);
            
            this.MenuBackColor = Color.FromArgb(230, 250, 255);  // Light cyan
            this.MenuForeColor = Color.FromArgb(0, 0, 0);  // Black text
            this.MenuBorderColor = Color.FromArgb(200, 200, 200);  // Light gray
            
            // Main menu items
            this.MenuMainItemForeColor = Color.FromArgb(0, 0, 0);
            this.MenuMainItemHoverForeColor = Color.FromArgb(0, 0, 0);
            this.MenuMainItemHoverBackColor = Color.FromArgb(210, 240, 250);  // Light blue hover
            this.MenuMainItemSelectedForeColor = Color.FromArgb(0, 0, 0);
            this.MenuMainItemSelectedBackColor = Color.FromArgb(180, 230, 250);  // Medium blue selected
            
            // Sub menu items
            this.MenuItemForeColor = Color.FromArgb(0, 0, 0);
            this.MenuItemHoverForeColor = Color.FromArgb(0, 0, 0);
            this.MenuItemHoverBackColor = Color.FromArgb(210, 240, 250);  // Light blue hover
            this.MenuItemSelectedForeColor = Color.FromArgb(0, 0, 0);
            this.MenuItemSelectedBackColor = Color.FromArgb(180, 230, 250);  // Medium blue selected
            
            // Soft gradient
            this.MenuGradiantStartColor = Color.FromArgb(245, 248, 255);
            this.MenuGradiantEndColor = Color.FromArgb(220, 245, 250);
            this.MenuGradiantMiddleColor = Color.FromArgb(230, 250, 255);
            this.MenuGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
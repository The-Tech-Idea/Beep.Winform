using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyAppBar()
        {
            // Fluent AppBar - light, modern design
            this.AppBarBackColor = Color.FromArgb(245, 246, 248);  // Light gray-blue
            this.AppBarForeColor = Color.FromArgb(32, 32, 32);  // Dark gray text
            this.AppBarButtonForeColor = Color.FromArgb(0, 120, 215);  // Blue buttons
            this.AppBarButtonBackColor = Color.FromArgb(245, 246, 248);
            this.AppBarTextBoxBackColor = Color.FromArgb(255, 255, 255);  // White text boxes
            this.AppBarTextBoxForeColor = Color.FromArgb(32, 32, 32);
            this.AppBarLabelForeColor = Color.FromArgb(32, 32, 32);
            this.AppBarLabelBackColor = Color.FromArgb(245, 246, 248);
            this.AppBarTitleForeColor = Color.FromArgb(32, 32, 32);
            this.AppBarTitleBackColor = Color.FromArgb(245, 246, 248);
            this.AppBarSubTitleForeColor = Color.FromArgb(100, 100, 100);  // Medium gray
            this.AppBarSubTitleBackColor = Color.FromArgb(245, 246, 248);
            
            // System buttons - standard colors
            this.AppBarCloseButtonColor = Color.FromArgb(0, 0, 0);
            this.AppBarMaxButtonColor = Color.FromArgb(0, 0, 0);
            this.AppBarMinButtonColor = Color.FromArgb(0, 0, 0);
            
            // Typography
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 13f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.15f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 12f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.4f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 12f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.4f };
            
            // Subtle gradient
            this.AppBarGradiantStartColor = Color.FromArgb(245, 246, 248);
            this.AppBarGradiantEndColor = Color.FromArgb(235, 237, 240);
            this.AppBarGradiantMiddleColor = Color.FromArgb(240, 242, 245);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
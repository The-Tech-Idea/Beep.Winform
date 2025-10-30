using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GlassTheme
    {
        private void ApplyAppBar()
        {
            // Glass AppBar - frosted glass aesthetic
            this.AppBarBackColor = Color.FromArgb(236, 244, 255);  // Light blue
            this.AppBarForeColor = Color.FromArgb(17, 24, 39);  // Dark gray text
            this.AppBarButtonForeColor = Color.FromArgb(99, 102, 241);  // Indigo buttons
            this.AppBarButtonBackColor = Color.FromArgb(236, 244, 255);
            this.AppBarTextBoxBackColor = Color.FromArgb(250, 252, 255);  // Almost white
            this.AppBarTextBoxForeColor = Color.FromArgb(17, 24, 39);
            this.AppBarLabelForeColor = Color.FromArgb(17, 24, 39);
            this.AppBarLabelBackColor = Color.FromArgb(236, 244, 255);
            this.AppBarTitleForeColor = Color.FromArgb(17, 24, 39);
            this.AppBarTitleBackColor = Color.FromArgb(236, 244, 255);
            this.AppBarSubTitleForeColor = Color.FromArgb(100, 115, 140);  // Medium gray
            this.AppBarSubTitleBackColor = Color.FromArgb(236, 244, 255);
            
            // System buttons - standard colors
            this.AppBarCloseButtonColor = Color.FromArgb(0, 0, 0);
            this.AppBarMaxButtonColor = Color.FromArgb(0, 0, 0);
            this.AppBarMinButtonColor = Color.FromArgb(0, 0, 0);
            
            // Typography
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 14f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.2f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            
            // Frosted gradient
            this.AppBarGradiantStartColor = Color.FromArgb(240, 248, 255);
            this.AppBarGradiantEndColor = Color.FromArgb(220, 235, 250);
            this.AppBarGradiantMiddleColor = Color.FromArgb(230, 242, 255);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
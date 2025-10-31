using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordicTheme
    {
        private void ApplyAppBar()
        {
            // Nordic AppBar - Scandinavian minimalist design
            this.AppBarBackColor = Color.FromArgb(252, 252, 252);  // Light gray
            this.AppBarForeColor = Color.FromArgb(60, 60, 60);  // Dark gray text
            this.AppBarButtonForeColor = Color.FromArgb(60, 60, 60);  // Dark gray buttons
            this.AppBarButtonBackColor = Color.FromArgb(252, 252, 252);
            this.AppBarTextBoxBackColor = Color.FromArgb(255, 255, 255);  // White text boxes
            this.AppBarTextBoxForeColor = Color.FromArgb(60, 60, 60);
            this.AppBarLabelForeColor = Color.FromArgb(60, 60, 60);
            this.AppBarLabelBackColor = Color.FromArgb(252, 252, 252);
            this.AppBarTitleForeColor = Color.FromArgb(60, 60, 60);
            this.AppBarTitleBackColor = Color.FromArgb(252, 252, 252);
            this.AppBarSubTitleForeColor = Color.FromArgb(140, 140, 140);  // Medium gray
            this.AppBarSubTitleBackColor = Color.FromArgb(252, 252, 252);
            
            // System buttons - dark colors
            this.AppBarCloseButtonColor = Color.FromArgb(100, 100, 100);  // Dark gray
            this.AppBarMaxButtonColor = Color.FromArgb(100, 100, 100);
            this.AppBarMinButtonColor = Color.FromArgb(100, 100, 100);
            
            // Typography
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Inter", FontSize = 13.5f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.2f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Inter", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Inter", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            
            // Minimal gradient
            this.AppBarGradiantStartColor = Color.FromArgb(255, 255, 255);
            this.AppBarGradiantEndColor = Color.FromArgb(252, 252, 252);
            this.AppBarGradiantMiddleColor = Color.FromArgb(254, 254, 254);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
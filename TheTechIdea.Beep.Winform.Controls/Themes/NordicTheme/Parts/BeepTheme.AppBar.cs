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
            this.AppBarBackColor = SurfaceColor;  // Light gray
            this.AppBarForeColor = ForeColor;  // Dark gray text
            this.AppBarButtonForeColor = PrimaryColor;  // Dark gray buttons
            this.AppBarButtonBackColor = SurfaceColor;
            this.AppBarTextBoxBackColor = SurfaceColor;  // White text boxes
            this.AppBarTextBoxForeColor = ForeColor;
            this.AppBarLabelForeColor = ForeColor;
            this.AppBarLabelBackColor = SurfaceColor;
            this.AppBarTitleForeColor = ForeColor;
            this.AppBarTitleBackColor = SurfaceColor;
            this.AppBarSubTitleForeColor = Color.FromArgb(140, 140, 140);  // Medium gray
            this.AppBarSubTitleBackColor = SurfaceColor;
            
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

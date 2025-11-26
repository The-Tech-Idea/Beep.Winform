using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyAppBar()
        {
            // Solarized AppBar - scientifically crafted color palette
            this.AppBarBackColor = BackgroundColor;  // #073642 caption
            this.AppBarForeColor = ForeColor;  // Light beige text
            this.AppBarButtonForeColor = SecondaryColor;  // Cyan buttons
            this.AppBarButtonBackColor = BackgroundColor;
            this.AppBarTextBoxBackColor = SurfaceColor;
            this.AppBarTextBoxForeColor = ForeColor;
            this.AppBarLabelForeColor = ForeColor;
            this.AppBarLabelBackColor = BackgroundColor;
            this.AppBarTitleForeColor = ForeColor;
            this.AppBarTitleBackColor = BackgroundColor;
            this.AppBarSubTitleForeColor = Color.FromArgb(101, 123, 131);  // #657B83 dimmed blue
            this.AppBarSubTitleBackColor = Color.FromArgb(7, 54, 66);
            
            // System buttons - orange
            this.AppBarCloseButtonColor = Color.FromArgb(203, 75, 22);  // Orange
            this.AppBarMaxButtonColor = Color.FromArgb(203, 75, 22);
            this.AppBarMinButtonColor = Color.FromArgb(203, 75, 22);
            
            // Typography
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Source Sans 3", FontSize = 14f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.2f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Source Sans 3", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Source Sans 3", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            
            // Dark gradient
            this.AppBarGradiantStartColor = Color.FromArgb(0, 43, 54);
            this.AppBarGradiantEndColor = Color.FromArgb(0, 43, 54);
            this.AppBarGradiantMiddleColor = Color.FromArgb(0, 43, 54);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}

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
            this.AppBarSubTitleForeColor = ThemeUtil.Darken(ForeColor, 0.18);  // dimmed from ForeColor
            this.AppBarSubTitleBackColor = SurfaceColor;
            
            // System buttons - accent orange
            this.AppBarCloseButtonColor = AccentColor;
            this.AppBarMaxButtonColor = AccentColor;
            this.AppBarMinButtonColor = AccentColor;
            
            // Typography
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Source Sans 3", FontSize = 14f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.2f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Source Sans 3", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Source Sans 3", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            
            // Dark gradient based on background
            this.AppBarGradiantStartColor = BackgroundColor;
            this.AppBarGradiantEndColor = BackgroundColor;
            this.AppBarGradiantMiddleColor = ThemeUtil.Lighten(BackgroundColor, 0.04);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}

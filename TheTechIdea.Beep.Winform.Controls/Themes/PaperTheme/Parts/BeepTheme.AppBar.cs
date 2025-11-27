using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class PaperTheme
    {
        private void ApplyAppBar()
        {
            // Paper AppBar - Material Design aesthetic
            this.AppBarBackColor = BackgroundColor;  // Pure white
            this.AppBarForeColor = ForeColor;  // Dark grey text
            this.AppBarButtonForeColor = PrimaryColor;  // Medium grey buttons
            this.AppBarButtonBackColor = BackgroundColor;
            this.AppBarTextBoxBackColor = SurfaceColor;  // Pure white
            this.AppBarTextBoxForeColor = ForeColor;
            this.AppBarLabelForeColor = ForeColor;
            this.AppBarLabelBackColor = BackgroundColor;
            this.AppBarTitleForeColor = ForeColor;
            this.AppBarTitleBackColor = BackgroundColor;
            this.AppBarSubTitleForeColor = SecondaryTextColor;  // Medium grey subtitle
            this.AppBarSubTitleBackColor = BackgroundColor;
            
            // System buttons - Material Design colors
            this.AppBarCloseButtonColor = ForeColor;  // Medium grey
            this.AppBarMaxButtonColor = ForeColor;
            this.AppBarMinButtonColor = ForeColor;
            
            // Typography
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Roboto", FontSize = 16f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.25f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Roboto", FontSize = 13.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.52f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Roboto", FontSize = 13.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.52f };
            
            // Clean gradient
            this.AppBarGradiantStartColor = PanelGradiantStartColor;
            this.AppBarGradiantEndColor = PanelGradiantEndColor;
            this.AppBarGradiantMiddleColor = PanelGradiantMiddleColor;
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}

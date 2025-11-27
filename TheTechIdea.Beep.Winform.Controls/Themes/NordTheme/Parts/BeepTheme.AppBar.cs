using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordTheme
    {
        private void ApplyAppBar()
        {
            // Nord AppBar - Arctic-inspired dark theme
            this.AppBarBackColor = BackgroundColor;  // Dark blue-gray
            this.AppBarForeColor = ForeColor;  // Light gray-blue text
            this.AppBarButtonForeColor = PrimaryColor;  // Nord cyan buttons
            this.AppBarButtonBackColor = BackgroundColor;
            this.AppBarTextBoxBackColor = SurfaceColor;
            this.AppBarTextBoxForeColor = ForeColor;
            this.AppBarLabelForeColor = ForeColor;
            this.AppBarLabelBackColor = BackgroundColor;
            this.AppBarTitleForeColor = ForeColor;
            this.AppBarTitleBackColor = BackgroundColor;
            this.AppBarSubTitleForeColor = InactiveBorderColor;  // dimmed blue
            this.AppBarSubTitleBackColor = PanelBackColor;
            
            // System buttons - Nord cyan
            this.AppBarCloseButtonColor = PrimaryColor;  // Nord cyan
            this.AppBarMaxButtonColor = PrimaryColor;
            this.AppBarMinButtonColor = PrimaryColor;
            
            // Typography
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Nunito Sans", FontSize = 13.5f, FontWeight = FontWeight.SemiBold, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.18f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Nunito Sans", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Nunito Sans", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            
            // Dark gradient
            this.AppBarGradiantStartColor = PanelGradiantStartColor;
            this.AppBarGradiantEndColor = PanelGradiantEndColor;
            this.AppBarGradiantMiddleColor = PanelGradiantMiddleColor;
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}

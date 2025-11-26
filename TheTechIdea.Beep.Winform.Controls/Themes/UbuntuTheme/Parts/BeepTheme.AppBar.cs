using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplyAppBar()
        {
            // Ubuntu AppBar - Ubuntu Linux desktop aesthetic
            this.AppBarBackColor = BackgroundColor;  // Pure white
            this.AppBarForeColor = ForeColor;  // Dark grey text
            this.AppBarButtonForeColor = PrimaryColor;  // Ubuntu orange buttons
            this.AppBarButtonBackColor = BackgroundColor;
            this.AppBarTextBoxBackColor = SurfaceColor;  // Pure white
            this.AppBarTextBoxForeColor = ForeColor;
            this.AppBarLabelForeColor = ForeColor;
            this.AppBarLabelBackColor = BackgroundColor;
            this.AppBarTitleForeColor = ForeColor;
            this.AppBarTitleBackColor = BackgroundColor;
            this.AppBarSubTitleForeColor = Color.FromArgb(120, 120, 120);  // Medium grey subtitle
            this.AppBarSubTitleBackColor = BackgroundColor;
            
            // System buttons - dark colors
            this.AppBarCloseButtonColor = Color.FromArgb(44, 44, 44);  // Dark grey
            this.AppBarMaxButtonColor = Color.FromArgb(44, 44, 44);
            this.AppBarMinButtonColor = Color.FromArgb(44, 44, 44);
            
            // Typography
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Ubuntu", FontSize = 14f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.2f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Ubuntu", FontSize = 13f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Ubuntu", FontSize = 13f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            
            // Clean gradient
            this.AppBarGradiantStartColor = Color.White;
            this.AppBarGradiantEndColor = Color.FromArgb(242, 242, 245);
            this.AppBarGradiantMiddleColor = Color.FromArgb(250, 250, 253);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}

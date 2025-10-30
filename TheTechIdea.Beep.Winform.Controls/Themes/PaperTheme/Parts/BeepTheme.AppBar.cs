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
            this.AppBarBackColor = Color.White;  // Pure white
            this.AppBarForeColor = Color.FromArgb(33, 33, 33);  // Dark grey text
            this.AppBarButtonForeColor = Color.FromArgb(66, 66, 66);  // Medium grey buttons
            this.AppBarButtonBackColor = Color.White;
            this.AppBarTextBoxBackColor = Color.White;  // Pure white
            this.AppBarTextBoxForeColor = Color.FromArgb(33, 33, 33);
            this.AppBarLabelForeColor = Color.FromArgb(33, 33, 33);
            this.AppBarLabelBackColor = Color.White;
            this.AppBarTitleForeColor = Color.FromArgb(33, 33, 33);
            this.AppBarTitleBackColor = Color.White;
            this.AppBarSubTitleForeColor = Color.FromArgb(117, 117, 117);  // Medium grey subtitle
            this.AppBarSubTitleBackColor = Color.White;
            
            // System buttons - Material Design colors
            this.AppBarCloseButtonColor = Color.FromArgb(66, 66, 66);  // Medium grey
            this.AppBarMaxButtonColor = Color.FromArgb(66, 66, 66);
            this.AppBarMinButtonColor = Color.FromArgb(66, 66, 66);
            
            // Typography
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Roboto", FontSize = 16f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.25f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Roboto", FontSize = 13.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.52f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Roboto", FontSize = 13.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.52f };
            
            // Clean gradient
            this.AppBarGradiantStartColor = Color.White;
            this.AppBarGradiantEndColor = Color.FromArgb(250, 250, 250);
            this.AppBarGradiantMiddleColor = Color.FromArgb(255, 255, 255);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
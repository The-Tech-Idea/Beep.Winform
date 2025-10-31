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
            this.AppBarBackColor = Color.FromArgb(46, 52, 64);  // Dark blue-gray
            this.AppBarForeColor = Color.FromArgb(216, 222, 233);  // Light gray-blue text
            this.AppBarButtonForeColor = Color.FromArgb(136, 192, 208);  // Nord cyan buttons
            this.AppBarButtonBackColor = Color.FromArgb(46, 52, 64);
            this.AppBarTextBoxBackColor = Color.FromArgb(59, 66, 82);
            this.AppBarTextBoxForeColor = Color.FromArgb(216, 222, 233);
            this.AppBarLabelForeColor = Color.FromArgb(216, 222, 233);
            this.AppBarLabelBackColor = Color.FromArgb(46, 52, 64);
            this.AppBarTitleForeColor = Color.FromArgb(216, 222, 233);
            this.AppBarTitleBackColor = Color.FromArgb(46, 52, 64);
            this.AppBarSubTitleForeColor = Color.FromArgb(129, 161, 193);  // #81A1C1 dimmed blue
            this.AppBarSubTitleBackColor = Color.FromArgb(46, 52, 64);
            
            // System buttons - Nord cyan
            this.AppBarCloseButtonColor = Color.FromArgb(136, 192, 208);  // Nord cyan
            this.AppBarMaxButtonColor = Color.FromArgb(136, 192, 208);
            this.AppBarMinButtonColor = Color.FromArgb(136, 192, 208);
            
            // Typography
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Nunito Sans", FontSize = 13.5f, FontWeight = FontWeight.SemiBold, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.18f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Nunito Sans", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Nunito Sans", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            
            // Dark gradient
            this.AppBarGradiantStartColor = Color.FromArgb(59, 66, 82);
            this.AppBarGradiantEndColor = Color.FromArgb(59, 66, 82);
            this.AppBarGradiantMiddleColor = Color.FromArgb(46, 52, 64);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class iOSTheme
    {
        private void ApplyAppBar()
        {
            // iOS AppBar - clean, modern design
            this.AppBarBackColor = Color.FromArgb(248, 248, 252);  // Very light gray
            this.AppBarForeColor = Color.FromArgb(28, 28, 30);  // Dark gray text
            this.AppBarButtonForeColor = Color.FromArgb(10, 132, 255);  // iOS blue buttons
            this.AppBarButtonBackColor = Color.FromArgb(248, 248, 252);
            this.AppBarTextBoxBackColor = Color.FromArgb(255, 255, 255);  // White text boxes
            this.AppBarTextBoxForeColor = Color.FromArgb(28, 28, 30);
            this.AppBarLabelForeColor = Color.FromArgb(28, 28, 30);
            this.AppBarLabelBackColor = Color.FromArgb(248, 248, 252);
            this.AppBarTitleForeColor = Color.FromArgb(28, 28, 30);
            this.AppBarTitleBackColor = Color.FromArgb(248, 248, 252);
            this.AppBarSubTitleForeColor = Color.FromArgb(142, 142, 147);  // Medium gray
            this.AppBarSubTitleBackColor = Color.FromArgb(248, 248, 252);
            
            // System buttons - standard colors
            this.AppBarCloseButtonColor = Color.FromArgb(0, 0, 0);
            this.AppBarMaxButtonColor = Color.FromArgb(0, 0, 0);
            this.AppBarMinButtonColor = Color.FromArgb(0, 0, 0);
            
            // Typography
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "SF Pro Text", FontSize = 17f, FontWeight = FontWeight.SemiBold, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.2f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "SF Pro Text", FontSize = 13f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "SF Pro Text", FontSize = 13f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            
            // Clean gradient
            this.AppBarGradiantStartColor = Color.FromArgb(255, 255, 255);
            this.AppBarGradiantEndColor = Color.FromArgb(252, 252, 252);
            this.AppBarGradiantMiddleColor = Color.FromArgb(248, 248, 248);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
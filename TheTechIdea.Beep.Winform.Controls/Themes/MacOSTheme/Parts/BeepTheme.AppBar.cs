using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MacOSTheme
    {
        private void ApplyAppBar()
        {
            // MacOS AppBar - clean macOS design
            this.AppBarBackColor = Color.FromArgb(250, 250, 252);  // Light gray
            this.AppBarForeColor = Color.FromArgb(28, 28, 30);  // Dark gray text
            this.AppBarButtonForeColor = Color.FromArgb(80, 80, 80);  // Dark gray buttons
            this.AppBarButtonBackColor = Color.FromArgb(250, 250, 252);
            this.AppBarTextBoxBackColor = Color.FromArgb(255, 255, 255);  // White text boxes
            this.AppBarTextBoxForeColor = Color.FromArgb(28, 28, 30);
            this.AppBarLabelForeColor = Color.FromArgb(28, 28, 30);
            this.AppBarLabelBackColor = Color.FromArgb(250, 250, 252);
            this.AppBarTitleForeColor = Color.FromArgb(28, 28, 30);
            this.AppBarTitleBackColor = Color.FromArgb(250, 250, 252);
            this.AppBarSubTitleForeColor = Color.FromArgb(130, 130, 130);  // Medium gray
            this.AppBarSubTitleBackColor = Color.FromArgb(250, 250, 252);
            
            // System buttons - dark colors
            this.AppBarCloseButtonColor = Color.FromArgb(0, 0, 0);
            this.AppBarMaxButtonColor = Color.FromArgb(0, 0, 0);
            this.AppBarMinButtonColor = Color.FromArgb(0, 0, 0);
            
            // Typography
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "SF Pro Text", FontSize = 13f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.15f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "SF Pro Text", FontSize = 12f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.4f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "SF Pro Text", FontSize = 12f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.4f };
            
            // Clean gradient
            this.AppBarGradiantStartColor = Color.FromArgb(255, 255, 255);
            this.AppBarGradiantEndColor = Color.FromArgb(250, 250, 252);
            this.AppBarGradiantMiddleColor = Color.FromArgb(252, 252, 254);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
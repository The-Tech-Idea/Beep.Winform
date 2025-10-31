using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class KDETheme
    {
        private void ApplyAppBar()
        {
            // KDE AppBar - clean Linux design
            this.AppBarBackColor = Color.FromArgb(252, 252, 252);  // Light gray
            this.AppBarForeColor = Color.FromArgb(35, 38, 41);  // Dark gray text
            this.AppBarButtonForeColor = Color.FromArgb(61, 174, 233);  // KDE blue buttons
            this.AppBarButtonBackColor = Color.FromArgb(252, 252, 252);
            this.AppBarTextBoxBackColor = Color.FromArgb(255, 255, 255);  // White text boxes
            this.AppBarTextBoxForeColor = Color.FromArgb(33, 37, 41);
            this.AppBarLabelForeColor = Color.FromArgb(35, 38, 41);
            this.AppBarLabelBackColor = Color.FromArgb(252, 252, 252);
            this.AppBarTitleForeColor = Color.FromArgb(35, 38, 41);
            this.AppBarTitleBackColor = Color.FromArgb(252, 252, 252);
            this.AppBarSubTitleForeColor = Color.FromArgb(127, 140, 141);  // Medium gray
            this.AppBarSubTitleBackColor = Color.FromArgb(252, 252, 252);
            
            // System buttons - dark colors
            this.AppBarCloseButtonColor = Color.FromArgb(0, 0, 0);
            this.AppBarMaxButtonColor = Color.FromArgb(0, 0, 0);
            this.AppBarMinButtonColor = Color.FromArgb(0, 0, 0);
            
            // Typography
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Noto Sans", FontSize = 14f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.2f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Noto Sans", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Noto Sans", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            
            // Clean gradient
            this.AppBarGradiantStartColor = Color.FromArgb(255, 255, 255);
            this.AppBarGradiantEndColor = Color.FromArgb(248, 249, 250);
            this.AppBarGradiantMiddleColor = Color.FromArgb(252, 252, 253);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
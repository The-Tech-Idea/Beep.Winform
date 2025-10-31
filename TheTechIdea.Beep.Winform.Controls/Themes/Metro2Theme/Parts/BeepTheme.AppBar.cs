using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class Metro2Theme
    {
        private void ApplyAppBar()
        {
            // Metro2 AppBar - Windows Metro with accent stripe
            this.AppBarBackColor = Color.FromArgb(240, 240, 240);  // Light gray
            this.AppBarForeColor = Color.FromArgb(0, 0, 0);  // Black text
            this.AppBarButtonForeColor = Color.FromArgb(0, 120, 215);  // Metro blue buttons
            this.AppBarButtonBackColor = Color.FromArgb(240, 240, 240);
            this.AppBarTextBoxBackColor = Color.FromArgb(255, 255, 255);  // White text boxes
            this.AppBarTextBoxForeColor = Color.FromArgb(0, 0, 0);
            this.AppBarLabelForeColor = Color.FromArgb(0, 0, 0);
            this.AppBarLabelBackColor = Color.FromArgb(240, 240, 240);
            this.AppBarTitleForeColor = Color.FromArgb(0, 0, 0);
            this.AppBarTitleBackColor = Color.FromArgb(240, 240, 240);
            this.AppBarSubTitleForeColor = Color.FromArgb(100, 100, 100);  // Medium gray
            this.AppBarSubTitleBackColor = Color.FromArgb(240, 240, 240);
            
            // System buttons - Metro blue
            this.AppBarCloseButtonColor = Color.FromArgb(0, 120, 215);  // Metro blue
            this.AppBarMaxButtonColor = Color.FromArgb(0, 120, 215);
            this.AppBarMinButtonColor = Color.FromArgb(0, 120, 215);
            
            // Typography
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 14f, FontWeight = FontWeight.SemiBold, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.2f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            
            // Clean gradient
            this.AppBarGradiantStartColor = Color.FromArgb(255, 255, 255);
            this.AppBarGradiantEndColor = Color.FromArgb(240, 240, 240);
            this.AppBarGradiantMiddleColor = Color.FromArgb(248, 248, 248);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
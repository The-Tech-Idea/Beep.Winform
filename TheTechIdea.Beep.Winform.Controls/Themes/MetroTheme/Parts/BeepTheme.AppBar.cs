using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MetroTheme
    {
        private void ApplyAppBar()
        {
            // Metro AppBar - Windows Metro design
            this.AppBarBackColor = Color.FromArgb(0, 120, 215);  // Metro blue caption
            this.AppBarForeColor = Color.FromArgb(255, 255, 255);  // White text
            this.AppBarButtonForeColor = Color.FromArgb(255, 255, 255);  // White buttons
            this.AppBarButtonBackColor = Color.FromArgb(0, 120, 215);
            this.AppBarTextBoxBackColor = Color.FromArgb(255, 255, 255);  // White text boxes
            this.AppBarTextBoxForeColor = Color.FromArgb(32, 31, 30);
            this.AppBarLabelForeColor = Color.FromArgb(255, 255, 255);
            this.AppBarLabelBackColor = Color.FromArgb(0, 120, 215);
            this.AppBarTitleForeColor = Color.FromArgb(255, 255, 255);
            this.AppBarTitleBackColor = Color.FromArgb(0, 120, 215);
            this.AppBarSubTitleForeColor = Color.FromArgb(240, 240, 240);  // Light gray subtitle
            this.AppBarSubTitleBackColor = Color.FromArgb(0, 120, 215);
            
            // System buttons - white for Metro
            this.AppBarCloseButtonColor = Color.FromArgb(255, 255, 255);  // White
            this.AppBarMaxButtonColor = Color.FromArgb(255, 255, 255);
            this.AppBarMinButtonColor = Color.FromArgb(255, 255, 255);
            
            // Typography
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 14f, FontWeight = FontWeight.SemiBold, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.2f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Segoe UI", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            
            // Metro blue gradient
            this.AppBarGradiantStartColor = Color.FromArgb(0, 120, 215);
            this.AppBarGradiantEndColor = Color.FromArgb(0, 100, 195);
            this.AppBarGradiantMiddleColor = Color.FromArgb(0, 110, 205);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
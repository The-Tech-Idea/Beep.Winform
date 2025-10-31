using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyAppBar()
        {
            // Solarized AppBar - scientifically crafted color palette
            this.AppBarBackColor = Color.FromArgb(7, 54, 66);  // #073642 caption
            this.AppBarForeColor = Color.FromArgb(238, 232, 213);  // Light beige text
            this.AppBarButtonForeColor = Color.FromArgb(42, 161, 152);  // Cyan buttons
            this.AppBarButtonBackColor = Color.FromArgb(7, 54, 66);
            this.AppBarTextBoxBackColor = Color.FromArgb(0, 43, 54);
            this.AppBarTextBoxForeColor = Color.FromArgb(238, 232, 213);
            this.AppBarLabelForeColor = Color.FromArgb(238, 232, 213);
            this.AppBarLabelBackColor = Color.FromArgb(7, 54, 66);
            this.AppBarTitleForeColor = Color.FromArgb(238, 232, 213);
            this.AppBarTitleBackColor = Color.FromArgb(7, 54, 66);
            this.AppBarSubTitleForeColor = Color.FromArgb(101, 123, 131);  // #657B83 dimmed blue
            this.AppBarSubTitleBackColor = Color.FromArgb(7, 54, 66);
            
            // System buttons - orange
            this.AppBarCloseButtonColor = Color.FromArgb(203, 75, 22);  // Orange
            this.AppBarMaxButtonColor = Color.FromArgb(203, 75, 22);
            this.AppBarMinButtonColor = Color.FromArgb(203, 75, 22);
            
            // Typography
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Source Sans 3", FontSize = 14f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.2f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Source Sans 3", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Source Sans 3", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            
            // Dark gradient
            this.AppBarGradiantStartColor = Color.FromArgb(0, 43, 54);
            this.AppBarGradiantEndColor = Color.FromArgb(0, 43, 54);
            this.AppBarGradiantMiddleColor = Color.FromArgb(0, 43, 54);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
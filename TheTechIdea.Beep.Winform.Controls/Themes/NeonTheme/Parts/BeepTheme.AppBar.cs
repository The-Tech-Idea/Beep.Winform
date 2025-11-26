using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplyAppBar()
        {
            // Neon AppBar - vibrant neon aesthetic
            this.AppBarBackColor = BackgroundColor;  // Dark navy caption
            this.AppBarForeColor = ForeColor;  // Cyan text (#00FFFF)
            this.AppBarButtonForeColor = SecondaryColor;  // Cyan buttons
            this.AppBarButtonBackColor = BackgroundColor;
            this.AppBarTextBoxBackColor = SurfaceColor;
            this.AppBarTextBoxForeColor = ForeColor;
            this.AppBarLabelForeColor = ForeColor;
            this.AppBarLabelBackColor = BackgroundColor;
            this.AppBarTitleForeColor = ForeColor;
            this.AppBarTitleBackColor = BackgroundColor;
            this.AppBarSubTitleForeColor = Color.FromArgb(80, 180, 200);  // Dimmed cyan subtitle
            this.AppBarSubTitleBackColor = Color.FromArgb(20, 20, 35);
            
            // System buttons - cyan glow
            this.AppBarCloseButtonColor = Color.FromArgb(0, 255, 200);  // Cyan
            this.AppBarMaxButtonColor = Color.FromArgb(0, 255, 200);
            this.AppBarMinButtonColor = Color.FromArgb(0, 255, 200);
            
            // Typography
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Montserrat", FontSize = 13.5f, FontWeight = FontWeight.Bold, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LetterSpacing = 0.04f, LineHeight = 1.12f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Montserrat", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Montserrat", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            
            // Dark gradient
            this.AppBarGradiantStartColor = Color.FromArgb(20, 24, 38);
            this.AppBarGradiantEndColor = Color.FromArgb(15, 18, 30);
            this.AppBarGradiantMiddleColor = Color.FromArgb(10, 12, 20);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}

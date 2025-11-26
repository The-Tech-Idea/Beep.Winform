using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyAppBar()
        {
            // Cyberpunk AppBar - dark with neon cyan
            this.AppBarBackColor = BackgroundColor;  // Dark background
            this.AppBarForeColor = ForeColor;  // Cyan text
            this.AppBarButtonForeColor = ForeColor;
            this.AppBarButtonBackColor = BackgroundColor;
            this.AppBarTextBoxBackColor = SurfaceColor;
            this.AppBarTextBoxForeColor = ForeColor;
            this.AppBarLabelForeColor = ForeColor;
            this.AppBarLabelBackColor = BackgroundColor;
            this.AppBarTitleForeColor = ForeColor;
            this.AppBarTitleBackColor = BackgroundColor;
            this.AppBarSubTitleForeColor = Color.FromArgb(100, 200, 200);  // Lighter cyan
            this.AppBarSubTitleBackColor = BackgroundColor;
            
            // Neon system buttons - cyan
            this.AppBarCloseButtonColor = Color.FromArgb(0, 255, 255);
            this.AppBarMaxButtonColor = Color.FromArgb(0, 255, 255);
            this.AppBarMinButtonColor = Color.FromArgb(0, 255, 255);
            
            // Futuristic typography
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Rajdhani", FontSize = 13.5f, FontWeight = FontWeight.SemiBold, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.1f, LetterSpacing = 0.04f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Rajdhani", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = AppBarSubTitleForeColor, LineHeight = 1.45f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Rajdhani", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = AppBarForeColor, LineHeight = 1.45f };
            
            // Dark gradient
            this.AppBarGradiantStartColor = Color.FromArgb(10, 10, 20);
            this.AppBarGradiantEndColor = Color.FromArgb(20, 20, 40);
            this.AppBarGradiantMiddleColor = Color.FromArgb(15, 15, 30);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}

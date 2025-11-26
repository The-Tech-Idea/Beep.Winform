using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplyAppBar()
        {
            // Holographic AppBar - futuristic gradient
            this.AppBarBackColor = BackgroundColor;  // Very dark purple
            this.AppBarForeColor = ForeColor;  // Light purple text
            this.AppBarButtonForeColor = SecondaryColor;  // Cyan buttons
            this.AppBarButtonBackColor = BackgroundColor;
            this.AppBarTextBoxBackColor = SurfaceColor;
            this.AppBarTextBoxForeColor = ForeColor;
            this.AppBarLabelForeColor = ForeColor;
            this.AppBarLabelBackColor = BackgroundColor;
            this.AppBarTitleForeColor = ForeColor;
            this.AppBarTitleBackColor = BackgroundColor;
            this.AppBarSubTitleForeColor = AccentColor;  // Medium purple
            this.AppBarSubTitleBackColor = BackgroundColor;
            
            // System buttons - cyan
            this.AppBarCloseButtonColor = Color.FromArgb(150, 200, 255);  // Cyan
            this.AppBarMaxButtonColor = Color.FromArgb(150, 200, 255);
            this.AppBarMinButtonColor = Color.FromArgb(150, 200, 255);
            
            // Typography
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Sora", FontSize = 13.5f, FontWeight = FontWeight.SemiBold, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LetterSpacing = 0.03f, LineHeight = 1.14f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Sora", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.52f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Sora", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.52f };
            
            // Gradient effect
            this.AppBarGradiantStartColor = Color.FromArgb(255, 122, 217);  // Pink
            this.AppBarGradiantEndColor = Color.FromArgb(122, 252, 255);  // Cyan
            this.AppBarGradiantMiddleColor = Color.FromArgb(176, 141, 255);  // Purple
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}

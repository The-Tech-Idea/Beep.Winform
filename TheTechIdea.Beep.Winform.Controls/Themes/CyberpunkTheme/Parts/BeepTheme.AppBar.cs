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
            this.AppBarSubTitleForeColor = PrimaryColor;  // Lighter cyan
            this.AppBarSubTitleBackColor = BackgroundColor;
            
            // Neon system buttons - cyan
            this.AppBarCloseButtonColor = PrimaryColor;
            this.AppBarMaxButtonColor = PrimaryColor;
            this.AppBarMinButtonColor = PrimaryColor;
            
            // Futuristic typography
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Rajdhani", FontSize = 13.5f, FontWeight = FontWeight.SemiBold, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.1f, LetterSpacing = 0.04f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Rajdhani", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = AppBarSubTitleForeColor, LineHeight = 1.45f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Rajdhani", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = AppBarForeColor, LineHeight = 1.45f };
            
            // Dark gradient
            this.AppBarGradiantStartColor = PanelGradiantStartColor;
            this.AppBarGradiantEndColor = PanelGradiantEndColor;
            this.AppBarGradiantMiddleColor = PanelGradiantMiddleColor;
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}

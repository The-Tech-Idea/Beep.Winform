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
            this.AppBarSubTitleForeColor = InactiveBorderColor;  // Dimmed cyan subtitle
            this.AppBarSubTitleBackColor = PanelBackColor;
            
            // System buttons - cyan glow
            this.AppBarCloseButtonColor = BorderColor;  // Cyan
            this.AppBarMaxButtonColor = BorderColor;
            this.AppBarMinButtonColor = BorderColor;
            
            // Typography
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Montserrat", FontSize = 13.5f, FontWeight = FontWeight.Bold, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LetterSpacing = 0.04f, LineHeight = 1.12f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Montserrat", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Montserrat", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            
            // Dark gradient
            this.AppBarGradiantStartColor = PanelGradiantStartColor;
            this.AppBarGradiantEndColor = PanelGradiantEndColor;
            this.AppBarGradiantMiddleColor = PanelGradiantMiddleColor;
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}

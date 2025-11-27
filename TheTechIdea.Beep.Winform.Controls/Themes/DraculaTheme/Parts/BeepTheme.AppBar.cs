using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class DraculaTheme
    {
        private void ApplyAppBar()
        {
            // Dracula AppBar - dark with cream text
            this.AppBarBackColor = BackgroundColor;  // Dark background
            this.AppBarForeColor = ForeColor;  // Cream text
            this.AppBarButtonForeColor = PrimaryColor;  // Purple buttons
            this.AppBarButtonBackColor = BackgroundColor;
            this.AppBarTextBoxBackColor = SurfaceColor;
            this.AppBarTextBoxForeColor = ForeColor;
            this.AppBarLabelForeColor = ForeColor;
            this.AppBarLabelBackColor = BackgroundColor;
            this.AppBarTitleForeColor = ForeColor;
            this.AppBarTitleBackColor = BackgroundColor;
            this.AppBarSubTitleForeColor = AccentColor;  // Cyan subtitle
            this.AppBarSubTitleBackColor = BackgroundColor;
            
            // System buttons - purple/pink
            this.AppBarCloseButtonColor = ErrorColor;  // Red
            this.AppBarMaxButtonColor = WarningColor;  // Yellow
            this.AppBarMinButtonColor = SuccessColor;  // Green
            
            // Typography
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "JetBrains Mono", FontSize = 13.5f, FontWeight = FontWeight.SemiBold, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.15f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "JetBrains Mono", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = AppBarSubTitleForeColor, LineHeight = 1.5f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "JetBrains Mono", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = AppBarForeColor, LineHeight = 1.5f };
            
            // Purple gradient
            this.AppBarGradiantStartColor = PanelGradiantStartColor;
            this.AppBarGradiantEndColor = PanelGradiantEndColor;
            this.AppBarGradiantMiddleColor = PanelGradiantMiddleColor;
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}

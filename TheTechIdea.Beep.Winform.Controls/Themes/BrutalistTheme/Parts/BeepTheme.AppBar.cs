using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class BrutalistTheme
    {
        private void ApplyAppBar()
        {
            // Brutalist AppBar - white background with black text
            this.AppBarBackColor = BackgroundColor;  // Pure white
            this.AppBarForeColor = ForeColor;  // Black text
            this.AppBarButtonForeColor = ForeColor;  // Black buttons
            this.AppBarButtonBackColor = SurfaceColor;  // White button background
            this.AppBarTextBoxBackColor = SurfaceColor;  // White text boxes
            this.AppBarTextBoxForeColor = ForeColor;  // Black text
            this.AppBarLabelForeColor = ForeColor;  // Black labels
            this.AppBarLabelBackColor = BackgroundColor;  // White label background
            this.AppBarTitleForeColor = ForeColor;  // Black title
            this.AppBarTitleBackColor = BackgroundColor;  // White title background
            this.AppBarSubTitleForeColor = ForeColor;  // Black subtitle
            this.AppBarSubTitleBackColor = BackgroundColor;  // White subtitle background
            
            // System buttons - black for brutalist aesthetic
            this.AppBarCloseButtonColor = Color.FromArgb(0, 0, 0);
            this.AppBarMaxButtonColor = Color.FromArgb(0, 0, 0);
            this.AppBarMinButtonColor = Color.FromArgb(0, 0, 0);
            
            // Typography
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Inter", FontSize = 14f, FontWeight = FontWeight.Bold, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.1f, LetterSpacing = 0.02f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Inter", FontSize = 13f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.45f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Inter", FontSize = 13f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.45f };
            
            // No gradient for brutalist
            this.AppBarGradiantStartColor = Color.FromArgb(255, 255, 255);
            this.AppBarGradiantEndColor = Color.FromArgb(255, 255, 255);
            this.AppBarGradiantMiddleColor = Color.FromArgb(255, 255, 255);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
        }
    }
}

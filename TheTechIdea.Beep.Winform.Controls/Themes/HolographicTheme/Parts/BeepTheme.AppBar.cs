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
            this.AppBarBackColor = Color.FromArgb(25, 20, 35);  // Very dark purple
            this.AppBarForeColor = Color.FromArgb(200, 150, 255);  // Light purple text
            this.AppBarButtonForeColor = Color.FromArgb(122, 252, 255);  // Cyan buttons
            this.AppBarButtonBackColor = Color.FromArgb(25, 20, 35);
            this.AppBarTextBoxBackColor = Color.FromArgb(40, 30, 60);
            this.AppBarTextBoxForeColor = Color.FromArgb(200, 150, 255);
            this.AppBarLabelForeColor = Color.FromArgb(200, 150, 255);
            this.AppBarLabelBackColor = Color.FromArgb(25, 20, 35);
            this.AppBarTitleForeColor = Color.FromArgb(200, 150, 255);
            this.AppBarTitleBackColor = Color.FromArgb(25, 20, 35);
            this.AppBarSubTitleForeColor = Color.FromArgb(150, 100, 200);  // Medium purple
            this.AppBarSubTitleBackColor = Color.FromArgb(25, 20, 35);
            
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
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
            this.AppBarBackColor = Color.FromArgb(40, 42, 54);  // Dark background
            this.AppBarForeColor = Color.FromArgb(248, 248, 242);  // Cream text
            this.AppBarButtonForeColor = Color.FromArgb(189, 147, 249);  // Purple buttons
            this.AppBarButtonBackColor = Color.FromArgb(40, 42, 54);
            this.AppBarTextBoxBackColor = Color.FromArgb(68, 71, 90);
            this.AppBarTextBoxForeColor = Color.FromArgb(248, 248, 242);
            this.AppBarLabelForeColor = Color.FromArgb(248, 248, 242);
            this.AppBarLabelBackColor = Color.FromArgb(40, 42, 54);
            this.AppBarTitleForeColor = Color.FromArgb(248, 248, 242);
            this.AppBarTitleBackColor = Color.FromArgb(40, 42, 54);
            this.AppBarSubTitleForeColor = Color.FromArgb(139, 233, 253);  // Cyan subtitle
            this.AppBarSubTitleBackColor = Color.FromArgb(40, 42, 54);
            
            // System buttons - purple/pink
            this.AppBarCloseButtonColor = Color.FromArgb(255, 85, 85);  // Red
            this.AppBarMaxButtonColor = Color.FromArgb(241, 250, 140);  // Yellow
            this.AppBarMinButtonColor = Color.FromArgb(80, 250, 123);  // Green
            
            // Typography
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "JetBrains Mono", FontSize = 13.5f, FontWeight = FontWeight.SemiBold, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.15f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "JetBrains Mono", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "JetBrains Mono", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            
            // Purple gradient
            this.AppBarGradiantStartColor = Color.FromArgb(68, 71, 90);
            this.AppBarGradiantEndColor = Color.FromArgb(52, 55, 72);
            this.AppBarGradiantMiddleColor = Color.FromArgb(60, 63, 82);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}
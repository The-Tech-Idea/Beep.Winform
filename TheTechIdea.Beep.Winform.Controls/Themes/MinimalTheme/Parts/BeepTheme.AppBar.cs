using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyAppBar()
        {
            // Minimal AppBar - clean, minimal aesthetic
            this.AppBarBackColor = BackgroundColor;  // White
            this.AppBarForeColor = ForeColor;  // Dark grey text
            this.AppBarButtonForeColor = ForeColor;  // Dark grey buttons
            this.AppBarButtonBackColor = BackgroundColor;
            this.AppBarTextBoxBackColor = SurfaceColor;  // Very light grey
            this.AppBarTextBoxForeColor = ForeColor;
            this.AppBarLabelForeColor = ForeColor;
            this.AppBarLabelBackColor = BackgroundColor;
            this.AppBarTitleForeColor = ForeColor;
            this.AppBarTitleBackColor = BackgroundColor;
            this.AppBarSubTitleForeColor = SecondaryColor;  // Medium grey
            this.AppBarSubTitleBackColor = BackgroundColor;
            
            // System buttons - bright colors
            this.AppBarCloseButtonColor = MinimalCloseColor;  // Red (#F87171)
            this.AppBarMaxButtonColor = MinimalMaximizeColor;  // Green (#86EFAC)
            this.AppBarMinButtonColor = MinimalMinimizeColor;  // Yellow (#FDE047)
            
            // Typography
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Inter", FontSize = 13f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.15f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Inter", FontSize = 12f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.4f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Inter", FontSize = 12f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.4f };
            
            // Clean gradient
            this.AppBarGradiantStartColor = BackgroundColor;
            this.AppBarGradiantEndColor = BackgroundColor;
            this.AppBarGradiantMiddleColor = BackgroundColor;
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }

    }
}


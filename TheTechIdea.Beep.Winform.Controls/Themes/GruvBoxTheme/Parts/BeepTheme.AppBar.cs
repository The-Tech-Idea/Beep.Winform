using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GruvBoxTheme
    {
        private void ApplyAppBar()
        {
            // GruvBox AppBar - warm retro aesthetic
            this.AppBarBackColor = BackgroundColor;  // Dark gray
            this.AppBarForeColor = ForeColor;  // Beige text
            this.AppBarButtonForeColor = AccentColor;  // Orange buttons
            this.AppBarButtonBackColor = BackgroundColor;
            this.AppBarTextBoxBackColor = SurfaceColor;
            this.AppBarTextBoxForeColor = ForeColor;
            this.AppBarLabelForeColor = ForeColor;
            this.AppBarLabelBackColor = BackgroundColor;
            this.AppBarTitleForeColor = ForeColor;
            this.AppBarTitleBackColor = BackgroundColor;
            this.AppBarSubTitleForeColor = Color.FromArgb(146, 131, 116);  // #928374 muted brown
            this.AppBarSubTitleBackColor = Color.FromArgb(40, 40, 40);
            
            // System buttons - warm colors
            this.AppBarCloseButtonColor = Color.FromArgb(251, 73, 52);  // Red
            this.AppBarMaxButtonColor = Color.FromArgb(184, 187, 38);  // Yellow-green
            this.AppBarMinButtonColor = Color.FromArgb(142, 192, 124);  // Green
            
            // Typography
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Source Code Pro", FontSize = 13.5f, FontWeight = FontWeight.SemiBold, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.15f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Source Code Pro", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Source Code Pro", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            
            // Dark gradient
            this.AppBarGradiantStartColor = Color.FromArgb(60, 56, 54);
            this.AppBarGradiantEndColor = Color.FromArgb(50, 48, 47);
            this.AppBarGradiantMiddleColor = Color.FromArgb(40, 40, 40);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}

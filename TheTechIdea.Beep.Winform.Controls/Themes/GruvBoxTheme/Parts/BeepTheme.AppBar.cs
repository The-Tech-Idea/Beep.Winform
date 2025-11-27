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
            this.AppBarBackColor = SurfaceColor;
            this.AppBarForeColor = ForeColor;
            this.AppBarButtonForeColor = AccentColor;
            this.AppBarButtonBackColor = SurfaceColor;
            this.AppBarTextBoxBackColor = SurfaceColor;
            this.AppBarTextBoxForeColor = ForeColor;
            this.AppBarLabelForeColor = ForeColor;
            this.AppBarLabelBackColor = SurfaceColor;
            this.AppBarTitleForeColor = ForeColor;
            this.AppBarTitleBackColor = SurfaceColor;
            this.AppBarSubTitleForeColor = SecondaryColor;
            this.AppBarSubTitleBackColor = SurfaceColor;

            // System buttons - use palette tokens or accessible colors
            this.AppBarCloseButtonColor = ErrorColor;
            this.AppBarMaxButtonColor = WarningColor;
            this.AppBarMinButtonColor = SuccessColor;

            // Typography
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Source Code Pro", FontSize = 13.5f, FontWeight = FontWeight.SemiBold, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.15f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Source Code Pro", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = SecondaryColor, LineHeight = 1.5f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Source Code Pro", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };

            // Dark gradient
            this.AppBarGradiantStartColor = SecondaryColor;
            this.AppBarGradiantEndColor = SurfaceColor;
            this.AppBarGradiantMiddleColor = SurfaceColor;
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}

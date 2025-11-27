using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplyAppBar()
        {
            this.AppBarBackColor = SurfaceColor;
            this.AppBarForeColor = ForeColor;
            this.AppBarButtonForeColor = ForeColor;
            this.AppBarButtonBackColor = SurfaceColor;
            this.AppBarTextBoxBackColor = SurfaceColor;
            this.AppBarTextBoxForeColor = ForeColor;
            this.AppBarLabelForeColor = ForeColor;
            this.AppBarLabelBackColor = SurfaceColor;
            this.AppBarTitleForeColor = ForeColor;
            this.AppBarTitleBackColor = SurfaceColor;
            this.AppBarSubTitleForeColor = SecondaryColor;
            this.AppBarSubTitleBackColor = SurfaceColor;

            this.AppBarCloseButtonColor = ErrorColor;
            this.AppBarMaxButtonColor = AccentColor;
            this.AppBarMinButtonColor = WarningColor;

            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Comic Sans MS", FontSize = 16f, FontWeight = FontWeight.Bold, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.2f, LetterSpacing = 0.2f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Comic Sans MS", FontSize = 13f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = SecondaryColor, LineHeight = 1.5f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Comic Sans MS", FontSize = 13f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };

            this.AppBarGradiantStartColor = SecondaryColor;
            this.AppBarGradiantEndColor = SurfaceColor;
            this.AppBarGradiantMiddleColor = SurfaceColor;
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}

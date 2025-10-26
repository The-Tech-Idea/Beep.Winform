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
            this.AppBarBackColor = BackgroundColor;
            this.AppBarForeColor = ForeColor;
            this.AppBarButtonForeColor = ForeColor;
            this.AppBarButtonBackColor = BackgroundColor;
            this.AppBarTextBoxBackColor = SurfaceColor;
            this.AppBarTextBoxForeColor = ForeColor;
            this.AppBarLabelForeColor = ForeColor;
            this.AppBarLabelBackColor = BackgroundColor;
            this.AppBarTitleForeColor = ForeColor;
            this.AppBarTitleBackColor = BackgroundColor;
            this.AppBarSubTitleForeColor = SecondaryColor;
            this.AppBarSubTitleBackColor = BackgroundColor;
            this.AppBarCloseButtonColor = MinimalCloseColor;
            this.AppBarMaxButtonColor = MinimalMaximizeColor;
            this.AppBarMinButtonColor = MinimalMinimizeColor;
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Inter", FontSize = 13f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.15f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Inter", FontSize = 12f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.4f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Inter", FontSize = 12f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.4f };
            this.AppBarGradiantStartColor = BackgroundColor;
            this.AppBarGradiantEndColor = BackgroundColor;
            this.AppBarGradiantMiddleColor = BackgroundColor;
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }

    }
}


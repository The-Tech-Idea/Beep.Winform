using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyAppBar()
        {
            this.AppBarBackColor = BackgroundColor;
            this.AppBarForeColor = ForeColor;
            this.AppBarButtonForeColor = ForeColor;
            this.AppBarButtonBackColor = SurfaceColor;
            this.AppBarTextBoxBackColor = SurfaceColor;
            this.AppBarTextBoxForeColor = ForeColor;
            this.AppBarLabelForeColor = ForeColor;
            this.AppBarLabelBackColor = BackgroundColor;
            this.AppBarTitleForeColor = ForeColor;
            this.AppBarTitleBackColor = BackgroundColor;
            this.AppBarSubTitleForeColor = ThemeUtil.Lighten(ForeColor, 0.1);
            this.AppBarSubTitleBackColor = BackgroundColor;
            this.AppBarCloseButtonColor = AccentColor;
            this.AppBarMaxButtonColor = ForeColor;
            this.AppBarMinButtonColor = ForeColor;
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Noto Sans", FontSize = 14f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.2f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Noto Sans", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Noto Sans", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.AppBarGradiantStartColor = ThemeUtil.Darken(BackgroundColor, 0.05);
            this.AppBarGradiantEndColor = BackgroundColor;
            this.AppBarGradiantMiddleColor = ThemeUtil.Lighten(BackgroundColor, 0.05);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}

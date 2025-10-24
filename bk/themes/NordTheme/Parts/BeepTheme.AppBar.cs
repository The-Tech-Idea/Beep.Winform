using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordTheme
    {
        private void ApplyAppBar()
        {
            this.AppBarBackColor = ThemeUtil.Darken(BackgroundColor, 0.04);
            this.AppBarForeColor = Color.FromArgb(216,222,233);
            this.AppBarButtonForeColor = Color.FromArgb(216,222,233);
            this.AppBarButtonBackColor = ThemeUtil.Darken(BackgroundColor, 0.04);
            this.AppBarTextBoxBackColor = ThemeUtil.Darken(BackgroundColor, 0.04);
            this.AppBarTextBoxForeColor = Color.FromArgb(216,222,233);
            this.AppBarLabelForeColor = Color.FromArgb(216,222,233);
            this.AppBarLabelBackColor = ThemeUtil.Darken(BackgroundColor, 0.04);
            this.AppBarTitleForeColor = Color.FromArgb(216,222,233);
            this.AppBarTitleBackColor = ThemeUtil.Darken(BackgroundColor, 0.04);
            this.AppBarSubTitleForeColor = Color.FromArgb(216,222,233);
            this.AppBarSubTitleBackColor = ThemeUtil.Darken(BackgroundColor, 0.04);
            this.AppBarCloseButtonColor = Color.FromArgb(46,52,64);
            this.AppBarMaxButtonColor = Color.FromArgb(46,52,64);
            this.AppBarMinButtonColor = Color.FromArgb(46,52,64);
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Nunito Sans", FontSize = 13.5f, FontWeight = FontWeight.SemiBold, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.18f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Nunito Sans", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Nunito Sans", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.AppBarGradiantStartColor = Color.FromArgb(46,52,64);
            this.AppBarGradiantEndColor = Color.FromArgb(46,52,64);
            this.AppBarGradiantMiddleColor = Color.FromArgb(46,52,64);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
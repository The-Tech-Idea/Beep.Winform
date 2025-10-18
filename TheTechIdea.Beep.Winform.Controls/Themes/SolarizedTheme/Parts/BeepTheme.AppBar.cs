using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyAppBar()
        {
            this.AppBarBackColor = ThemeUtil.Darken(BackgroundColor, 0.04);
            this.AppBarForeColor = Color.FromArgb(147,161,161);
            this.AppBarButtonForeColor = Color.FromArgb(147,161,161);
            this.AppBarButtonBackColor = ThemeUtil.Darken(BackgroundColor, 0.04);
            this.AppBarTextBoxBackColor = ThemeUtil.Darken(BackgroundColor, 0.04);
            this.AppBarTextBoxForeColor = Color.FromArgb(147,161,161);
            this.AppBarLabelForeColor = Color.FromArgb(147,161,161);
            this.AppBarLabelBackColor = ThemeUtil.Darken(BackgroundColor, 0.04);
            this.AppBarTitleForeColor = Color.FromArgb(147,161,161);
            this.AppBarTitleBackColor = ThemeUtil.Darken(BackgroundColor, 0.04);
            this.AppBarSubTitleForeColor = Color.FromArgb(147,161,161);
            this.AppBarSubTitleBackColor = ThemeUtil.Darken(BackgroundColor, 0.04);
            this.AppBarCloseButtonColor = Color.FromArgb(0,43,54);
            this.AppBarMaxButtonColor = Color.FromArgb(0,43,54);
            this.AppBarMinButtonColor = Color.FromArgb(0,43,54);
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Source Sans 3", FontSize = 14f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.2f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Source Sans 3", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Source Sans 3", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.AppBarGradiantStartColor = Color.FromArgb(0,43,54);
            this.AppBarGradiantEndColor = Color.FromArgb(0,43,54);
            this.AppBarGradiantMiddleColor = Color.FromArgb(0,43,54);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordicTheme
    {
        private void ApplyAppBar()
        {
            this.AppBarBackColor = ThemeUtil.Lighten(BackgroundColor, 0.01);
            this.AppBarForeColor = Color.FromArgb(31,41,55);
            this.AppBarButtonForeColor = Color.FromArgb(31,41,55);
            this.AppBarButtonBackColor = ThemeUtil.Lighten(BackgroundColor, 0.01);
            this.AppBarTextBoxBackColor = ThemeUtil.Lighten(BackgroundColor, 0.01);
            this.AppBarTextBoxForeColor = Color.FromArgb(31,41,55);
            this.AppBarLabelForeColor = Color.FromArgb(31,41,55);
            this.AppBarLabelBackColor = ThemeUtil.Lighten(BackgroundColor, 0.01);
            this.AppBarTitleForeColor = Color.FromArgb(31,41,55);
            this.AppBarTitleBackColor = ThemeUtil.Lighten(BackgroundColor, 0.01);
            this.AppBarSubTitleForeColor = Color.FromArgb(31,41,55);
            this.AppBarSubTitleBackColor = ThemeUtil.Lighten(BackgroundColor, 0.01);
            this.AppBarCloseButtonColor = Color.FromArgb(250,250,251);
            this.AppBarMaxButtonColor = Color.FromArgb(250,250,251);
            this.AppBarMinButtonColor = Color.FromArgb(250,250,251);
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Inter", FontSize = 13.5f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.2f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Inter", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Inter", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.AppBarGradiantStartColor = Color.FromArgb(250,250,251);
            this.AppBarGradiantEndColor = Color.FromArgb(250,250,251);
            this.AppBarGradiantMiddleColor = Color.FromArgb(250,250,251);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
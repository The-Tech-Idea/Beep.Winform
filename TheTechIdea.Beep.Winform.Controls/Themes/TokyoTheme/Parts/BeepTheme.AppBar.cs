using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class TokyoTheme
    {
        private void ApplyAppBar()
        {
            this.AppBarBackColor = ThemeUtil.Darken(BackgroundColor, 0.05);
            this.AppBarForeColor = Color.FromArgb(192,202,245);
            this.AppBarButtonForeColor = Color.FromArgb(192,202,245);
            this.AppBarButtonBackColor = ThemeUtil.Darken(BackgroundColor, 0.05);
            this.AppBarTextBoxBackColor = ThemeUtil.Darken(BackgroundColor, 0.05);
            this.AppBarTextBoxForeColor = Color.FromArgb(192,202,245);
            this.AppBarLabelForeColor = Color.FromArgb(192,202,245);
            this.AppBarLabelBackColor = ThemeUtil.Darken(BackgroundColor, 0.05);
            this.AppBarTitleForeColor = Color.FromArgb(192,202,245);
            this.AppBarTitleBackColor = ThemeUtil.Darken(BackgroundColor, 0.05);
            this.AppBarSubTitleForeColor = Color.FromArgb(192,202,245);
            this.AppBarSubTitleBackColor = ThemeUtil.Darken(BackgroundColor, 0.05);
            this.AppBarCloseButtonColor = Color.FromArgb(26,27,38);
            this.AppBarMaxButtonColor = Color.FromArgb(26,27,38);
            this.AppBarMinButtonColor = Color.FromArgb(26,27,38);
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Inter", FontSize = 13.5f, FontWeight = FontWeight.SemiBold, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.16f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Inter", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Inter", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.AppBarGradiantStartColor = Color.FromArgb(26,27,38);
            this.AppBarGradiantEndColor = Color.FromArgb(26,27,38);
            this.AppBarGradiantMiddleColor = Color.FromArgb(26,27,38);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
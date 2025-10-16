using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyAppBar()
        {
            this.AppBarBackColor = ThemeUtil.Darken(BackgroundColor, 0.04);
            this.AppBarForeColor = Color.FromArgb(228,244,255);
            this.AppBarButtonForeColor = Color.FromArgb(228,244,255);
            this.AppBarButtonBackColor = ThemeUtil.Darken(BackgroundColor, 0.04);
            this.AppBarTextBoxBackColor = ThemeUtil.Darken(BackgroundColor, 0.04);
            this.AppBarTextBoxForeColor = Color.FromArgb(228,244,255);
            this.AppBarLabelForeColor = Color.FromArgb(228,244,255);
            this.AppBarLabelBackColor = ThemeUtil.Darken(BackgroundColor, 0.04);
            this.AppBarTitleForeColor = Color.FromArgb(228,244,255);
            this.AppBarTitleBackColor = ThemeUtil.Darken(BackgroundColor, 0.04);
            this.AppBarSubTitleForeColor = Color.FromArgb(228,244,255);
            this.AppBarSubTitleBackColor = ThemeUtil.Darken(BackgroundColor, 0.04);
            this.AppBarCloseButtonColor = Color.FromArgb(10,8,20);
            this.AppBarMaxButtonColor = Color.FromArgb(10,8,20);
            this.AppBarMinButtonColor = Color.FromArgb(10,8,20);
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Rajdhani", FontSize = 13.5f, FontWeight = FontWeight.SemiBold, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.1f, LetterSpacing = 0.04f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Rajdhani", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.45f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Rajdhani", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.45f };
            this.AppBarGradiantStartColor = Color.FromArgb(10,8,20);
            this.AppBarGradiantEndColor = Color.FromArgb(10,8,20);
            this.AppBarGradiantMiddleColor = Color.FromArgb(10,8,20);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}
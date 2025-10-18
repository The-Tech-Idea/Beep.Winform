using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class DraculaTheme
    {
        private void ApplyAppBar()
        {
            this.AppBarBackColor = ThemeUtil.Darken(BackgroundColor, 0.06);
            this.AppBarForeColor = Color.FromArgb(248,248,242);
            this.AppBarButtonForeColor = Color.FromArgb(248,248,242);
            this.AppBarButtonBackColor = ThemeUtil.Darken(BackgroundColor, 0.06);
            this.AppBarTextBoxBackColor = ThemeUtil.Darken(BackgroundColor, 0.06);
            this.AppBarTextBoxForeColor = Color.FromArgb(248,248,242);
            this.AppBarLabelForeColor = Color.FromArgb(248,248,242);
            this.AppBarLabelBackColor = ThemeUtil.Darken(BackgroundColor, 0.06);
            this.AppBarTitleForeColor = Color.FromArgb(248,248,242);
            this.AppBarTitleBackColor = ThemeUtil.Darken(BackgroundColor, 0.06);
            this.AppBarSubTitleForeColor = Color.FromArgb(248,248,242);
            this.AppBarSubTitleBackColor = ThemeUtil.Darken(BackgroundColor, 0.06);
            this.AppBarCloseButtonColor = Color.FromArgb(40,42,54);
            this.AppBarMaxButtonColor = Color.FromArgb(40,42,54);
            this.AppBarMinButtonColor = Color.FromArgb(40,42,54);
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "JetBrains Mono", FontSize = 13.5f, FontWeight = FontWeight.SemiBold, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.15f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "JetBrains Mono", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "JetBrains Mono", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.AppBarGradiantStartColor = Color.FromArgb(40,42,54);
            this.AppBarGradiantEndColor = Color.FromArgb(40,42,54);
            this.AppBarGradiantMiddleColor = Color.FromArgb(40,42,54);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}
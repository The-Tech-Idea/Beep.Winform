using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplyAppBar()
        {
            this.AppBarBackColor = ThemeUtil.Darken(BackgroundColor, 0.04);
            this.AppBarForeColor = Color.FromArgb(245,247,255);
            this.AppBarButtonForeColor = Color.FromArgb(245,247,255);
            this.AppBarButtonBackColor = ThemeUtil.Darken(BackgroundColor, 0.04);
            this.AppBarTextBoxBackColor = ThemeUtil.Darken(BackgroundColor, 0.04);
            this.AppBarTextBoxForeColor = Color.FromArgb(245,247,255);
            this.AppBarLabelForeColor = Color.FromArgb(245,247,255);
            this.AppBarLabelBackColor = ThemeUtil.Darken(BackgroundColor, 0.04);
            this.AppBarTitleForeColor = Color.FromArgb(245,247,255);
            this.AppBarTitleBackColor = ThemeUtil.Darken(BackgroundColor, 0.04);
            this.AppBarSubTitleForeColor = Color.FromArgb(245,247,255);
            this.AppBarSubTitleBackColor = ThemeUtil.Darken(BackgroundColor, 0.04);
            this.AppBarCloseButtonColor = Color.FromArgb(15,16,32);
            this.AppBarMaxButtonColor = Color.FromArgb(15,16,32);
            this.AppBarMinButtonColor = Color.FromArgb(15,16,32);
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Sora", FontSize = 13.5f, FontWeight = FontWeight.SemiBold, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LetterSpacing = 0.03f, LineHeight = 1.14f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Sora", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.52f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Sora", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.52f };
            this.AppBarGradiantStartColor = Color.FromArgb(15,16,32);
            this.AppBarGradiantEndColor = Color.FromArgb(15,16,32);
            this.AppBarGradiantMiddleColor = Color.FromArgb(15,16,32);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}
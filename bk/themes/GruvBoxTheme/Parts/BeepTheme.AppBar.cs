using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GruvBoxTheme
    {
        private void ApplyAppBar()
        {
            this.AppBarBackColor = ThemeUtil.Darken(BackgroundColor, 0.05);
            this.AppBarForeColor = Color.FromArgb(235,219,178);
            this.AppBarButtonForeColor = Color.FromArgb(235,219,178);
            this.AppBarButtonBackColor = ThemeUtil.Darken(BackgroundColor, 0.05);
            this.AppBarTextBoxBackColor = ThemeUtil.Darken(BackgroundColor, 0.05);
            this.AppBarTextBoxForeColor = Color.FromArgb(235,219,178);
            this.AppBarLabelForeColor = Color.FromArgb(235,219,178);
            this.AppBarLabelBackColor = ThemeUtil.Darken(BackgroundColor, 0.05);
            this.AppBarTitleForeColor = Color.FromArgb(235,219,178);
            this.AppBarTitleBackColor = ThemeUtil.Darken(BackgroundColor, 0.05);
            this.AppBarSubTitleForeColor = Color.FromArgb(235,219,178);
            this.AppBarSubTitleBackColor = ThemeUtil.Darken(BackgroundColor, 0.05);
            this.AppBarCloseButtonColor = Color.FromArgb(40,40,40);
            this.AppBarMaxButtonColor = Color.FromArgb(40,40,40);
            this.AppBarMinButtonColor = Color.FromArgb(40,40,40);
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Source Code Pro", FontSize = 13.5f, FontWeight = FontWeight.SemiBold, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.15f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Source Code Pro", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Source Code Pro", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.AppBarGradiantStartColor = Color.FromArgb(40,40,40);
            this.AppBarGradiantEndColor = Color.FromArgb(40,40,40);
            this.AppBarGradiantMiddleColor = Color.FromArgb(40,40,40);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
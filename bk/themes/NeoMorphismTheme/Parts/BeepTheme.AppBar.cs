using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplyAppBar()
        {
            this.AppBarBackColor = ThemeUtil.Lighten(BackgroundColor, 0.02);
            this.AppBarForeColor = Color.FromArgb(58,66,86);
            this.AppBarButtonForeColor = Color.FromArgb(58,66,86);
            this.AppBarButtonBackColor = ThemeUtil.Lighten(BackgroundColor, 0.02);
            this.AppBarTextBoxBackColor = ThemeUtil.Lighten(BackgroundColor, 0.02);
            this.AppBarTextBoxForeColor = Color.FromArgb(58,66,86);
            this.AppBarLabelForeColor = Color.FromArgb(58,66,86);
            this.AppBarLabelBackColor = ThemeUtil.Lighten(BackgroundColor, 0.02);
            this.AppBarTitleForeColor = Color.FromArgb(58,66,86);
            this.AppBarTitleBackColor = ThemeUtil.Lighten(BackgroundColor, 0.02);
            this.AppBarSubTitleForeColor = Color.FromArgb(58,66,86);
            this.AppBarSubTitleBackColor = ThemeUtil.Lighten(BackgroundColor, 0.02);
            this.AppBarCloseButtonColor = Color.FromArgb(236,240,243);
            this.AppBarMaxButtonColor = Color.FromArgb(236,240,243);
            this.AppBarMinButtonColor = Color.FromArgb(236,240,243);
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Poppins", FontSize = 13.5f, FontWeight = FontWeight.SemiBold, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.2f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Poppins", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Poppins", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.AppBarGradiantStartColor = Color.FromArgb(236,240,243);
            this.AppBarGradiantEndColor = Color.FromArgb(236,240,243);
            this.AppBarGradiantMiddleColor = Color.FromArgb(236,240,243);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}
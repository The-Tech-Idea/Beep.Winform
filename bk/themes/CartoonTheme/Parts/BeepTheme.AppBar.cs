using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplyAppBar()
        {
            this.AppBarBackColor = BackgroundColor;
            this.AppBarForeColor = Color.FromArgb(33,37,41);
            this.AppBarButtonForeColor = Color.FromArgb(33,37,41);
            this.AppBarButtonBackColor = BackgroundColor;
            this.AppBarTextBoxBackColor = BackgroundColor;
            this.AppBarTextBoxForeColor = Color.FromArgb(33,37,41);
            this.AppBarLabelForeColor = Color.FromArgb(33,37,41);
            this.AppBarLabelBackColor = BackgroundColor;
            this.AppBarTitleForeColor = Color.FromArgb(33,37,41);
            this.AppBarTitleBackColor = BackgroundColor;
            this.AppBarSubTitleForeColor = Color.FromArgb(33,37,41);
            this.AppBarSubTitleBackColor = BackgroundColor;
            this.AppBarCloseButtonColor = Color.FromArgb(255,251,235);
            this.AppBarMaxButtonColor = Color.FromArgb(255,251,235);
            this.AppBarMinButtonColor = Color.FromArgb(255,251,235);
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Comic Sans MS", FontSize = 16f, FontWeight = FontWeight.Bold, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.2f, LetterSpacing = 0.2f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Comic Sans MS", FontSize = 13f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Comic Sans MS", FontSize = 13f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.AppBarGradiantStartColor = Color.FromArgb(255,251,235);
            this.AppBarGradiantEndColor = Color.FromArgb(255,251,235);
            this.AppBarGradiantMiddleColor = Color.FromArgb(255,251,235);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}
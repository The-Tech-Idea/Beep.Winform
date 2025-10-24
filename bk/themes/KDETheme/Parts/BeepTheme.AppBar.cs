using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class KDETheme
    {
        private void ApplyAppBar()
        {
            this.AppBarBackColor = Color.White;
            this.AppBarForeColor = Color.FromArgb(33,37,41);
            this.AppBarButtonForeColor = Color.FromArgb(33,37,41);
            this.AppBarButtonBackColor = Color.White;
            this.AppBarTextBoxBackColor = Color.White;
            this.AppBarTextBoxForeColor = Color.FromArgb(33,37,41);
            this.AppBarLabelForeColor = Color.FromArgb(33,37,41);
            this.AppBarLabelBackColor = Color.White;
            this.AppBarTitleForeColor = Color.FromArgb(33,37,41);
            this.AppBarTitleBackColor = Color.White;
            this.AppBarSubTitleForeColor = Color.FromArgb(33,37,41);
            this.AppBarSubTitleBackColor = Color.White;
            this.AppBarCloseButtonColor = Color.FromArgb(248,249,250);
            this.AppBarMaxButtonColor = Color.FromArgb(248,249,250);
            this.AppBarMinButtonColor = Color.FromArgb(248,249,250);
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Noto Sans", FontSize = 14f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.2f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Noto Sans", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Noto Sans", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.AppBarGradiantStartColor = Color.FromArgb(248,249,250);
            this.AppBarGradiantEndColor = Color.FromArgb(248,249,250);
            this.AppBarGradiantMiddleColor = Color.FromArgb(248,249,250);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
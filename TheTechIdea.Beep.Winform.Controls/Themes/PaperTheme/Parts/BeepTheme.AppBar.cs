using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class PaperTheme
    {
        private void ApplyAppBar()
        {
            this.AppBarBackColor = Color.White;
            this.AppBarForeColor = Color.FromArgb(33,33,33);
            this.AppBarButtonForeColor = Color.FromArgb(33,33,33);
            this.AppBarButtonBackColor = Color.White;
            this.AppBarTextBoxBackColor = Color.White;
            this.AppBarTextBoxForeColor = Color.FromArgb(33,33,33);
            this.AppBarLabelForeColor = Color.FromArgb(33,33,33);
            this.AppBarLabelBackColor = Color.White;
            this.AppBarTitleForeColor = Color.FromArgb(33,33,33);
            this.AppBarTitleBackColor = Color.White;
            this.AppBarSubTitleForeColor = Color.FromArgb(33,33,33);
            this.AppBarSubTitleBackColor = Color.White;
            this.AppBarCloseButtonColor = Color.FromArgb(250,250,250);
            this.AppBarMaxButtonColor = Color.FromArgb(250,250,250);
            this.AppBarMinButtonColor = Color.FromArgb(250,250,250);
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Roboto", FontSize = 16f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.25f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Roboto", FontSize = 13.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.52f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Roboto", FontSize = 13.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.52f };
            this.AppBarGradiantStartColor = Color.FromArgb(250,250,250);
            this.AppBarGradiantEndColor = Color.FromArgb(250,250,250);
            this.AppBarGradiantMiddleColor = Color.FromArgb(250,250,250);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
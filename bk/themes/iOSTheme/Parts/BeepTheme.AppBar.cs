using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class iOSTheme
    {
        private void ApplyAppBar()
        {
            this.AppBarBackColor = Color.White;
            this.AppBarForeColor = Color.FromArgb(28,28,30);
            this.AppBarButtonForeColor = Color.FromArgb(28,28,30);
            this.AppBarButtonBackColor = Color.White;
            this.AppBarTextBoxBackColor = Color.White;
            this.AppBarTextBoxForeColor = Color.FromArgb(28,28,30);
            this.AppBarLabelForeColor = Color.FromArgb(28,28,30);
            this.AppBarLabelBackColor = Color.White;
            this.AppBarTitleForeColor = Color.FromArgb(28,28,30);
            this.AppBarTitleBackColor = Color.White;
            this.AppBarSubTitleForeColor = Color.FromArgb(28,28,30);
            this.AppBarSubTitleBackColor = Color.White;
            this.AppBarCloseButtonColor = Color.FromArgb(242,242,247);
            this.AppBarMaxButtonColor = Color.FromArgb(242,242,247);
            this.AppBarMinButtonColor = Color.FromArgb(242,242,247);
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "SF Pro Text", FontSize = 17f, FontWeight = FontWeight.SemiBold, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.2f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "SF Pro Text", FontSize = 13f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "SF Pro Text", FontSize = 13f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.AppBarGradiantStartColor = Color.FromArgb(242,242,247);
            this.AppBarGradiantEndColor = Color.FromArgb(242,242,247);
            this.AppBarGradiantMiddleColor = Color.FromArgb(242,242,247);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
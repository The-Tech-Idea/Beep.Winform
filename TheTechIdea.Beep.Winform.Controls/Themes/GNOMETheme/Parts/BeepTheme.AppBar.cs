using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GNOMETheme
    {
        private void ApplyAppBar()
        {
            this.AppBarBackColor = BackgroundColor;
            this.AppBarForeColor = Color.FromArgb(46,52,54);
            this.AppBarButtonForeColor = Color.FromArgb(46,52,54);
            this.AppBarButtonBackColor = BackgroundColor;
            this.AppBarTextBoxBackColor = BackgroundColor;
            this.AppBarTextBoxForeColor = Color.FromArgb(46,52,54);
            this.AppBarLabelForeColor = Color.FromArgb(46,52,54);
            this.AppBarLabelBackColor = BackgroundColor;
            this.AppBarTitleForeColor = Color.FromArgb(46,52,54);
            this.AppBarTitleBackColor = BackgroundColor;
            this.AppBarSubTitleForeColor = Color.FromArgb(46,52,54);
            this.AppBarSubTitleBackColor = BackgroundColor;
            this.AppBarCloseButtonColor = Color.FromArgb(246,245,244);
            this.AppBarMaxButtonColor = Color.FromArgb(246,245,244);
            this.AppBarMinButtonColor = Color.FromArgb(246,245,244);
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Cantarell", FontSize = 13.5f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.2f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Cantarell", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Cantarell", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.AppBarGradiantStartColor = Color.FromArgb(246,245,244);
            this.AppBarGradiantEndColor = Color.FromArgb(246,245,244);
            this.AppBarGradiantMiddleColor = Color.FromArgb(246,245,244);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
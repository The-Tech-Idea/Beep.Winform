using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class BrutalistTheme
    {
        private void ApplyAppBar()
        {
            this.AppBarBackColor = Color.White;
            this.AppBarForeColor = Color.FromArgb(20,20,20);
            this.AppBarButtonForeColor = Color.FromArgb(20,20,20);
            this.AppBarButtonBackColor = Color.White;
            this.AppBarTextBoxBackColor = Color.White;
            this.AppBarTextBoxForeColor = Color.FromArgb(20,20,20);
            this.AppBarLabelForeColor = Color.FromArgb(20,20,20);
            this.AppBarLabelBackColor = Color.White;
            this.AppBarTitleForeColor = Color.FromArgb(20,20,20);
            this.AppBarTitleBackColor = Color.White;
            this.AppBarSubTitleForeColor = Color.FromArgb(20,20,20);
            this.AppBarSubTitleBackColor = Color.White;
            this.AppBarCloseButtonColor = Color.FromArgb(250,250,250);
            this.AppBarMaxButtonColor = Color.FromArgb(250,250,250);
            this.AppBarMinButtonColor = Color.FromArgb(250,250,250);
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Inter", FontSize = 14f, FontWeight = FontWeight.Bold, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.1f, LetterSpacing = 0.02f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Inter", FontSize = 13f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.45f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Inter", FontSize = 13f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.45f };
            this.AppBarGradiantStartColor = Color.FromArgb(250,250,250);
            this.AppBarGradiantEndColor = Color.FromArgb(250,250,250);
            this.AppBarGradiantMiddleColor = Color.FromArgb(250,250,250);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
        }
    }
}
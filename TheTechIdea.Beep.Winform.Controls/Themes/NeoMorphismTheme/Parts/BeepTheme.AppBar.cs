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
            // NeoMorphism AppBar - soft neomorphic design
            this.AppBarBackColor = BackgroundColor;  // Light gray-blue
            this.AppBarForeColor = ForeColor;  // Dark gray text
            this.AppBarButtonForeColor = PrimaryColor;  // Dark gray buttons
            this.AppBarButtonBackColor = BackgroundColor;
            this.AppBarTextBoxBackColor = SurfaceColor;  // Slightly lighter
            this.AppBarTextBoxForeColor = ForeColor;
            this.AppBarLabelForeColor = ForeColor;
            this.AppBarLabelBackColor = BackgroundColor;
            this.AppBarTitleForeColor = ForeColor;
            this.AppBarTitleBackColor = BackgroundColor;
            this.AppBarSubTitleForeColor = Color.FromArgb(130, 130, 140);  // Medium gray
            this.AppBarSubTitleBackColor = BackgroundColor;
            
            // System buttons - dark colors
            this.AppBarCloseButtonColor = Color.FromArgb(80, 80, 90);  // Dark gray
            this.AppBarMaxButtonColor = Color.FromArgb(80, 80, 90);
            this.AppBarMinButtonColor = Color.FromArgb(80, 80, 90);
            
            // Typography
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Poppins", FontSize = 13.5f, FontWeight = FontWeight.SemiBold, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.2f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Poppins", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Poppins", FontSize = 12.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.55f };
            
            // Soft gradient
            this.AppBarGradiantStartColor = ThemeUtil.Lighten(BackgroundColor, 0.06);
            this.AppBarGradiantEndColor = ThemeUtil.Darken(BackgroundColor, 0.07);
            this.AppBarGradiantMiddleColor = ThemeUtil.Lighten(BackgroundColor, 0.03);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}

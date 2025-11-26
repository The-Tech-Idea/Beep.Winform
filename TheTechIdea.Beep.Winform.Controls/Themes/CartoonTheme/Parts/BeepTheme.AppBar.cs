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
            // Cartoon AppBar - playful pink background with purple text
            this.AppBarBackColor = BackColor;
            this.AppBarForeColor = ForeColor;
            this.AppBarButtonForeColor = ForeColor;
            this.AppBarButtonBackColor = BackColor;
            this.AppBarTextBoxBackColor = SurfaceColor;  // White text boxes
            this.AppBarTextBoxForeColor = ForeColor;
            this.AppBarLabelForeColor = ForeColor;
            this.AppBarLabelBackColor = BackColor;
            this.AppBarTitleForeColor = ForeColor;
            this.AppBarTitleBackColor = BackColor;
            this.AppBarSubTitleForeColor = Color.FromArgb(120, 60, 160);  // Lighter purple
            this.AppBarSubTitleBackColor = BackColor;
            
            // Playful system buttons - bright colors
            this.AppBarCloseButtonColor = Color.FromArgb(255, 69, 180);  // Pink
            this.AppBarMaxButtonColor = Color.FromArgb(255, 105, 180);  // Pink
            this.AppBarMinButtonColor = Color.FromArgb(255, 215, 0);  // Yellow
            
            // Playful typography
            this.AppBarTitleStyle = new TypographyStyle { FontFamily = "Comic Sans MS", FontSize = 16f, FontWeight = FontWeight.Bold, FontStyle = FontStyle.Regular, TextColor = AppBarTitleForeColor, LineHeight = 1.2f, LetterSpacing = 0.2f };
            this.AppBarSubTitleStyle = new TypographyStyle { FontFamily = "Comic Sans MS", FontSize = 13f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            this.AppBarTextStyle = new TypographyStyle { FontFamily = "Comic Sans MS", FontSize = 13f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = ForeColor, LineHeight = 1.5f };
            
            // Subtle gradient
            this.AppBarGradiantStartColor = Color.FromArgb(255, 250, 255);
            this.AppBarGradiantEndColor = Color.FromArgb(255, 230, 250);
            this.AppBarGradiantMiddleColor = Color.FromArgb(255, 240, 255);
            this.AppBarGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}

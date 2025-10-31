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
            this.AppBarBackColor = Color.FromArgb(255, 240, 255);  // Pink-tinted background
            this.AppBarForeColor = Color.FromArgb(80, 0, 120);  // Purple text
            this.AppBarButtonForeColor = Color.FromArgb(80, 0, 120);  // Purple buttons
            this.AppBarButtonBackColor = Color.FromArgb(255, 240, 255);
            this.AppBarTextBoxBackColor = Color.FromArgb(255, 255, 255);  // White text boxes
            this.AppBarTextBoxForeColor = Color.FromArgb(80, 0, 120);
            this.AppBarLabelForeColor = Color.FromArgb(80, 0, 120);
            this.AppBarLabelBackColor = Color.FromArgb(255, 240, 255);
            this.AppBarTitleForeColor = Color.FromArgb(80, 0, 120);
            this.AppBarTitleBackColor = Color.FromArgb(255, 240, 255);
            this.AppBarSubTitleForeColor = Color.FromArgb(120, 60, 160);  // Lighter purple
            this.AppBarSubTitleBackColor = Color.FromArgb(255, 240, 255);
            
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
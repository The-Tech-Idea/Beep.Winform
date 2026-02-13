using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class TokyoTheme
    {
        private void ApplyButtons()
        {
            // Tokyo Night buttons - inspired by Tokyo Night VSCode theme
            // Default: Dark purple background with light purple text
            this.ButtonBackColor = Color.FromArgb(52, 58, 82); // Dark purple
            this.ButtonForeColor = ForeColor;  // Light purple-blue
            this.ButtonBorderColor = BorderColor;  // #56617F
            
            // Hover: Slightly lighter
            this.ButtonHoverBackColor = ThemeUtil.Lighten(SurfaceColor, 0.06);  // #24283B
            this.ButtonHoverForeColor = PrimaryColor;  // Tokyo cyan
            this.ButtonHoverBorderColor = PrimaryColor;  // Tokyo cyan
            
            // Selected: Tokyo cyan background
            this.ButtonSelectedBackColor = PrimaryColor;  // Tokyo cyan
            this.ButtonSelectedForeColor = OnPrimaryColor;  // Dark text on cyan
            this.ButtonSelectedBorderColor = PrimaryColor;
            
            // Selected hover: Lighter cyan (map to PrimaryColor tokens)
            this.ButtonSelectedHoverBackColor = PrimaryColor;
            this.ButtonSelectedHoverForeColor = OnPrimaryColor;
            this.ButtonSelectedHoverBorderColor = PrimaryColor;
            
            // Pressed: Darker
            this.ButtonPressedBackColor = ThemeUtil.Darken(BackgroundColor, 0.08);
            this.ButtonPressedForeColor = ForeColor;
            this.ButtonPressedBorderColor = BorderColor;
            
            // Error button: Pink-red background with dark text
            this.ButtonErrorBackColor = ErrorColor;  // Pink-red
            this.ButtonErrorForeColor = OnPrimaryColor;  // Text on pink
            this.ButtonErrorBorderColor = ErrorColor;
        }
    }
}











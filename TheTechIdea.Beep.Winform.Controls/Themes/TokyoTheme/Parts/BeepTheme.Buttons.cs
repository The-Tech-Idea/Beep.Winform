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
            this.ButtonBackColor = BackgroundColor;  // Dark purple
            this.ButtonForeColor = ForeColor;  // Light purple-blue
            this.ButtonBorderColor = BorderColor;  // #56617F
            
            // Hover: Slightly lighter
            this.ButtonHoverBackColor = Color.FromArgb(36, 40, 59);  // #24283B
            this.ButtonHoverForeColor = PrimaryColor;  // Tokyo cyan
            this.ButtonHoverBorderColor = PrimaryColor;  // Tokyo cyan
            
            // Selected: Tokyo cyan background
            this.ButtonSelectedBackColor = PrimaryColor;  // Tokyo cyan
            this.ButtonSelectedForeColor = OnPrimaryColor;  // Dark text on cyan
            this.ButtonSelectedBorderColor = PrimaryColor;
            
            // Selected hover: Lighter cyan
            this.ButtonSelectedHoverBackColor = Color.FromArgb(142, 182, 255);
            this.ButtonSelectedHoverForeColor = OnPrimaryColor;
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(142, 182, 255);
            
            // Pressed: Darker
            this.ButtonPressedBackColor = Color.FromArgb(18, 19, 28);
            this.ButtonPressedForeColor = ForeColor;
            this.ButtonPressedBorderColor = BorderColor;
            
            // Error button: Pink-red background with dark text
            this.ButtonErrorBackColor = ErrorColor;  // Pink-red
            this.ButtonErrorForeColor = OnPrimaryColor;  // Text on pink
            this.ButtonErrorBorderColor = ErrorColor;
        }
    }
}

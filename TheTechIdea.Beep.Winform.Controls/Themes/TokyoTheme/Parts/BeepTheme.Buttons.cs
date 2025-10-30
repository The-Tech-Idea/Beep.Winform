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
            this.ButtonBackColor = Color.FromArgb(26, 27, 38);  // Dark purple
            this.ButtonForeColor = Color.FromArgb(169, 177, 214);  // Light purple-blue
            this.ButtonBorderColor = Color.FromArgb(86, 95, 137);  // #56617F
            
            // Hover: Slightly lighter
            this.ButtonHoverBackColor = Color.FromArgb(36, 40, 59);  // #24283B
            this.ButtonHoverForeColor = Color.FromArgb(122, 162, 247);  // Tokyo cyan
            this.ButtonHoverBorderColor = Color.FromArgb(122, 162, 247);  // Tokyo cyan
            
            // Selected: Tokyo cyan background
            this.ButtonSelectedBackColor = Color.FromArgb(122, 162, 247);  // Tokyo cyan
            this.ButtonSelectedForeColor = Color.FromArgb(26, 27, 38);  // Dark text on cyan
            this.ButtonSelectedBorderColor = Color.FromArgb(122, 162, 247);
            
            // Selected hover: Lighter cyan
            this.ButtonSelectedHoverBackColor = Color.FromArgb(142, 182, 255);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(26, 27, 38);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(142, 182, 255);
            
            // Pressed: Darker
            this.ButtonPressedBackColor = Color.FromArgb(18, 19, 28);
            this.ButtonPressedForeColor = Color.FromArgb(169, 177, 214);
            this.ButtonPressedBorderColor = Color.FromArgb(86, 95, 137);
            
            // Error button: Pink-red background with dark text
            this.ButtonErrorBackColor = Color.FromArgb(247, 118, 142);  // Pink-red
            this.ButtonErrorForeColor = Color.FromArgb(26, 27, 38);  // Dark text on pink
            this.ButtonErrorBorderColor = Color.FromArgb(220, 0, 60);
        }
    }
}
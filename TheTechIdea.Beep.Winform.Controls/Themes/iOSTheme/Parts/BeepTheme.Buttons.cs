using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class iOSTheme
    {
        private void ApplyButtons()
        {
            this.ButtonHoverBackColor = Color.FromArgb(242,242,247);
            this.ButtonHoverForeColor = Color.FromArgb(28,28,30);
            this.ButtonHoverBorderColor = Color.FromArgb(198,198,207);
            this.ButtonSelectedBorderColor = Color.FromArgb(198,198,207);
            this.ButtonSelectedBackColor = Color.FromArgb(242,242,247);
            this.ButtonSelectedForeColor = Color.FromArgb(28,28,30);
            this.ButtonSelectedHoverBackColor = Color.FromArgb(242,242,247);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(28,28,30);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(198,198,207);
            this.ButtonBackColor = Color.FromArgb(242,242,247);
            this.ButtonForeColor = Color.FromArgb(28,28,30);
            this.ButtonBorderColor = Color.FromArgb(198,198,207);
            this.ButtonErrorBackColor = Color.FromArgb(242,242,247);
            this.ButtonErrorForeColor = Color.FromArgb(28,28,30);
            this.ButtonErrorBorderColor = Color.FromArgb(198,198,207);
            this.ButtonPressedBackColor = Color.FromArgb(242,242,247);
            this.ButtonPressedForeColor = Color.FromArgb(28,28,30);
            this.ButtonPressedBorderColor = Color.FromArgb(198,198,207);
        }
    }
}
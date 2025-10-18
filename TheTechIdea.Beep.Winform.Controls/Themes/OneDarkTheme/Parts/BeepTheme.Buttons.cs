using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class OneDarkTheme
    {
        private void ApplyButtons()
        {
            this.ButtonHoverBackColor = Color.FromArgb(40,44,52);
            this.ButtonHoverForeColor = Color.FromArgb(171,178,191);
            this.ButtonHoverBorderColor = Color.FromArgb(92,99,112);
            this.ButtonSelectedBorderColor = Color.FromArgb(92,99,112);
            this.ButtonSelectedBackColor = Color.FromArgb(40,44,52);
            this.ButtonSelectedForeColor = Color.FromArgb(171,178,191);
            this.ButtonSelectedHoverBackColor = Color.FromArgb(40,44,52);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(171,178,191);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(92,99,112);
            this.ButtonBackColor = Color.FromArgb(40,44,52);
            this.ButtonForeColor = Color.FromArgb(171,178,191);
            this.ButtonBorderColor = Color.FromArgb(92,99,112);
            this.ButtonErrorBackColor = Color.FromArgb(40,44,52);
            this.ButtonErrorForeColor = Color.FromArgb(171,178,191);
            this.ButtonErrorBorderColor = Color.FromArgb(92,99,112);
            this.ButtonPressedBackColor = Color.FromArgb(40,44,52);
            this.ButtonPressedForeColor = Color.FromArgb(171,178,191);
            this.ButtonPressedBorderColor = Color.FromArgb(92,99,112);
        }
    }
}
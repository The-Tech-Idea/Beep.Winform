using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyButtons()
        {
            this.ButtonHoverBackColor = Color.FromArgb(0,43,54);
            this.ButtonHoverForeColor = Color.FromArgb(147,161,161);
            this.ButtonHoverBorderColor = Color.FromArgb(88,110,117);
            this.ButtonSelectedBorderColor = Color.FromArgb(88,110,117);
            this.ButtonSelectedBackColor = Color.FromArgb(0,43,54);
            this.ButtonSelectedForeColor = Color.FromArgb(147,161,161);
            this.ButtonSelectedHoverBackColor = Color.FromArgb(0,43,54);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(147,161,161);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(88,110,117);
            this.ButtonBackColor = Color.FromArgb(0,43,54);
            this.ButtonForeColor = Color.FromArgb(147,161,161);
            this.ButtonBorderColor = Color.FromArgb(88,110,117);
            this.ButtonErrorBackColor = Color.FromArgb(0,43,54);
            this.ButtonErrorForeColor = Color.FromArgb(147,161,161);
            this.ButtonErrorBorderColor = Color.FromArgb(88,110,117);
            this.ButtonPressedBackColor = Color.FromArgb(0,43,54);
            this.ButtonPressedForeColor = Color.FromArgb(147,161,161);
            this.ButtonPressedBorderColor = Color.FromArgb(88,110,117);
        }
    }
}
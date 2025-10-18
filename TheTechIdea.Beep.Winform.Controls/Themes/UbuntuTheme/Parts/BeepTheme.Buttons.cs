using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplyButtons()
        {
            this.ButtonHoverBackColor = Color.FromArgb(242,242,245);
            this.ButtonHoverForeColor = Color.FromArgb(44,44,44);
            this.ButtonHoverBorderColor = Color.FromArgb(218,218,222);
            this.ButtonSelectedBorderColor = Color.FromArgb(218,218,222);
            this.ButtonSelectedBackColor = Color.FromArgb(242,242,245);
            this.ButtonSelectedForeColor = Color.FromArgb(44,44,44);
            this.ButtonSelectedHoverBackColor = Color.FromArgb(242,242,245);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(44,44,44);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(218,218,222);
            this.ButtonBackColor = Color.FromArgb(242,242,245);
            this.ButtonForeColor = Color.FromArgb(44,44,44);
            this.ButtonBorderColor = Color.FromArgb(218,218,222);
            this.ButtonErrorBackColor = Color.FromArgb(192,28,40);
            this.ButtonErrorForeColor = Color.FromArgb(44,44,44);
            this.ButtonErrorBorderColor = Color.FromArgb(218,218,222);
            this.ButtonPressedBackColor = Color.FromArgb(242,242,245);
            this.ButtonPressedForeColor = Color.FromArgb(44,44,44);
            this.ButtonPressedBorderColor = Color.FromArgb(218,218,222);
        }
    }
}
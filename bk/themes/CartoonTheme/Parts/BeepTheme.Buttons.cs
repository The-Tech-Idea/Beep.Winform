using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplyButtons()
        {
            this.ButtonHoverBackColor = Color.FromArgb(255,251,235);
            this.ButtonHoverForeColor = Color.FromArgb(33,37,41);
            this.ButtonHoverBorderColor = Color.FromArgb(247,208,136);
            this.ButtonSelectedBorderColor = Color.FromArgb(247,208,136);
            this.ButtonSelectedBackColor = Color.FromArgb(255,251,235);
            this.ButtonSelectedForeColor = Color.FromArgb(33,37,41);
            this.ButtonSelectedHoverBackColor = Color.FromArgb(255,251,235);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(33,37,41);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(247,208,136);
            this.ButtonBackColor = Color.FromArgb(255,251,235);
            this.ButtonForeColor = Color.FromArgb(33,37,41);
            this.ButtonBorderColor = Color.FromArgb(247,208,136);
            this.ButtonErrorBackColor = Color.FromArgb(255,82,82);
            this.ButtonErrorForeColor = Color.FromArgb(33,37,41);
            this.ButtonErrorBorderColor = Color.FromArgb(247,208,136);
            this.ButtonPressedBackColor = Color.FromArgb(255,251,235);
            this.ButtonPressedForeColor = Color.FromArgb(33,37,41);
            this.ButtonPressedBorderColor = Color.FromArgb(247,208,136);
        }
    }
}
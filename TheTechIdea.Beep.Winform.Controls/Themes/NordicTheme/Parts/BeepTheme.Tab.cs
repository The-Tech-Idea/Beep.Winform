using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordicTheme
    {
        private void ApplyTab()
        {
            this.TabBackColor = Color.FromArgb(250,250,251);
            this.TabForeColor = Color.FromArgb(31,41,55);
            this.TabBorderColor = Color.FromArgb(229,231,235);
            this.TabHoverBackColor = Color.FromArgb(250,250,251);
            this.TabHoverForeColor = Color.FromArgb(31,41,55);
            this.TabSelectedBackColor = Color.FromArgb(250,250,251);
            this.TabSelectedForeColor = Color.FromArgb(31,41,55);
            this.TabSelectedBorderColor = Color.FromArgb(229,231,235);
            this.TabHoverBorderColor = Color.FromArgb(229,231,235);
        }
    }
}
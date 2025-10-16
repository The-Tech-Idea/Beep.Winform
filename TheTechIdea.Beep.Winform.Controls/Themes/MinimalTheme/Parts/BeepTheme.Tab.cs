using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyTab()
        {
            this.TabBackColor = Color.FromArgb(255,255,255);
            this.TabForeColor = Color.FromArgb(31,41,55);
            this.TabBorderColor = Color.FromArgb(209,213,219);
            this.TabHoverBackColor = Color.FromArgb(255,255,255);
            this.TabHoverForeColor = Color.FromArgb(31,41,55);
            this.TabSelectedBackColor = Color.FromArgb(255,255,255);
            this.TabSelectedForeColor = Color.FromArgb(31,41,55);
            this.TabSelectedBorderColor = Color.FromArgb(209,213,219);
            this.TabHoverBorderColor = Color.FromArgb(209,213,219);
        }
    }
}
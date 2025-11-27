using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplyLabels()
        {
            this.LabelBackColor = SurfaceColor;
            this.LabelForeColor = ForeColor;
            this.LabelBorderColor = BorderColor;
            this.LabelHoverBorderColor = BorderColor;
            this.LabelHoverBackColor = SecondaryColor;
            this.LabelHoverForeColor = ForeColor;
            this.LabelSelectedBorderColor = BorderColor;
            this.LabelSelectedBackColor = AccentColor;
            this.LabelSelectedForeColor = ForeColor;
            this.LabelDisabledBackColor = SurfaceColor;
            this.LabelDisabledForeColor = ForeColor;
            this.LabelDisabledBorderColor = BorderColor;
        }
    }
}
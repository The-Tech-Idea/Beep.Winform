using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyLabels()
        {
            this.LabelBackColor = SurfaceColor;
            this.LabelForeColor = ForeColor;
            this.LabelBorderColor = BorderColor;
            this.LabelHoverBorderColor = ActiveBorderColor;
            this.LabelHoverBackColor = SecondaryColor;
            this.LabelHoverForeColor = ForeColor;
            this.LabelSelectedBorderColor = PrimaryColor;
            this.LabelSelectedBackColor = PrimaryColor;
            this.LabelSelectedForeColor = OnPrimaryColor;
            this.LabelDisabledBackColor = DisabledColor;
            this.LabelDisabledForeColor = OnDisabledColor;
            this.LabelDisabledBorderColor = DisabledBorderColor;
        }
    }
}
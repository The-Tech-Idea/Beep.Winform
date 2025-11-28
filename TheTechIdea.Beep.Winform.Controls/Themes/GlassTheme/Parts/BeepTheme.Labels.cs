using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GlassTheme
    {
        private void ApplyLabels()
        {
            this.LabelBackColor = SurfaceColor;
            this.LabelForeColor = ForeColor;
            this.LabelBorderColor = InactiveBorderColor;
            this.LabelHoverBorderColor = ActiveBorderColor;
            this.LabelHoverBackColor = PanelGradiantStartColor;
            this.LabelHoverForeColor = ForeColor;
            this.LabelSelectedBorderColor = PrimaryColor;
            this.LabelSelectedBackColor = PrimaryColor;
            this.LabelSelectedForeColor = OnPrimaryColor;
            this.LabelDisabledBackColor = BackgroundColor;
            this.LabelDisabledForeColor = InactiveBorderColor;
            this.LabelDisabledBorderColor = InactiveBorderColor;
        }
    }
}
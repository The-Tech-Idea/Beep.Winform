using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme
    {
        private void ApplyLabels()
        {
            this.LabelBackColor = PanelGradiantStartColor;
            this.LabelForeColor = ForeColor;
            this.LabelBorderColor = BorderColor;
            this.LabelHoverBorderColor = BorderColor;
            this.LabelHoverBackColor = PanelGradiantMiddleColor;
            this.LabelHoverForeColor = ForeColor;
            this.LabelSelectedBorderColor = BorderColor;
            this.LabelSelectedBackColor = PanelGradiantStartColor;
            this.LabelSelectedForeColor = ForeColor;
            this.LabelDisabledBackColor = PanelGradiantStartColor;
            this.LabelDisabledForeColor = ForeColor;
            this.LabelDisabledBorderColor = BorderColor;
        }
    }
}
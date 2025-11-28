using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme
    {
        private void ApplyCheckBox()
        {
            this.CheckBoxBackColor = PanelGradiantStartColor;
            this.CheckBoxForeColor = ForeColor;
            this.CheckBoxBorderColor = BorderColor;
            this.CheckBoxCheckedBackColor = PrimaryColor;
            this.CheckBoxCheckedForeColor = OnPrimaryColor;
            this.CheckBoxCheckedBorderColor = ActiveBorderColor;
            this.CheckBoxHoverBackColor = PanelGradiantMiddleColor;
            this.CheckBoxHoverForeColor = ForeColor;
            this.CheckBoxHoverBorderColor = BorderColor;
        }
    }
}
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordTheme
    {
        private void ApplyCard()
        {
            this.CardTextForeColor = ForeColor;
            this.CardBackColor = PanelBackColor;
            this.CardTitleForeColor = ForeColor;
            this.CardSubTitleForeColor = ForeColor;
            this.CardrGradiantStartColor = PanelGradiantStartColor;
            this.CardGradiantEndColor = PanelGradiantEndColor;
            this.CardGradiantMiddleColor = PanelGradiantMiddleColor;
            this.CardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
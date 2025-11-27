using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyCard()
        {
            this.CardTextForeColor = ForeColor;
            this.CardBackColor = SurfaceColor;
            this.CardTitleForeColor = ForeColor;
            this.CardSubTitleForeColor = ForeColor;
            this.CardrGradiantStartColor = SecondaryColor;
            this.CardGradiantEndColor = SurfaceColor;
            this.CardGradiantMiddleColor = SurfaceColor;
            this.CardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
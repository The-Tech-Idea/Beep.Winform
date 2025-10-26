using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyCard()
        {
            this.CardTextForeColor = ForeColor;
            this.CardBackColor = BackgroundColor;
            this.CardTitleForeColor = ForeColor;
            this.CardSubTitleForeColor = ForeColor;
            this.CardrGradiantStartColor = BackgroundColor;
            this.CardGradiantEndColor = BackgroundColor;
            this.CardGradiantMiddleColor = BackgroundColor;
            this.CardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}

using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplyCard()
        {
            this.CardTextForeColor = Color.FromArgb(58,66,86);
            this.CardBackColor = Color.FromArgb(236,240,243);
            this.CardTitleForeColor = Color.FromArgb(58,66,86);
            this.CardSubTitleForeColor = Color.FromArgb(58,66,86);
            this.CardrGradiantStartColor = Color.FromArgb(236,240,243);
            this.CardGradiantEndColor = Color.FromArgb(236,240,243);
            this.CardGradiantMiddleColor = Color.FromArgb(236,240,243);
            this.CardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}
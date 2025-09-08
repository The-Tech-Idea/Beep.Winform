using System;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Helpers
{
    /// <summary>
    /// Applies style preset metrics (border radius, shadow depth, glow configuration) decoupled from theme colors.
    /// For now only basic mapping; can be extended with a BeepFormStyle enum reused from original implementation.
    /// </summary>
    internal enum BasicFormStyle
    {
        Modern,
        Minimal,
        MaterialLike,
        Classic
    }

    internal sealed class FormStyleHelper
    {
        private readonly IBeepModernFormHost _host;
        private readonly FormShadowGlowPainter _shadowGlow;

        public FormStyleHelper(IBeepModernFormHost host, FormShadowGlowPainter shadowGlow)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));
            _shadowGlow = shadowGlow ?? throw new ArgumentNullException(nameof(shadowGlow));
        }

        public void Apply(BasicFormStyle style, IBeepTheme theme)
        {
            switch (style)
            {
                case BasicFormStyle.Modern:
                    _host.BorderRadius = 8;
                    _host.BorderThickness = 1;
                    _shadowGlow.ShadowDepth = 6;
                    _shadowGlow.EnableGlow = true;
                    _shadowGlow.GlowSpread = 8f;
                    _shadowGlow.GlowColor = System.Drawing.Color.FromArgb(100, 72, 170, 255);
                    break;
                case BasicFormStyle.Minimal:
                    _host.BorderRadius = 4;
                    _host.BorderThickness = 1;
                    _shadowGlow.ShadowDepth = 0;
                    _shadowGlow.EnableGlow = false;
                    break;
                case BasicFormStyle.MaterialLike:
                    _host.BorderRadius = 8;
                    _host.BorderThickness = 0;
                    _shadowGlow.ShadowDepth = 5;
                    _shadowGlow.EnableGlow = true;
                    _shadowGlow.GlowSpread = 12f;
                    _shadowGlow.GlowColor = System.Drawing.Color.FromArgb(60, 0, 0, 0);
                    break;
                case BasicFormStyle.Classic:
                    _host.BorderRadius = 0;
                    _host.BorderThickness = 1;
                    _shadowGlow.ShadowDepth = 0;
                    _shadowGlow.EnableGlow = false;
                    break;
            }
            _host.UpdateRegion();
            _host.Invalidate();
        }
    }
}

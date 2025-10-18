using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Helpers
{
    internal sealed class FormStyleHelper
    {
        private readonly IBeepModernFormHost _host;
        private readonly FormShadowGlowPainter _shadowGlow;

        public FormStyleHelper(IBeepModernFormHost host, FormShadowGlowPainter shadowGlow)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));
            _shadowGlow = shadowGlow ?? throw new ArgumentNullException(nameof(shadowGlow));
        }

        // Entry point: mirror BeepiForm.ApplyFormStyle behavior
        public void ApplyFormStyle(BeepFormStyle style, IBeepTheme theme = null)
        {
            if (_host.IsInDesignMode)
            {
                _host.Invalidate();
                return;
            }

            var effectiveTheme = theme ?? _host.CurrentTheme;

            // 1) Metrics (border radius/thickness, glow spread, shadow) from defaults
            if (BeepFormStyleMetricsDefaults.Map.TryGetValue(style, out var m))
            {
                _host.BorderRadius = Math.Max(0, m.BorderRadius);
                _host.BorderThickness = Math.Max(0, m.BorderThickness);

                _shadowGlow.ShadowDepth = Math.Max(0, m.ShadowDepth);
                _shadowGlow.EnableGlow = m.EnableGlow;
                _shadowGlow.GlowSpread = Math.Max(0f, m.GlowSpread);
            }

            // 2) Visual tweaks per style (glow color + background/border)
            switch (style)
            {
                case BeepFormStyle.Modern:
                    ApplyThemeMapping(effectiveTheme);
                    _shadowGlow.EnableGlow = false;
                    _shadowGlow.GlowColor = Color.Transparent;
                    break;

                case BeepFormStyle.Metro:
                    ApplyThemeMapping(effectiveTheme);
                    _shadowGlow.GlowColor = Color.FromArgb(80, 0, 100, 200);
                    break;

                case BeepFormStyle.Glass:
                    ApplyThemeMapping(effectiveTheme);
                    _shadowGlow.GlowColor = Color.FromArgb(120, 255, 255, 255);
                    break;

                case BeepFormStyle.Office:
                    ApplyThemeMapping(effectiveTheme);
                    _shadowGlow.GlowColor = Color.FromArgb(90, 50, 100, 200);
                    break;

                case BeepFormStyle.ModernDark:
                    _shadowGlow.GlowColor = Color.FromArgb(80, 0, 0, 0);
                    _host.AsForm.BackColor = Color.FromArgb(32, 32, 32);
                    TrySetBorderColor(Color.FromArgb(64, 64, 64));
                    break;

                case BeepFormStyle.Material:
                    ApplyThemeMapping(effectiveTheme);
                    _shadowGlow.GlowColor = Color.FromArgb(60, 0, 0, 0);
                    TrySetBorderColor(Color.Transparent);
                    break;

                case BeepFormStyle.Minimal:
                    ApplyThemeMapping(effectiveTheme);
                    // No extra glow color override needed
                    break;

                case BeepFormStyle.Classic:
                    _host.AsForm.BackColor = SystemColors.Control;
                    TrySetBorderColor(SystemColors.ActiveBorder);
                    // No glow override
                    break;

                case BeepFormStyle.Custom:
                    ApplyThemeMapping(effectiveTheme);
                    // Keep metrics/theme colors as-is
                    break;
            }

            // 3) Region + repaint
            _host.UpdateRegion();
            _host.Invalidate();
        }

        // Apply theme → form BackColor and BorderColor (if property exists)
        private void ApplyThemeMapping(IBeepTheme theme)
        {
            var form = _host.AsForm;

            var back = TryGetThemeColor(theme, "BackColor") ?? SystemColors.Control;
            if (back == Color.Transparent || back == Color.Empty)
                back = SystemColors.Control;
            form.BackColor = back;

            var border = TryGetThemeColor(theme, "BorderColor") ?? SystemColors.ControlDark;
            if (border == Color.Empty) border = SystemColors.ControlDark;
            TrySetBorderColor(border);
        }

        // Attempt to set BeepiForm.BorderColor if present
        private void TrySetBorderColor(Color color)
        {
            var form = _host.AsForm;
            var prop = form.GetType().GetProperty("BorderColor");
            if (prop != null && prop.PropertyType == typeof(Color) && prop.CanWrite)
            {
                try { prop.SetValue(form, color); } catch { /* ignore */ }
            }
        }

        // Safely retrieve a color from theme via reflection (avoids hard dependency on concrete theme type)
        private static Color? TryGetThemeColor(IBeepTheme theme, string propertyName)
        {
            if (theme == null) return null;
            try
            {
                var p = theme.GetType().GetProperty(propertyName);
                if (p != null && p.PropertyType == typeof(Color))
                {
                    var val = (Color)p.GetValue(theme);
                    return val;
                }
            }
            catch { /* ignore */ }
            return null;
        }
    }
}
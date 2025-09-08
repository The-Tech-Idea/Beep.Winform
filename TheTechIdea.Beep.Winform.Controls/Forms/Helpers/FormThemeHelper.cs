using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Helpers
{
    /// <summary>
    /// Applies theme values to the host form and (optionally) child controls.
    /// </summary>
    internal sealed class FormThemeHelper
    {
        private readonly IBeepModernFormHost _host;

        public bool ApplyThemeToChildren { get; set; } = true;

        public FormThemeHelper(IBeepModernFormHost host)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));
        }

        public void ApplyTheme()
        {
            var form = _host.AsForm;
            var theme = _host.CurrentTheme;
            if (theme == null) return;

            Color back = theme.BackColor;
            if (back == Color.Empty || back == Color.Transparent)
                back = SystemColors.Control;
            form.BackColor = back;

            // Additional theming hooks can be added here (caption bar, system buttons, etc.)

            if (ApplyThemeToChildren)
            {
                foreach (Control c in form.Controls)
                {
                    ApplyThemeToChild(c, theme);
                }
            }
        }

        private void ApplyThemeToChild(Control c, IBeepTheme theme)
        {
            try
            {
                // Simple heuristic: set BackColor if default / transparent.
                if (c.BackColor == Color.Empty || c.BackColor == Color.Transparent)
                {
                    c.BackColor = theme.PanelBackColor != Color.Empty ? theme.PanelBackColor : _host.AsForm.BackColor;
                }
            }
            catch { }
        }
    }
}

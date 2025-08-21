using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Advanced.Helpers
{
    internal class ControlEffectHelper
    {
        private readonly BeepControlAdvanced _owner;

        public ControlEffectHelper(BeepControlAdvanced owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        [Browsable(true)] public bool ShowFocusIndicator { get; set; } = false;
        [Browsable(true)] public Color FocusIndicatorColor { get; set; } = Color.RoyalBlue;
        [Browsable(true)] public bool EnableRippleEffect { get; set; } = false;

        public void DrawOverlays(Graphics g)
        {
            if (ShowFocusIndicator && _owner.Focused)
            {
                var rect = new Rectangle(0, 0, _owner.Width, _owner.Height);
                using var brush = new SolidBrush(Color.FromArgb(60, FocusIndicatorColor));
                g.FillRectangle(brush, rect);
            }
            // Ripple can be added here later; keep minimal now
        }
    }
}

using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.NavBars
{
    /// <summary>
    /// Partial class for BeepNavBar drawing logic
    /// </summary>
    public partial class BeepNavBar
    {
        /// <summary>
        /// Override DrawContent to delegate to the current painter
        /// </summary>
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);

            if (_currentPainter == null)
            {
                InitializePainter();
            }
            if (UseThemeColors && _currentTheme != null)
            {
                BackColor = _currentTheme.SideMenuBackColor;
                g.Clear(BackColor);
            }
            else
            {
                // Paint background based on selected Style
                BeepStyling.PaintStyleBackground(g, DrawingRect, Style);
            }
            if (_currentPainter != null)
            {
                var adapter = new BeepNavBarAdapter(this);
                _currentPainter.Draw(g, adapter, DrawingRect);
            }
        }
    }
}

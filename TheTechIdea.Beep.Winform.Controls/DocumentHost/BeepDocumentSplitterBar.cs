// BeepDocumentSplitterBar.cs
// Typed splitter-bar Panel used between document groups in BeepDocumentHost.
// Replaces the anonymous Panel used in earlier versions.
// ─────────────────────────────────────────────────────────────────────────────
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Themes;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// A thin, themed, draggable bar that separates two adjacent document groups.
    /// The actual drag logic is wired in <c>BeepDocumentHost.Layout.cs</c>.
    /// </summary>
    internal sealed class BeepDocumentSplitterBar : Panel
    {
        private bool _isHorizontal = true;

        public BeepDocumentSplitterBar()
        {
            // Non-focusable — the bar should never steal keyboard focus
            SetStyle(ControlStyles.Selectable, false);
            TabStop = false;
            Cursor  = Cursors.VSplit;
        }

        /// <summary>
        /// When <c>true</c> the bar is vertical, separating left/right panes.
        /// When <c>false</c> the bar is horizontal, separating top/bottom panes.
        /// Setting this property updates the cursor automatically.
        /// </summary>
        public bool IsHorizontal
        {
            get => _isHorizontal;
            set
            {
                _isHorizontal = value;
                Cursor = value ? Cursors.VSplit : Cursors.HSplit;
            }
        }

        /// <summary>Applies the Beep theme's border colour to the bar.</summary>
        public void ApplyTheme(IBeepTheme? theme)
        {
            BackColor = theme?.BorderColor ?? System.Drawing.SystemColors.ControlDark;
        }
    }
}

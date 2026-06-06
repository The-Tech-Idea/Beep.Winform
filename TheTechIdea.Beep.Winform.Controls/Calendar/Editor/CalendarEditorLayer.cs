using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Editor
{
    /// <summary>
    /// Transparent child <see cref="Panel"/> that lives inside
    /// <c>BeepCalendar.Controls</c> and hosts the real WinForms editors
    /// (<see cref="BeepTextBox"/>, <see cref="BeepDateTimePicker"/>,
    /// <see cref="BeepComboBox"/>, <see cref="BeepCheckBoxBool"/>) produced
    /// by the sample editor factories in W4.
    ///
    /// The layer is intentionally minimal:
    /// <list type="bullet">
    ///   <item>Transparent background so the calendar paints show through.</item>
    ///   <item><see cref="OnPaintBackground"/> is no-op so the parent's
    ///         clipped <c>OnPaintBackground</c> / <c>OnPaint</c> calls do
    ///         not double-paint underneath hosted controls.</item>
    ///   <item><see cref="Site"/> is cleared to keep the host form's
    ///         designer from serializing the layer or any hosted editors
    ///         into <c>*.Designer.cs</c>.</item>
    /// </list>
    /// </summary>
    [ToolboxItem(false)]
    public class CalendarEditorLayer : Panel
    {
        public CalendarEditorLayer()
        {
            BackColor = Color.Transparent;
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            Visible = false;
            Dock = DockStyle.None;
            // Block designer from serializing the layer into the host form.
            Site = null;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // No-op: the parent BeepCalendar already clips its OnPaintBackground
            // to exclude this layer's bounds, so we do not want to paint anything
            // here that would show through the transparent backdrop.
        }
    }
}

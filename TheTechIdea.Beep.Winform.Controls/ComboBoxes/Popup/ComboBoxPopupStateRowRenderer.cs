using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Popup
{
    internal static class ComboBoxPopupStateRowRenderer
    {
        internal static bool TryDrawStateRow(Graphics g, Rectangle bounds, ComboBoxPopupRowModel row, Font font, Color foreColor)
        {
            if (!ComboBoxPopupRowBehavior.IsStateRow(row))
            {
                return false;
            }

            string message = string.IsNullOrWhiteSpace(row?.Text) ? "No items" : row.Text;
            TextRenderer.DrawText(g, message, font, bounds, foreColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            return true;
        }
    }
}

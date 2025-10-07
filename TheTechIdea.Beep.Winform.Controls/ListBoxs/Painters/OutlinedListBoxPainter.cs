using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Outlined list box painter - clear borders with dividers
    /// </summary>
    internal class OutlinedListBoxPainter : StandardListBoxPainter
    {
        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            base.DrawItemBackground(g, itemRect, isHovered, isSelected);
            
            // Draw subtle divider
            using (var pen = new Pen(Color.FromArgb(230, 230, 230), 1f))
            {
                g.DrawLine(pen, itemRect.Left + 8, itemRect.Bottom - 1, itemRect.Right - 8, itemRect.Bottom - 1);
            }
        }
        
        public override System.Windows.Forms.Padding GetPreferredPadding()
        {
            return new System.Windows.Forms.Padding(12, 6, 12, 6);
        }
    }
}

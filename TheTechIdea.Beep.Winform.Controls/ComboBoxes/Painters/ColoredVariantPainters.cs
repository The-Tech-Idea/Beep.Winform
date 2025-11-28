using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters
{
    /// <summary>
    /// Blue themed dropdown painter with colored accents
    /// </summary>
    internal class BlueDropdownPainter : OutlinedComboBoxPainter
    {
   
        protected override void DrawBorder(Graphics g, Rectangle rect)
        {
            Color borderColor = Color.FromArgb(66, 133, 244); // Blue
            var pen = PaintersFactory.GetPen(borderColor, _owner.Focused ? 2f : 1f);
            g.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
        }
    }
    
    /// <summary>
    /// Green themed dropdown with success/positive styling
    /// </summary>
    internal class GreenDropdownPainter : OutlinedComboBoxPainter
    {
        
        protected override void DrawBorder(Graphics g, Rectangle rect)
        {
            Color borderColor = Color.FromArgb(52, 168, 83); // Green
            var pen = PaintersFactory.GetPen(borderColor, _owner.Focused ? 2f : 1f);
            g.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
        }
    }
    
    /// <summary>
    /// Inverted color scheme dropdown (dark background)
    /// </summary>
    internal class InvertedComboBoxPainter : OutlinedComboBoxPainter
    {
    
        
        protected override void DrawBorder(Graphics g, Rectangle rect)
        {
            Color borderColor = Color.FromArgb(100, 100, 100);
            var pen = PaintersFactory.GetPen(borderColor, 1f);
            g.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
        }
        
        protected override void DrawText(Graphics g, Rectangle textAreaRect)
        {
            if (textAreaRect.IsEmpty) return;
            
            string displayText = _helper.GetDisplayText();
            if (string.IsNullOrEmpty(displayText)) return;
            
            Color textColor = Color.White; // Light text for dark background
            System.Windows.Forms.TextRenderer.DrawText(g, displayText, _owner.TextFont, 
                textAreaRect, textColor, 
                System.Windows.Forms.TextFormatFlags.Left | 
                System.Windows.Forms.TextFormatFlags.VerticalCenter);
        }
    }
    
    /// <summary>
    /// Error state dropdown with red/error styling
    /// </summary>
    internal class ErrorComboBoxPainter : OutlinedComboBoxPainter
    {
       
        
        protected override void DrawBorder(Graphics g, Rectangle rect)
        {
            Color borderColor = Color.FromArgb(220, 53, 69); // Error red
            var pen = PaintersFactory.GetPen(borderColor, 2f);
            g.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
        }
    }
}

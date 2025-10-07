using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters
{
    /// <summary>
    /// Blue themed dropdown painter with colored accents
    /// </summary>
    internal class BlueDropdownPainter : OutlinedComboBoxPainter
    {
        protected override void DrawBackground(Graphics g, Rectangle rect)
        {
            // Light blue tinted background
            Color bgColor = Color.FromArgb(240, 245, 255);
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, rect);
            }
        }
        
        protected override void DrawBorder(Graphics g, Rectangle rect)
        {
            Color borderColor = Color.FromArgb(66, 133, 244); // Blue
            using (var pen = new Pen(borderColor, _owner.Focused ? 2f : 1f))
            {
                g.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
            }
        }
    }
    
    /// <summary>
    /// Green themed dropdown with success/positive styling
    /// </summary>
    internal class GreenDropdownPainter : OutlinedComboBoxPainter
    {
        protected override void DrawBackground(Graphics g, Rectangle rect)
        {
            // Light green tinted background
            Color bgColor = Color.FromArgb(240, 255, 245);
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, rect);
            }
        }
        
        protected override void DrawBorder(Graphics g, Rectangle rect)
        {
            Color borderColor = Color.FromArgb(52, 168, 83); // Green
            using (var pen = new Pen(borderColor, _owner.Focused ? 2f : 1f))
            {
                g.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
            }
        }
    }
    
    /// <summary>
    /// Inverted color scheme dropdown (dark background)
    /// </summary>
    internal class InvertedComboBoxPainter : OutlinedComboBoxPainter
    {
        protected override void DrawBackground(Graphics g, Rectangle rect)
        {
            // Dark background
            Color bgColor = Color.FromArgb(45, 45, 48);
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, rect);
            }
        }
        
        protected override void DrawBorder(Graphics g, Rectangle rect)
        {
            Color borderColor = Color.FromArgb(100, 100, 100);
            using (var pen = new Pen(borderColor, 1f))
            {
                g.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
            }
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
        protected override void DrawBackground(Graphics g, Rectangle rect)
        {
            // Light red tinted background
            Color bgColor = Color.FromArgb(255, 245, 245);
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, rect);
            }
        }
        
        protected override void DrawBorder(Graphics g, Rectangle rect)
        {
            Color borderColor = Color.FromArgb(220, 53, 69); // Error red
            using (var pen = new Pen(borderColor, 2f))
            {
                g.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
            }
        }
    }
}

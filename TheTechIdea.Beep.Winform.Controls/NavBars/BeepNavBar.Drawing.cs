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

            // Design-time: render simple placeholder to prevent flickering
            if (IsDesignModeSafe)
            {
                PaintDesignTimePlaceholder(g);
                return;
            }

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
        
        /// <summary>
        /// Simple placeholder rendering for Visual Studio Designer
        /// Prevents flickering and unresponsiveness during design-time
        /// </summary>
        private void PaintDesignTimePlaceholder(Graphics g)
        {
            // Light background
            g.Clear(Color.FromArgb(248, 249, 250));
            
            // Border
            using (var pen = new Pen(Color.FromArgb(218, 220, 224), 1))
            {
                g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
            }
            
            // Title
            using (var font = new Font("Segoe UI", 10, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.FromArgb(100, 100, 100)))
            {
                string orientationText = _orientation == NavBarOrientation.Horizontal ? "Horizontal" : "Vertical";
                g.DrawString($"BeepNavBar ({orientationText})", font, brush, 8, 4);
            }
            
            // Item count info
            using (var font = new Font("Segoe UI", 9))
            using (var brush = new SolidBrush(Color.FromArgb(120, 120, 120)))
            {
                int itemCount = _items?.Count ?? 0;
                g.DrawString($"Items: {itemCount} | Style: {_style}", font, brush, 8, 24);
            }
            
            // Draw placeholder items
            if (_items != null && _items.Count > 0)
            {
                int padding = 8;
                int previewItemSize = 40;
                
                if (_orientation == NavBarOrientation.Horizontal)
                {
                    int x = padding;
                    int y = Height - previewItemSize - padding;
                    
                    for (int i = 0; i < System.Math.Min(_items.Count, 6); i++)
                    {
                        using (var fillBrush = new SolidBrush(Color.FromArgb(230, 230, 230)))
                        using (var borderPen = new Pen(Color.FromArgb(200, 200, 200)))
                        {
                            var rect = new Rectangle(x, y, previewItemSize, previewItemSize - 10);
                            g.FillRectangle(fillBrush, rect);
                            g.DrawRectangle(borderPen, rect);
                        }
                        x += previewItemSize + 4;
                    }
                }
                else
                {
                    int x = padding;
                    int y = 48;
                    
                    for (int i = 0; i < System.Math.Min(_items.Count, 4); i++)
                    {
                        using (var fillBrush = new SolidBrush(Color.FromArgb(230, 230, 230)))
                        using (var borderPen = new Pen(Color.FromArgb(200, 200, 200)))
                        {
                            var rect = new Rectangle(x, y, Width - padding * 2, 24);
                            g.FillRectangle(fillBrush, rect);
                            g.DrawRectangle(borderPen, rect);
                        }
                        y += 28;
                    }
                }
            }
        }
    }
}

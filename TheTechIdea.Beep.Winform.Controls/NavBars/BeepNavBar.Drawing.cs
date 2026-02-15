using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Helpers;
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
            var baseFont = _textFont ?? BeepFontManager.DefaultFont;
            using (var titleFont = new Font(baseFont.FontFamily, Math.Min(10f, baseFont.Size + 1), FontStyle.Bold))
            using (var brush = new SolidBrush(Color.FromArgb(100, 100, 100)))
            {
                int pad = DpiScalingHelper.ScaleValue(8, this);
                string orientationText = _orientation == NavBarOrientation.Horizontal ? "Horizontal" : "Vertical";
                g.DrawString($"BeepNavBar ({orientationText})", titleFont, brush, pad, DpiScalingHelper.ScaleValue(4, this));
            }
            
            // Item count info
            var infoFont = _textFont ?? BeepFontManager.DefaultFont;
            using (var brush = new SolidBrush(Color.FromArgb(120, 120, 120)))
            {
                int itemCount = _items?.Count ?? 0;
                int pad = DpiScalingHelper.ScaleValue(8, this);
                g.DrawString($"Items: {itemCount} | Style: {_style}", infoFont, brush, pad, DpiScalingHelper.ScaleValue(24, this));
            }
            
            // Draw placeholder items
            if (_items != null && _items.Count > 0)
            {
                int padding = DpiScalingHelper.ScaleValue(8, this);
                int previewItemSize = DpiScalingHelper.ScaleValue(40, this);
                int spacing = DpiScalingHelper.ScaleValue(4, this);
                
                if (_orientation == NavBarOrientation.Horizontal)
                {
                    int x = padding;
                    int y = Height - previewItemSize - padding;
                    int itemHeight = previewItemSize - DpiScalingHelper.ScaleValue(10, this);
                    for (int i = 0; i < System.Math.Min(_items.Count, 6); i++)
                    {
                        using (var fillBrush = new SolidBrush(Color.FromArgb(230, 230, 230)))
                        using (var borderPen = new Pen(Color.FromArgb(200, 200, 200)))
                        {
                            var rect = new Rectangle(x, y, previewItemSize, itemHeight);
                            g.FillRectangle(fillBrush, rect);
                            g.DrawRectangle(borderPen, rect);
                        }
                        x += previewItemSize + spacing;
                    }
                }
                else
                {
                    int x = padding;
                    int y = DpiScalingHelper.ScaleValue(48, this);
                    int rowHeight = DpiScalingHelper.ScaleValue(28, this);
                    for (int i = 0; i < System.Math.Min(_items.Count, 4); i++)
                    {
                        using (var fillBrush = new SolidBrush(Color.FromArgb(230, 230, 230)))
                        using (var borderPen = new Pen(Color.FromArgb(200, 200, 200)))
                        {
                            var rect = new Rectangle(x, y, Width - padding * 2, DpiScalingHelper.ScaleValue(24, this));
                            g.FillRectangle(fillBrush, rect);
                            g.DrawRectangle(borderPen, rect);
                        }
                        y += rowHeight;
                    }
                }
            }
        }
    }
}

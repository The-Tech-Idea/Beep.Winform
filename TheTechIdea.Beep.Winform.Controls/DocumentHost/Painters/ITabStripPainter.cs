using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost.Painters
{
    public interface ITabStripPainter
    {
        string Name { get; }
        void PaintTab(Graphics g, BeepDocumentTab tab, int index, TabStripPaintContext context);
        void PaintTabBackground(Graphics g, BeepDocumentTab tab, TabStripPaintContext context);
        void PaintTabContent(Graphics g, BeepDocumentTab tab, int index, TabStripPaintContext context);
        void PaintTabIcon(Graphics g, BeepDocumentTab tab, TabStripPaintContext context);
        void PaintTabText(Graphics g, BeepDocumentTab tab, TabStripPaintContext context);
        void PaintCloseButton(Graphics g, BeepDocumentTab tab, TabStripPaintContext context);
        void PaintBadge(Graphics g, BeepDocumentTab tab, TabStripPaintContext context);
        void PaintDirtyDot(Graphics g, BeepDocumentTab tab, TabStripPaintContext context);
        void PaintAccentBar(Graphics g, BeepDocumentTab tab, TabStripPaintContext context);
        void PaintPinnedTab(Graphics g, BeepDocumentTab tab, int index, TabStripPaintContext context);
        void PaintEmptyState(Graphics g, Rectangle bounds, TabStripPaintContext context);
        void PaintStripBackground(Graphics g, Rectangle bounds, TabStripPaintContext context);
        void PaintSeparator(Graphics g, Rectangle bounds, bool isVertical, TabStripPaintContext context);
        void PaintAddButton(Graphics g, Rectangle bounds, bool isHovered, TabStripPaintContext context);
        void PaintScrollButton(Graphics g, Rectangle bounds, bool isLeft, bool isHovered, bool isVertical, TabStripPaintContext context);
        void PaintOverflowButton(Graphics g, Rectangle bounds, bool isHovered, TabStripPaintContext context);
        void PaintGroupHeader(Graphics g, TabGroup group, BeepDocumentTab firstTab, TabStripPaintContext context);
        void PaintFocusIndicator(Graphics g, BeepDocumentTab tab, bool isFocused, TabStripPaintContext context);
        GraphicsPath CreateTabPath(Rectangle bounds, TabStripPaintContext context);
    }
}

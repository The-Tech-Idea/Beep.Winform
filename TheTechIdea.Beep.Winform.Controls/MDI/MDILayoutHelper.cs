using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.MDI
{
    internal enum ScrollButtonType { Left, Right }

    internal class MDILayoutResult
    {
        public bool NeedsScrolling { get; set; }
        public Rectangle ScrollLeftRect { get; set; }
        public Rectangle ScrollRightRect { get; set; }
        public Rectangle NewDocumentRect { get; set; }
        public int FirstVisibleIndex { get; set; }
        public int LastVisibleIndex { get; set; }
    }

    internal class MDILayoutHelper
    {
        private const int ScrollButtonSize = 26;
        private const int NewDocButtonSize = 26;
        private const int TabPaddingHorizontal = 14;
        private const int CloseButtonAreaWidth = 18; // clickable area width
        private const int IconAreaWidth = 20;
        private const int PinAreaWidth = 16;
        public int PinnedSectionWidth { get; private set; }

        public MDILayoutResult CalculateLayout(List<MDIDocument> docs, Rectangle headerRect, int minWidth, int maxWidth, int scrollOffset, Font font)
        {
            var result = new MDILayoutResult();
            if (docs.Count == 0) return result;

            int availableWidth = headerRect.Width;

            // Separate pinned and regular documents
            var pinned = docs.Where(d => d.IsPinned).ToList();
            var normal = docs.Where(d => !d.IsPinned).ToList();

            var desiredWidths = new Dictionary<MDIDocument, int>();
            using (var g = Graphics.FromHwnd(IntPtr.Zero))
            {
                foreach (var d in docs)
                {
                    int extras = CloseButtonAreaWidth + IconAreaWidth + (d.IsPinned ? PinAreaWidth : 0);
                    var size = TextRenderer.MeasureText(g, d.Title, font, new Size(int.MaxValue, headerRect.Height), TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine);
                    int w = size.Width + TabPaddingHorizontal * 2 + extras;
                    w = Math.Max(minWidth, Math.Min(maxWidth, w));
                    desiredWidths[d] = w;
                }
            }

            int pinnedWidth = pinned.Sum(p => desiredWidths[p]);
            PinnedSectionWidth = pinnedWidth;
            int normalTotalWidth = normal.Sum(n => desiredWidths[n]);

            if (pinnedWidth + normalTotalWidth > availableWidth)
            {
                result.NeedsScrolling = true;
                availableWidth -= (ScrollButtonSize * 2 + NewDocButtonSize + 8);
            }

            // Layout pinned left side
            int xPos = headerRect.Left + 2;
            int y = headerRect.Top;
            int height = headerRect.Height - 1;
            foreach (var p in pinned)
            {
                int w = desiredWidths[p];
                p.TabBounds = new Rectangle(xPos, y + 1, w, height - 2);
                p.IsVisible = true;
                xPos += w;
            }

            if (result.NeedsScrolling)
            {
                int startIndex = scrollOffset;
                if (startIndex >= normal.Count) startIndex = Math.Max(0, normal.Count - 1);
                result.FirstVisibleIndex = startIndex;
                for (int i = startIndex; i < normal.Count; i++)
                {
                    int w = desiredWidths[normal[i]];
                    if (xPos + w > headerRect.Left + availableWidth)
                    {
                        normal[i].TabBounds = Rectangle.Empty;
                        normal[i].IsVisible = false;
                    }
                    else
                    {
                        normal[i].TabBounds = new Rectangle(xPos, y + 1, w, height - 2);
                        normal[i].IsVisible = true;
                        xPos += w;
                        result.LastVisibleIndex = i;
                    }
                }

                int right = headerRect.Right - 2;
                result.NewDocumentRect = new Rectangle(right - NewDocButtonSize, y + (headerRect.Height - NewDocButtonSize) / 2, NewDocButtonSize, NewDocButtonSize);
                right -= (NewDocButtonSize + 2);
                result.ScrollRightRect = new Rectangle(right - ScrollButtonSize, y + (headerRect.Height - ScrollButtonSize) / 2, ScrollButtonSize, ScrollButtonSize);
                right -= (ScrollButtonSize + 2);
                result.ScrollLeftRect = new Rectangle(right - ScrollButtonSize, y + (headerRect.Height - ScrollButtonSize) / 2, ScrollButtonSize, ScrollButtonSize);
            }
            else
            {
                foreach (var n in normal)
                {
                    int w = desiredWidths[n];
                    n.TabBounds = new Rectangle(xPos, y + 1, w, height - 2);
                    n.IsVisible = true;
                    xPos += w;
                }
            }
            return result;
        }

        public Rectangle GetCloseButtonRect(Rectangle tabBounds)
        {
            if (tabBounds.IsEmpty) return Rectangle.Empty;
            return new Rectangle(tabBounds.Right - 18, tabBounds.Top + (tabBounds.Height - 16) / 2, 14, 14);
        }

        public Rectangle GetIconRect(Rectangle tabBounds)
        {
            if (tabBounds.IsEmpty) return Rectangle.Empty;
            return new Rectangle(tabBounds.Left + 6, tabBounds.Top + (tabBounds.Height - 16) / 2, 16, 16);
        }

        public Rectangle GetPinRect(Rectangle tabBounds)
        {
            if (tabBounds.IsEmpty) return Rectangle.Empty;
            return new Rectangle(tabBounds.Right - 34, tabBounds.Top + (tabBounds.Height - 14) / 2, 14, 14);
        }
    }
}

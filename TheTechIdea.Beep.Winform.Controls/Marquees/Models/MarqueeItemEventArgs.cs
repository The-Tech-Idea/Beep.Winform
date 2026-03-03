using System;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Marquees.Models
{
    /// <summary>
    /// Sprint 5 — Event arguments for <see cref="BeepMarquee.ItemClicked"/> and
    /// <see cref="BeepMarquee.ItemHovered"/>.
    /// </summary>
    public class MarqueeItemEventArgs : EventArgs
    {
        /// <summary>The item that was clicked or hovered.</summary>
        public MarqueeItem Item { get; }

        /// <summary>Zero-based index of <see cref="Item"/> within the current items list.</summary>
        public int ItemIndex { get; }

        /// <summary>Client-coordinate mouse position at the time of the event.</summary>
        public Point Location { get; }

        public MarqueeItemEventArgs(MarqueeItem item, int itemIndex, Point location)
        {
            Item      = item;
            ItemIndex = itemIndex;
            Location  = location;
        }
    }
}

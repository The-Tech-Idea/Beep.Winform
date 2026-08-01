using System;

namespace TheTechIdea.Beep.Winform.Controls.Trees.Models
{
    /// <summary>
    /// Controls whether a node reserves horizontal space for its icon even when it has none.
    /// <para>
    /// The expander slot is always reserved, because a tree whose leaves sit left of their
    /// expandable siblings reads as a broken hierarchy. Icons are different: reserving space
    /// unconditionally indents every label in a tree that has no icons at all, so the behaviour is
    /// a choice rather than a fix.
    /// </para>
    /// </summary>
    public enum IconSlotMode
    {
        /// <summary>
        /// Reserve the icon slot on every row as soon as at least one visible node has an icon.
        /// Text-only trees keep their tight layout; mixed trees stay aligned. This is the default
        /// because it is correct in both cases without the host having to think about it.
        /// </summary>
        WhenAnyNodeHasIcon = 0,

        /// <summary>
        /// Always reserve the icon slot, even when no node currently has an icon. Use this when
        /// icons are loaded lazily and you would rather hold the space than have labels shift
        /// sideways when the images arrive.
        /// </summary>
        Always = 1,

        /// <summary>
        /// Never reserve the slot: only nodes that actually have an icon consume the width.
        /// This is the historical behaviour and leaves labels ragged in mixed trees, but it is the
        /// tightest layout when every node has an icon (or none do).
        /// </summary>
        Never = 2
    }
}

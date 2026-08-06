using System.Collections.Generic;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Lovs
{
    /// <summary>
    /// One window of LOV rows, plus how many there are in total.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The total is the point. Without it the popup can say "here are 100 rows" but not "of 4,213", and
    /// cannot tell whether another page exists — so the user has no idea whether the value they want is
    /// off the end of what they can see.
    /// </para>
    /// <para>
    /// Return <see cref="Total"/> as -1 when counting is genuinely expensive. The popup then pages by
    /// whether a full window came back, and reports the count as approximate rather than inventing one.
    /// </para>
    /// </remarks>
    public sealed class LovPage
    {
        public LovPage() { }

        public LovPage(List<SimpleItem> rows, int total)
        {
            Rows = rows ?? new List<SimpleItem>();
            Total = total;
        }

        /// <summary>The rows in this window.</summary>
        public List<SimpleItem> Rows { get; set; } = new List<SimpleItem>();

        /// <summary>Total rows matching the search, or -1 when unknown.</summary>
        public int Total { get; set; } = -1;

        /// <summary>Whether the source reported a total.</summary>
        public bool HasTotal => Total >= 0;

        /// <summary>An empty page, for a source with nothing to return.</summary>
        public static LovPage Empty => new LovPage(new List<SimpleItem>(), 0);
    }
}

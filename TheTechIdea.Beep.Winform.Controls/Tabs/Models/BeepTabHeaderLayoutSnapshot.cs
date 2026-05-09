using System;
using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Models
{
    /// <summary>
    /// Immutable-ish layout snapshot for a full header render pass.
    /// </summary>
    public sealed class BeepTabHeaderLayoutSnapshot
    {
        public Rectangle HeaderBounds { get; set; } = Rectangle.Empty;
        public Rectangle ContentBounds { get; set; } = Rectangle.Empty;
        public TabHeaderPosition HeaderPosition { get; set; } = TabHeaderPosition.Top;
        public int SelectedIndex { get; set; } = -1;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public List<BeepTabHeaderItemLayout> Items { get; } = new List<BeepTabHeaderItemLayout>();
    }
}
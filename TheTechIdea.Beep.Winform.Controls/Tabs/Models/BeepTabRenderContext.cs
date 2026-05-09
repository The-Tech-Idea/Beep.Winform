using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Models
{
    /// <summary>
    /// Shared render state consumed by the current BeepTabs shell and the premium header host.
    /// </summary>
    public sealed class BeepTabRenderContext
    {
        public Rectangle HeaderBounds { get; set; } = Rectangle.Empty;
        public Rectangle ContentBounds { get; set; } = Rectangle.Empty;
        public TabHeaderPosition HeaderPosition { get; set; } = TabHeaderPosition.Top;
        public Font TextFont { get; set; } = SystemFonts.DefaultFont;
        public IBeepTheme? Theme { get; set; }
        public bool ShowCloseButtons { get; set; }
        public int MinTouchTargetWidth { get; set; }
    }
}
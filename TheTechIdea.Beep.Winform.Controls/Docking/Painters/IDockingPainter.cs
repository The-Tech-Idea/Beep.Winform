using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Painters
{
    /// <summary>
    /// Theme/metrics provider for the docking system.
    ///
    /// Element painting is owned by the distinct docking element renderers
    /// (<c>CaptionRenderer</c>, the upcoming tab/splitter/strip/guide renderers) driven by their
    /// layout managers — see <see cref="IDockingElementRenderer"/>. This interface only supplies
    /// the theme-aware palette, fonts, and layout metrics those surfaces consume, and raises
    /// invalidation when the active theme changes.
    /// </summary>
    public interface IDockingPainter : IDisposable
    {
        /// <summary>
        /// Gets the theme-aware color for the background surface.
        /// </summary>
        Color BackgroundColor { get; }

        /// <summary>
        /// Gets the theme-aware color for text/foreground.
        /// </summary>
        Color ForegroundColor { get; }

        /// <summary>
        /// Gets the theme-aware color for borders and separators.
        /// </summary>
        Color BorderColor { get; }

        /// <summary>
        /// Gets the theme-aware color for hovered items.
        /// </summary>
        Color HoverColor { get; }

        /// <summary>
        /// Gets the theme-aware color for selected/active items.
        /// </summary>
        Color SelectedColor { get; }

        /// <summary>
        /// Gets the theme-aware color for disabled items.
        /// </summary>
        Color DisabledColor { get; }

        /// <summary>
        /// Gets the default font for UI text.
        /// </summary>
        Font UIFont { get; }

        /// <summary>
        /// Gets the font for tab text.
        /// </summary>
        Font TabFont { get; }

        /// <summary>
        /// Gets the preferred height of a tab strip (includes padding).
        /// </summary>
        int TabStripHeight { get; }

        /// <summary>
        /// Gets the width of a splitter handle.
        /// </summary>
        int SplitterWidth { get; }

        /// <summary>
        /// Invalidates any cached resources (e.g., when theme changes).
        /// Called when the active theme switches.
        /// </summary>
        void InvalidateCache();
    }

    /// <summary>
    /// Information about a single tab in the tab strip.
    /// </summary>
    public struct TabInfo
    {
        /// <summary>Unique identifier for this tab.</summary>
        public string Key { get; set; }

        /// <summary>Display title text.</summary>
        public string Title { get; set; }

        /// <summary>Optional icon image.</summary>
        public Image Icon { get; set; }

        /// <summary>Whether this tab has unsaved changes (display indicator).</summary>
        public bool IsDirty { get; set; }

        /// <summary>Whether this tab can be closed.</summary>
        public bool CanClose { get; set; }

        /// <summary>Optional user data associated with this tab.</summary>
        public object Tag { get; set; }

        public override string ToString() => $"Tab[{Key}, {Title}]";
    }

    /// <summary>
    /// Orientation of a splitter handle.
    /// </summary>
    public enum SplitterOrientation
    {
        /// <summary>Horizontal splitter (mouse Y changes).</summary>
        Horizontal = 0,

        /// <summary>Vertical splitter (mouse X changes).</summary>
        Vertical = 1
    }
}

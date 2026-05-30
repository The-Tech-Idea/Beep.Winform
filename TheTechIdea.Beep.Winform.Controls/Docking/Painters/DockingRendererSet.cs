using TheTechIdea.Beep.Winform.Controls.Docking.Painters.AutoHide;
using TheTechIdea.Beep.Winform.Controls.Docking.Painters.Caption;
using TheTechIdea.Beep.Winform.Controls.Docking.Painters.Splitter;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Painters
{
    /// <summary>
    /// Bundle of the distinct docking element renderers selected for a given
    /// <c>BeepControlStyle</c>. Vended (and cached) by <see cref="DockingPainterFactory"/> so a
    /// single place decides which concrete renderer paints each surface. Renderers stay stateless;
    /// per-paint state arrives through <see cref="DockingPainterContext"/>.
    /// </summary>
    internal sealed class DockingRendererSet
    {
        public DockingRendererSet(CaptionRenderer caption, SplitterRenderer splitter, AutoHideStripRenderer autoHide)
        {
            Caption = caption;
            Splitter = splitter;
            AutoHide = autoHide;
        }

        /// <summary>Renderer for panel captions and dockspace headers (shared strip).</summary>
        public CaptionRenderer Caption { get; }

        /// <summary>Renderer for the resize splitter bar and grip.</summary>
        public SplitterRenderer Splitter { get; }

        /// <summary>Renderer for the auto-hide edge strips.</summary>
        public AutoHideStripRenderer AutoHide { get; }
    }
}

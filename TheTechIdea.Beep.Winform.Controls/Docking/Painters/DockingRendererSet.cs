using TheTechIdea.Beep.Winform.Controls.Docking.Painters.AutoHide;
using TheTechIdea.Beep.Winform.Controls.Docking.Painters.Caption;
using TheTechIdea.Beep.Winform.Controls.Docking.Painters.Splitter;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Painters
{
    internal sealed class DockingRendererSet : System.IDisposable
    {
        public DockingRendererSet(CaptionRenderer caption, SplitterRenderer splitter, AutoHideStripRenderer autoHide)
        {
            Caption = caption;
            Splitter = splitter;
            AutoHide = autoHide;
        }

        public CaptionRenderer Caption { get; }
        public SplitterRenderer Splitter { get; }
        public AutoHideStripRenderer AutoHide { get; }

        public void Dispose()
        {
            Caption?.Dispose();
            Splitter?.Dispose();
            AutoHide?.Dispose();
        }
    }
}

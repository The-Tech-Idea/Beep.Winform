using TheTechIdea.Beep.Winform.Controls.Tabs.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Models
{
    /// <summary>
    /// Typed render request passed from the current BeepTabs shell to the custom header host.
    /// </summary>
    public sealed class BeepTabHeaderRenderRequest
    {
        public ITabPainter PrimaryPainter { get; set; } = null!;
        public ITabPainter? TransitionFromPainter { get; set; }
        public ITabPainter? TransitionToPainter { get; set; }
        public float TransitionProgress { get; set; }

        public bool HasTransition =>
            TransitionProgress > 0f &&
            TransitionFromPainter != null &&
            TransitionToPainter != null;
    }
}
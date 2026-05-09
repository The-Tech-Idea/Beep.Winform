namespace TheTechIdea.Beep.Winform.Controls.Tabs.Models
{
    /// <summary>
    /// Document/workspace-only state carried alongside the core premium tab item.
    /// This isolates workspace semantics from painter and shell mechanics.
    /// </summary>
    public sealed class BeepTabWorkspaceState
    {
        public bool IsPinned { get; set; }
        public bool IsPreview { get; set; }
        public bool IsDirty { get; set; }
        public bool ReusePreviewSlot { get; set; } = true;
        public string GroupKey { get; set; } = string.Empty;

        public BeepTabWorkspaceState Clone()
        {
            return new BeepTabWorkspaceState
            {
                IsPinned = IsPinned,
                IsPreview = IsPreview,
                IsDirty = IsDirty,
                ReusePreviewSlot = ReusePreviewSlot,
                GroupKey = GroupKey
            };
        }
    }
}
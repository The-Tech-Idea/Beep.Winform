using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models
{
    public sealed class DialogManagerOptions
    {
        public BeepControlStyle DefaultStyle { get; set; } = BeepControlStyle.Material3;
        public DialogShowAnimation DefaultAnimation { get; set; } = DialogShowAnimation.FadeIn;
        public DialogPlacementStrategy DefaultPlacement { get; set; } = DialogPlacementStrategy.CenterOwner;
        public DialogMotionProfile MotionProfile { get; set; } = DialogMotionProfile.Default;
        public bool ReducedMotion { get; set; } = false;
        public int MaxVisibleToasts { get; set; } = 3;
    }
}

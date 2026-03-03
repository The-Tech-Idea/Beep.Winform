using System;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models
{
    public sealed class DialogMotionProfile
    {
        public int DurationXS { get; set; } = 120;
        public int DurationSM { get; set; } = 180;
        public int DurationMD { get; set; } = 240;
        public int DurationLG { get; set; } = 320;
        public DialogAnimationEasing Standard { get; set; } = DialogAnimationEasing.EaseOutCubic;
        public DialogAnimationEasing Decelerate { get; set; } = DialogAnimationEasing.EaseOutCubic;
        public DialogAnimationEasing Accelerate { get; set; } = DialogAnimationEasing.EaseInOutQuad;
        public DialogAnimationEasing Emphasized { get; set; } = DialogAnimationEasing.EaseOutBack;

        public static DialogMotionProfile Default { get; } = new DialogMotionProfile();
    }
}

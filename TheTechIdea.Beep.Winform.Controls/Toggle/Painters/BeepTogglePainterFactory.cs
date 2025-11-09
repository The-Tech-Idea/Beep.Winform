using TheTechIdea.Beep.Winform.Controls.Toggle.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Toggle.Painters
{
    /// <summary>
    /// Factory for creating toggle painters based on style
    /// </summary>
    internal static class BeepTogglePainterFactory
    {
        public static BeepTogglePainterBase CreatePainter(ToggleStyle style, BeepToggle owner, BeepToggleLayoutHelper layout)
        {
            return style switch
            {
                // Classic styles
                ToggleStyle.Classic => new ClassicTogglePainter(owner, layout),
                ToggleStyle.iOS => new ClassicTogglePainter(owner, layout),
                
                // Labeled and icon styles
                ToggleStyle.LabeledTrack => new LabeledTrackTogglePainter(owner, layout),
                ToggleStyle.IconThumb => new IconThumbTogglePainter(owner, layout),
                ToggleStyle.RectangularLabeled => new RectangularTogglePainter(owner, layout),
                
                // Minimal and button styles
                ToggleStyle.Minimal => new MinimalTogglePainter(owner, layout),
                ToggleStyle.ButtonStyle => new ButtonStyleTogglePainter(owner, layout),
                ToggleStyle.CheckboxStyle => new CheckboxStyleTogglePainter(owner, layout),
                
                // Material Design styles
                ToggleStyle.MaterialPill => new MaterialPillTogglePainter(owner, layout),
                ToggleStyle.MaterialSquare => new MaterialPillTogglePainter(owner, layout), // TODO: Create dedicated painter
                ToggleStyle.MaterialSlider => new MaterialPillTogglePainter(owner, layout), // TODO: Create dedicated painter
                ToggleStyle.MaterialCheckbox => new CheckboxStyleTogglePainter(owner, layout), // Reuse checkbox
                ToggleStyle.MaterialSquareButton => new ButtonStyleTogglePainter(owner, layout), // Reuse button
                
                // Icon variant styles
                ToggleStyle.IconLock => new IconLockTogglePainter(owner, layout),
                ToggleStyle.IconSettings => new IconSettingsTogglePainter(owner, layout),
                ToggleStyle.IconMood => new IconMoodTogglePainter(owner, layout),
                ToggleStyle.IconCheck => new IconCheckTogglePainter(owner, layout),
                ToggleStyle.IconCircle => new ClassicTogglePainter(owner, layout),
                ToggleStyle.IconEye => new IconEyeTogglePainter(owner, layout),
                ToggleStyle.IconVolume => new IconVolumeTogglePainter(owner, layout),
                ToggleStyle.IconMic => new IconMicTogglePainter(owner, layout),
                ToggleStyle.IconPower => new IconPowerTogglePainter(owner, layout),
                ToggleStyle.IconHeart => new IconHeartTogglePainter(owner, layout),
                ToggleStyle.IconLike => new IconLikeTogglePainter(owner, layout),
                ToggleStyle.IconCustom => new IconCustomTogglePainter(owner, layout),
                
                // Default fallback
                _ => new ClassicTogglePainter(owner, layout)
            };
        }
    }
}

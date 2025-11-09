using System;

namespace TheTechIdea.Beep.Winform.Controls.Toggle
{
    /// <summary>
    /// Toggle switch visual styles based on modern UI patterns
    /// </summary>
    public enum ToggleStyle
    {
        /// <summary>
        /// Style 1: Classic rounded pill toggle with sliding circle
        /// </summary>
        Classic = 0,

        /// <summary>
        /// Style 2: Toggle with ON/OFF text labels on the track
        /// </summary>
        LabeledTrack = 1,

        /// <summary>
        /// Style 3: Toggle with checkmark (ON) and X (OFF) icons in thumb
        /// </summary>
        IconThumb = 2,

        /// <summary>
        /// Style 4: Rectangular toggle with ON/OFF text and colored thumb
        /// </summary>
        RectangularLabeled = 3,

        /// <summary>
        /// Style 5: Minimal flat toggle with sliding indicator
        /// </summary>
        Minimal = 4,

        /// <summary>
        /// Style 6: Button-style toggle with ON/OFF labels and icons
        /// </summary>
        ButtonStyle = 5,

        /// <summary>
        /// Style 7: Checkbox-style toggle with checkmark
        /// </summary>
        CheckboxStyle = 6,

        /// <summary>
        /// Style 8: iOS-style rounded toggle with smooth thumb
        /// </summary>
        iOS = 7,

        /// <summary>
        /// Additional styles from second image
        /// </summary>
        MaterialPill = 8,
        MaterialSquare = 9,
        MaterialSlider = 10,
        MaterialCheckbox = 11,
        MaterialSquareButton = 12,

        /// <summary>
        /// Styles with icons
        /// </summary>
        
        /// <summary>Style 13: Lock/Unlock icons</summary>
        IconLock = 13,
        
        /// <summary>Style 14: Tool/gear icon with rotation</summary>
        IconSettings = 14,
        
        /// <summary>Style 15: Smile/Frown icons</summary>
        IconMood = 15,
        
        /// <summary>Style 16: CheckCircle/XCircle icons</summary>
        IconCheck = 16,
        
        /// <summary>Style 17: Classic circle style</summary>
        IconCircle = 17,
        
        /// <summary>Style 18: Eye/EyeOff (Show/Hide)</summary>
        IconEye = 18,
        
        /// <summary>Style 19: Volume/VolumeX (Sound)</summary>
        IconVolume = 19,
        
        /// <summary>Style 20: Mic/MicOff</summary>
        IconMic = 20,
        
        /// <summary>Style 21: Power/ZapOff</summary>
        IconPower = 21,
        
        /// <summary>Style 22: Heart filled/outline (Favorite)</summary>
        IconHeart = 22,
        
        /// <summary>Style 23: ThumbsUp/ThumbsDown (Like/Dislike)</summary>
        IconLike = 23,
        
        /// <summary>Style 24: Custom icons - developer sets OnIconPath and OffIconPath properties</summary>
        IconCustom = 24
    }

    /// <summary>
    /// Toggle thumb shapes
    /// </summary>
    public enum ToggleThumbShape
    {
        Circle,
        Square,
        RoundedSquare
    }

    /// <summary>
    /// Toggle track shapes
    /// </summary>
    public enum ToggleTrackShape
    {
        Pill,
        Rectangle,
        RoundedRectangle
    }

    /// <summary>
    /// Hit test regions for toggle interaction
    /// </summary>
    public enum ToggleHitRegion
    {
        None,
        Track,
        Thumb,
        OnLabel,
        OffLabel,
        Icon
    }
}

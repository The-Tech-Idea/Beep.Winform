namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Enums
{
    /// <summary>
    /// Defines the visual style of the advanced button
    /// </summary>
    public enum AdvancedButtonStyle
    {
        /// <summary>Solid filled button with background color</summary>
        Solid,
        
        /// <summary>Icon-only button, no text</summary>
        Icon,
        
        /// <summary>Text-only button, minimal styling</summary>
        Text,
        
        /// <summary>Toggle button with on/off states</summary>
        Toggle,
        
        /// <summary>Floating Action Button (circular, elevated)</summary>
        FAB,
        
        /// <summary>Ghost button with subtle hover effects</summary>
        Ghost,
        
        /// <summary>Outlined button with border, no fill</summary>
        Outlined,
        
        /// <summary>Link-style button</summary>
        Link,
        
        /// <summary>Gradient filled button</summary>
        Gradient,
        
        /// <summary>Button with icon and text</summary>
        IconText,
        
        /// <summary>Chip/Tag button with pill shape</summary>
        Chip,
        
        /// <summary>Contact/CTA button with icon sections</summary>
        Contact,
        
        /// <summary>Navigation chevron button with angled edges</summary>
        NavigationChevron,
        
        /// <summary>Neon/glow button with glowing borders and effects</summary>
        NeonGlow,
        
        /// <summary>News/broadcast banner button with badges</summary>
        NewsBanner,

        /// <summary>Flat web UI button family (badges, search bars, segmented actions)</summary>
        FlatWeb,

        /// <summary>Broadcast lower-third UI family (headline bars, live tags, tickers)</summary>
        LowerThird,

        /// <summary>Comic/sticker style label family (speech bubbles, bursts, ribbons)</summary>
        StickerLabel
    }

    /// <summary>
    /// Defines the size preset for the button
    /// </summary>
    public enum AdvancedButtonSize
    {
        /// <summary>Small button - 32px height</summary>
        Small,
        
        /// <summary>Medium button - 40px height (default)</summary>
        Medium,
        
        /// <summary>Large button - 48px height</summary>
        Large
    }

    /// <summary>
    /// Defines the current state of the button for rendering
    /// </summary>
    public enum AdvancedButtonState
    {
        /// <summary>Normal/default state</summary>
        Normal,
        
        /// <summary>Mouse hovering over button</summary>
        Hover,
        
        /// <summary>Mouse button pressed down</summary>
        Pressed,
        
        /// <summary>Button is disabled</summary>
        Disabled,
        
        /// <summary>Button is focused (keyboard navigation)</summary>
        Focused
    }

    /// <summary>
    /// Semantic intent used to map button visuals to consistent color roles.
    /// </summary>
    public enum ButtonIntent
    {
        /// <summary>Primary call-to-action.</summary>
        Primary,

        /// <summary>Secondary supporting action.</summary>
        Secondary,

        /// <summary>Tertiary low emphasis action.</summary>
        Tertiary,

        /// <summary>Destructive action.</summary>
        Destructive,

        /// <summary>Positive/success action.</summary>
        Success,

        /// <summary>Neutral action.</summary>
        Neutral
    }

    /// <summary>
    /// Defines the shape of the button
    /// </summary>
    public enum ButtonShape
    {
        /// <summary>Rectangle with slightly rounded corners (default for most buttons)</summary>
        RoundedRectangle,
        
        /// <summary>Perfect circle (default for FAB)</summary>
        Circle,
        
        /// <summary>Pill shape with fully rounded ends (default for Ghost)</summary>
        Pill,
        
        /// <summary>Sharp rectangle with no rounded corners</summary>
        Rectangle,
        
        /// <summary>Square shape</summary>
        Square,
        
        /// <summary>Split button with two clickable areas (default for Toggle)</summary>
        Split,
        
        /// <summary>Custom shape defined by painter</summary>
        Custom
    }

    /// <summary>
    /// Explicit variant selector for news banner painter layouts.
    /// </summary>
    public enum NewsBannerVariant
    {
        /// <summary>Use painter heuristics to select a layout.</summary>
        Auto,
        CircleBadgeLeft,
        RectangleBadgeLeft,
        AngledBadgeLeft,
        ChevronRight,
        ChevronBoth,
        FlagLeft,
        AngledTwoTone,
        SlantedEdges,
        PillWithIcon,
        BNLiveCircleBanner,
        BNSquareGreenBanner,
        BreakingNewsGlobe,
        LightningBreakingNews,
        LightningBreakingNewsLive,
        LiveWorldNewsPill,
        MorningLiveYellowBanner,
        NewsLiveCirclePink,
        SportNewsCirclePill,
        TwentyFourTVNews,
        TwentyFourWorldNewsHexagon,
        WorldNewsGlobePill
    }

    /// <summary>
    /// Explicit variant selector for contact/CTA painter layouts.
    /// </summary>
    public enum ContactVariant
    {
        /// <summary>Use painter heuristics to select a layout.</summary>
        Auto,
        IconCircleLeft1,
        IconSquareLeft2,
        IconCirclesLeftPill3,
        IconCircleRightPill4,
        IconDiagonalLeft5,
        IconSquareLeft6,
        IconArrowLeftPill7,
        IconInsidePillBorder8,
        Standard,
        IconLeftSplit,
        IconRightSplit,
        IconLeftCircle,
        IconRightCircle,
        IconLeftAngled,
        IconLeftArrow,
        Outlined
    }

    /// <summary>
    /// Explicit variant selector for navigation chevron painter layouts.
    /// </summary>
    public enum ChevronVariant
    {
        /// <summary>Use painter heuristics to select a layout.</summary>
        Auto,
        LeftIconRightChevron,
        LeftChevronRightIcon,
        BothChevrons,
        LeftIconCenterChevron
    }

    /// <summary>
    /// Explicit variants for flat web button family.
    /// </summary>
    public enum FlatWebVariant
    {
        Auto,
        LeftBadgeAction,
        RightNotchSearch,
        SegmentedIconAction,
        SearchPillNotch,
        ToolbarSegment,
        RightArrowTagSearch,
        LeftPointTagSearch,
        MagnifierBubbleLeft
    }

    /// <summary>
    /// Explicit variants for broadcast lower-third family.
    /// </summary>
    public enum LowerThirdVariant
    {
        Auto,
        HeadlineBar,
        LiveTagHeadline,
        ReportSplit,
        TickerStrip,
        TickerChevron,
        LocationHeadlineBlock,
        CompactLiveTag,
        ReportStacked
    }

    /// <summary>
    /// Explicit variants for comic/sticker label family.
    /// </summary>
    public enum StickerLabelVariant
    {
        Auto,
        SpeechBubble,
        CloudTag,
        BurstBadge,
        ComicRibbon
    }
}

namespace TheTechIdea.Beep.Winform.Controls.Marquees
{
    /// <summary>Sprint 1 — How the marquee loops/transitions between items.</summary>
    public enum MarqueeScrollMode
    {
        /// <summary>Seamless continuous loop (default, current behaviour).</summary>
        Continuous,
        /// <summary>Scrolls to the edge then reverses direction.</summary>
        PingPong,
        /// <summary>Shows one item at a time; fades out then fades in the next item.</summary>
        NewsTicker
    }

    /// <summary>Sprint 1 — Primary scroll axis and direction.</summary>
    public enum MarqueeScrollDirection
    {
        /// <summary>Scrolls from left to right (items enter from left side).</summary>
        LeftToRight,
        /// <summary>Scrolls from right to left (default, HTML marquee default).</summary>
        RightToLeft,
        /// <summary>Scrolls from top to bottom.</summary>
        TopToBottom,
        /// <summary>Scrolls from bottom to top.</summary>
        BottomToTop
    }

    /// <summary>Sprint 3 — Visual rendering style applied to each marquee item.</summary>
    public enum MarqueeStyle
    {
        /// <summary>Plain text / IBeepUIComponent pass-through (current behaviour).</summary>
        Default,
        /// <summary>Each item in a rounded card with optional shadow (Material 3).</summary>
        Card,
        /// <summary>Items as coloured pill/badge chips (Bootstrap badge).</summary>
        Pill,
        /// <summary>Symbol · price · Δ% with green/red colouring (Bloomberg).</summary>
        StockTicker,
        /// <summary>Full-width category tag + headline text (news apps).</summary>
        NewsBanner,
        /// <summary>16 px icon + text, no background, tight spacing.</summary>
        Minimal
    }
}

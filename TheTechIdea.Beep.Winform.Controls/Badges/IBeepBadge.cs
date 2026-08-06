namespace TheTechIdea.Beep.Winform.Controls.Badges
{
    /// <summary>
    /// A badge that attaches itself to another control and tracks its position.
    /// </summary>
    /// <remarks>
    /// This interface used to carry the <i>border</i> colour but not the badge's own fill, text colour
    /// or shape — so <c>BeepBadgeFactory.Create("Counter")</c> handed back something that could not be
    /// styled without a cast to the concrete type, which defeated most of the point of the factory.
    /// </remarks>
    public interface IBeepBadge
    {
        Control? Target { get; }
        BadgeLocation Location { get; set; }

        /// <summary>Which theme colour the badge takes when no explicit colour is set.</summary>
        BeepFloatingBadge.BadgeRole Role { get; set; }

        BadgeShape Shape { get; set; }

        /// <summary>The badge's fill. Setting it opts this badge out of theme-driven colour.</summary>
        Color BadgeBackColor { get; set; }

        /// <summary>The badge's text or glyph colour. Setting it opts out of theme-driven colour.</summary>
        Color BadgeForeColor { get; set; }

        bool ShowDropShadow { get; set; }
        bool ShowBorder { get; set; }
        Color BorderColor { get; set; }

        void Attach(Control target);
        void Detach();
        void Reposition();

        /// <summary>Re-reads the current theme.</summary>
        void ApplyTheme();
    }

    /// <summary>
    /// A badge that displays text.
    /// </summary>
    /// <remarks>
    /// Separate from <see cref="IBeepBadge"/> because text means nothing on a dot badge. Putting
    /// <c>DisplayText</c> on the base interface would force four of the six built-ins to carry a member
    /// they ignore.
    /// </remarks>
    public interface IBeepTextBadge : IBeepBadge
    {
        string DisplayText { get; set; }
    }
}

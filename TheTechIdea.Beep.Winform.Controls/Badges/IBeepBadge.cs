namespace TheTechIdea.Beep.Winform.Controls.Badges
{
    public interface IBeepBadge
    {
        Control? Target { get; }
        BadgeLocation Location { get; set; }
        bool ShowDropShadow { get; set; }
        bool ShowBorder { get; set; }
        Color BorderColor { get; set; }
        void Attach(Control target);
        void Detach();
        void Reposition();
    }
}

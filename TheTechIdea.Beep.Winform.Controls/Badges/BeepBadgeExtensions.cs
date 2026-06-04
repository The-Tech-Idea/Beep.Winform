using TheTechIdea.Beep.Winform.Controls.Badges.Builtin;

namespace TheTechIdea.Beep.Winform.Controls.Badges
{
    public static class BeepBadgeExtensions
    {
        public static IBeepBadge ShowBadge(this Control control, IBeepBadge badge)
        {
            if (control is null)
                throw new ArgumentNullException(nameof(control));
            if (badge is null)
                throw new ArgumentNullException(nameof(badge));
            if (control.Parent is null)
                throw new InvalidOperationException("Target control must have a parent before attaching a badge.");

            ClearBadge(control);
            badge.Attach(control);
            BeepBadgeManager.RegisterBadge(control.Parent, badge);
            return badge;
        }

        public static TBadge ShowBadge<TBadge>(this Control control, Action<TBadge>? configure = null)
            where TBadge : IBeepBadge, new()
        {
            var badge = new TBadge();
            configure?.Invoke(badge);
            ShowBadge(control, badge);
            return badge;
        }

        public static void ClearBadge(this Control control)
        {
            if (control is null) return;
            BeepBadgeManager.ClearBadges(control);
        }

        public static BeepCounterBadge ShowCounterBadge(this Control control, string text,
            Color? backColor = null, BadgeAnchor anchor = BadgeAnchor.TopRight)
        {
            var badge = new BeepCounterBadge(text)
            {
                Anchor = anchor
            };
            if (backColor.HasValue) badge.BadgeBackColor = backColor.Value;
            ShowBadge(control, badge);
            return badge;
        }

        public static BeepDotBadge ShowDotBadge(this Control control, Color? color = null,
            BadgeAnchor anchor = BadgeAnchor.TopRight)
        {
            var badge = new BeepDotBadge
            {
                Anchor = anchor
            };
            if (color.HasValue) badge.BadgeBackColor = color.Value;
            ShowBadge(control, badge);
            return badge;
        }

        public static BeepIconBadge ShowIconBadge(this Control control, string svgPath,
            Color? tint = null, BadgeAnchor anchor = BadgeAnchor.TopRight)
        {
            var badge = new BeepIconBadge(svgPath)
            {
                Anchor = anchor
            };
            if (tint.HasValue) badge.BadgeForeColor = tint.Value;
            ShowBadge(control, badge);
            return badge;
        }

        public static BeepTextBadge ShowTextBadge(this Control control, string text,
            Color? backColor = null, Color? foreColor = null)
        {
            var badge = new BeepTextBadge(text);
            if (backColor.HasValue) badge.BadgeBackColor = backColor.Value;
            if (foreColor.HasValue) badge.BadgeForeColor = foreColor.Value;
            ShowBadge(control, badge);
            return badge;
        }

        public static BeepValidationBadge ShowValidationBadge(this Control control,
            ValidationState state, string? message = null)
        {
            var badge = new BeepValidationBadge(state) { Message = message ?? string.Empty };
            ShowBadge(control, badge);
            return badge;
        }

        public static BeepNotificationBadge ShowNotificationBadge(this Control control,
            string text, Color? backColor = null)
        {
            var badge = new BeepNotificationBadge(text);
            if (backColor.HasValue) badge.BadgeBackColor = backColor.Value;
            ShowBadge(control, badge);
            return badge;
        }
    }
}

using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Painters
{
    /// <summary>
    /// Creates notification painters routed first on <see cref="NotificationLayout"/>
    /// (layout-specific painters), then on <see cref="NotificationVisualStyle"/>
    /// (16 fully-differentiated visual painters). See plan §9 for routing matrix.
    /// </summary>
    public static class NotificationPainterFactory
    {
        /// <summary>
        /// Creates a painter, wires <paramref name="ownerControl"/> and pushes
        /// <paramref name="theme"/> fonts in one call.
        /// </summary>
        public static INotificationPainter CreatePainter(
            NotificationLayout layout,
            NotificationVisualStyle style = NotificationVisualStyle.Material3,
            Control ownerControl = null,
            IBeepTheme theme = null)
        {
            INotificationPainter painter = layout switch
            {
                // ── Existing 5 layout painters ────────────────────────────────
                NotificationLayout.Standard  => CreateStylePainter(style),
                NotificationLayout.Compact   => new CompactNotificationPainter(),
                NotificationLayout.Prominent => new ProminentNotificationPainter(),
                NotificationLayout.Banner    => new BannerNotificationPainter(),
                NotificationLayout.Toast     => new ToastNotificationPainter(),

                // ── 11 new layout-specific painters (Phase 3) ─────────────────
                NotificationLayout.Elevated    => new ElevatedNotificationPainter(),
                NotificationLayout.Snackbar    => new SnackbarNotificationPainter(),
                NotificationLayout.InlineAlert => new InlineAlertNotificationPainter(),
                NotificationLayout.Callout     => new CalloutNotificationPainter(),
                NotificationLayout.Chip        => new ChipNotificationPainter(),
                NotificationLayout.Timeline    => new TimelineNotificationPainter(),
                NotificationLayout.MediaRich   => new MediaRichNotificationPainter(),
                NotificationLayout.ActionSheet => new ActionSheetNotificationPainter(),
                NotificationLayout.StatusBar   => new StatusBarNotificationPainter(),
                NotificationLayout.Overlay     => new OverlayNotificationPainter(),
                NotificationLayout.FullWidth   => new FullWidthNotificationPainter(),
                _                              => CreateStylePainter(style),
            };

            // Wire owner and push theme fonts
            painter.OwnerControl = ownerControl;
            if (theme != null) painter.RefreshFonts(theme);

            return painter;
        }

        /// <summary>Legacy overload — creates a Standard-style painter for the layout.</summary>
        public static INotificationPainter CreatePainter(NotificationLayout layout)
            => CreatePainter(layout, NotificationVisualStyle.Material3);

        // ── 16 visual-style painters (style routing) ──────────────────────────

        private static INotificationPainter CreateStylePainter(NotificationVisualStyle style)
            => style switch
            {
                NotificationVisualStyle.Material3       => new Material3NotificationPainter(),
                NotificationVisualStyle.iOS15           => new iOS15NotificationPainter(),
                NotificationVisualStyle.AntDesign       => new AntDesignNotificationPainter(),
                NotificationVisualStyle.Fluent2         => new Fluent2NotificationPainter(),
                NotificationVisualStyle.MaterialYou     => new MaterialYouNotificationPainter(),
                NotificationVisualStyle.Windows11Mica   => new Windows11MicaNotificationPainter(),
                NotificationVisualStyle.MacOSBigSur     => new MacOSBigSurNotificationPainter(),
                NotificationVisualStyle.ChakraUI        => new ChakraUINotificationPainter(),
                NotificationVisualStyle.TailwindCard    => new TailwindCardNotificationPainter(),
                NotificationVisualStyle.NotionMinimal   => new NotionMinimalNotificationPainter(),
                NotificationVisualStyle.Minimal         => new MinimalNotificationPainter(),
                NotificationVisualStyle.VercelClean     => new VercelCleanNotificationPainter(),
                NotificationVisualStyle.StripeDashboard => new StripeDashboardNotificationPainter(),
                NotificationVisualStyle.DarkGlow        => new DarkGlowNotificationPainter(),
                NotificationVisualStyle.DiscordStyle    => new DiscordStyleNotificationPainter(),
                NotificationVisualStyle.GradientModern  => new GradientModernNotificationPainter(),
                _                                       => new Material3NotificationPainter(),
            };
    }
}

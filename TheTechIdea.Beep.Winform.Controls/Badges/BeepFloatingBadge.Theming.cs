using System.ComponentModel;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Badges
{
    public partial class BeepFloatingBadge
    {
        /// <summary>
        /// Which theme colour a badge takes when the caller has not chosen one.
        /// </summary>
        public enum BadgeRole
        {
            /// <summary>The theme's badge colours — the ordinary counter or dot.</summary>
            Default,

            /// <summary>The theme's accent — an informational or branded badge.</summary>
            Accent,

            /// <summary>The theme's surface, for a badge that carries an icon rather than a colour.</summary>
            Surface,

            Error,
            Success,
            Warning,
            Info,
        }

        private BadgeRole _role = BadgeRole.Default;

        // A colour the caller set explicitly must survive a theme change. Without these flags,
        // ApplyTheme would overwrite a deliberate choice every time the theme moved.
        private bool _backColorExplicit;
        private bool _foreColorExplicit;
        private bool _borderColorExplicit;
        private bool _shadowColorExplicit;

        private bool _subscribedToTheme;

        /// <summary>
        /// The theme colours this badge takes when the caller has not overridden them.
        /// </summary>
        /// <remarks>
        /// The four state roles carry meaning rather than style — an error badge is red because red
        /// means error. They resolve from the theme's semantic slots, so they follow a dark or
        /// high-contrast palette while staying recognisable, instead of being the literal ARGB values
        /// this class used to hold.
        /// </remarks>
        [Category("Appearance")]
        [Description("Which theme colour the badge takes when no explicit colour is set.")]
        [DefaultValue(BadgeRole.Default)]
        public BadgeRole Role
        {
            get => _role;
            set
            {
                if (_role == value) return;
                _role = value;
                ApplyTheme();
                Invalidate();
            }
        }

        private Font? _badgeFont;

        /// <summary>
        /// The font family and style the badge's text is drawn in. Size is derived from badge height.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <c>BaseControl.BadgeFont</c> was a public property that was written, disposed on teardown,
        /// and <b>never read</b>: <c>SyncBadgeAppearance</c> copied the badge's colours across and not
        /// its font, and no badge had a font property to copy one into. A caller could set it, read it
        /// back unchanged, and never see it render.
        /// </para>
        /// <para>
        /// Size is not taken from this font. A badge sizes its text to its own height, so the size a
        /// caller supplies would be overridden on the next repaint anyway; family and style are what
        /// carry across.
        /// </para>
        /// </remarks>
        [Category("Appearance")]
        [Description("Font family and style for badge text. Size is derived from the badge's height.")]
        [DefaultValue(null)]
        public Font? BadgeFont
        {
            get => _badgeFont;
            set
            {
                if (ReferenceEquals(_badgeFont, value)) return;
                _badgeFont?.Dispose();
                _badgeFont = value;
                ApplyBadgeSize();
                Invalidate();
            }
        }

        /// <summary>
        /// A font at <paramref name="size"/>, in the caller's family or the system default.
        /// </summary>
        /// <remarks>
        /// The badges used to construct <c>new Font("Segoe UI", size, Bold)</c> inline. Naming a family
        /// that may not be installed, and that is wrong for several locales, is a rendering risk for no
        /// gain — the system default font family is the right fallback and needs no lookup table.
        /// </remarks>
        protected Font BadgeFontFor(float size)
        {
            var family = _badgeFont?.FontFamily ?? SystemFonts.DefaultFont.FontFamily;
            var style = _badgeFont?.Style ?? FontStyle.Bold;
            return new Font(family, size, style);
        }

        /// <summary>The theme in force, or the default when none has been selected.</summary>
        protected static IBeepTheme? CurrentTheme
        {
            get
            {
                var theme = BeepThemesManager.GetTheme(BeepThemesManager.CurrentThemeName);
                return theme ?? BeepThemesManager.GetDefaultTheme();
            }
        }

        /// <summary>
        /// Follows the current theme. Called on construction and whenever the theme changes.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This class derives from <see cref="UserControl"/> rather than <c>BaseControl</c>, so it does
        /// not inherit the automatic re-theming every other Beep control gets. Subscribing directly is
        /// the cheap half of that: it buys the behaviour that was missing — badges following the theme —
        /// without inheriting a large base control's painting, hit-testing, hover and focus machinery
        /// into a 10-24px decoration that is <c>TabStop = false</c>.
        /// </para>
        /// <para>
        /// Re-parenting onto <c>BaseControl</c> stays available if a later stage finds it buys something
        /// this does not. It would have to keep <c>SupportsTransparentBackColor</c> and the
        /// behind-the-target z-order working, which is why it was not the first move.
        /// </para>
        /// </remarks>
        public virtual void ApplyTheme()
        {
            var theme = CurrentTheme;
            if (theme is null)
            {
                // No theme registered at all. System colours follow the OS's own light/dark and
                // high-contrast settings, which is the closest thing to "themed" available here, and
                // is not a brand literal picked out of the air.
                if (!_backColorExplicit) _badgeBackColor = SystemColors.Highlight;
                if (!_foreColorExplicit) _badgeForeColor = SystemColors.HighlightText;
                if (!_borderColorExplicit) _borderColor = SystemColors.Window;
                if (!_shadowColorExplicit) _shadowColor = Color.FromArgb(80, SystemColors.ControlDarkDark);
                InvalidateCachedBrushes();
                return;
            }

            if (!_backColorExplicit) _badgeBackColor = ResolveBackColor(theme);
            if (!_foreColorExplicit) _badgeForeColor = ResolveForeColor(theme);
            if (!_borderColorExplicit) _borderColor = theme.SurfaceColor;
            if (!_shadowColorExplicit) _shadowColor = Color.FromArgb(80, theme.ShadowColor);

            InvalidateCachedBrushes();
        }

        private Color ResolveBackColor(IBeepTheme theme) => _role switch
        {
            BadgeRole.Accent => theme.AccentColor,
            BadgeRole.Surface => theme.SurfaceColor,
            BadgeRole.Error => theme.ErrorColor,
            BadgeRole.Success => theme.SuccessColor,
            BadgeRole.Warning => theme.WarningColor,
            BadgeRole.Info => theme.AccentColor,
            _ => theme.BadgeBackColor,
        };

        private Color ResolveForeColor(IBeepTheme theme) => _role switch
        {
            BadgeRole.Surface => theme.PrimaryColor,
            _ => theme.BadgeForeColor,
        };

        private void SubscribeToTheme()
        {
            if (_subscribedToTheme) return;
            BeepThemesManager.ThemeChanged += OnGlobalThemeChanged;
            _subscribedToTheme = true;
        }

        private void UnsubscribeFromTheme()
        {
            if (!_subscribedToTheme) return;
            BeepThemesManager.ThemeChanged -= OnGlobalThemeChanged;
            _subscribedToTheme = false;
        }

        private void OnGlobalThemeChanged(object? sender, ThemeChangeEventArgs e)
        {
            if (IsDisposed) return;
            ApplyTheme();
            Invalidate();
        }
    }
}

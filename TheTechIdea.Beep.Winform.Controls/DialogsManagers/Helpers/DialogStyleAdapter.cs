using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Helpers
{
    public static class DialogStyleAdapter
    {
        private static readonly Color _defaultAccent = Color.FromArgb(59, 130, 246);
        private static readonly Color _defaultError = Color.FromArgb(220, 38, 38);
        private static readonly Color _defaultWarning = Color.FromArgb(245, 158, 11);
        private static readonly Color _defaultSuccess = Color.FromArgb(16, 185, 129);
        private static readonly Color _defaultInfo = Color.FromArgb(33, 150, 243);
        private static readonly Color _defaultOrange = Color.FromArgb(255, 152, 0);
        private static readonly Color _defaultGreen = Color.FromArgb(76, 175, 80);
        private static readonly Color _defaultRed = Color.FromArgb(244, 67, 54);

        public static BeepControlStyle GetBeepControlStyle(DialogConfig config)
        {
            if (config == null)
                return BeepStyling.CurrentControlStyle;

            return config.Style;
        }

        public static string GetIconPath(DialogConfig config)
        {
            if (config == null)
                return string.Empty;

            if (!string.IsNullOrEmpty(config.IconPath))
                return config.IconPath;

            return config.IconType switch
            {
                BeepDialogIcon.Information => Svgs.Information,
                BeepDialogIcon.Warning => Svgs.InfoWarning,
                BeepDialogIcon.Error => Svgs.Error,
                BeepDialogIcon.Question => Svgs.Question,
                BeepDialogIcon.Success => Svgs.CheckCircle,
                BeepDialogIcon.None => string.Empty,
                _ => Svgs.Information
            };
        }

        public static Color GetIconColor(DialogConfig config, IBeepTheme theme)
        {
            if (theme == null)
                return GetDefaultIconColor(config.IconType);

            return config.IconType switch
            {
                BeepDialogIcon.Information => theme.AccentColor != Color.Empty ? theme.AccentColor : _defaultInfo,
                BeepDialogIcon.Warning => theme.WarningColor != Color.Empty ? theme.WarningColor : _defaultOrange,
                BeepDialogIcon.Error => theme.ErrorColor != Color.Empty ? theme.ErrorColor : _defaultRed,
                BeepDialogIcon.Question => theme.AccentColor != Color.Empty ? theme.AccentColor : _defaultInfo,
                BeepDialogIcon.Success => theme.SuccessColor != Color.Empty ? theme.SuccessColor : _defaultGreen,
                _ => theme.ForeColor != Color.Empty ? theme.ForeColor : ColorUtils.MapSystemColor(SystemColors.ControlText)
            };
        }

        private static Color GetDefaultIconColor(BeepDialogIcon iconType)
        {
            return iconType switch
            {
                BeepDialogIcon.Information => _defaultInfo,
                BeepDialogIcon.Warning => _defaultOrange,
                BeepDialogIcon.Error => _defaultRed,
                BeepDialogIcon.Question => _defaultInfo,
                BeepDialogIcon.Success => _defaultGreen,
                _ => ColorUtils.MapSystemColor(SystemColors.ControlText)
            };
        }

        public static DialogColors GetColors(DialogConfig config, IBeepTheme theme)
        {
            var colors = new DialogColors();

            if (theme != null && config.UseBeepThemeColors)
            {
                colors.Background = config.BackColor ?? (theme.DialogBackColor != Color.Empty ? theme.DialogBackColor : ColorUtils.MapSystemColor(SystemColors.Window));
                colors.Foreground = config.ForeColor ?? (theme.DialogForeColor != Color.Empty ? theme.DialogForeColor : ColorUtils.MapSystemColor(SystemColors.WindowText));
                colors.Border = config.BorderColor ?? (theme.BorderColor != Color.Empty ? theme.BorderColor : ColorUtils.MapSystemColor(SystemColors.ControlDark));
                colors.Accent = theme.AccentColor != Color.Empty ? theme.AccentColor : _defaultAccent;
                colors.TitleBackground = theme.AccentColor != Color.Empty ? theme.AccentColor : _defaultAccent;
                colors.TitleForeground = ColorUtils.GetContrastColor(colors.TitleBackground);
                colors.ButtonBackground = theme.DialogOkButtonBackColor != Color.Empty ? theme.DialogOkButtonBackColor : ColorUtils.MapSystemColor(SystemColors.Control);
                colors.ButtonForeground = theme.DialogOkButtonForeColor != Color.Empty ? theme.DialogOkButtonForeColor : ColorUtils.MapSystemColor(SystemColors.ControlText);
                colors.ButtonBorder = theme.DialogOkButtonHoverBorderColor != Color.Empty ? theme.DialogOkButtonHoverBorderColor : ColorUtils.MapSystemColor(SystemColors.ControlDark);
            }
            else
            {
                colors.Background = config.BackColor ?? ColorUtils.MapSystemColor(SystemColors.Window);
                colors.Foreground = config.ForeColor ?? ColorUtils.MapSystemColor(SystemColors.WindowText);
                colors.Border = config.BorderColor ?? ColorUtils.MapSystemColor(SystemColors.ControlDark);
                colors.Accent = _defaultAccent;
                colors.TitleBackground = _defaultAccent;
                colors.TitleForeground = Color.White;
                colors.ButtonBackground = ColorUtils.MapSystemColor(SystemColors.Control);
                colors.ButtonForeground = ColorUtils.MapSystemColor(SystemColors.ControlText);
                colors.ButtonBorder = ColorUtils.MapSystemColor(SystemColors.ControlDark);
            }

            if (config.ShadowColor.HasValue)
                colors.Shadow = config.ShadowColor.Value;
            else if (theme != null)
                colors.Shadow = Color.FromArgb(100, ColorUtils.MapSystemColor(SystemColors.ControlText));
            else
                colors.Shadow = Color.FromArgb(80, ColorUtils.MapSystemColor(SystemColors.ControlText));

            return colors;
        }

        /// <summary>
        /// Resolves a dialog's severity once: explicit override, then preset, then icon, then neutral.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is the single source stage 06 exists to create. Before it, the colour resolvers keyed
        /// off <see cref="DialogPreset"/> alone while the icon came from
        /// <see cref="DialogConfig.IconType"/> — so <c>IconType = Error</c> with no matching preset
        /// produced an error icon sitting on a neutral dialog, and nothing reconciled the two.
        /// </para>
        /// <para>
        /// The precedence is deliberate. A preset is a statement about what the dialog <i>is</i>
        /// ("this is a destructive confirmation"); an icon is a statement about how it <i>looks</i>.
        /// When they disagree the stronger claim wins, so the whole dialog is coloured by its meaning
        /// and the icon follows — rather than the header saying one thing and the glyph another.
        /// </para>
        /// </remarks>
        public static DialogSeverity ResolveSeverity(DialogConfig config)
        {
            if (config == null) return DialogSeverity.Neutral;
            if (config.Severity.HasValue) return config.Severity.Value;

            var fromPreset = SeverityOf(config.Preset);
            if (fromPreset != DialogSeverity.Neutral) return fromPreset;

            return SeverityOf(config.IconType);
        }

        private static DialogSeverity SeverityOf(DialogPreset preset) => preset switch
        {
            DialogPreset.DestructiveConfirm => DialogSeverity.Error,
            DialogPreset.BlockingError      => DialogSeverity.Error,
            DialogPreset.Danger             => DialogSeverity.Error,
            DialogPreset.Warning            => DialogSeverity.Warning,
            DialogPreset.UnsavedChanges     => DialogSeverity.Warning,
            DialogPreset.InlineValidation   => DialogSeverity.Warning,
            DialogPreset.SessionTimeout     => DialogSeverity.Warning,
            DialogPreset.Success            => DialogSeverity.Success,
            DialogPreset.SuccessWithUndo    => DialogSeverity.Success,
            DialogPreset.Information        => DialogSeverity.Info,
            DialogPreset.Question           => DialogSeverity.Info,
            _                               => DialogSeverity.Neutral
        };

        private static DialogSeverity SeverityOf(BeepDialogIcon icon) => icon switch
        {
            BeepDialogIcon.Error       => DialogSeverity.Error,
            BeepDialogIcon.Warning     => DialogSeverity.Warning,
            BeepDialogIcon.Success     => DialogSeverity.Success,
            BeepDialogIcon.Information => DialogSeverity.Info,
            BeepDialogIcon.Question    => DialogSeverity.Info,
            _                          => DialogSeverity.Neutral
        };

        /// <summary>
        /// The saturated colour for a severity — buttons, icons, the <c>ColorBlock</c> header fill.
        /// </summary>
        /// <remarks>
        /// Resolved from the theme, never from a literal. The reference images' greens and reds are
        /// the <i>design</i>; the palette belongs to the theme, so switching theme must move every
        /// severity colour with it. The fallbacks apply only when a theme leaves a slot empty.
        /// </remarks>
        public static Color GetSeverityAccent(DialogSeverity severity, IBeepTheme? theme)
        {
            var accent  = theme?.AccentColor  is { IsEmpty: false } a ? a : _defaultAccent;
            var error   = theme?.ErrorColor   is { IsEmpty: false } e ? e : _defaultError;
            var warning = theme?.WarningColor is { IsEmpty: false } w ? w : _defaultWarning;
            var success = theme?.SuccessColor is { IsEmpty: false } s ? s : _defaultSuccess;

            return severity switch
            {
                DialogSeverity.Error   => error,
                DialogSeverity.Warning => warning,
                DialogSeverity.Success => success,
                DialogSeverity.Info    => accent,
                _                      => accent
            };
        }

        /// <summary>
        /// The low-saturation wash for a severity — the <c>Strip</c> header and the tinted surface.
        /// </summary>
        /// <remarks>
        /// Composited against <paramref name="over"/> rather than returned with an alpha channel.
        /// A translucent <c>BackColor</c> is not honoured by every control that inherits it, so an
        /// opaque blend is what actually renders the same on the band and on the labels sitting in it.
        /// Neutral has no wash — the dialog keeps its plain background.
        /// </remarks>
        public static Color GetSeverityWash(DialogSeverity severity, IBeepTheme? theme, Color over, double strength = 0.10)
        {
            if (severity == DialogSeverity.Neutral) return over;

            Color accent = GetSeverityAccent(severity, theme);
            double k = Math.Clamp(strength, 0d, 1d);

            return Color.FromArgb(
                (int)Math.Round(over.R + (accent.R - over.R) * k),
                (int)Math.Round(over.G + (accent.G - over.G) * k),
                (int)Math.Round(over.B + (accent.B - over.B) * k));
        }

        public static string GetButtonText(Vis.Modules.BeepDialogButtons button)
        {
            return button switch
            {
                Vis.Modules.BeepDialogButtons.Ok => "OK",
                Vis.Modules.BeepDialogButtons.Cancel => "Cancel",
                Vis.Modules.BeepDialogButtons.Yes => "Yes",
                Vis.Modules.BeepDialogButtons.No => "No",
                Vis.Modules.BeepDialogButtons.Abort => "Abort",
                Vis.Modules.BeepDialogButtons.Retry => "Retry",
                Vis.Modules.BeepDialogButtons.Ignore => "Ignore",
                Vis.Modules.BeepDialogButtons.Stop => "Stop",
                Vis.Modules.BeepDialogButtons.Continue => "Continue",
                _ => button.ToString()
            };
        }
    }

    public class DialogColors
    {
        public Color Background { get; set; }
        public Color Foreground { get; set; }
        public Color Border { get; set; }
        public Color Shadow { get; set; }
        public Color Accent { get; set; }
        public Color TitleBackground { get; set; }
        public Color TitleForeground { get; set; }
        public Color ButtonBackground { get; set; }
        public Color ButtonForeground { get; set; }
        public Color ButtonBorder { get; set; }
    }
}

using TheTechIdea.Beep.Icons;

namespace TheTechIdea.Beep.Winform.Controls.TextFields.Models
{
    /// <summary>
    /// Curated built-in icons for <see cref="BeepTextBox"/>, designer-selectable as a dropdown.
    /// Each value maps to an embedded SVG in <see cref="SvgsUIcons"/> — the validated registry
    /// that reports (once) when a constant names a resource that is not embedded.
    /// <see cref="None"/> means no built-in icon; a manually set ImagePath always wins and
    /// resets the kind to None.
    /// </summary>
    public enum TextBoxIconKind
    {
        None,
        Search,
        User,
        Password,
        Email,
        Phone,
        Calendar,
        Clock,
        Folder,
        Document,
        Edit,
        Filter,
        Settings,
        Info,
        Warning,
        Error,
        Success,
        Home,
        Tag,
        Location,
        Star,
        Heart,
        CreditCard,
        Money,
        Wifi,
        Camera,
        Image,
        Bell,
        Visible,
        Hidden,
    }

    /// <summary>Maps <see cref="TextBoxIconKind"/> to embedded SVG resource paths.</summary>
    public static class TextBoxIconRegistry
    {
        /// <summary>
        /// Resource path for the given kind; empty for <see cref="TextBoxIconKind.None"/> or a
        /// kind whose resource is missing (the registry reports that once by itself).
        /// </summary>
        public static string GetPath(TextBoxIconKind kind) => kind switch
        {
            TextBoxIconKind.Search => SvgsUIcons.Common.Search,
            // No plain fi-tr-user.svg is embedded - the probe caught the guess.
            TextBoxIconKind.User => SvgsUIcons.ToResourcePath("fi-tr-circle-user.svg") ?? string.Empty,
            TextBoxIconKind.Password => SvgsUIcons.Common.Lock,
            TextBoxIconKind.Email => SvgsUIcons.Common.Envelope,
            TextBoxIconKind.Phone => SvgsUIcons.Common.Phone,
            TextBoxIconKind.Calendar => SvgsUIcons.Common.Calendar,
            TextBoxIconKind.Clock => SvgsUIcons.Common.Clock,
            TextBoxIconKind.Folder => SvgsUIcons.Common.Folder,
            TextBoxIconKind.Document => SvgsUIcons.Common.Document,
            TextBoxIconKind.Edit => SvgsUIcons.Common.Edit,
            TextBoxIconKind.Filter => SvgsUIcons.Common.Filter,
            TextBoxIconKind.Settings => SvgsUIcons.Common.Settings,
            TextBoxIconKind.Info => SvgsUIcons.Common.Info,
            TextBoxIconKind.Warning => SvgsUIcons.Common.Warning,
            TextBoxIconKind.Error => SvgsUIcons.Common.Error,
            TextBoxIconKind.Success => SvgsUIcons.Common.Success,
            TextBoxIconKind.Home => SvgsUIcons.Common.Home,
            TextBoxIconKind.Tag => SvgsUIcons.Common.Tag,
            TextBoxIconKind.Location => SvgsUIcons.Common.MapPin,
            TextBoxIconKind.Star => SvgsUIcons.Common.Star,
            TextBoxIconKind.Heart => SvgsUIcons.Common.Heart,
            TextBoxIconKind.CreditCard => SvgsUIcons.Finance.CreditCard,
            TextBoxIconKind.Money => SvgsUIcons.Finance.MoneyBillWave,
            TextBoxIconKind.Wifi => SvgsUIcons.NetworkCloud.Wifi,
            TextBoxIconKind.Camera => SvgsUIcons.Common.Camera,
            TextBoxIconKind.Image => SvgsUIcons.Common.Image,
            TextBoxIconKind.Bell => SvgsUIcons.Common.Bell,
            TextBoxIconKind.Visible => SvgsUIcons.Common.Visible,
            TextBoxIconKind.Hidden => SvgsUIcons.Common.Hidden,
            _ => string.Empty,
        };
    }
}

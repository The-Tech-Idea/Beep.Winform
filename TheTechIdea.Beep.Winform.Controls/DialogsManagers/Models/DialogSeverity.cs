namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models
{
    /// <summary>
    /// What a dialog means, as one value.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Severity was previously implied by two independent inputs — <see cref="DialogConfig.Preset"/>
    /// and <see cref="DialogConfig.IconType"/> — and only <c>Preset</c> reached the colour resolvers.
    /// A caller who set <c>IconType = Error</c> without a matching preset got an error icon on a
    /// neutral dialog: two inputs, one visual outcome, no single source.
    /// </para>
    /// <para>
    /// Everything downstream reads this one value — header tint, icon colour, button colour
    /// (stage 07) and callout accent (stage 08) — which is why it is resolved once, in
    /// <c>DialogStyleAdapter.ResolveSeverity</c>.
    /// </para>
    /// </remarks>
    public enum DialogSeverity
    {
        /// <summary>No semantic colour; the dialog uses the theme's neutral chrome.</summary>
        Neutral,

        /// <summary>Informational. `dialog1.png`'s "Dialog default" and "Action dialog" rows.</summary>
        Info,

        /// <summary>A positive outcome. `dialog1.png`'s "Positive dialog", `dialog4.png`'s green band.</summary>
        Success,

        /// <summary>Caution, but not failure.</summary>
        Warning,

        /// <summary>A failure or a destructive action. `dialog1.png`'s "Error dialog".</summary>
        Error
    }

    /// <summary>
    /// How a dialog's header is presented.
    /// </summary>
    /// <remarks>
    /// The reference images use three, not one: `dialog1.png` puts a tinted strip across the top,
    /// `dialog4.png` replaces it with a saturated colour block carrying the title and a large icon,
    /// and `dialog5.png` / `dialog6.png` have no header at all — the severity lives in a centred mark
    /// instead.
    /// </remarks>
    public enum DialogHeaderStyle
    {
        /// <summary>A tinted strip carrying the title. The default, and what the shell rendered before.</summary>
        Strip,

        /// <summary>A saturated full-bleed block carrying the title and icon, with on-accent text.</summary>
        ColorBlock,

        /// <summary>No header band; used by the centred layouts in stage 08.</summary>
        None
    }

    /// <summary>
    /// Whether the dialog body carries the severity colour.
    /// </summary>
    /// <remarks>
    /// The two columns of `dialog1.png`: the left keeps a white body under a tinted header, the right
    /// tints the whole surface at low saturation.
    /// </remarks>
    public enum DialogSurfaceTreatment
    {
        /// <summary>Body stays the theme's dialog background.</summary>
        Plain,

        /// <summary>Body carries the severity colour at low saturation.</summary>
        Tinted
    }

    /// <summary>
    /// How a dialog arranges its icon, text and actions.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Read together, the reference images are four presentations of the same content rather than one
    /// design with variations:
    /// </para>
    /// <list type="bullet">
    /// <item><b>TitleBar</b> — `dialog1.png`. Icon in a left gutter, text left-aligned beside it,
    /// actions bottom-right. What every dialog in this folder renders today.</item>
    /// <item><b>Centred</b> — `dialog2.png`. Icon above the title, everything centred, the two actions
    /// side by side and centred. The shape Apple HIG and Material 3 both use for a destructive
    /// confirmation.</item>
    /// <item><b>HeroBand</b> — `dialog4.png`. A large centred mark and headline at the top, message
    /// centred beneath, a single centred action.</item>
    /// <item><b>Flat</b> — `dialog6.png`. An oversized mark, small left-aligned label and message, and
    /// the action rendered as text rather than a button.</item>
    /// </list>
    /// <para>
    /// This is <i>arrangement</i>, which the dialogs own through their grid. The colour those designs
    /// also carry — a tinted header strip, a saturated band — is the theme's, not this folder's; see
    /// the master tracker.
    /// </para>
    /// </remarks>
    public enum DialogPresentation
    {
        /// <summary>Icon gutter, left-aligned text, right-aligned actions. The default.</summary>
        TitleBar,

        /// <summary>Icon above centred text, with centred actions.</summary>
        Centred,

        /// <summary>A large centred mark and headline, message beneath, one centred action.</summary>
        HeroBand,

        /// <summary>Oversized mark, left-aligned text, text-style action.</summary>
        Flat
    }
}

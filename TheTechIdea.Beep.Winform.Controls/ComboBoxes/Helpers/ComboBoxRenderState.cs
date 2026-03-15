using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers
{
    /// <summary>
    /// Resolved color and font tokens derived from the active theme.
    /// Built once per paint cycle by <see cref="ComboBoxStateFactory"/> so painters
    /// never query <c>IBeepTheme</c> fields more than once per frame.
    /// </summary>
    internal sealed class ComboBoxThemeTokens
    {
        // ── Theme name for scrollbar/child theming ──────────────────────────
        public string ThemeName          { get; init; }
        
        // ── Field shell ─────────────────────────────────────────────────────
        public Color BackColor           { get; init; }
        public Color ForeColor           { get; init; }
        public Color BorderColor         { get; init; }
        public Color HoverBorderColor    { get; init; }
        public Color FocusBorderColor    { get; init; }
        public Color OpenBorderColor     { get; init; }

        // ── Disabled ────────────────────────────────────────────────────────
        public Color DisabledBackColor   { get; init; }
        public Color DisabledForeColor   { get; init; }

        // ── Validation ──────────────────────────────────────────────────────
        public Color ErrorBorderColor    { get; init; }
        public Color WarningBorderColor  { get; init; }
        public Color SuccessBorderColor  { get; init; }
        public Color ErrorForeColor      { get; init; }

        // ── Affordances ─────────────────────────────────────────────────────
        public Color PlaceholderColor       { get; init; }
        public Color ChevronColor           { get; init; }
        public Color ClearButtonColor       { get; init; }
        public Color SelectionHighlight     { get; init; }
        public Color ButtonHoverBackground  { get; init; }

        // ── Chip / selected-item tokens ──────────────────────────────────
        public Color SelectedBackColor      { get; init; }
        public Color SelectedForeColor      { get; init; }
        public Color SelectedBorderColor    { get; init; }

        // ── Popup surface ───────────────────────────────────────────────────
        public Color PopupBackColor         { get; init; }
        public Color PopupBorderColor       { get; init; }
        public Color PopupRowHoverColor     { get; init; }
        public Color PopupRowSelectedColor  { get; init; }
        public Color PopupRowFocusColor     { get; init; }
        public Color PopupGroupHeaderBack   { get; init; }
        public Color PopupGroupHeaderFore   { get; init; }
        public Color PopupSeparatorColor    { get; init; }
        public Color PopupSubTextColor      { get; init; }

        // ── Typography ──────────────────────────────────────────────────────
        public Font LabelFont   { get; init; }
        public Font SubTextFont { get; init; }

        /// <summary>Safe fallback used when no active theme is available.</summary>
        public static ComboBoxThemeTokens Fallback() => new ComboBoxThemeTokens
        {
            ThemeName              = BeepThemesManager.CurrentThemeName,
            BackColor              = Color.White,
            ForeColor              = Color.FromArgb(33, 33, 33),
            BorderColor            = Color.FromArgb(176, 176, 176),
            HoverBorderColor       = Color.FromArgb(110, 110, 110),
            FocusBorderColor       = Color.FromArgb(25, 118, 210),
            OpenBorderColor        = Color.FromArgb(25, 118, 210),
            DisabledBackColor      = Color.FromArgb(245, 245, 245),
            DisabledForeColor      = Color.FromArgb(158, 158, 158),
            ErrorBorderColor       = Color.FromArgb(183, 28, 28),
            WarningBorderColor     = Color.FromArgb(230, 81, 0),
            SuccessBorderColor     = Color.FromArgb(27, 94, 32),
            ErrorForeColor         = Color.FromArgb(183, 28, 28),
            PlaceholderColor       = Color.FromArgb(158, 158, 158),
            ChevronColor           = Color.FromArgb(97, 97, 97),
            ClearButtonColor       = Color.FromArgb(120, 120, 120),
            SelectionHighlight     = Color.FromArgb(235, 245, 255),
            ButtonHoverBackground  = Color.FromArgb(20, 0, 0, 0),
            SelectedBackColor      = Color.FromArgb(227, 242, 253),
            SelectedForeColor      = Color.FromArgb(25, 118, 210),
            SelectedBorderColor    = Color.FromArgb(144, 202, 249),
            PopupBackColor         = Color.White,
            PopupBorderColor       = Color.FromArgb(200, 200, 200),
            PopupRowHoverColor     = Color.FromArgb(240, 248, 255),
            PopupRowSelectedColor  = Color.FromArgb(227, 242, 253),
            PopupRowFocusColor     = Color.FromArgb(200, 230, 255),
            PopupGroupHeaderBack   = Color.FromArgb(245, 245, 245),
            PopupGroupHeaderFore   = Color.FromArgb(97, 97, 97),
            PopupSeparatorColor    = Color.FromArgb(220, 220, 220),
            PopupSubTextColor      = Color.FromArgb(117, 117, 117),
            LabelFont              = SystemFonts.DefaultFont,
            SubTextFont            = SystemFonts.DefaultFont,
        };
    }

    /// <summary>
    /// Normalized snapshot of all visual and interaction state a painter
    /// needs to render one frame of <see cref="BeepComboBox"/>.
    /// <para>
    /// Built by <see cref="ComboBoxStateFactory.Build"/> and passed to painters
    /// and <see cref="ComboBoxLayoutEngine.Compute"/>. Does not hold a reference
    /// back to the owner control — no live state is read during painting.
    /// </para>
    /// </summary>
    internal sealed class ComboBoxRenderState
    {
        // ── Interaction flags ────────────────────────────────────────────────
        public bool IsHovered            { get; init; }
        public bool IsButtonHovered      { get; init; }
        public bool IsClearButtonHovered { get; init; }
        public bool IsFocused            { get; init; }
        public bool IsOpen               { get; init; }
        public bool IsDisabled           { get; init; }
        public bool IsReadOnly           { get; init; }

        // ── Loading / skeleton ───────────────────────────────────────────────
        public bool  IsLoading       { get; init; }
        public bool  IsSkeleton      { get; init; }
        public float LoadingAngle    { get; init; }   // degrees 0-360, animates per timer
        public float SkeletonOffset  { get; init; }   // 0-1 shimmer sweep position

        // ── Validation ───────────────────────────────────────────────────────
        public BeepComboBoxValidationState ValidationState   { get; init; }
        public string                      ValidationMessage { get; init; }

        // ── Value / display ──────────────────────────────────────────────────
        public bool   IsShowingPlaceholder { get; init; }
        public string DisplayText          { get; init; }
        public string PlaceholderText      { get; init; }
        public string InputText            { get; init; }

        // Image resolved from LeadingImagePath/LeadingIconPath or
        // selected SimpleItem.ImagePath (factory selects the correct source).
        public string LeadingImagePath { get; init; }
        public bool   HasLeadingImage  { get; init; }

        // ── Multi-select ─────────────────────────────────────────────────────
        public bool                      IsMultiSelect { get; init; }
        public IReadOnlyList<SimpleItem> SelectedChips { get; init; }

        // ── Affordances ──────────────────────────────────────────────────────
        public bool  ShowClearButton { get; init; }
        public float ChevronAngle    { get; init; }   // 0 = closed, 180 = open

        // ── Behavior family (derived from ComboBoxType) ───────────────────────
        // Standard select: none of the below — fully owner-drawn, no inline editor.
        public bool IsSearchable       { get; init; }
        public bool IsEditable         { get; init; }
        public bool IsVisualDisplay    { get; init; }   // image/color/icon picker
        public bool InlineEditorActive { get; init; }

        // ── Visual tokens ────────────────────────────────────────────────────
        public ComboBoxVisualTokens VisualTokens { get; init; }

        // ── Resolved theme tokens ─────────────────────────────────────────────
        public ComboBoxThemeTokens ThemeTokens { get; init; }
    }
}

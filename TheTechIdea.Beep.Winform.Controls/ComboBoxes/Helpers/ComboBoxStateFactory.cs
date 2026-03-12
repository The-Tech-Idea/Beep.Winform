using System;
using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers
{
    /// <summary>
    /// Builds a <see cref="ComboBoxRenderState"/> snapshot from the live state of a
    /// <see cref="BeepComboBox"/> owner control.
    /// <para>
    /// Call <see cref="Build"/> once at the start of <c>DrawContent</c> and
    /// <c>Draw(Graphics, Rectangle)</c>, then pass the snapshot to
    /// <see cref="ComboBoxLayoutEngine.Compute"/> and to the active painter.
    /// This ensures painters never reach back into the owner during a paint cycle.
    /// </para>
    /// </summary>
    internal static class ComboBoxStateFactory
    {
        /// <summary>
        /// Captures the current state of <paramref name="owner"/> into an immutable
        /// <see cref="ComboBoxRenderState"/> snapshot.
        /// </summary>
        public static ComboBoxRenderState Build(BeepComboBox owner)
        {
            if (owner == null)
                throw new ArgumentNullException(nameof(owner));

            var theme   = owner._currentTheme;
            var tokens  = ComboBoxVisualTokenCatalog.Resolve(owner.ComboBoxType);
            var helper  = owner.Helper;

            // ── Theme tokens ─────────────────────────────────────────────────
            var themeTokens = ResolveThemeTokens(owner, theme);

            // ── Behavior family flags ─────────────────────────────────────────
            bool isSearchable = ComboBoxVisualTokenCatalog.SupportsSearch(owner.ComboBoxType)
                                || owner.ShowSearchInDropdown;

            bool isEditable   = owner.AllowFreeText;

            // Visual-display variants show a non-text rendering (image, color swatch, icon).
            // Currently no ComboBoxType enum value maps to this — it will be added later.
            // Keeping the flag in the state so painters can react when it is introduced.
            bool isVisualDisplay = false;

            // ── Leading image path ────────────────────────────────────────────
            // Priority: LeadingImagePath on control > LeadingIconPath > selected item image
            string leadingPath = owner.LeadingImagePath;
            if (string.IsNullOrEmpty(leadingPath))
                leadingPath = owner.LeadingIconPath;
            if (string.IsNullOrEmpty(leadingPath) && owner.SelectedItem != null)
                leadingPath = owner.SelectedItem.ImagePath;

            // ── Display text ──────────────────────────────────────────────────
            bool   isPlaceholder = helper.IsShowingPlaceholder();
            string displayText   = helper.GetDisplayText();

            // ── Multi-select chips ────────────────────────────────────────────
            bool isMulti = owner.AllowMultipleSelection;
            IReadOnlyList<SimpleItem> chips = isMulti
                ? (IReadOnlyList<SimpleItem>)owner.SelectedItems ?? Array.Empty<SimpleItem>()
                : Array.Empty<SimpleItem>();

            // ── Clear button visibility ───────────────────────────────────────
            // Clear is shown if the property is on AND there is something to clear.
            bool showClear = owner.ShowClearButton
                && (owner.SelectedItem != null
                    || !string.IsNullOrEmpty(owner.InputText)
                    || (isMulti && owner.SelectedItems?.Count > 0));

            return new ComboBoxRenderState
            {
                // Interaction
                IsHovered            = owner.IsControlHovered,
                IsButtonHovered      = owner.IsButtonHovered,
                IsClearButtonHovered = owner.ClearButtonHovered,
                IsFocused            = owner.Focused,
                IsOpen               = owner.IsDropdownOpen,
                IsDisabled           = !owner.Enabled,
                IsReadOnly           = owner.IsReadOnly,

                // Loading / skeleton
                IsLoading            = owner.IsLoading,
                IsSkeleton           = owner.ShowSkeleton,
                LoadingAngle         = owner.LoadingRotationAngle,
                SkeletonOffset       = owner.SkeletonOffset,

                // Validation
                ValidationState      = owner.ValidationState,
                ValidationMessage    = owner.ErrorText,

                // Value / display
                IsShowingPlaceholder = isPlaceholder,
                DisplayText          = displayText,
                PlaceholderText      = owner.PlaceholderText,
                InputText            = owner.InputText,
                LeadingImagePath     = leadingPath,
                HasLeadingImage      = !string.IsNullOrEmpty(leadingPath),

                // Multi-select
                IsMultiSelect        = isMulti,
                SelectedChips        = chips,

                // Affordances
                ShowClearButton      = showClear,
                ChevronAngle         = owner.ChevronAngle,

                // Behavior family
                IsSearchable         = isSearchable,
                IsEditable           = isEditable,
                IsVisualDisplay      = isVisualDisplay,
                InlineEditorActive   = owner.IsEditingInline,

                // Resolved tokens
                VisualTokens         = tokens,
                ThemeTokens          = themeTokens,
            };
        }

        // ────────────────────────────────────────────────────────────────────
        // Theme token resolution
        // ────────────────────────────────────────────────────────────────────

        private static ComboBoxThemeTokens ResolveThemeTokens(BeepComboBox owner, Vis.Modules.IBeepTheme theme)
        {
            if (theme == null)
                return ComboBoxThemeTokens.Fallback();

            // Font resolution via BeepThemesManager (theme-driven, cached)
            Font labelFont   = BeepThemesManager.ToFont(theme.LabelFont)   ?? SystemFonts.DefaultFont;
            Font subFont     = BeepThemesManager.ToFont(theme.ComboBoxListFont) ?? SystemFonts.DefaultFont;

            return new ComboBoxThemeTokens
            {
                BackColor              = owner.BackColor,
                ForeColor              = owner.ForeColor,
                BorderColor            = theme.ComboBoxBorderColor != Color.Empty
                                         ? theme.ComboBoxBorderColor
                                         : theme.BorderColor,
                HoverBorderColor       = theme.ComboBoxHoverBorderColor != Color.Empty
                                         ? theme.ComboBoxHoverBorderColor
                                         : theme.ActiveBorderColor,
                FocusBorderColor       = theme.ActiveBorderColor != Color.Empty
                                         ? theme.ActiveBorderColor
                                         : theme.PrimaryColor,
                OpenBorderColor        = theme.ActiveBorderColor != Color.Empty
                                         ? theme.ActiveBorderColor
                                         : theme.PrimaryColor,
                DisabledBackColor      = theme.DisabledBackColor,
                DisabledForeColor      = theme.DisabledForeColor,
                ErrorBorderColor       = theme.ComboBoxErrorForeColor != Color.Empty
                                         ? theme.ComboBoxErrorForeColor
                                         : theme.ErrorColor,
                WarningBorderColor     = theme.WarningColor,
                SuccessBorderColor     = theme.SuccessColor,
                ErrorForeColor         = theme.ComboBoxErrorForeColor != Color.Empty
                                         ? theme.ComboBoxErrorForeColor
                                         : theme.ErrorColor,
                PlaceholderColor       = Color.FromArgb(158, theme.ComboBoxForeColor != Color.Empty
                                         ? theme.ComboBoxForeColor
                                         : theme.ForeColor),
                ChevronColor           = theme.SecondaryColor,
                ClearButtonColor       = theme.SecondaryColor,
                SelectionHighlight     = theme.ComboBoxSelectedBackColor != Color.Empty
                                         ? theme.ComboBoxSelectedBackColor
                                         : Color.FromArgb(235, 245, 255),
                ButtonHoverBackground  = Color.FromArgb(20, 0, 0, 0),
                PopupBackColor         = theme.ComboBoxBackColor != Color.Empty
                                         ? theme.ComboBoxBackColor
                                         : theme.BackColor,
                PopupBorderColor       = theme.ComboBoxBorderColor != Color.Empty
                                         ? theme.ComboBoxBorderColor
                                         : theme.BorderColor,
                PopupRowHoverColor     = theme.ComboBoxHoverBackColor != Color.Empty
                                         ? theme.ComboBoxHoverBackColor
                                         : Color.FromArgb(240, 248, 255),
                PopupRowSelectedColor  = theme.ComboBoxSelectedBackColor != Color.Empty
                                         ? theme.ComboBoxSelectedBackColor
                                         : Color.FromArgb(227, 242, 253),
                PopupRowFocusColor     = Color.FromArgb(200, 230, 255),
                PopupGroupHeaderBack   = theme.GridHeaderBackColor != Color.Empty
                                         ? theme.GridHeaderBackColor
                                         : Color.FromArgb(245, 245, 245),
                PopupGroupHeaderFore   = theme.SecondaryColor,
                PopupSeparatorColor    = theme.InactiveBorderColor != Color.Empty
                                         ? theme.InactiveBorderColor
                                         : Color.FromArgb(220, 220, 220),
                PopupSubTextColor      = theme.SecondaryColor,
                LabelFont              = labelFont,
                SubTextFont            = subFont,
            };
        }
    }
}
